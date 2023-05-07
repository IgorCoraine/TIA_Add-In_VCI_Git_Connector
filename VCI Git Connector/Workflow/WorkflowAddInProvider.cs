using Siemens.Applications.AddIns.VCIGitConnector.AddIn;
using Siemens.Engineering;
using Siemens.Engineering.AddIn.VersionControl;

namespace Siemens.Applications.AddIns.VCIGitConnector.Workflow
{
    public class WorkflowAddInProvider : VciWorkflowAddInSupport
    {
        private readonly TiaPortal _tiaPortal;
        private readonly Settings _settings;

        public WorkflowAddInProvider(TiaPortal tiaPortal, Settings settings)
        {
            _tiaPortal = tiaPortal;
            _settings = settings;
        }
        
        public override VciInitialExportSupport CreateInitialExportSupport()
        {
            return new InitialExport(_tiaPortal, _settings);
        }

        public override VciSyncExportSupport CreateSyncExportSupport()
        {
            return new SyncExport(_tiaPortal, _settings);
        }
    }
}
