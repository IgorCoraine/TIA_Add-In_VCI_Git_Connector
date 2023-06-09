﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Siemens.Applications.AddIns.VCIGitConnector.AddIn;
using Siemens.Applications.AddIns.VCIGitConnector.Utility;
using Siemens.Engineering;
using Siemens.Engineering.AddIn.Menu;
using Siemens.Engineering.AddIn.VersionControl;
using System.IO;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;
using System.Windows.Shapes;
using System.Windows.Input;
using Siemens.Engineering.HmiUnified.UI.Shapes;
using Path = System.IO.Path;

namespace Siemens.Applications.AddIns.VCIGitConnector.ContextMenu
{
    internal class WorkspaceAddIn : ContextMenuAddIn
    {
        private static TiaPortal _tiaPortal;
        private readonly Settings _settings;

        public WorkspaceAddIn(TiaPortal tiaPortal, Settings settings) : base("Git")
        {
            _tiaPortal = tiaPortal;
            _settings = settings;
        }

        protected override void BuildContextMenuItems(ContextMenuAddInRoot addInRootSubmenu)
        {
            addInRootSubmenu.Items.AddActionItem<WorkspaceFile, WorkspaceFolder>("Add", GitAddClick);
            addInRootSubmenu.Items.AddActionItem<WorkspaceFile, WorkspaceFolder>("Commit", GitCommitClick);
            addInRootSubmenu.Items.AddActionItem<WorkspaceFile, WorkspaceFolder>("Log", GitLogClick); //Changed to log --oneline
            addInRootSubmenu.Items.AddActionItem<WorkspaceFile, WorkspaceFolder>("Status", GitStatusClick); //Changed to general status instead of object status
            addInRootSubmenu.Items.AddActionItem<WorkspaceFile, WorkspaceFolder>("Reset", GitResetClick);
            addInRootSubmenu.Items.AddActionItem<WorkspaceFile, WorkspaceFolder>("Restore", GitRestoreClick);
            addInRootSubmenu.Items.AddActionItem<WorkspaceFile, WorkspaceFolder>("Push", GitPushClick);
            addInRootSubmenu.Items.AddActionItem<WorkspaceFile, WorkspaceFolder>("Pull", GitPullClick);
            addInRootSubmenu.Items.AddActionItem<WorkspaceFile, WorkspaceFolder>("Free Command", GitFreeClick); //Added Item
            addInRootSubmenu.Items.AddActionItem<WorkspaceFile, WorkspaceFolder>("Export Log", GitExportClick); //Added Item
            addInRootSubmenu.Items.AddActionItem<WorkspaceFile, WorkspaceFolder>("Init", GitInitClick); //Added Item
            addInRootSubmenu.Items.AddActionItem<WorkspaceFile, WorkspaceFolder>("Ignore", GitIgnoreClick); //Added Item
            addInRootSubmenu.Items.AddActionItem<WorkspaceFile, WorkspaceFolder>("Clone", GitCloneClick); //Added Item
            addInRootSubmenu.Items.AddActionItem<WorkspaceFile, WorkspaceFolder>("Archive and Push", GitArchiveClick); //Added Item
            addInRootSubmenu.Items.AddActionItem<WorkspaceFile, WorkspaceFolder>("Add/Set Origin", GitOriginClick); //Added Item
            var settingsSubmenu = addInRootSubmenu.Items.AddSubmenu("Settings");
            settingsSubmenu.Items.AddActionItemWithCheckBox<WorkspaceFile, WorkspaceFolder>("Commit on VCI synchronize", _settings.GitCommitOnSyncClick, _settings.GitCommitOnSyncStatus);
            settingsSubmenu.Items.AddActionItemWithCheckBox<WorkspaceFile, WorkspaceFolder>("Push on VCI synchronize", _settings.GitPushOnSyncClick, _settings.GitPushOnSyncStatus);
        }

        // GIT ADD
        private static void GitAddClick(MenuSelectionProvider<WorkspaceFile, WorkspaceFolder> menuSelectionProvider)
        {
            var workspacePath = string.Empty;
            var objectPaths = new List<string>();

            if (menuSelectionProvider.GetSelection<WorkspaceFile>().Any())
            {
                workspacePath = menuSelectionProvider.GetSelection<WorkspaceFile>().First().Workspace.RootPath.FullName;
            }
            else if (menuSelectionProvider.GetSelection<WorkspaceFolder>().Any())
            {
                workspacePath = menuSelectionProvider.GetSelection<WorkspaceFolder>().First().Workspace.RootPath.FullName;
            }

            foreach (var file in menuSelectionProvider.GetSelection<WorkspaceFile>())
            {
                objectPaths.Add("\"" + file.FileInfo.FullName + "\"");
            }

            foreach (var folder in menuSelectionProvider.GetSelection<WorkspaceFolder>())
            {
                objectPaths.Add("\"" + folder.DirectoryInfo.FullName.TrimEnd('\\') + "\"");
            }

            if (workspacePath != string.Empty && objectPaths.Any())
            {
                var addProcess = Git.CreateGitProcess($"add {string.Join(" ", objectPaths)}", workspacePath);

                string outputMessage;
                string errorMessage;

                var addSuccessful = Git.ExecuteGitCommand(addProcess, out outputMessage, out errorMessage, _tiaPortal);

                if (addSuccessful)
                {
                    UserInteraction.ShowOutputDialog("Git Add", SystemIcons.Information.ToBitmap(), "Output message from Git add command", outputMessage + Environment.NewLine + errorMessage);
                }
                else
                {
                    UserInteraction.ShowOutputDialog("Git Add", SystemIcons.Error.ToBitmap(), "Error message from Git add command", outputMessage + Environment.NewLine + errorMessage);
                }
            }
        }

        // GIT COMMIT
        private static void GitCommitClick(MenuSelectionProvider<WorkspaceFile, WorkspaceFolder> menuSelectionProvider)
        {
            var workspacePath = string.Empty;
            var objectPaths = new List<string>();

            if (menuSelectionProvider.GetSelection<WorkspaceFile>().Any())
            {
                workspacePath = menuSelectionProvider.GetSelection<WorkspaceFile>().First().Workspace.RootPath.FullName;
            }
            else if (menuSelectionProvider.GetSelection<WorkspaceFolder>().Any())
            {
                workspacePath = menuSelectionProvider.GetSelection<WorkspaceFolder>().First().Workspace.RootPath.FullName;
            }

            foreach (var file in menuSelectionProvider.GetSelection<WorkspaceFile>())
            {
                objectPaths.Add("\"" + file.FileInfo.FullName + "\"");
            }

            foreach (var folder in menuSelectionProvider.GetSelection<WorkspaceFolder>())
            {
                objectPaths.Add("\"" + folder.DirectoryInfo.FullName.TrimEnd('\\') + "\"");
            }

            if (workspacePath != string.Empty && objectPaths.Any())
            {
                string commitMessage;

                if (UserInteraction.ShowInputDialog("Git Commit", "Enter a commit message", string.Empty, out commitMessage))
                {
                    var commitProcess = Git.CreateGitProcess($"commit -m \"{commitMessage}\" {string.Join(" ", objectPaths)}", workspacePath);

                    string outputMessage;
                    string errorMessage;

                    var commitSuccessful = Git.ExecuteGitCommand(commitProcess, out outputMessage, out errorMessage, _tiaPortal);

                    if (commitSuccessful)
                    {
                        UserInteraction.ShowOutputDialog("Git Commit", SystemIcons.Information.ToBitmap(), "Output message from Git commit command", outputMessage + Environment.NewLine + errorMessage);
                    }
                    else
                    {
                        UserInteraction.ShowOutputDialog("Git Commit", SystemIcons.Error.ToBitmap(), "Error message from Git commit command", outputMessage + Environment.NewLine + errorMessage);
                    }
                }
            }
        }

        // GIT LOG
        private static void GitLogClick(MenuSelectionProvider<WorkspaceFile, WorkspaceFolder> menuSelectionProvider)
        {
            var workspacePath = string.Empty;
            var objectPaths = new List<string>();

            if (menuSelectionProvider.GetSelection<WorkspaceFile>().Any())
            {
                workspacePath = menuSelectionProvider.GetSelection<WorkspaceFile>().First().Workspace.RootPath.FullName;
            }
            else if (menuSelectionProvider.GetSelection<WorkspaceFolder>().Any())
            {
                workspacePath = menuSelectionProvider.GetSelection<WorkspaceFolder>().First().Workspace.RootPath.FullName;
            }

            foreach (var file in menuSelectionProvider.GetSelection<WorkspaceFile>())
            {
                objectPaths.Add("\"" + file.FileInfo.FullName + "\"");
            }

            foreach (var folder in menuSelectionProvider.GetSelection<WorkspaceFolder>())
            {
                objectPaths.Add("\"" + folder.DirectoryInfo.FullName.TrimEnd('\\') + "\"");
            }

            if (workspacePath != string.Empty && objectPaths.Any())
            {
                var logProcess = Git.CreateGitProcess($"log --oneline", workspacePath);

                string outputMessage;
                string errorMessage;

                var logSuccessful = Git.ExecuteGitCommand(logProcess, out outputMessage, out errorMessage, _tiaPortal);

                if (logSuccessful)
                {
                    UserInteraction.ShowOutputDialog("Git Log", SystemIcons.Information.ToBitmap(), "Output message from Git log command", outputMessage + Environment.NewLine + errorMessage);
                }
                else
                {
                    UserInteraction.ShowOutputDialog("Git Log", SystemIcons.Error.ToBitmap(), "Error message from Git log command", outputMessage + Environment.NewLine + errorMessage);
                }
            }
        }

        // GIT STATUS
        private static void GitStatusClick(MenuSelectionProvider<WorkspaceFile, WorkspaceFolder> menuSelectionProvider)
        {
            var workspacePath = string.Empty;
            var objectPaths = new List<string>();

            if (menuSelectionProvider.GetSelection<WorkspaceFile>().Any())
            {
                workspacePath = menuSelectionProvider.GetSelection<WorkspaceFile>().First().Workspace.RootPath.FullName;
            }
            else if (menuSelectionProvider.GetSelection<WorkspaceFolder>().Any())
            {
                workspacePath = menuSelectionProvider.GetSelection<WorkspaceFolder>().First().Workspace.RootPath.FullName;
            }

            foreach (var file in menuSelectionProvider.GetSelection<WorkspaceFile>())
            {
                objectPaths.Add("\"" + file.FileInfo.FullName + "\"");
            }

            foreach (var folder in menuSelectionProvider.GetSelection<WorkspaceFolder>())
            {
                objectPaths.Add("\"" + folder.DirectoryInfo.FullName.TrimEnd('\\') + "\"");
            }

            if (workspacePath != string.Empty && objectPaths.Any())
            {
                var statusProcess = Git.CreateGitProcess($"status", workspacePath);

                string outputMessage;
                string errorMessage;

                var statusSuccessful = Git.ExecuteGitCommand(statusProcess, out outputMessage, out errorMessage, _tiaPortal);

                if (statusSuccessful)
                {
                    UserInteraction.ShowOutputDialog("Git Status", SystemIcons.Information.ToBitmap(), "Output message from Git status command", outputMessage + Environment.NewLine + errorMessage);
                }
                else
                {
                    UserInteraction.ShowOutputDialog("Git Status", SystemIcons.Error.ToBitmap(), "Error message from Git status command", outputMessage + Environment.NewLine + errorMessage);
                }
            }
        }

        // GIT RESET
        private static void GitResetClick(MenuSelectionProvider<WorkspaceFile, WorkspaceFolder> menuSelectionProvider)
        {
            var workspacePath = string.Empty;
            var objectPaths = new List<string>();

            if (menuSelectionProvider.GetSelection<WorkspaceFile>().Any())
            {
                workspacePath = menuSelectionProvider.GetSelection<WorkspaceFile>().First().Workspace.RootPath.FullName;
            }
            else if (menuSelectionProvider.GetSelection<WorkspaceFolder>().Any())
            {
                workspacePath = menuSelectionProvider.GetSelection<WorkspaceFolder>().First().Workspace.RootPath.FullName;
            }

            foreach (var file in menuSelectionProvider.GetSelection<WorkspaceFile>())
            {
                objectPaths.Add("\"" + file.FileInfo.FullName + "\"");
            }

            foreach (var folder in menuSelectionProvider.GetSelection<WorkspaceFolder>())
            {
                objectPaths.Add("\"" + folder.DirectoryInfo.FullName.TrimEnd('\\') + "\"");
            }

            if (workspacePath != string.Empty && objectPaths.Any())
            {
                var resetProcess = Git.CreateGitProcess($"reset HEAD {string.Join(" ", objectPaths)}", workspacePath);

                string outputMessage;
                string errorMessage;

                var resetSuccessful = Git.ExecuteGitCommand(resetProcess, out outputMessage, out errorMessage, _tiaPortal);

                if (resetSuccessful)
                {
                    UserInteraction.ShowOutputDialog("Git Reset", SystemIcons.Information.ToBitmap(), "Output message from Git reset command", outputMessage + Environment.NewLine + errorMessage);
                }
                else
                {
                    UserInteraction.ShowOutputDialog("Git Reset", SystemIcons.Error.ToBitmap(), "Error message from Git reset command", outputMessage + Environment.NewLine + errorMessage);
                }
            }
        }

        // GIT RESTORE
        private void GitRestoreClick(MenuSelectionProvider<WorkspaceFile, WorkspaceFolder> menuSelectionProvider)
        {
            var workspacePath = string.Empty;
            var objectPaths = new List<string>();

            if (menuSelectionProvider.GetSelection<WorkspaceFile>().Any())
            {
                workspacePath = menuSelectionProvider.GetSelection<WorkspaceFile>().First().Workspace.RootPath.FullName;
            }
            else if (menuSelectionProvider.GetSelection<WorkspaceFolder>().Any())
            {
                workspacePath = menuSelectionProvider.GetSelection<WorkspaceFolder>().First().Workspace.RootPath.FullName;
            }

            foreach (var file in menuSelectionProvider.GetSelection<WorkspaceFile>())
            {
                objectPaths.Add("\"" + file.FileInfo.FullName + "\"");
            }

            foreach (var folder in menuSelectionProvider.GetSelection<WorkspaceFolder>())
            {
                objectPaths.Add("\"" + folder.DirectoryInfo.FullName.TrimEnd('\\') + "\"");
            }

            if (workspacePath != string.Empty && objectPaths.Any() && UserInteraction.ShowYesNoDialog("Do you really want to restore the selected objects?" + Environment.NewLine + "This command will revert your local files to the state of last commit.", "Git Restore"))
            {
                var restoreProcess = Git.CreateGitProcess($"restore {string.Join(" ", objectPaths)}", workspacePath);

                string outputMessage;
                string errorMessage;

                var restoreSuccessful = Git.ExecuteGitCommand(restoreProcess, out outputMessage, out errorMessage, _tiaPortal);

                if (restoreSuccessful)
                {
                    UserInteraction.ShowOutputDialog("Git Restore", SystemIcons.Information.ToBitmap(), "Output message from Git restore command", outputMessage + Environment.NewLine + errorMessage);
                }
                else
                {
                    UserInteraction.ShowOutputDialog("Git Restore", SystemIcons.Error.ToBitmap(), "Error message from Git restore command", outputMessage + Environment.NewLine + errorMessage);
                }
            }
        }

        // GIT PUSH
        private static void GitPushClick(MenuSelectionProvider<WorkspaceFile, WorkspaceFolder> menuSelectionProvider)
        {
            var workspacePath = string.Empty;

            if (menuSelectionProvider.GetSelection<WorkspaceFile>().Any())
            {
                workspacePath = menuSelectionProvider.GetSelection<WorkspaceFile>().First().Workspace.RootPath.FullName;
            }
            else if (menuSelectionProvider.GetSelection<WorkspaceFolder>().Any())
            {
                workspacePath = menuSelectionProvider.GetSelection<WorkspaceFolder>().First().Workspace.RootPath.FullName;
            }

            if (workspacePath != string.Empty)
            {
                var pushProcess = Git.CreateGitProcess("push", workspacePath);

                string outputMessage;
                string errorMessage;

                var pushSuccessful = Git.ExecuteGitCommand(pushProcess, out outputMessage, out errorMessage, _tiaPortal);

                if (pushSuccessful)
                {
                    UserInteraction.ShowOutputDialog("Git Push", SystemIcons.Information.ToBitmap(), "Output message from Git push command", outputMessage + Environment.NewLine + errorMessage);
                }
                else
                {
                    UserInteraction.ShowOutputDialog("Git Push", SystemIcons.Error.ToBitmap(), "Error message from Git push command", outputMessage + Environment.NewLine + errorMessage);
                }
            }
        }

        // GIT PULL
        private static void GitPullClick(MenuSelectionProvider<WorkspaceFile, WorkspaceFolder> menuSelectionProvider)
        {
            var workspacePath = string.Empty;

            if (menuSelectionProvider.GetSelection<WorkspaceFile>().Any())
            {
                workspacePath = menuSelectionProvider.GetSelection<WorkspaceFile>().First().Workspace.RootPath.FullName;
            }
            else if (menuSelectionProvider.GetSelection<WorkspaceFolder>().Any())
            {
                workspacePath = menuSelectionProvider.GetSelection<WorkspaceFolder>().First().Workspace.RootPath.FullName;
            }

            if (workspacePath != string.Empty)
            {
                var pullProcess = Git.CreateGitProcess("pull", workspacePath);

                string outputMessage;
                string errorMessage;

                var pullSuccessful = Git.ExecuteGitCommand(pullProcess, out outputMessage, out errorMessage, _tiaPortal);

                if (pullSuccessful)
                {
                    UserInteraction.ShowOutputDialog("Git Pull", SystemIcons.Information.ToBitmap(), "Output message from Git pull command", outputMessage + Environment.NewLine + errorMessage);
                }
                else
                {
                    UserInteraction.ShowOutputDialog("Git Pull", SystemIcons.Error.ToBitmap(), "Error message from Git pull command", outputMessage + Environment.NewLine + errorMessage);
                }
            }
        }

        //GIT FREE COMMAND
        private static void GitFreeClick(MenuSelectionProvider<WorkspaceFile, WorkspaceFolder> menuSelectionProvider)
        {
            var workspacePath = string.Empty;
            var objectPaths = new List<string>();

            if (menuSelectionProvider.GetSelection<WorkspaceFile>().Any())
            {
                workspacePath = menuSelectionProvider.GetSelection<WorkspaceFile>().First().Workspace.RootPath.FullName;
            }
            else if (menuSelectionProvider.GetSelection<WorkspaceFolder>().Any())
            {
                workspacePath = menuSelectionProvider.GetSelection<WorkspaceFolder>().First().Workspace.RootPath.FullName;
            }

            foreach (var file in menuSelectionProvider.GetSelection<WorkspaceFile>())
            {
                objectPaths.Add("\"" + file.FileInfo.FullName + "\"");
            }

            foreach (var folder in menuSelectionProvider.GetSelection<WorkspaceFolder>())
            {
                objectPaths.Add("\"" + folder.DirectoryInfo.FullName.TrimEnd('\\') + "\"");
            }

            if (workspacePath != string.Empty && objectPaths.Any())
            {
                string gitCommand;

                if (UserInteraction.ShowInputDialog("Git Command", "Enter any git command WHITHOUT the 'git' word", string.Empty, out gitCommand))
                {
                    var commandProcess = Git.CreateGitProcess($"{gitCommand}", workspacePath);

                    string outputMessage;
                    string errorMessage;

                    var commandSuccessful = Git.ExecuteGitCommand(commandProcess, out outputMessage, out errorMessage, _tiaPortal);

                    if (commandSuccessful)
                    {
                        UserInteraction.ShowOutputDialog("Git Command", SystemIcons.Information.ToBitmap(), "Output message from Git free command", outputMessage + Environment.NewLine + errorMessage);
                    }
                    else
                    {
                        UserInteraction.ShowOutputDialog("Git Command", SystemIcons.Error.ToBitmap(), "Error message from Git free command", outputMessage + Environment.NewLine + errorMessage);
                    }
                }
            }
        }

        //GIT CLONE
        private static void GitCloneClick(MenuSelectionProvider<WorkspaceFile, WorkspaceFolder> menuSelectionProvider)
        {
            var workspacePath = string.Empty;
            var objectPaths = new List<string>();

            if (menuSelectionProvider.GetSelection<WorkspaceFile>().Any())
            {
                workspacePath = menuSelectionProvider.GetSelection<WorkspaceFile>().First().Workspace.RootPath.FullName;
            }
            else if (menuSelectionProvider.GetSelection<WorkspaceFolder>().Any())
            {
                workspacePath = menuSelectionProvider.GetSelection<WorkspaceFolder>().First().Workspace.RootPath.FullName;
            }

            foreach (var file in menuSelectionProvider.GetSelection<WorkspaceFile>())
            {
                objectPaths.Add("\"" + file.FileInfo.FullName + "\"");
            }

            foreach (var folder in menuSelectionProvider.GetSelection<WorkspaceFolder>())
            {
                objectPaths.Add("\"" + folder.DirectoryInfo.FullName.TrimEnd('\\') + "\"");
            }

            if (workspacePath != string.Empty && objectPaths.Any())
            {
                string cloneURL;

                if (UserInteraction.ShowInputDialog("Git Clone", "Enter the remote repository Address or URL", string.Empty, out cloneURL))
                {
                    var commandProcess = Git.CreateGitProcess($"clone {cloneURL}", workspacePath);

                    string outputMessage;
                    string errorMessage;

                    var commandSuccessful = Git.ExecuteGitCommand(commandProcess, out outputMessage, out errorMessage, _tiaPortal);

                    if (commandSuccessful)
                    {
                        UserInteraction.ShowOutputDialog("Git Command", SystemIcons.Information.ToBitmap(), "Output message from Git free command", outputMessage + Environment.NewLine + errorMessage);
                    }
                    else
                    {
                        UserInteraction.ShowOutputDialog("Git Command", SystemIcons.Error.ToBitmap(), "Error message from Git free command", outputMessage + Environment.NewLine + errorMessage);
                    }
                }
            }
        }

        //ARCHIVE AND GIT PUSH *.ZAP**
        private static void GitArchiveClick(MenuSelectionProvider<WorkspaceFile, WorkspaceFolder> menuSelectionProvider)
        {
            var workspacePath = string.Empty;
            var objectPaths = new List<string>();
            var exportPath = new DirectoryInfo("C:\\Users\\cassioli\\Desktop\\testArc");

            if (menuSelectionProvider.GetSelection<WorkspaceFile>().Any())
            {
                workspacePath = menuSelectionProvider.GetSelection<WorkspaceFile>().First().Workspace.RootPath.FullName;
            }
            else if (menuSelectionProvider.GetSelection<WorkspaceFolder>().Any())
            {
                workspacePath = menuSelectionProvider.GetSelection<WorkspaceFolder>().First().Workspace.RootPath.FullName;
            }

            foreach (var file in menuSelectionProvider.GetSelection<WorkspaceFile>())
            {
                objectPaths.Add("\"" + file.FileInfo.FullName + "\"");
                exportPath = file.FileInfo.Directory;
            }

            foreach (var folder in menuSelectionProvider.GetSelection<WorkspaceFolder>())
            {
                objectPaths.Add("\"" + folder.DirectoryInfo.FullName.TrimEnd('\\') + "\"");
                exportPath = folder.DirectoryInfo;
            }

            if (workspacePath != string.Empty && objectPaths.Any())
            {
                //Archive project
                Project project = _tiaPortal.Projects.First();

                string archiveName = project.Name;
                string archiveFullName = exportPath + "\\" + archiveName;

                try
                {
                    if (File.Exists(archiveFullName))
                    {
                        File.Delete(archiveFullName);
                    }
                    project.Save();
                    project.Archive(exportPath, archiveName, ProjectArchivationMode.Compressed);
                }
                catch (Exception ex)
                {
                    UserInteraction.ShowOutputDialog("Archive Project", SystemIcons.Error.ToBitmap(), "Error message from archiving project", ex.ToString() + Environment.NewLine);
                }

                //GIT ADD
                var archiveAddProcess = Git.CreateGitProcess("add " + archiveName, workspacePath);

                string outputMessage;
                string errorMessage;

                var archiveAddSuccessful = Git.ExecuteGitCommand(archiveAddProcess, out outputMessage, out errorMessage, _tiaPortal);

                if (archiveAddSuccessful)
                {
                    //GIT COMMIT
                    var archiveCommitProcess = Git.CreateGitProcess("commit -m \"Added archived project\"" + archiveName, workspacePath);
                    var archiveCommitSuccessful = Git.ExecuteGitCommand(archiveCommitProcess, out outputMessage, out errorMessage, _tiaPortal);
                    if (archiveCommitSuccessful)
                    {
                        //GIT PUSH
                        string remoteRepo;
                        if (UserInteraction.ShowInputDialog("Remote Settings", "Enter the remote repository for 'git push REMOTE-NAME master'", string.Empty, out remoteRepo))
                        {
                            var archivePushProcess = Git.CreateGitProcess("push " + remoteRepo + " master", workspacePath);
                            var archivePushSuccessful = Git.ExecuteGitCommand(archivePushProcess, out outputMessage, out errorMessage, _tiaPortal);
                            if (archivePushSuccessful)
                            {
                                UserInteraction.ShowOutputDialog("Archive and Push", SystemIcons.Information.ToBitmap(), "Project archived to", exportPath + Environment.NewLine + "And pushed also pushed successfully");
                            }
                            else
                            {
                                UserInteraction.ShowOutputDialog("Git Push", SystemIcons.Error.ToBitmap(), "Error message from Git push command", outputMessage + Environment.NewLine + errorMessage);
                            }
                        }
                    }
                    else
                    {
                        UserInteraction.ShowOutputDialog("Git Commit", SystemIcons.Error.ToBitmap(), "Error message from Git commit archived command", outputMessage + Environment.NewLine + errorMessage);
                    }
                }
                else
                {
                    UserInteraction.ShowOutputDialog("Git Add", SystemIcons.Error.ToBitmap(), "Error message from Git add achived command", outputMessage + Environment.NewLine + errorMessage);
                }
            }
        }


        //EXPORT GIT LOG
        private static void GitExportClick(MenuSelectionProvider<WorkspaceFile, WorkspaceFolder> menuSelectionProvider)
        {
            var workspacePath = string.Empty;
            var objectPaths = new List<string>();
            string exportPath = "";

            if (menuSelectionProvider.GetSelection<WorkspaceFile>().Any())
            {
                workspacePath = menuSelectionProvider.GetSelection<WorkspaceFile>().First().Workspace.RootPath.FullName;
            }
            else if (menuSelectionProvider.GetSelection<WorkspaceFolder>().Any())
            {
                workspacePath = menuSelectionProvider.GetSelection<WorkspaceFolder>().First().Workspace.RootPath.FullName;
            }

            foreach (var file in menuSelectionProvider.GetSelection<WorkspaceFile>())
            {
                objectPaths.Add("\"" + file.FileInfo.FullName + "\"");
                exportPath = file.FileInfo.DirectoryName;
            }

            foreach (var folder in menuSelectionProvider.GetSelection<WorkspaceFolder>())
            {
                objectPaths.Add("\"" + folder.DirectoryInfo.FullName.TrimEnd('\\') + "\"");
                exportPath = folder.DirectoryInfo.FullName.TrimEnd('\\') + "\"";
            }

            if (workspacePath != string.Empty && objectPaths.Any())
            {
                var logProcess = Git.CreateGitProcess($"log", workspacePath);

                string outputMessage;
                string errorMessage;
                string htmlLine;

                var logSuccessful = Git.ExecuteGitCommand(logProcess, out outputMessage, out errorMessage, _tiaPortal);

                //spliting lines to rebuild later as html
                var splitedMessage = Regex.Split(outputMessage, "\r\n|\r|\n");

                if (logSuccessful)
                {
                    //{string.Join(" ", objectPaths)}
                    string path = exportPath + "\\gitLog.html";
                    try
                    {
                        // Create the file, or overwrite if the file exists.
                        using (FileStream fs = File.Create(path))
                        {
                            // Add the log to the file.
                            foreach (var line in splitedMessage)
                            {
                                if (line.Contains("commit"))
                                {

                                    htmlLine = "<h3>" + line + "</h3>";
                                }
                                else if (line.Contains("Author") || line.Contains("Date"))
                                {
                                    htmlLine = "<p>" + line + "</p>";
                                }
                                else if (line != "")
                                {
                                    htmlLine = "<p><strong>" + line + "</strong></p><br><hr style=\"border-top: 3px solid #bbb\">";
                                }
                                else
                                {
                                    htmlLine = line;
                                }
                                byte[] log = new UTF8Encoding(true).GetBytes(htmlLine);
                                fs.Write(log, 0, log.Length);
                            }
                        }
                        UserInteraction.ShowOutputDialog("Export Git Log", SystemIcons.Information.ToBitmap(), "File Successfully Exported to", path + Environment.NewLine + errorMessage);
                    }

                    catch (Exception ex)
                    {
                        UserInteraction.ShowOutputDialog("Export Git Log", SystemIcons.Error.ToBitmap(), "Error message from exporting file", ex.ToString() + Environment.NewLine + errorMessage);
                    }
                }
                else
                {
                    UserInteraction.ShowOutputDialog("Export Git Log", SystemIcons.Error.ToBitmap(), "Error message from Git log command", outputMessage + Environment.NewLine + errorMessage);
                }
            }
        }

        //EXPORT GIT IGNORE
        private static void GitIgnoreClick(MenuSelectionProvider<WorkspaceFile, WorkspaceFolder> menuSelectionProvider)
        {
            var workspacePath = string.Empty;
            var objectPaths = new List<string>();
            string exportPath = "";
            string ignoredItem = "";

            if (menuSelectionProvider.GetSelection<WorkspaceFile>().Any())
            {
                workspacePath = menuSelectionProvider.GetSelection<WorkspaceFile>().First().Workspace.RootPath.FullName;
            }
            else if (menuSelectionProvider.GetSelection<WorkspaceFolder>().Any())
            {
                workspacePath = menuSelectionProvider.GetSelection<WorkspaceFolder>().First().Workspace.RootPath.FullName;
            }

            foreach (var file in menuSelectionProvider.GetSelection<WorkspaceFile>())
            {
                objectPaths.Add("\"" + file.FileInfo.FullName + "\"");
                exportPath = file.FileInfo.DirectoryName;
                ignoredItem = file.FileInfo.Name;
            }

            foreach (var folder in menuSelectionProvider.GetSelection<WorkspaceFolder>())
            {
                objectPaths.Add("\"" + folder.DirectoryInfo.FullName.TrimEnd('\\') + "\"");
                exportPath = folder.DirectoryInfo.Parent.FullName;
                ignoredItem = folder.DirectoryInfo.Name + "/";
            }

            if (workspacePath != string.Empty && objectPaths.Any())
            {
                string outputMessage;
                string errorMessage;

                //{string.Join(" ", objectPaths)}
                string path = exportPath + "\\.gitignore";
                if (!File.Exists(path))
                {
                    try
                    {
                        // Create the file, or overwrite if the file exists.
                        using (FileStream fs = File.Create(path))
                        {
                            byte[] file = new UTF8Encoding(true).GetBytes(ignoredItem);
                            fs.Write(file, 0, file.Length);
                        }

                        UserInteraction.ShowOutputDialog("Git .ignore", SystemIcons.Information.ToBitmap(), ignoredItem + " Ignored", ignoredItem+" will no longer be watched by git" + Environment.NewLine);
                    }

                    catch (Exception ex)
                    {
                        UserInteraction.ShowOutputDialog("Git .ignore", SystemIcons.Error.ToBitmap(), "Error message from creating the .ignore file", ex.ToString() + Environment.NewLine);
                    }
                }
                else
                {
                    try
                    {
                        File.AppendAllText(path, Environment.NewLine + ignoredItem);
                        UserInteraction.ShowOutputDialog("Git .ignore", SystemIcons.Information.ToBitmap(), ignoredItem + " Ignored", ignoredItem + " will no longer be watched by git" + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {
                        UserInteraction.ShowOutputDialog("Git .ignore", SystemIcons.Error.ToBitmap(), "Error message from writing in the .ignore file", ex.ToString() + Environment.NewLine);
                    }
                }
            }
        }
    


        //EXPORT GIT INIT
        private static void GitInitClick(MenuSelectionProvider<WorkspaceFile, WorkspaceFolder> menuSelectionProvider)
        {
            var workspacePath = string.Empty;
            var objectPaths = new List<string>();

            if (menuSelectionProvider.GetSelection<WorkspaceFile>().Any())
            {
                workspacePath = menuSelectionProvider.GetSelection<WorkspaceFile>().First().Workspace.RootPath.FullName;
            }
            else if (menuSelectionProvider.GetSelection<WorkspaceFolder>().Any())
            {
                workspacePath = menuSelectionProvider.GetSelection<WorkspaceFolder>().First().Workspace.RootPath.FullName;
            }

            foreach (var file in menuSelectionProvider.GetSelection<WorkspaceFile>())
            {
                objectPaths.Add("\"" + file.FileInfo.FullName + "\"");
            }

            foreach (var folder in menuSelectionProvider.GetSelection<WorkspaceFolder>())
            {
                objectPaths.Add("\"" + folder.DirectoryInfo.FullName.TrimEnd('\\') + "\"");
            }

            if (workspacePath != string.Empty && objectPaths.Any())
            {
                var initProcess = Git.CreateGitProcess($"init", workspacePath);

                string outputInitMessage;
                string outputMessage;
                string errorMessage;

                var initSuccessful = Git.ExecuteGitCommand(initProcess, out outputInitMessage, out errorMessage, _tiaPortal);

                if (initSuccessful)
                {
                    //Git Config user.name
                    string configName;
                    string configEmail;
                    if (UserInteraction.ShowInputDialog("Git Config", "Enter the user.name", string.Empty, out configName))
                    {
                        initProcess = Git.CreateGitProcess($"config user.name \"{configName}\"", workspacePath);

                        var configNameSuccessful = Git.ExecuteGitCommand(initProcess, out outputMessage, out errorMessage, _tiaPortal);
                        if (configNameSuccessful)
                        {
                            //Git Config user.email
                            if (UserInteraction.ShowInputDialog("Git Config", "Enter the user.email", string.Empty, out configEmail))
                            {
                                initProcess = Git.CreateGitProcess($"config user.email \"{configEmail}\"", workspacePath);

                                var configEmailSuccessful = Git.ExecuteGitCommand(initProcess, out outputMessage, out errorMessage, _tiaPortal);
                                if (!configEmailSuccessful)
                                {
                                    UserInteraction.ShowOutputDialog("Git Config user.email", SystemIcons.Error.ToBitmap(), "Error message from Git config command", outputMessage + Environment.NewLine + errorMessage);
                                }
                                else
                                {
                                    UserInteraction.ShowOutputDialog("Git Init and Config", SystemIcons.Information.ToBitmap(), "Output message from successful init command", outputInitMessage + Environment.NewLine + errorMessage);
                                }
                            }
                        }
                        else
                        {
                            UserInteraction.ShowOutputDialog("Git Config user.name", SystemIcons.Error.ToBitmap(), "Error message from Git config command", outputMessage + Environment.NewLine + errorMessage);
                        }

                    }
                }
                else
                {
                    UserInteraction.ShowOutputDialog("Export Git Init", SystemIcons.Error.ToBitmap(), "Error message from Git init command", outputInitMessage + Environment.NewLine + errorMessage);
                }
            }
        }

        //GIT REMOTE ADD ORIGIN
        private static void GitOriginClick(MenuSelectionProvider<WorkspaceFile, WorkspaceFolder> menuSelectionProvider)
        {
            var workspacePath = string.Empty;
            var objectPaths = new List<string>();

            if (menuSelectionProvider.GetSelection<WorkspaceFile>().Any())
            {
                workspacePath = menuSelectionProvider.GetSelection<WorkspaceFile>().First().Workspace.RootPath.FullName;
            }
            else if (menuSelectionProvider.GetSelection<WorkspaceFolder>().Any())
            {
                workspacePath = menuSelectionProvider.GetSelection<WorkspaceFolder>().First().Workspace.RootPath.FullName;
            }

            foreach (var file in menuSelectionProvider.GetSelection<WorkspaceFile>())
            {
                objectPaths.Add("\"" + file.FileInfo.FullName + "\"");
            }

            foreach (var folder in menuSelectionProvider.GetSelection<WorkspaceFolder>())
            {
                objectPaths.Add("\"" + folder.DirectoryInfo.FullName.TrimEnd('\\') + "\"");
            }

            if (workspacePath != string.Empty && objectPaths.Any())
            {
                string gitOrigin;

                if (UserInteraction.ShowInputDialog("Git Remote Add Origin", "Enter the URL of the Origin to be added", string.Empty, out gitOrigin))
                {
                    var commandProcess = Git.CreateGitProcess($"remote add origin \"{gitOrigin}\"", workspacePath);

                    string outputMessage;
                    string errorMessage;

                    var commandSuccessful = Git.ExecuteGitCommand(commandProcess, out outputMessage, out errorMessage, _tiaPortal);

                    if (commandSuccessful)
                    {
                        UserInteraction.ShowOutputDialog("Git Remote Add Origin", SystemIcons.Information.ToBitmap(), "Output message from Git remote add origin", outputMessage + Environment.NewLine + errorMessage);
                    }
                    else
                    {
                        commandProcess = Git.CreateGitProcess($"remote set-url origin \"{gitOrigin}\"", workspacePath);
                        commandSuccessful = Git.ExecuteGitCommand(commandProcess, out outputMessage, out errorMessage, _tiaPortal);
                        if (commandSuccessful)
                        {
                            UserInteraction.ShowOutputDialog("Git Remote Add Origin", SystemIcons.Information.ToBitmap(), "Output message from Git remote add origin", outputMessage + Environment.NewLine + errorMessage);
                        }
                        else
                        {
                            UserInteraction.ShowOutputDialog("Git Remote Add Origin", SystemIcons.Error.ToBitmap(), "Error message from Git remote add origin", outputMessage + Environment.NewLine + errorMessage);
                        }
                    }
                }
            }
        }

    }
}