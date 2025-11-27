using System;
using System.Windows.Forms;


namespace MarkAnywhere
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var mainForm = new MainForm();
            var toolsForm = new ToolsForm(mainForm);

            toolsForm.Show();
            Application.Run(mainForm);
        }
    }
}