using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Threading;
using System.Windows.Forms;

namespace PluginTester
{
    internal static class Program
    {
        public static readonly Version Version = new Version(1, 6);
        public static LanguageManager LanguageManager;
        public static Utils Utils;

        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Utils = new Utils();
                LanguageManager = new LanguageManager();

                FirstLanguageCheck();

                if (File.Exists(Utils.ConfigPath))
                {
                    string json = File.ReadAllText(Utils.ConfigPath);

                    Configuration configuration = Utils.JsonToObject<Configuration>(json);

                    if (!LanguageManager.LoadLanguageFromName(configuration.SelectedLanguage, LanguageManager))
                    {
                        FirstLanguageCheck();
                    }
                }

                Thread thread = new Thread(() =>
                {
                    try
                    {
                        Version latestVersion;
                        using (WebClient webClient = new WebClient())
                        {
                            byte[] raw = webClient.DownloadData(URL.LastVersionUrl);
                            string versionString = System.Text.Encoding.UTF8.GetString(raw);
                            latestVersion = Version.Parse(versionString);
                        }

                        if (latestVersion.CompareTo(Version) > 0)
                        {
                            MessageBoxManager.OK = "Visit";
                            MessageBoxManager.Cancel = "Ok";
                            MessageBoxManager.Register();
                            DialogResult result = MessageBox.Show(LanguageManager.NewUpdateMessage.Replace("<url>", URL.ResourceUrl), "Update Cheker", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                            if (result == DialogResult.OK) 
                            {
                                Process.Start(URL.ResourceUrl);
                            }
                            MessageBoxManager.Unregister();
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(LanguageManager.UnableToCheckUpdateMessage.Replace("<error>", e.Message), "Update Cheker", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                });
                thread.Start();
                thread.Join();

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Main(LanguageManager, Utils));
            }
            else
            {
                MessageBox.Show(LanguageManager.OSNotSupportedMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void SetupDefaultLanguage(LanguageManager language)
        {
            language.LanguageName = "English";
            language.NewUpdateMessage = $"There is a new update download it from here <url>";
            language.OSNotSupportedMessage = "Your current OS is not supported!";
            language.UnableToCheckUpdateMessage = "Unable to check for updates: <error>";
            language.SelectJavaTitle = "Select Java";
            language.JavaArgumentsTitle = "Java Arguments";
            language.VersionTitle = "Version";
            language.ServerTypeTitle = "Server Type";
            language.DownloadProgressTitle = "Download Progress";
            language.DeletePluginsFolderTitle = "Delete Plugins Folder";
            language.TestTitle = "Test";
            language.LanguageTitle = "Language";
            language.NotSelectedJavaMessage = "Select Java First Please";
            language.VersionListErrorMessage = "Cant contact with the server pls try later!";
        }

        public static void FirstLanguageCheck()
        {
            if (Directory.Exists(Utils.LanguagesPath))
            {
                if (Directory.GetFiles(Utils.LanguagesPath).ToList().Count == 0)
                {
                    LanguageManager language = new LanguageManager();

                    SetupDefaultLanguage(language);

                    string json = JsonConvert.SerializeObject(language, Formatting.Indented);

                    File.WriteAllText(Path.Combine(Utils.LanguagesPath, language.LanguageName + ".json"), json);

                    LanguageManager = language;
                }
                else
                {
                    LanguageManager.LoadFirstLanguage(LanguageManager);
                }
            }
            else
            {
                Directory.CreateDirectory(Utils.LanguagesPath);

                LanguageManager language = new LanguageManager();

                SetupDefaultLanguage(language);

                string json = JsonConvert.SerializeObject(language, Formatting.Indented);

                File.WriteAllText(Path.Combine(Utils.LanguagesPath, language.LanguageName + ".json"), json);

                LanguageManager = language;
            }
        }
    }
}
