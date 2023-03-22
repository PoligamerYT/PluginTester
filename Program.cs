using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace PluginTester
{
    internal static class Program
    {
        public static double Version = 1.1;
        public static LanguageManager LanguageManager;
        public static string Path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                LanguageManager = new LanguageManager();

                FirstLanguageCheck();

                if (File.Exists(System.IO.Path.Combine(Path, "Config.json")))
                {
                    using (StreamReader sr = File.OpenText(System.IO.Path.Combine(Path, "Config.json")))
                    {
                        string json = sr.ReadToEnd();

                        Configuration configuration = JsonConvert.DeserializeObject<Configuration>(json);

                        if (!LanguageManager.LoadLanguageFromName(configuration.SelectedLanguage)) 
                        {
                            FirstLanguageCheck();
                        }

                        sr.Close();
                    }
                }

                Thread thread = new Thread(() =>
                {
                    try
                    {
                        var url = "https://raw.githubusercontent.com/PoligamerYT/PluginTester/master/LastVersion.txt";
                        double version = 0;

                        using (WebClient webClient = new WebClient()) 
                        {
                            byte[] raw = webClient.DownloadData(url);
                            version = Convert.ToDouble(System.Text.Encoding.UTF8.GetString(raw));
                        }

                        if (!version.Equals(Version))
                        {
                            MessageBoxManager.OK = "Visit";
                            MessageBoxManager.Cancel = "Ok";
                            MessageBoxManager.Register();
                            DialogResult result = MessageBox.Show(LanguageManager.NewUpdateMessage.Replace("<url>", "https://www.spigotmc.org/resources/plugin-tester.108280/"), "Update Cheker", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                            if (result == DialogResult.OK) 
                            {
                                Process.Start("https://www.spigotmc.org/resources/plugin-tester.108280/");
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
                Application.Run(new Form1(LanguageManager));
            }
            else
            {
                MessageBox.Show(LanguageManager.OSNotSupportedMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void SetupDefaultLanguage(LanguageManager language)
        {
            language.LanguageName = "English";
            language.NewUpdateMessage = $"There is a new update dowload it from here <url>";
            language.OSNotSupportedMessage = "Your current OS is not supported!";
            language.UnableToCheckUpdateMessage = "Unable to chek for updates: <error>";
            language.SelectJavaTitle = "Select Java";
            language.JavaArgumentsTitle = "Java Arguments";
            language.VersionTitle = "Version";
            language.ServerTypeTitle = "Server Type";
            language.DowloadProgressTitle = "Dowload Progress";
            language.DeletePluginsFolderTitle = "Delete Plugins Folder";
            language.TestTitle = "Test";
            language.LanguageTitle = "Language";
            language.NotSelectedJavaMessage = "Select Java First Please";
        }

        public static void FirstLanguageCheck()
        {
            if (Directory.Exists(System.IO.Path.Combine(Path, "Languages")))
            {
                if (Directory.GetFiles(System.IO.Path.Combine(Path, "Languages")).ToList().Count == 0)
                {
                    LanguageManager language = new LanguageManager();

                    SetupDefaultLanguage(language);

                    string json = JsonConvert.SerializeObject(language, Formatting.Indented);

                    File.WriteAllText(System.IO.Path.Combine(Path, "Languages", language.LanguageName + ".json"), json);

                    LanguageManager = language;
                }
                else
                {
                    if (LanguageManager == null)
                    {
                        if (LanguageManager.LoadFirstLanguage()) { }
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(System.IO.Path.Combine(Path, "Languages"));

                LanguageManager language = new LanguageManager();

                SetupDefaultLanguage(language);

                string json = JsonConvert.SerializeObject(language, Formatting.Indented);

                File.WriteAllText(System.IO.Path.Combine(Path, "Languages", language.LanguageName + ".json"), json);

                LanguageManager = language;
            }
        }
    }
}
