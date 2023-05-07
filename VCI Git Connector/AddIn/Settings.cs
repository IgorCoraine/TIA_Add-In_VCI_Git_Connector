using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using Siemens.Engineering.AddIn.Menu;
using Siemens.Engineering.AddIn.VersionControl;

namespace Siemens.Applications.AddIns.VCIGitConnector.AddIn
{
    [XmlRoot]
    [XmlType]
    public class Settings
    {
        public bool CommitOnSync { get; set; }
        public bool PushOnSync { get; set; }

        private static readonly string SettingsFilePath;

        static Settings()
        {
            var settingsDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TIA Add-Ins", Assembly.GetCallingAssembly().GetName().Name, Assembly.GetCallingAssembly().GetName().Version.ToString());
            var settingsDirectory = Directory.CreateDirectory(settingsDirectoryPath);
            SettingsFilePath = Path.Combine(settingsDirectory.FullName, string.Concat(typeof(Settings).Name, ".xml"));
        }

        public Settings()
        {
            CommitOnSync = true;
            PushOnSync = false;
        }

        public static Settings Load()
        {
            if (File.Exists(SettingsFilePath) == false)
            {
                return new Settings();
            }

            using (FileStream readStream = new FileStream(SettingsFilePath, FileMode.Open))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                return serializer.Deserialize(readStream) as Settings;
            }
        }

        public void Save()
        {
            using (FileStream writeStream = new FileStream(SettingsFilePath, FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                serializer.Serialize(writeStream, this);
            }
        }

        internal void GitCommitOnSyncClick(MenuSelectionProvider<WorkspaceFile, WorkspaceFolder> menuSelectionProvider)
        {
            CommitOnSync = !CommitOnSync;
            Save();
        }
        internal MenuStatus GitCommitOnSyncStatus(MenuSelectionProvider<WorkspaceFile, WorkspaceFolder> menuSelectionProvider, CheckBoxActionItemStyle checkBoxStyle)
        {
            checkBoxStyle.State = CommitOnSync == true ? CheckBoxState.Checked : CheckBoxState.Unchecked;
            return MenuStatus.Enabled;
        }

        internal void GitPushOnSyncClick(MenuSelectionProvider<WorkspaceFile, WorkspaceFolder> menuSelectionProvider)
        {
            PushOnSync = !PushOnSync;
            Save();
        }
        internal MenuStatus GitPushOnSyncStatus(MenuSelectionProvider<WorkspaceFile, WorkspaceFolder> menuSelectionProvider, CheckBoxActionItemStyle checkBoxStyle)
        {
            checkBoxStyle.State = PushOnSync == true ? CheckBoxState.Checked : CheckBoxState.Unchecked;
            return MenuStatus.Enabled;
        }
    }
}
