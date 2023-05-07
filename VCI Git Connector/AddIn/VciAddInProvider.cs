using System.Collections.Generic;
using Siemens.Engineering;
using Siemens.Engineering.AddIn.VersionControl;

namespace Siemens.Applications.AddIns.VCIGitConnector.AddIn
{
    public class VciAddInProvider : VciRepositoryAddInProvider
    {
        private readonly TiaPortal _tiaPortal;

        public VciAddInProvider(TiaPortal tiaPortal)
        {
            _tiaPortal = tiaPortal;
        }

        public override IEnumerable<VciRepositoryAddIn> GetVciRepositoryAddIns()
        {
            yield return new VciAddIn(_tiaPortal);
        }
    }
}
