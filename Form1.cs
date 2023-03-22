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
        public string Path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        public string JavaPath = string.Empty;
        public bool Loading = false;
        public string BashPath = string.Empty;
        public LanguageManager LanguageManager;

        public Form1(LanguageManager language)
        {
            InitializeComponent();

            this.LanguageManager = language;

            foreach(string name in LanguageManager.GetLanguages()) 
            {
                comboBox3.Items.Add(name);
            }

            this.Focus();
            this.Activate();
            this.BringToFront();

            comboBox2.SelectedIndex = 0;
            comboBox1.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;

            LoadLanguage(LanguageManager.LanguageName);

            tabControl1.Appearance = TabAppearance.FlatButtons;
            tabControl1.ItemSize = new Size(0, 1);
            tabControl1.SizeMode = TabSizeMode.Fixed;
            this.Size = new Size(this.Size.Width, this.Size.Height + 10);

            if (!Directory.Exists(System.IO.Path.Combine(Path, "Plugins")))
            {
                Directory.CreateDirectory(System.IO.Path.Combine(Path, "Plugins"));
            }

            if (!Directory.Exists(System.IO.Path.Combine(Path, "Servers")))
            {
                Directory.CreateDirectory(System.IO.Path.Combine(Path, "Servers"));
            }

            if (!File.Exists(System.IO.Path.Combine(Path, "Config.json")))
            {
                Configuration configuration = new Configuration();
                configuration.JavaPath = JavaPath;
                configuration.JavaArgument = textBox1.Text;
                configuration.SelectedVersion = comboBox2.SelectedIndex;
                configuration.SelectedServerType = comboBox1.SelectedIndex;
                configuration.SelectedLanguage = comboBox3.SelectedText;

                string json = JsonConvert.SerializeObject(configuration, Formatting.Indented,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });

                using (FileStream fs = File.Create(System.IO.Path.Combine(Path, "Config.json")))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(json);
                    fs.Write(info, 0, info.Length);

                    fs.Close();
                }
            }
            else
            {
                using (StreamReader sr = File.OpenText(System.IO.Path.Combine(Path, "Config.json")))
                {
                    string json = sr.ReadToEnd();

                    Configuration configuration = JsonConvert.DeserializeObject<Configuration>(json); 

                    JavaPath = configuration.JavaPath;
                    textBox1.Text = configuration.JavaArgument;
                    comboBox2.SelectedIndex = configuration.SelectedVersion;
                    comboBox1.SelectedIndex = configuration.SelectedServerType;
                    comboBox3.SelectedItem = configuration.SelectedLanguage;

                    sr.Close();
                }
            }
        }

        public void StartServer()
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
                
                if(!Directory.Exists(System.IO.Path.Combine(Path, "Servers", $"{comboBox2.Text}-{comboBox1.Text}"))) 
                {
                    Directory.CreateDirectory(System.IO.Path.Combine(Path, "Servers", $"{comboBox2.Text}-{comboBox1.Text}"));

                    WebClient wc = new WebClient();

                    if (comboBox1.SelectedIndex == 0)
                    {
                        wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                        wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
                        wc.DownloadFileAsync(new Uri($"https://download.getbukkit.org/spigot/spigot-{comboBox2.Text}.jar"), System.IO.Path.Combine(Path, "Servers", $"{comboBox2.Text}-{comboBox1.Text}", $"spigot-{comboBox2.Text}.jar"));
                        SetupFiles(System.IO.Path.Combine(Path, "Servers", $"{comboBox2.Text}-{comboBox1.Text}"), $"spigot-{comboBox2.Text}.jar");
                    }
                    else if (comboBox1.SelectedIndex == 1)
                    {
                        wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                        wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
                        wc.DownloadFileAsync(new Uri($"https://download.getbukkit.org/craftbukkit/craftbukkit-{comboBox2.Text}.jar"), System.IO.Path.Combine(Path, "Servers", $"{comboBox2.Text}-{comboBox1.Text}", $"craftbukkit-{comboBox2.Text}.jar"));
                        SetupFiles(System.IO.Path.Combine(Path, "Servers", $"{comboBox2.Text}-{comboBox1.Text}"), $"craftbukkit-{comboBox2.Text}.jar");
                    }
                    else if (comboBox1.SelectedIndex == 2)
                    {
                        switch (comboBox2.Text)
                        {
                            case "1.19":
                                wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                                wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
                                wc.DownloadFileAsync(new Uri($"https://api.papermc.io/v2/projects/paper/versions/1.19.4/builds/466/downloads/paper-1.19.4-466.jar"), System.IO.Path.Combine(Path, "Servers", $"{comboBox2.Text}-{comboBox1.Text}", $"paper-{comboBox2.Text}.jar"));
                                SetupFiles(System.IO.Path.Combine(Path, "Servers", $"{comboBox2.Text}-{comboBox1.Text}"), $"paper-{comboBox2.Text}.jar");
                                break;
                            case "1.18":
                                wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                                wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
                                wc.DownloadFileAsync(new Uri($"https://api.papermc.io/v2/projects/paper/versions/1.18.2/builds/388/downloads/paper-1.18.2-388.jar"), System.IO.Path.Combine(Path, "Servers", $"{comboBox2.Text}-{comboBox1.Text}", $"paper-{comboBox2.Text}.jar"));
                                SetupFiles(System.IO.Path.Combine(Path, "Servers", $"{comboBox2.Text}-{comboBox1.Text}"), $"paper-{comboBox2.Text}.jar");
                                break;
                            case "1.17":
                                wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                                wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
                                wc.DownloadFileAsync(new Uri($"https://api.papermc.io/v2/projects/paper/versions/1.17.1/builds/411/downloads/paper-1.17.1-411.jar"), System.IO.Path.Combine(Path, "Servers", $"{comboBox2.Text}-{comboBox1.Text}", $"paper-{comboBox2.Text}.jar"));
                                SetupFiles(System.IO.Path.Combine(Path, "Servers", $"{comboBox2.Text}-{comboBox1.Text}"), $"paper-{comboBox2.Text}.jar");
                                break;
                            case "1.16":
                                wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                                wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
                                wc.DownloadFileAsync(new Uri($"https://api.papermc.io/v2/projects/paper/versions/1.16.5/builds/794/downloads/paper-1.16.5-794.jar"), System.IO.Path.Combine(Path, "Servers", $"{comboBox2.Text}-{comboBox1.Text}", $"paper-{comboBox2.Text}.jar"));
                                SetupFiles(System.IO.Path.Combine(Path, "Servers", $"{comboBox2.Text}-{comboBox1.Text}"), $"paper-{comboBox2.Text}.jar");
                                break;
                            case "1.15":
                                wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                                wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
                                wc.DownloadFileAsync(new Uri($"https://api.papermc.io/v2/projects/paper/versions/1.15.2/builds/393/downloads/paper-1.15.2-393.jar"), System.IO.Path.Combine(Path, "Servers", $"{comboBox2.Text}-{comboBox1.Text}", $"paper-{comboBox2.Text}.jar"));
                                SetupFiles(System.IO.Path.Combine(Path, "Servers", $"{comboBox2.Text}-{comboBox1.Text}"), $"paper-{comboBox2.Text}.jar");
                                break;
                            case "1.14":
                                wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                                wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
                                wc.DownloadFileAsync(new Uri($"https://api.papermc.io/v2/projects/paper/versions/1.14.4/builds/245/downloads/paper-1.14.4-245.jar"), System.IO.Path.Combine(Path, "Servers", $"{comboBox2.Text}-{comboBox1.Text}", $"paper-{comboBox2.Text}.jar"));
                                SetupFiles(System.IO.Path.Combine(Path, "Servers", $"{comboBox2.Text}-{comboBox1.Text}"), $"paper-{comboBox2.Text}.jar");
                                break;
                            case "1.13":
                                wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                                wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
                                wc.DownloadFileAsync(new Uri($"https://api.papermc.io/v2/projects/paper/versions/1.13.2/builds/657/downloads/paper-1.13.2-657.jar"), System.IO.Path.Combine(Path, "Servers", $"{comboBox2.Text}-{comboBox1.Text}", $"paper-{comboBox2.Text}.jar"));
                                SetupFiles(System.IO.Path.Combine(Path, "Servers", $"{comboBox2.Text}-{comboBox1.Text}"), $"paper-{comboBox2.Text}.jar");
                                break;
                            case "1.12":
                                wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                                wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
                                wc.DownloadFileAsync(new Uri($"https://api.papermc.io/v2/projects/paper/versions/1.12.2/builds/1620/downloads/paper-1.12.2-1620.jar"), System.IO.Path.Combine(Path, "Servers", $"{comboBox2.Text}-{comboBox1.Text}", $"paper-{comboBox2.Text}.jar"));
                                SetupFiles(System.IO.Path.Combine(Path, "Servers", $"{comboBox2.Text}-{comboBox1.Text}"), $"paper-{comboBox2.Text}.jar");
                                break;
                            case "1.11":
                                wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                                wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
                                wc.DownloadFileAsync(new Uri($"https://api.papermc.io/v2/projects/paper/versions/1.11.2/builds/1106/downloads/paper-1.11.2-1106.jar"), System.IO.Path.Combine(Path, "Servers", $"{comboBox2.Text}-{comboBox1.Text}", $"paper-{comboBox2.Text}.jar"));
                                SetupFiles(System.IO.Path.Combine(Path, "Servers", $"{comboBox2.Text}-{comboBox1.Text}"), $"paper-{comboBox2.Text}.jar");
                                break;
                            case "1.10":
                                wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                                wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
                                wc.DownloadFileAsync(new Uri($"https://api.papermc.io/v2/projects/paper/versions/1.10.2/builds/918/downloads/paper-1.10.2-918.jar"), System.IO.Path.Combine(Path, "Servers", $"{comboBox2.Text}-{comboBox1.Text}", $"paper-{comboBox2.Text}.jar"));
                                SetupFiles(System.IO.Path.Combine(Path, "Servers", $"{comboBox2.Text}-{comboBox1.Text}"), $"paper-{comboBox2.Text}.jar");
                                break;
                            case "1.9":
                                wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                                wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
                                wc.DownloadFileAsync(new Uri($"https://api.papermc.io/v2/projects/paper/versions/1.9.4/builds/775/downloads/paper-1.9.4-775.jar"), System.IO.Path.Combine(Path, "Servers", $"{comboBox2.Text}-{comboBox1.Text}", $"paper-{comboBox2.Text}.jar"));
                                SetupFiles(System.IO.Path.Combine(Path, "Servers", $"{comboBox2.Text}-{comboBox1.Text}"), $"paper-{comboBox2.Text}.jar");
                                break;
                            case "1.8":
                                wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                                wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
                                wc.DownloadFileAsync(new Uri($"https://api.papermc.io/v2/projects/paper/versions/1.8.8/builds/445/downloads/paper-1.8.8-445.jar"), System.IO.Path.Combine(Path, "Servers", $"{comboBox2.Text}-{comboBox1.Text}", $"paper-{comboBox2.Text}.jar"));
                                SetupFiles(System.IO.Path.Combine(Path, "Servers", $"{comboBox2.Text}-{comboBox1.Text}"), $"paper-{comboBox2.Text}.jar");
                                break;
                        }
                    }
                }
                else
                {
                    if(!File.Exists(System.IO.Path.Combine(Path, "Servers", $"{comboBox2.Text}-{comboBox1.Text}", $"{comboBox1.Text.ToLower()}-{comboBox2.Text}.jar")))
                    {
                        Directory.Delete(System.IO.Path.Combine(Path, "Servers", $"{comboBox2.Text}-{comboBox1.Text}"), true);
                        StartServer();
                        return;
                    }
                    else
                    {
                        SetupFiles(System.IO.Path.Combine(Path, "Servers", $"{comboBox2.Text}-{comboBox1.Text}"), $"{comboBox1.Text.ToLower()}-{comboBox2.Text}.jar");

                        StartBat();
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

        private void Wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            progressBar1.Value = 0;

            StartBat();
        }

        public void StartBat()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                string filePathExecute = System.IO.Path.Combine(Path, "Servers", $"{comboBox2.Text}-{comboBox1.Text}", "Start.sh");
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
                Process.Start(System.IO.Path.Combine(Path, "Servers", $"{comboBox2.Text}-{comboBox1.Text}", "Start.bat"));
            }

            textBox1.Enabled = true;
            comboBox1.Enabled = true;
            comboBox2.Enabled = true;
            button1.Enabled = true;
            button2.Enabled = true;
            Loading = false;
        }

        private void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(JavaPath))
            {
                MessageBox.Show(LanguageManager.NotSelectedJavaMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            StartServer();
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

                if(result == DialogResult.OK) 
                {
                    JavaPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(dlg.FileName), dlg.FileName);
                }
            }
            catch(Exception ex) 
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!Loading)
            {
                if (Directory.Exists(System.IO.Path.Combine(Path, "Servers", $"{comboBox2.Text}-{comboBox1.Text}", "Plugins")))
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

            string json = JsonConvert.SerializeObject(configuration, Formatting.Indented,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });

            using (FileStream fs = File.Create(System.IO.Path.Combine(Path, "Config.json")))
            {
                byte[] info = new UTF8Encoding(true).GetBytes(json);
                fs.Write(info, 0, info.Length);

                fs.Close();
            }
        }

        public void SetupFiles(string directory, string version)
        {
            if(!Directory.Exists(System.IO.Path.Combine(directory, "Plugins"))) 
            {
                Directory.CreateDirectory(System.IO.Path.Combine(directory, "Plugins"));

                CopyAll(new DirectoryInfo(System.IO.Path.Combine(Path, "Plugins")), new DirectoryInfo(System.IO.Path.Combine(directory, "Plugins")));
            }
            
            if(!File.Exists(System.IO.Path.Combine(directory, "eula.txt")))
            {
                using (FileStream fs = File.Create(System.IO.Path.Combine(directory, "eula.txt")))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes("eula=true");
                    fs.Write(info, 0, info.Length);

                    fs.Close();
                }
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (!File.Exists(System.IO.Path.Combine(directory, "Start.sh")))
                {
                    using (FileStream fs = File.Create(System.IO.Path.Combine(directory, "Start.sh")))
                    {
                        string first = textBox1.Text.Replace("<Java>", $"\"{JavaPath}\"");
                        string second = first.Replace("<Jar>", version);

                        byte[] info = new UTF8Encoding(true).GetBytes($"#!/bin/bash{Environment.NewLine}cd \"{directory}\"{Environment.NewLine}{second}");
                        fs.Write(info, 0, info.Length);

                        fs.Close();
                    }
                }
                else
                {
                    File.Delete(System.IO.Path.Combine(directory, "Start.sh"));

                    using (FileStream fs = File.Create(System.IO.Path.Combine(directory, "Start.sh")))
                    {
                        string first = textBox1.Text.Replace("<Java>", $"\"{JavaPath}\"");
                        string second = first.Replace("<Jar>", version);

                        byte[] info = new UTF8Encoding(true).GetBytes($"#!/bin/bash{Environment.NewLine}cd \"{directory}\"{Environment.NewLine}{second}");
                        fs.Write(info, 0, info.Length);

                        fs.Close();
                    }
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (!File.Exists(System.IO.Path.Combine(directory, "Start.bat")))
                {
                    using (FileStream fs = File.Create(System.IO.Path.Combine(directory, "Start.bat")))
                    {
                        string first = textBox1.Text.Replace("<Java>", $"\"{JavaPath}\"");
                        string second = first.Replace("<Jar>", version);

                        byte[] info = new UTF8Encoding(true).GetBytes($"cd \"{directory}\" \r\n{second} \r\npause");
                        fs.Write(info, 0, info.Length);

                        fs.Close();
                    }
                }
                else if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    File.Delete(System.IO.Path.Combine(directory, "Start.bat"));

                    using (FileStream fs = File.Create(System.IO.Path.Combine(directory, "Start.bat")))
                    {
                        string first = textBox1.Text.Replace("<Java>", $"\"{JavaPath}\"");
                        string second = first.Replace("<Jar>", version);

                        byte[] info = new UTF8Encoding(true).GetBytes($"cd \"{directory}\" \r\n{second} \r\npause");
                        fs.Write(info, 0, info.Length);

                        fs.Close();
                    }
                }
            }
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(System.IO.Path.Combine(target.FullName, fi.Name), true);
            }

            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(System.IO.Path.Combine(Path, "Servers", $"{comboBox2.Text}-{comboBox1.Text}", "Plugins")))
            {
                Directory.Delete(System.IO.Path.Combine(Path, "Servers", $"{comboBox2.Text}-{comboBox1.Text}", "Plugins"), true);
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

        public void LoadLanguage(string language)
        {
            LanguageManager.LoadLanguageFromName(language);

            label5.Text = LanguageManager.LanguageTitle;
            label1.Text = LanguageManager.JavaArgumentsTitle;
            label2.Text = LanguageManager.VersionTitle;
            label3.Text = LanguageManager.ServerTypeTitle;
            label4.Text = LanguageManager.DowloadProgressTitle;
            button2.Text = LanguageManager.SelectJavaTitle;
            button3.Text = LanguageManager.DeletePluginsFolderTitle;
            button1.Text = LanguageManager.TestTitle;
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadLanguage(comboBox3.Text);
        }
    }
}
