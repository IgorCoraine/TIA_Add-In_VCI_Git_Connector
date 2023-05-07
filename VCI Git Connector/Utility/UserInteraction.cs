using System.Drawing;
using System.Windows.Forms;
using Siemens.Applications.AddIns.VCIGitConnector.Properties;
using Siemens.Applications.AddIns.VCIGitConnectorUI;

namespace Siemens.Applications.AddIns.VCIGitConnector.Utility
{
    public static class UserInteraction
    {
        private static Form GetForegroundWindow()
        {
            var form = new Form {Width = 0, Height = 0, Opacity = 0, Icon = Resources.FormIcon};
            form.Show();
            form.TopMost = true;
            form.Activate();
            form.TopMost = false;
            return form;
        }

        public static void ShowOutputDialog(string windowTitle, Image icon, string messageTitle, string message)
        {
            using (var form = GetForegroundWindow())
            {
                var outputDialog = new OutputForm(windowTitle, icon, messageTitle, message) {Owner = form};
                outputDialog.ShowDialog();
            }
        }

        public static bool ShowYesNoDialog(string text, string caption)
        {
            using (var form = GetForegroundWindow())
            {
                return MessageBox.Show(form, text, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
            }
        }

        public static bool ShowInputDialog(string windowTitle, string messageTitle, string defaultMessage, out string message)
        {
            using (var form = GetForegroundWindow())
            {
                var inputDialog = new InputForm(windowTitle, messageTitle, defaultMessage) {Owner = form};
                if (inputDialog.ShowDialog() == DialogResult.OK)
                {
                    message = inputDialog.Message.Replace("\"", "'");
                    return true;
                }
            }

            message = defaultMessage;
            return false;
        }
    }
}