using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace PluginTester
{
    public partial class Form1 : Form
    {
        public string JavaPath = string.Empty;
        public bool Loading = false;
        public string BashPath = string.Empty;
        public LanguageManager LanguageManager;
        public Utils Utils;
        public VersionList VersionList;

        public Form1(LanguageManager language, Utils utils)
        {
            InitializeComponent();

            LanguageManager = language;
            Utils = utils;  
        }

        #region Functions

        private void OnLoadForm(object sender, EventArgs e)
        {
            if (LoadLanguage(LanguageManager.LanguageName))
            {
                if (!Directory.Exists(Utils.PluginsPath))
                {
                    Directory.CreateDirectory(Utils.PluginsPath);
                }

                if (!Directory.Exists(Utils.ServersPath))
                {
                    Directory.CreateDirectory(Utils.ServersPath);
                }

                if (!Directory.Exists(Utils.DataPath))
                {
                    Directory.CreateDirectory(Utils.DataPath);
                }

                if (!File.Exists(Utils.ConfigPath))
                {
                    Configuration configuration = new Configuration();
                    configuration.JavaPath = JavaPath;
                    configuration.JavaArgument = textBox1.Text;
                    configuration.SelectedVersion = comboBox2.SelectedIndex;
                    configuration.SelectedServerType = comboBox1.SelectedIndex;
                    configuration.SelectedLanguage = comboBox3.Text;

                    string json = Utils.ObjectToJson(configuration);

                    File.WriteAllText(Utils.ConfigPath, json);
                }
                else
                {
                    string json = File.ReadAllText(Utils.ConfigPath);

                    Configuration configuration = Utils.JsonToObject<Configuration>(json);

                    JavaPath = configuration.JavaPath;
                    textBox1.Text = configuration.JavaArgument;
                    comboBox2.SelectedIndex = configuration.SelectedVersion;
                    comboBox1.SelectedIndex = configuration.SelectedServerType;
                    comboBox3.SelectedItem = configuration.SelectedLanguage;
                }

                PrepareForm();
            }
        }

        public void PrepareForm()
        {
            foreach (string name in LanguageManager.GetLanguages())
            {
                comboBox3.Items.Add(name);
            }

            if (CheckVersionsList())
            {
                comboBox2.Items.Clear();

                foreach (string versionName in VersionList.Versions)
                {
                    comboBox2.Items.Add(versionName);
                }

                this.Focus();
                this.Activate();
                this.BringToFront();

                tabControl1.Appearance = TabAppearance.FlatButtons;
                tabControl1.ItemSize = new Size(0, 1);
                tabControl1.SizeMode = TabSizeMode.Fixed;
                this.Size = new Size(this.Size.Width, this.Size.Height + 10);

                comboBox2.SelectedIndex = 0;
                comboBox1.SelectedIndex = 0;
                comboBox3.SelectedIndex = 0;
            }
            else
            {
                Application.Exit();
            }
        }

        public void PrepareServer()
        {
            Loading = true;

            try
            {
                textBox1.Enabled = false;
                comboBox1.Enabled = false;
                comboBox2.Enabled = false;
                button1.Enabled = false;
                button2.Enabled = false;  
                button3.Enabled = false;
                
                if(!Directory.Exists(Path.Combine(Utils.ServersPath, $"{comboBox1.Text}-{comboBox2.Text}"))) 
                {
                    Directory.CreateDirectory(Path.Combine(Utils.ServersPath, $"{comboBox1.Text}-{comboBox2.Text}"));

                    WebClient wc = new WebClient();

                    switch (comboBox1.SelectedIndex)
                    {
                        case 0:
                            DowloadVersion(wc, comboBox1.Text, comboBox2.Text);
                            break;
                        case 1:
                            DowloadVersion(wc, comboBox1.Text, comboBox2.Text);
                            break;
                        case 2:
                            DowloadPaperVersion(wc, comboBox2.Text);
                            break;
                    }
                }
                else
                {
                    if(!File.Exists(Path.Combine(Utils.ServersPath, $"{comboBox1.Text}-{comboBox2.Text}", $"{comboBox1.Text.ToLower()}-{comboBox2.Text}.jar")))
                    {
                        Directory.Delete(Path.Combine(Utils.ServersPath, $"{comboBox1.Text}-{comboBox2.Text}"), true);
                        PrepareServer();
                        return;
                    }
                    else
                    {
                        SetupFiles(Path.Combine(Utils.ServersPath, $"{comboBox1.Text}-{comboBox2.Text}"), $"{comboBox1.Text.ToLower()}-{comboBox2.Text}.jar");

                        StartServer();
                    }
                }
            }
            catch(Exception ex) 
            {
                Console.WriteLine(ex.ToString());
                textBox1.Enabled = true;
                comboBox1.Enabled = true;
                comboBox2.Enabled = true;
                button1.Enabled = true;
                button2.Enabled = true;
                Loading = false;
            }
        }

        public void StartServer()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                string filePathExecute = Path.Combine(Utils.ServersPath, $"{comboBox1.Text}-{comboBox2.Text}", "Start.sh");
                FileInfo fileInfo = new FileInfo(filePathExecute);
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.CreateNoWindow = false;
                startInfo.UseShellExecute = false;
                startInfo.FileName = "/bin/bash";
                startInfo.Arguments = $"\"{fileInfo.FullName}\"";
                Process process = Process.Start(startInfo);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(Path.Combine(Utils.ServersPath, $"{comboBox1.Text}-{comboBox2.Text}", "Start.bat"));
            }

            textBox1.Enabled = true;
            comboBox1.Enabled = true;
            comboBox2.Enabled = true;
            button1.Enabled = true;
            button2.Enabled = true;
            Loading = false;
        }

        public void SetupFiles(string directory, string version)
        {
            if(!Directory.Exists(Utils.PluginsPath)) 
            {
                Directory.CreateDirectory(Utils.PluginsPath);

                CopyAll(new DirectoryInfo(Utils.PluginsPath), new DirectoryInfo(Utils.PluginsPath));
            }
            
            if(!File.Exists(Path.Combine(directory, "eula.txt")))
            {
                File.WriteAllText(Path.Combine(directory, "eula.txt"), "eula=true");
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (!File.Exists(Path.Combine(directory, "Start.sh")))
                {
                    string first = textBox1.Text.Replace("<Java>", $"\"{JavaPath}\"");
                    string second = first.Replace("<Jar>", version);

                    File.WriteAllText(Path.Combine(directory, "Start.sh"), $"#!/bin/bash{Environment.NewLine}cd \"{directory}\"{Environment.NewLine}{second}");
                }
                else
                {
                    string first = textBox1.Text.Replace("<Java>", $"\"{JavaPath}\"");
                    string second = first.Replace("<Jar>", version);

                    File.WriteAllText(Path.Combine(directory, "Start.sh"), $"#!/bin/bash{Environment.NewLine}cd \"{directory}\"{Environment.NewLine}{second}");
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (!File.Exists(Path.Combine(directory, "Start.bat")))
                {
                    string first = textBox1.Text.Replace("<Java>", $"\"{JavaPath}\"");
                    string second = first.Replace("<Jar>", version);

                    File.WriteAllText(Path.Combine(directory, "Start.bat"), $"cd \"{directory}\" \r\n{second} \r\npause");
                }
                else if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    string first = textBox1.Text.Replace("<Java>", $"\"{JavaPath}\"");
                    string second = first.Replace("<Jar>", version);

                    File.WriteAllText(Path.Combine(directory, "Start.bat"), $"cd \"{directory}\" \r\n{second} \r\npause");
                }
            }
        }

        public bool LoadLanguage(string language)
        {
            if(LanguageManager.LoadLanguageFromName(language, LanguageManager))
            {
                label5.Text = LanguageManager.LanguageTitle;
                label1.Text = LanguageManager.JavaArgumentsTitle;
                label2.Text = LanguageManager.VersionTitle;
                label3.Text = LanguageManager.ServerTypeTitle;
                label4.Text = LanguageManager.DownloadProgressTitle;
                button2.Text = LanguageManager.SelectJavaTitle;
                button3.Text = LanguageManager.DeletePluginsFolderTitle;
                button1.Text = LanguageManager.TestTitle;

                return true;
            }
            else 
            {
                return false; 
            }
        }

        private void Wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            progressBar1.Value = 0;

            StartServer();
        }

        private void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        public void DowloadVersion(WebClient wc, string type, string version)
        {
            string versionName = version;

            if (float.Parse(version) <= 1.10)
            {
                versionName += "-R0.1-SNAPSHOT-latest";
            }

            string url = $"{URL.BukkitOrSpigotURL}{type.ToLower()}/{type.ToLower()}-{versionName}.jar";

            wc.DownloadProgressChanged += wc_DownloadProgressChanged;
            wc.DownloadFileCompleted += Wc_DownloadFileCompleted;

            if (!ServerWorking(url))
            {
                url = $"{URL.OtherBukkitOrSpigotURL}{type.ToLower()}/{type.ToLower()}-{versionName}.jar";
            }

            wc.DownloadFileAsync(new Uri(url), Path.Combine(Utils.ServersPath, $"{type}-{version}", $"{type.ToLower()}-{version}.jar"));
            SetupFiles(Path.Combine(Utils.ServersPath, $"{type}-{version}"), $"{type.ToLower()}-{version}.jar");
        }

        public void DowloadPaperVersion(WebClient wc, string version)
        {
            string versionUrl = string.Empty;

            VersionList.PaperVersions.TryGetValue(version, out versionUrl);

            wc.DownloadProgressChanged += wc_DownloadProgressChanged;
            wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
            wc.DownloadFileAsync(new Uri(versionUrl), Path.Combine(Utils.ServersPath, $"Paper-{version}", $"paper-{version}.jar"));
            SetupFiles(Path.Combine(Utils.ServersPath, $"Paper-{version}"), $"paper-{version}.jar");
        }

        public bool ServerWorking(string url)
        {
            try
            {
                WebRequest request = WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response == null || response.StatusCode != HttpStatusCode.OK)
                    return false;
            }
            catch
            {
                return false;
            }

            return true;
        }

        public void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

        public bool CheckVersionsList()
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    byte[] raw = webClient.DownloadData(URL.VersiosListUrl);
                    string json = Encoding.UTF8.GetString(raw);
                    VersionList = JsonConvert.DeserializeObject<VersionList>(json);
                }

                File.WriteAllText(Utils.VersionsListPath, Utils.ObjectToJson(VersionList));

                return true;
            }
            catch
            {
                if (File.Exists(Utils.VersionsListPath))
                {
                    VersionList = Utils.JsonToObject<VersionList>(File.ReadAllText(Utils.VersionsListPath));

                    return true;
                }
                else
                {
                    LoadLanguage(comboBox3.Text);

                    MessageBox.Show(LanguageManager.VersionListErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }

        #endregion

        #region FormFunctions

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadLanguage(comboBox3.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(Path.Combine(Utils.ServersPath, $"{comboBox1.Text}-{comboBox2.Text}", "Plugins")))
            {
                Directory.Delete(Path.Combine(Utils.ServersPath, $"{comboBox1.Text}-{comboBox2.Text}", "Plugins"), true);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(1);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(JavaPath))
            {
                MessageBox.Show(LanguageManager.NotSelectedJavaMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            PrepareServer();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Multiselect = false;
                dlg.Title = LanguageManager.SelectJavaTitle;
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    dlg.Filter = "Java|java.exe";
                }
                if (Directory.Exists("C:\\Program Files\\Java"))
                {
                    dlg.InitialDirectory = "C:\\Program Files\\Java";
                }
                DialogResult result = dlg.ShowDialog();

                if (result == DialogResult.OK)
                {
                    JavaPath = Path.Combine(Path.GetDirectoryName(dlg.FileName), dlg.FileName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!Loading)
            {
                if (Directory.Exists(Path.Combine(Utils.ServersPath, $"{comboBox1.Text}-{comboBox2.Text}", "Plugins")))
                {
                    button3.Enabled = true;
                }
                else
                {
                    button3.Enabled = false;
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Configuration configuration = new Configuration();
            configuration.JavaPath = JavaPath;
            configuration.JavaArgument = textBox1.Text;
            configuration.SelectedVersion = comboBox2.SelectedIndex;
            configuration.SelectedServerType = comboBox1.SelectedIndex;
            configuration.SelectedLanguage = comboBox3.Text;

            string json = Utils.ObjectToJson(configuration);

            File.WriteAllText(Utils.ConfigPath, json);
        }

        #endregion
    }
}
