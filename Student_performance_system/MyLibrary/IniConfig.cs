using IniParser;
using IniParser.Model;
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

        private static string GetConnectionString()
        {
            string server = GetValue("Database", "Server", "localhost");
            string database = GetValue("Database", "Database", "university_journal");
            string uid = GetValue("Database", "Uid", "root");
            string pwd = GetValue("Database", "Pwd", "vertrigo");
            string port = GetValue("Database", "Port", "3306");

            return $"Server={server};Database={database};Uid={uid};Pwd={pwd};Port={port};";
        }

        private static string GetValue(string section, string key, string defaultValue)
        {
            try
            {
                FileIniDataParser parser = new FileIniDataParser();
                IniData data = parser.ReadFile("config.ini");
                return data[section][key] ?? defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
