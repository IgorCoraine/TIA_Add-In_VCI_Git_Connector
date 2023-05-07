using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Siemens.Applications.AddIns.VCIGitConnector.AddIn;
using Siemens.Applications.AddIns.VCIGitConnector.Utility;
using Siemens.Engineering;
using Siemens.Engineering.AddIn.VersionControl;

namespace Siemens.Applications.AddIns.VCIGitConnector.Workflow
{
    public class InitialExport : VciInitialExportSupport
    {
        private static TiaPortal _tiaPortal;
        private readonly Settings _settings;

        public InitialExport(TiaPortal tiaPortal, Settings settings)
        {
            _tiaPortal = tiaPortal;
            _settings = settings;
        }

        public override ExportResult PreExportExecute(IEnumerable<InitialPreExportInfo> itemsToExport, VciInitialExportAddInContext vciInitialExportAddInContext)
        {
            return ExportResult.Succeeded;
        }

        public override ExportResult PostExportExecute(IEnumerable<InitialPostExportInfo> itemsToExport, VciInitialExportAddInContext vciInitialExportAddInContext)
        {
            var exportedFiles = new List<string>();

            // Enumerate all exported files
            foreach (var exportInfo in itemsToExport)
            {
                if (exportInfo.ExportStatus == ExportStatus.Succeeded)
                {
                    exportedFiles.Add("\"" + exportInfo.FileToExportTo.FullName + "\"");
                }
            }

            if (!exportedFiles.Any())
            {
                return ExportResult.Failed;
            }

            // Git Add
            var process = Git.CreateGitProcess($"add {string.Join(" ", exportedFiles)}", vciInitialExportAddInContext.CurrentWorkspace.RootPath.FullName);

            string outputMessage;
            string errorMessage;

            var addSuccessful = Git.ExecuteGitCommand(process, out outputMessage, out errorMessage, _tiaPortal);

            if (!addSuccessful)
            {
                UserInteraction.ShowOutputDialog("Git Add", SystemIcons.Error.ToBitmap(), "Error message from Git add command", outputMessage + Environment.NewLine + errorMessage);
                return ExportResult.Failed;
            }

            // Git Commit
            string commitMessage;

            if (UserInteraction.ShowInputDialog("Git Commit", "Enter a commit message", string.Empty, out commitMessage))
            {
                process = Git.CreateGitProcess($"commit -m \"{commitMessage}\" {string.Join(" ", exportedFiles)}", vciInitialExportAddInContext.CurrentWorkspace.RootPath.FullName);

                var commitSuccessful = Git.ExecuteGitCommand(process, out outputMessage, out errorMessage, _tiaPortal);
                if (commitSuccessful)
                {
                    UserInteraction.ShowOutputDialog("Git Commit", SystemIcons.Information.ToBitmap(), "Output message from Git commit command", outputMessage + Environment.NewLine + errorMessage);
                    return ExportResult.Succeeded;
                }

                UserInteraction.ShowOutputDialog("Git Commit", SystemIcons.Error.ToBitmap(), "Error message from Git commit command", outputMessage + Environment.NewLine + errorMessage);
                return ExportResult.Failed;
            }

            return ExportResult.Aborted;
        }
    }
}