using System;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MasterServicePro.Services
{
    // Service to handle hardware identification, license checking and Pix payments
    // All comments must use ASCII characters only.
    public class LicenseService
    {
        private static readonly HttpClient httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
        private static string cachedHwid = null;

        // Base URL of the licensing API
        public static string ApiBaseUrl
        {
            get
            {
                string url = ConfigurationManager.AppSettings["LicenseApiUrl"];
                if (string.IsNullOrWhiteSpace(url))
                {
                    // Fallback URL for local testing or custom host
                    return "http://localhost/licenca/api";
                }
                return url.TrimEnd('/');
            }
        }

        // Retrieves or calculates a unique and consistent Hardware ID (HWID)
        public static string GetHardwareId()
        {
            if (!string.IsNullOrEmpty(cachedHwid))
                return cachedHwid;

            StringBuilder raw = new StringBuilder();

            // 1. Windows MachineGuid from Registry (Cryptographic identity of OS install)
            try
            {
                using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                using (var subKey = baseKey.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography"))
                {
                    if (subKey != null)
                    {
                        object guid = subKey.GetValue("MachineGuid");
                        if (guid != null)
                            raw.Append(guid.ToString());
                    }
                }
            }
            catch { }

            // 2. Machine specifications
            raw.Append(Environment.MachineName);
            raw.Append(Environment.UserName);
            raw.Append(Environment.ProcessorCount.ToString());

            // 3. System drive volume serial number
            try
            {
                string systemDrive = Path.GetPathRoot(Environment.SystemDirectory);
                DriveInfo drive = new DriveInfo(systemDrive);
                raw.Append(drive.TotalSize.ToString());
            }
            catch { }

            // Hash composite string with SHA-256 for a fixed-length string
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(raw.ToString()));
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    sb.Append(bytes[i].ToString("X2"));
                }
                cachedHwid = sb.ToString();
            }

            return cachedHwid;
        }

        // Local storage paths for storing the serial key
        private static string GetLicenseFilePath()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string dir = Path.Combine(appData, "MasterServicePro");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return Path.Combine(dir, "license.key");
        }

        // Retrieves the currently saved license key
        public static string GetStoredLicenseKey()
        {
            try
            {
                // First try registry
                using (var key = Registry.CurrentUser.OpenSubKey(@"Software\MasterServicePro"))
                {
                    if (key != null)
                    {
                        object val = key.GetValue("LicenseKey");
                        if (val != null && !string.IsNullOrWhiteSpace(val.ToString()))
                            return val.ToString().Trim();
                    }
                }

                // Second try local file
                string path = GetLicenseFilePath();
                if (File.Exists(path))
                {
                    string content = File.ReadAllText(path).Trim();
                    if (!string.IsNullOrWhiteSpace(content))
                        return content;
                }
            }
            catch { }

            return string.Empty;
        }

        // Saves the license key in registry and local file
        public static void SaveLicenseKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return;
            key = key.Trim();

            try
            {
                using (var regKey = Registry.CurrentUser.CreateSubKey(@"Software\MasterServicePro"))
                {
                    if (regKey != null)
                    {
                        regKey.SetValue("LicenseKey", key);
                    }
                }
            }
            catch { }

            try
            {
                string path = GetLicenseFilePath();
                File.WriteAllText(path, key);
            }
            catch { }
        }

        // Validates license with the remote server
        public static async Task<LicenseCheckResult> CheckLicenseAsync(string licenseKey = null)
        {
            if (string.IsNullOrWhiteSpace(licenseKey))
            {
                licenseKey = GetStoredLicenseKey();
            }

            if (string.IsNullOrWhiteSpace(licenseKey))
            {
                return new LicenseCheckResult
                {
                    Success = false,
                    Status = "missing_key",
                    Message = "Nenhuma chave de licença encontrada no sistema."
                };
            }

            string hwid = GetHardwareId();
            string endpoint = $"{ApiBaseUrl}/check.php?chave={Uri.EscapeDataString(licenseKey)}&hwid={Uri.EscapeDataString(hwid)}";

            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(endpoint);
                string json = await response.Content.ReadAsStringAsync();

                JObject data = JObject.Parse(json);
                string status = data["status"]?.ToString() ?? "unknown";

                return new LicenseCheckResult
                {
                    Success = data["success"]?.Value<bool>() ?? false,
                    Status = status,
                    Cliente = data["cliente"]?.ToString() ?? "",
                    Chave = data["chave"]?.ToString() ?? licenseKey,
                    VencimentoBr = data["vencimento_br"]?.ToString() ?? "",
                    DiasRestantes = data["dias_restantes"]?.Value<int>() ?? 0,
                    ValorMensalidade = data["valor_mensalidade"]?.Value<decimal>() ?? 80.00m,
                    Message = data["message"]?.ToString() ?? ""
                };
            }
            catch (Exception ex)
            {
                return new LicenseCheckResult
                {
                    Success = false,
                    Status = "network_error",
                    Message = "Não foi possível conectar ao servidor de licenças: " + ex.Message
                };
            }
        }

        // Requests a new Pix payment from the server
        public static async Task<PixGenerateResult> GeneratePixAsync(string licenseKey = null)
        {
            if (string.IsNullOrWhiteSpace(licenseKey))
            {
                licenseKey = GetStoredLicenseKey();
            }

            string endpoint = $"{ApiBaseUrl}/pix_generate.php";
            var payload = new { chave = licenseKey };
            string content = JsonConvert.SerializeObject(payload);

            try
            {
                var stringContent = new StringContent(content, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync(endpoint, stringContent);
                string json = await response.Content.ReadAsStringAsync();

                JObject data = JObject.Parse(json);

                return new PixGenerateResult
                {
                    Success = data["success"]?.Value<bool>() ?? false,
                    PaymentId = data["payment_id"]?.ToString() ?? "",
                    Valor = data["valor"]?.Value<decimal>() ?? 80.00m,
                    QrCodeBase64 = data["qr_code_base64"]?.ToString() ?? "",
                    CopiaCola = data["copia_cola"]?.ToString() ?? "",
                    Message = data["message"]?.ToString() ?? ""
                };
            }
            catch (Exception ex)
            {
                return new PixGenerateResult
                {
                    Success = false,
                    Message = "Erro ao solicitar QRCode Pix: " + ex.Message
                };
            }
        }

        // Checks payment status on the server
        public static async Task<PixStatusResult> CheckPixStatusAsync(string paymentId, string licenseKey = null)
        {
            if (string.IsNullOrWhiteSpace(licenseKey))
            {
                licenseKey = GetStoredLicenseKey();
            }

            string endpoint = $"{ApiBaseUrl}/pix_status.php?payment_id={Uri.EscapeDataString(paymentId ?? "")}&chave={Uri.EscapeDataString(licenseKey ?? "")}";

            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(endpoint);
                string json = await response.Content.ReadAsStringAsync();

                JObject data = JObject.Parse(json);

                return new PixStatusResult
                {
                    Success = data["success"]?.Value<bool>() ?? false,
                    Status = data["status"]?.ToString() ?? "pending",
                    VencimentoBr = data["vencimento_br"]?.ToString() ?? "",
                    Message = data["message"]?.ToString() ?? ""
                };
            }
            catch (Exception ex)
            {
                return new PixStatusResult
                {
                    Success = false,
                    Status = "error",
                    Message = "Erro ao checar status do Pix: " + ex.Message
                };
            }
        }
    }

    public class LicenseCheckResult
    {
        public bool Success { get; set; }
        public string Status { get; set; } // active, expired, blocked, not_found, hwid_mismatch, network_error
        public string Cliente { get; set; }
        public string Chave { get; set; }
        public string VencimentoBr { get; set; }
        public int DiasRestantes { get; set; }
        public decimal ValorMensalidade { get; set; }
        public string Message { get; set; }
    }

    public class PixGenerateResult
    {
        public bool Success { get; set; }
        public string PaymentId { get; set; }
        public decimal Valor { get; set; }
        public string QrCodeBase64 { get; set; }
        public string CopiaCola { get; set; }
        public string Message { get; set; }
    }

    public class PixStatusResult
    {
        public bool Success { get; set; }
        public string Status { get; set; } // pending, approved, cancelled, error
        public string VencimentoBr { get; set; }
        public string Message { get; set; }
    }
}
