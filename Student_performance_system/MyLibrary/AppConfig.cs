using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary
{
    public class AppConfig
    {
        public static string ConnectionString
        {
            get
            {
                var connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"];
                if (connectionString == null)
                    throw new Exception("Не найдена строка подключения 'MySqlConnection' в App.config");

                return connectionString.ConnectionString;
            }
        }

        public static string DefaultGroup => "П-10";
        public static string DefaultSubject => "Математика";
    }
}
