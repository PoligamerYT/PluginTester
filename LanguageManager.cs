using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PluginTester
{
    public class LanguageManager
    {
        public string LanguageName { get; set; }
        public string OSNotSupportedMessage { get; set; }
        public string UnableToCheckUpdateMessage { get; set; }
        public string NewUpdateMessage { get; set; }
        public string SelectJavaTitle { get; set; }
        public string JavaArgumentsTitle { get; set; }
        public string VersionTitle { get; set; }
        public string ServerTypeTitle { get; set; }
        public string DownloadProgressTitle { get; set; }
        public string DeletePluginsFolderTitle { get; set; }
        public string TestTitle { get; set; }
        public string LanguageTitle { get; set; }
        public string NotSelectedJavaMessage { get; set; }
        public string VersionListErrorMessage { get; set; }

        [JsonIgnore]
        public Utils Utils { get; set; }

        public LanguageManager()
        {
            Utils = new Utils();
        }

        public bool LoadLanguageFromName(string languageName, LanguageManager originalLanguage)
        {
            string directoryPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            List<string> files = Directory.GetFiles(Utils.LanguagesPath).ToList();
            foreach (var file in files)
            {
                try
                {
                    string json = File.ReadAllText(file);
                    LanguageManager language = JsonConvert.DeserializeObject<LanguageManager>(json);

                    if(language.LanguageName == languageName)
                    {
                        originalLanguage.LanguageName = language.LanguageName;
                        originalLanguage.OSNotSupportedMessage = language.OSNotSupportedMessage;
                        originalLanguage.UnableToCheckUpdateMessage = language.UnableToCheckUpdateMessage;
                        originalLanguage.NewUpdateMessage = language.NewUpdateMessage;
                        originalLanguage.SelectJavaTitle = language.SelectJavaTitle;
                        originalLanguage.JavaArgumentsTitle = language.JavaArgumentsTitle;
                        originalLanguage.VersionTitle = language.VersionTitle;
                        originalLanguage.ServerTypeTitle = language.ServerTypeTitle;
                        originalLanguage.DownloadProgressTitle = language.DownloadProgressTitle;
                        originalLanguage.TestTitle = language.TestTitle;
                        originalLanguage.LanguageTitle = language.LanguageTitle;
                        originalLanguage.DeletePluginsFolderTitle = language.DeletePluginsFolderTitle;
                        originalLanguage.NotSelectedJavaMessage = language.NotSelectedJavaMessage;
                        originalLanguage.VersionListErrorMessage = language.VersionListErrorMessage;

                        return true;
                    }
                }
                catch { }
            }

            return false;
        }

        public bool LoadFirstLanguage(LanguageManager originalLanguage)
        {
            string directoryPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            List<string> files = Directory.GetFiles(Utils.LanguagesPath).ToList();
            foreach (var file in files)
            {
                try
                {
                    string json = File.ReadAllText(file);
                    LanguageManager language = JsonConvert.DeserializeObject<LanguageManager>(json);

                    originalLanguage.LanguageName = language.LanguageName;
                    originalLanguage.OSNotSupportedMessage = language.OSNotSupportedMessage;
                    originalLanguage.UnableToCheckUpdateMessage = language.UnableToCheckUpdateMessage;
                    originalLanguage.NewUpdateMessage = language.NewUpdateMessage;
                    originalLanguage.SelectJavaTitle = language.SelectJavaTitle;
                    originalLanguage.JavaArgumentsTitle = language.JavaArgumentsTitle;
                    originalLanguage.VersionTitle = language.VersionTitle;
                    originalLanguage.ServerTypeTitle = language.ServerTypeTitle;
                    originalLanguage.DownloadProgressTitle = language.DownloadProgressTitle;
                    originalLanguage.TestTitle = language.TestTitle;
                    originalLanguage.LanguageTitle = language.LanguageTitle;
                    originalLanguage.DeletePluginsFolderTitle = language.DeletePluginsFolderTitle;
                    originalLanguage.NotSelectedJavaMessage = language.NotSelectedJavaMessage;
                    originalLanguage.VersionListErrorMessage = language.VersionListErrorMessage;

                    return true;
                }
                catch { }
            }

            return false;
        }

        public List<string> GetLanguages() 
        {
            string directoryPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            List<string> files = Directory.GetFiles(Utils.LanguagesPath).ToList();

            List<string> names = new List<string>();

            foreach (var file in files)
            {
                try
                {
                    string json = File.ReadAllText(file);
                    LanguageManager language = JsonConvert.DeserializeObject<LanguageManager>(json);

                    names.Add(language.LanguageName);
                }
                catch { }
            }

            return names;
        }
    }
}
