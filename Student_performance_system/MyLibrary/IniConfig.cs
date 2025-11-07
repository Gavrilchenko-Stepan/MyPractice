using IniParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary
{
    public class IniConfig
    {
        public static string ConnectionString => GetConnectionString();
        public static string DefaultGroup => GetValue("Settings", "DefaultGroup", "П-10");
        public static string DefaultSubject => GetValue("Settings", "DefaultSubject", "Математика");

        private static string GetConnectionString()
        {
            var server = GetValue("Database", "Server", "localhost");
            var database = GetValue("Database", "Database", "university_journal");
            var uid = GetValue("Database", "Uid", "root");
            var pwd = GetValue("Database", "Pwd", "vertrigo");
            var port = GetValue("Database", "Port", "3306");

            return $"Server={server};Database={database};Uid={uid};Pwd={pwd};Port={port};";
        }

        private static string GetValue(string section, string key, string defaultValue)
        {
            try
            {
                var parser = new FileIniDataParser();
                var data = parser.ReadFile("config.ini");
                return data[section][key] ?? defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
