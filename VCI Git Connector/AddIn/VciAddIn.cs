using System.Windows.Forms;
using Siemens.Applications.AddIns.VCIGitConnector.ContextMenu;
using Siemens.Applications.AddIns.VCIGitConnector.Workflow;
using Siemens.Engineering;
using Siemens.Engineering.AddIn.VersionControl;

namespace Siemens.Applications.AddIns.VCIGitConnector.AddIn
{
    public class VciAddIn : VciRepositoryAddIn
    {
        private readonly TiaPortal _tiaPortal;
        private readonly Settings _settings;

        public VciAddIn(TiaPortal tiaPortal) : base("Git")
        {
            Application.EnableVisualStyles();
            _tiaPortal = tiaPortal;
            _settings = Settings.Load();
        }

        public override VciWorkflowAddInSupport GetVciWorkflowAddInSupport()
        {
            return new WorkflowAddInProvider(_tiaPortal, _settings);
        }

        public override VciWorkspaceViewAddInProvider GetVciWorkspaceViewAddInProvider()
        {
            return new WorkspaceAddInProvider(_tiaPortal, _settings);
        }
    }
}