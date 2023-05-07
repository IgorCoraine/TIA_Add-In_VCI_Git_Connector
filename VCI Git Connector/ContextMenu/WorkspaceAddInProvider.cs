using System.Collections.Generic;
using Siemens.Applications.AddIns.VCIGitConnector.AddIn;
using Siemens.Engineering;
using Siemens.Engineering.AddIn.Menu;
using Siemens.Engineering.AddIn.VersionControl;

namespace Siemens.Applications.AddIns.VCIGitConnector.ContextMenu
{
    public class WorkspaceAddInProvider : VciWorkspaceViewAddInProvider
    {
        private readonly TiaPortal _tiaPortal;
        private readonly Settings _settings;

        public WorkspaceAddInProvider(TiaPortal tiaPortal, Settings settings)
        {
            _tiaPortal = tiaPortal;
            _settings = settings;
        }

        public override IEnumerable<ContextMenuAddIn> GetContextMenuAddIns()
        {
            yield return new WorkspaceAddIn(_tiaPortal, _settings);
        }
    }
}
