using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace FastPCB.Data.Configuration
{
    public static class EnvFileLoader
    {
        public static void LoadNearest(string startDirectory)
        {
            var envPath = FindNearest(startDirectory, ".env");
            if (envPath is null)
            {
                return;
            }

            foreach (var rawLine in File.ReadAllLines(envPath))
            {
                var line = rawLine.Trim();
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                {
                    continue;
                }

                var separatorIndex = line.IndexOf('=');
                if (separatorIndex <= 0)
                {
                    continue;
                }

                var key = line[..separatorIndex].Trim();
                var value = line[(separatorIndex + 1)..].Trim().Trim('"', '\'');

                if (!string.IsNullOrWhiteSpace(key) && Environment.GetEnvironmentVariable(key) is null)
                {
                    Environment.SetEnvironmentVariable(key, value);
                }
            }
        }

        private static string? FindNearest(string startDirectory, string fileName)
        {
            var directory = new DirectoryInfo(startDirectory);

            while (directory is not null)
            {
                var directPath = Path.Combine(directory.FullName, fileName);
                if (File.Exists(directPath))
                {
                    return directPath;
                }

                var nestedPath = Path.Combine(directory.FullName, "fastPCB", fileName);
                if (File.Exists(nestedPath))
                {
                    return nestedPath;
                }

                directory = directory.Parent;
            }

            return null;
        }
    }

    public static class MySqlConnectionStringResolver
    {
        public static string Resolve(IConfiguration configuration)
        {
            var configuredConnectionString =
                configuration["MYSQL_URL"]
                ?? configuration["DATABASE_URL"]
                ?? configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(configuredConnectionString))
            {
                throw new InvalidOperationException("MYSQL_URL, DATABASE_URL veya ConnectionStrings:DefaultConnection tanimli degil.");
            }

            if (Uri.TryCreate(configuredConnectionString, UriKind.Absolute, out var databaseUrl)
                && (databaseUrl.Scheme.Equals("mysql", StringComparison.OrdinalIgnoreCase)
                    || databaseUrl.Scheme.Equals("mariadb", StringComparison.OrdinalIgnoreCase)))
            {
                var credentials = databaseUrl.UserInfo.Split(':', 2);
                var databaseName = databaseUrl.AbsolutePath.TrimStart('/');

                return new MySqlConnectionStringBuilder
                {
                    Server = databaseUrl.Host,
                    Port = (uint)(databaseUrl.IsDefaultPort ? 3306 : databaseUrl.Port),
                    Database = Uri.UnescapeDataString(databaseName),
                    UserID = credentials.Length > 0 ? Uri.UnescapeDataString(credentials[0]) : string.Empty,
                    Password = credentials.Length > 1 ? Uri.UnescapeDataString(credentials[1]) : string.Empty,
                    SslMode = MySqlSslMode.Preferred
                }.ConnectionString;
            }

            return configuredConnectionString;
        }
    }
}
