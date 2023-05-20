using System.Drawing;
using System.Windows.Forms;

namespace Siemens.Applications.AddIns.VCIGitConnectorUI
{
    public sealed partial class OutputForm : Form
    {
        public OutputForm(string windowTitle, Image icon, string messageTitle, string message)
        {
            InitializeComponent();

            Text = windowTitle.Trim();
            pictureBoxIcon.Image = icon;
            labelTitle.Text = messageTitle.Trim();
            textBoxMessage.Text = message.Trim();
            textBoxMessage.SelectionStart = 0;
            textBoxMessage.SelectionLength = 0;
        }

        private void pictureBox1_Click(object sender, System.EventArgs e)
        {

        }
    }
}
