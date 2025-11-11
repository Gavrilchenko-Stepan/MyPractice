using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainForm
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                Application.Run(new MainForm());
            }
            catch (FileNotFoundException ex) when (ex.Message.Contains("config.ini"))
            {
                MessageBox.Show(
                    "Файл настроек config.ini не найден.\nСоздайте файл config.ini в папке приложения.",
                    "Ошибка конфигурации",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                MessageBox.Show(
                    $"Ошибка подключения к базе данных:\n{ex.Message}",
                    "Ошибка БД",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Критическая ошибка:\n{ex.Message}",
                    "Ошибка приложения",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}
