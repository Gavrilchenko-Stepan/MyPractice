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
                try
                {
                    // Пробуем получить из App.config
                    var connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"];
                    if (connectionString != null)
                        return connectionString.ConnectionString;
                }
                catch
                {
                    // Если не получилось, используем прямое подключение
                }

                // Прямая строка подключения (настройте под вашу БД)
                return "Server=localhost;Database=university_journal;Uid=root;Pwd=vertrigo;Port=3306;";
            }
        }

        public static string DefaultGroup => "П-10";
        public static string DefaultSubject => "Математика";
    }
}
