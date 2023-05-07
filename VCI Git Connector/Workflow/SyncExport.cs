using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Siemens.Applications.AddIns.VCIGitConnector.AddIn;
using Siemens.Applications.AddIns.VCIGitConnector.Utility;
using Siemens.Engineering;
using Siemens.Engineering.AddIn.VersionControl;

namespace Siemens.Applications.AddIns.VCIGitConnector.Workflow
{
    public class SyncExport : VciSyncExportSupport
    {
        private static TiaPortal _tiaPortal;
        private readonly Settings _settings;

        public SyncExport(TiaPortal tiaPortal, Settings settings)
        {
            _tiaPortal = tiaPortal;
            _settings = settings;
        }

        public override ExportResult PreExportExecute(IEnumerable<SyncPreExportInfo> itemsToExport, VciSyncExportAddInContext vciSyncExportAddInContext)
        {
            return ExportResult.Succeeded;
        }

        public override ExportResult PostExportExecute(IEnumerable<SyncPostExportInfo> itemsToExport, VciSyncExportAddInContext vciSyncExportAddInContext)
        {
            if (!_settings.CommitOnSync)
            {
                return ExportResult.Succeeded;
            }

            // Enumerate exported files
            var exportedFiles = new List<string>();
            foreach (var exportInfo in itemsToExport)
            {
                if (exportInfo.ExportStatus == ExportStatus.Succeeded)
                {
                    exportedFiles.Add("\"" + Path.Combine(vciSyncExportAddInContext.CurrentWorkspace.RootPath.FullName + exportInfo.WorkspaceEntry.RelativeWorkspacePath) + "\"");
                }
            }

            if (!exportedFiles.Any())
            {
                return ExportResult.Failed;
            }

            // Git Commit
            string commitMessage;

            if (UserInteraction.ShowInputDialog("Git Commit", "Enter a commit message", string.Empty, out commitMessage))
            {
                var process = Git.CreateGitProcess($"commit -m \"{commitMessage}\" {string.Join(" ", exportedFiles)}", vciSyncExportAddInContext.CurrentWorkspace.RootPath.FullName);

                string outputMessage;
                string errorMessage;

                var commitSuccessful = Git.ExecuteGitCommand(process, out outputMessage, out errorMessage, _tiaPortal);
                if (commitSuccessful)
                {
                    UserInteraction.ShowOutputDialog("Git Commit", SystemIcons.Information.ToBitmap(), "Output message from Git commit command", outputMessage + Environment.NewLine + errorMessage);

                    //Git Push
                    if (_settings.PushOnSync)
                    {
                        process = Git.CreateGitProcess("push", vciSyncExportAddInContext.CurrentWorkspace.RootPath.FullName);

                        var pushSuccessful = Git.ExecuteGitCommand(process, out outputMessage, out errorMessage, _tiaPortal);
                        if (pushSuccessful)
                        {
                            UserInteraction.ShowOutputDialog("Git Push", SystemIcons.Information.ToBitmap(), "Output message from Git push command", outputMessage + Environment.NewLine + errorMessage);
                        }
                        else
                        {
                            UserInteraction.ShowOutputDialog("Git Push", SystemIcons.Error.ToBitmap(), "Error message from Git push command", outputMessage + Environment.NewLine + errorMessage);
                        }
                    }

                    return ExportResult.Succeeded;
                }

                UserInteraction.ShowOutputDialog("Git Commit", SystemIcons.Error.ToBitmap(), "Error message from Git commit command", outputMessage + Environment.NewLine + errorMessage);
                return ExportResult.Failed;
            }

            return ExportResult.Aborted;
        }
    }
}