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
            addInRootSubmenu.Items.AddActionItem<WorkspaceFile, WorkspaceFolder>("Status", GitStatusClick);
            addInRootSubmenu.Items.AddActionItem<WorkspaceFile, WorkspaceFolder>("Reset", GitResetClick);
            addInRootSubmenu.Items.AddActionItem<WorkspaceFile, WorkspaceFolder>("Restore", GitRestoreClick);
            addInRootSubmenu.Items.AddActionItem<WorkspaceFile, WorkspaceFolder>("Push", GitPushClick);
            addInRootSubmenu.Items.AddActionItem<WorkspaceFile, WorkspaceFolder>("Pull", GitPullClick);
            addInRootSubmenu.Items.AddActionItem<WorkspaceFile, WorkspaceFolder>("FreeCmd", GitFreeClick); //Added Item
            //ignore
            //archieve and push
            //clone from remote
            addInRootSubmenu.Items.AddActionItem<WorkspaceFile, WorkspaceFolder>("Export Log", GitExportClick); //Added Item
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
                var statusProcess = Git.CreateGitProcess($"status {string.Join(" ", objectPaths)}", workspacePath);

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
                    string path = exportPath+"\\gitLog.html";
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
                                } else if (line.Contains("Author")||line.Contains("Date"))
                                {
                                    htmlLine = "<p>" + line + "</p>";
                                } else if(line!="")
                                {
                                    htmlLine = "<p><strong>" + line + "</strong></p><br><hr style=\"border-top: 3px solid #bbb\">";
                                } else
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
    }
}