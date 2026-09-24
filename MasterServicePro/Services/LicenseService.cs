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
        private const string ApiSecretSalt = "MasterDevSolutions_Secret_Key_2026_Salt";

        // Base URL of the licensing API
        public static string ApiBaseUrl
        {
            get
            {
                string url = ConfigurationManager.AppSettings["LicenseApiUrl"];
                if (string.IsNullOrWhiteSpace(url))
                {
                    // Fallback URL for production host
                    return "https://masterdevsolutions.com.br/server_api/api";
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

        // Local storage path for encrypted offline license cache
        private static string GetLicenseCacheFilePath()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string dir = Path.Combine(appData, "MasterServicePro");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return Path.Combine(dir, "license.cache");
        }

        // Computes server-aligned SHA-256 signature for license validation
        private static string ComputeServerSignature(string chave, string hwid, string vencimento, string status)
        {
            string payload = $"{chave}|{hwid}|{vencimento}|{status}|{ApiSecretSalt}";
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(payload));
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    sb.Append(bytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        // Generates an AES key derived from Hardware ID and secret salt
        private static byte[] GetAesKey()
        {
            using (SHA256 sha = SHA256.Create())
            {
                return sha.ComputeHash(Encoding.UTF8.GetBytes(GetHardwareId() + "_" + ApiSecretSalt));
            }
        }

        // Encrypts text using AES-256 with key tied to machine HWID
        private static string EncryptString(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return string.Empty;
            byte[] key = GetAesKey();
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.GenerateIV();
                using (MemoryStream ms = new MemoryStream())
                {
                    ms.Write(aes.IV, 0, aes.IV.Length);
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        // Decrypts text using AES-256
        private static string DecryptString(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return string.Empty;
            try
            {
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                byte[] key = GetAesKey();
                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    byte[] iv = new byte[aes.BlockSize / 8];
                    Array.Copy(cipherBytes, 0, iv, 0, iv.Length);
                    aes.IV = iv;
                    using (MemoryStream ms = new MemoryStream(cipherBytes, iv.Length, cipherBytes.Length - iv.Length))
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    using (StreamReader sr = new StreamReader(cs))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        // Saves encrypted license cache to both registry and local file
        public static void SaveOfflineLicenseCache(CachedLicenseData data)
        {
            if (data == null) return;
            try
            {
                string json = JsonConvert.SerializeObject(data);
                string encrypted = EncryptString(json);

                // Save to Registry
                try
                {
                    using (var regKey = Registry.CurrentUser.CreateSubKey(@"Software\MasterServicePro"))
                    {
                        if (regKey != null)
                        {
                            regKey.SetValue("LicenseCache", encrypted);
                        }
                    }
                }
                catch { }

                // Save to file
                try
                {
                    string path = GetLicenseCacheFilePath();
                    File.WriteAllText(path, encrypted);
                }
                catch { }
            }
            catch { }
        }

        // Retrieves encrypted license cache from registry or file
        private static string GetStoredLicenseCache()
        {
            try
            {
                // Try registry first
                using (var key = Registry.CurrentUser.OpenSubKey(@"Software\MasterServicePro"))
                {
                    if (key != null)
                    {
                        object val = key.GetValue("LicenseCache");
                        if (val != null && !string.IsNullOrWhiteSpace(val.ToString()))
                            return val.ToString().Trim();
                    }
                }

                // Try file second
                string path = GetLicenseCacheFilePath();
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

        // Validates local offline license cache when internet is unavailable
        public static LicenseCheckResult ValidateOfflineLicense(string licenseKey = null)
        {
            if (string.IsNullOrWhiteSpace(licenseKey))
            {
                licenseKey = GetStoredLicenseKey();
            }

            try
            {
                string rawEncrypted = GetStoredLicenseCache();
                if (string.IsNullOrWhiteSpace(rawEncrypted))
                {
                    return null;
                }

                string json = DecryptString(rawEncrypted);
                if (string.IsNullOrWhiteSpace(json))
                {
                    return null;
                }

                CachedLicenseData cached = JsonConvert.DeserializeObject<CachedLicenseData>(json);
                if (cached == null)
                {
                    return null;
                }

                // Validate Hardware ID
                string currentHwid = GetHardwareId();
                if (!string.Equals(cached.Hwid, currentHwid, StringComparison.OrdinalIgnoreCase))
                {
                    return new LicenseCheckResult
                    {
                        Success = false,
                        Status = "hwid_mismatch",
                        IsOffline = true,
                        Message = "Esta licenca foi vinculada a outro computador."
                    };
                }

                // Validate Key
                if (!string.IsNullOrWhiteSpace(licenseKey) && !string.Equals(cached.Chave, licenseKey, StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }

                // Validate digital signature from server
                string expectedSignature = ComputeServerSignature(cached.Chave, cached.Hwid, cached.Vencimento, "ativa");
                if (!string.Equals(cached.Signature, expectedSignature, StringComparison.OrdinalIgnoreCase))
                {
                    return new LicenseCheckResult
                    {
                        Success = false,
                        Status = "invalid_signature",
                        IsOffline = true,
                        Message = "Assinatura digital da licenca offline invalida ou corrompida."
                    };
                }

                // Check expiration date
                if (!DateTime.TryParse(cached.Vencimento, out DateTime vencimentoDate))
                {
                    return null;
                }

                DateTime now = DateTime.Now;

                // Anti-tampering check: clock should not be prior to last verification
                if (DateTime.TryParse(cached.LastVerified, out DateTime lastVerifiedDate))
                {
                    if (now < lastVerifiedDate.AddHours(-24))
                    {
                        return new LicenseCheckResult
                        {
                            Success = false,
                            Status = "clock_rollback",
                            IsOffline = true,
                            Message = "O relogio do computador foi alterado. Conecte-se a internet para sincronizar."
                        };
                    }
                }

                if (now > vencimentoDate)
                {
                    return new LicenseCheckResult
                    {
                        Success = true,
                        Status = "expired",
                        Cliente = cached.Cliente,
                        Chave = cached.Chave,
                        Vencimento = cached.Vencimento,
                        VencimentoBr = cached.VencimentoBr,
                        DiasRestantes = 0,
                        ValorMensalidade = cached.ValorMensalidade,
                        IsOffline = true,
                        Message = "Sua licenca expirou em " + cached.VencimentoBr + ". Conecte-se a internet para renovar."
                    };
                }

                int diasRestantes = Math.Max(0, (int)(vencimentoDate.Date - now.Date).TotalDays);

                return new LicenseCheckResult
                {
                    Success = true,
                    Status = "active",
                    Cliente = cached.Cliente,
                    Chave = cached.Chave,
                    Vencimento = cached.Vencimento,
                    VencimentoBr = cached.VencimentoBr,
                    DiasRestantes = diasRestantes,
                    ValorMensalidade = cached.ValorMensalidade,
                    IsOffline = true,
                    Message = "Licenca ativa (modo offline - sem conexao com a internet)."
                };
            }
            catch
            {
                return null;
            }
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

        // Validates license with the remote server or falls back to offline cache
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
                    Message = "Nenhuma chave de licenca encontrada no sistema."
                };
            }

            string hwid = GetHardwareId();
            string endpoint = $"{ApiBaseUrl}/check.php?chave={Uri.EscapeDataString(licenseKey)}&hwid={Uri.EscapeDataString(hwid)}";

            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(endpoint);
                string json = await response.Content.ReadAsStringAsync();

                JObject data = JObject.Parse(json);
                bool success = data["success"]?.Value<bool>() ?? false;
                string status = data["status"]?.ToString() ?? "unknown";

                var result = new LicenseCheckResult
                {
                    Success = success,
                    Status = status,
                    Cliente = data["cliente"]?.ToString() ?? "",
                    Chave = data["chave"]?.ToString() ?? licenseKey,
                    Vencimento = data["vencimento"]?.ToString() ?? "",
                    VencimentoBr = data["vencimento_br"]?.ToString() ?? "",
                    DiasRestantes = data["dias_restantes"]?.Value<int>() ?? 0,
                    ValorMensalidade = data["valor_mensalidade"]?.Value<decimal>() ?? 80.00m,
                    Message = data["message"]?.ToString() ?? "",
                    IsOffline = false
                };

                // Cache active license locally for seamless offline operation
                if (success && status == "active")
                {
                    string signature = data["signature"]?.ToString() ?? "";
                    SaveOfflineLicenseCache(new CachedLicenseData
                    {
                        Chave = result.Chave,
                        Cliente = result.Cliente,
                        Hwid = hwid,
                        Vencimento = result.Vencimento,
                        VencimentoBr = result.VencimentoBr,
                        Signature = signature,
                        LastVerified = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        ValorMensalidade = result.ValorMensalidade
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                // Network failure: validate against local encrypted cache
                var offlineResult = ValidateOfflineLicense(licenseKey);
                if (offlineResult != null)
                {
                    return offlineResult;
                }

                return new LicenseCheckResult
                {
                    Success = false,
                    Status = "network_error",
                    IsOffline = true,
                    Message = "Nao foi possivel conectar ao servidor de licencas e nao ha cache offline valido: " + ex.Message
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

    public class CachedLicenseData
    {
        public string Chave { get; set; }
        public string Cliente { get; set; }
        public string Hwid { get; set; }
        public string Vencimento { get; set; }
        public string VencimentoBr { get; set; }
        public string Signature { get; set; }
        public string LastVerified { get; set; }
        public decimal ValorMensalidade { get; set; }
    }

    public class LicenseCheckResult
    {
        public bool Success { get; set; }
        public string Status { get; set; } // active, expired, blocked, not_found, hwid_mismatch, network_error, invalid_signature, clock_rollback
        public string Cliente { get; set; }
        public string Chave { get; set; }
        public string Vencimento { get; set; }
        public string VencimentoBr { get; set; }
        public int DiasRestantes { get; set; }
        public decimal ValorMensalidade { get; set; }
        public string Message { get; set; }
        public bool IsOffline { get; set; }
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
