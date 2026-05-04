using System;
using System.Windows.Forms;

namespace TPV
{
    internal static class Program
    {
        /// <summary>
        /// Aplikazioaren sarrera puntua
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.ThreadException += (_, e) =>
            {
                try { MessageBox.Show(e.Exception?.ToString() ?? "Errore ezezaguna", "Errorea", MessageBoxButtons.OK, MessageBoxIcon.Error); } catch { }
            };
            AppDomain.CurrentDomain.UnhandledException += (_, e) =>
            {
                try { MessageBox.Show(e.ExceptionObject?.ToString() ?? "Errore ezezaguna", "Errorea", MessageBoxButtons.OK, MessageBoxIcon.Error); } catch { }
            };
            ApplicationConfiguration.Initialize();
            EguraldiaZerbitzua.KargatuGaurkoEguraldia();
            Application.ApplicationExit += (_, _) => TxatBezeroa.Instantzia.Deskonektatu();
            Application.Run(new Login());
        }
    }
}
