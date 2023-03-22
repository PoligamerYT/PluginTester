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
        public string DowloadProgressTitle { get; set; }
        public string DeletePluginsFolderTitle { get; set; }
        public string TestTitle { get; set; }
        public string LanguageTitle { get; set; }
        public string NotSelectedJavaMessage { get; set; }

        public bool LoadLanguageFromName(string languageName)
        {
            string directoryPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            List<string> files = Directory.GetFiles(Path.Combine(directoryPath, "Languages")).ToList();
            foreach (var file in files)
            {
                try
                {
                    string json = File.ReadAllText(file);
                    LanguageManager language = JsonConvert.DeserializeObject<LanguageManager>(json);

                    if(language.LanguageName == languageName)
                    {
                        this.LanguageName = language.LanguageName;
                        this.OSNotSupportedMessage = language.OSNotSupportedMessage;
                        this.UnableToCheckUpdateMessage = language.NewUpdateMessage;
                        this.NewUpdateMessage = language.NewUpdateMessage;
                        this.SelectJavaTitle = language.SelectJavaTitle;
                        this.JavaArgumentsTitle = language.JavaArgumentsTitle;
                        this.VersionTitle = language.VersionTitle;
                        this.ServerTypeTitle = language.ServerTypeTitle;
                        this.DowloadProgressTitle = language.DowloadProgressTitle;
                        this.TestTitle = language.TestTitle;
                        this.LanguageTitle = language.LanguageTitle;
                        this.DeletePluginsFolderTitle = language.DeletePluginsFolderTitle;
                        this.NotSelectedJavaMessage = language.NotSelectedJavaMessage;

                        return true;
                    }
                }
                catch { }
            }

            return false;
        }

        public bool LoadFirstLanguage()
        {
            string directoryPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            List<string> files = Directory.GetFiles(Path.Combine(directoryPath, "Languages")).ToList();
            foreach (var file in files)
            {
                try
                {
                    string json = File.ReadAllText(file);
                    LanguageManager language = JsonConvert.DeserializeObject<LanguageManager>(json);

                    this.LanguageName = language.LanguageName;
                    this.OSNotSupportedMessage = language.OSNotSupportedMessage;
                    this.UnableToCheckUpdateMessage = language.NewUpdateMessage;
                    this.NewUpdateMessage = language.NewUpdateMessage;
                    this.SelectJavaTitle = language.SelectJavaTitle;
                    this.JavaArgumentsTitle = language.JavaArgumentsTitle;
                    this.VersionTitle = language.VersionTitle;
                    this.ServerTypeTitle = language.ServerTypeTitle;
                    this.DowloadProgressTitle = language.DowloadProgressTitle;
                    this.TestTitle = language.TestTitle;
                    this.LanguageTitle = language.LanguageTitle;
                    this.DeletePluginsFolderTitle = language.DeletePluginsFolderTitle;
                    this.NotSelectedJavaMessage = language.NotSelectedJavaMessage;

                    return true;
                }
                catch { }
            }

            return false;
        }

        public List<string> GetLanguages() 
        {
            string directoryPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            List<string> files = Directory.GetFiles(Path.Combine(directoryPath, "Languages")).ToList();

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
