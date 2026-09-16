using System;
using System.IO;

namespace MasterServicePro.Utils
{
    public static class WebPathResolver
    {
        public static string GetHtmlPath(string relativeWebPath)
        {
            // Remove barras iniciais se houver
            relativeWebPath = relativeWebPath.TrimStart('\\', '/');

            // 1. Tenta buscar direto na pasta do Executável (Produção)
            string prodPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Web", relativeWebPath);
            if (File.Exists(prodPath))
                return Path.GetFullPath(prodPath);
            
            // 2. Tenta buscar voltando duas pastas (Desenvolvimento bin\Debug ou bin\Release)
            string devPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Web", relativeWebPath);
            if (File.Exists(devPath))
                return Path.GetFullPath(devPath);

            // 3. Fallback (vai dar erro na WebView, mas retorna o caminho onde DEVERIA estar em prod)
            return Path.GetFullPath(prodPath);
        }
    }
}
