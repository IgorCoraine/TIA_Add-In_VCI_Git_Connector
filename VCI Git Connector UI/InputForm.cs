using System.Windows.Forms;

namespace Siemens.Applications.AddIns.VCIGitConnectorUI
{
    public sealed partial class InputForm : Form
    {
        public InputForm(string windowTitle, string messageTitle, string defaultMessage)
        {
            InitializeComponent();
            Text = windowTitle.Trim();
            labelTitle.Text = messageTitle.Trim();
            textBoxMessage.Text = defaultMessage.Trim();
        }

        public string Message => textBoxMessage.Text;
    }
}
