using Newtonsoft.Json.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PluginTester
{
    public class Utils
    {
        public string MainPath;
        public string ServersPath;
        public string ConfigPath;
        public string LanguagesPath;
        public string PluginsPath;
        public string DataPath;
        public string VersionsListPath;

        public Utils()
        {
            MainPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            DataPath = Path.Combine(MainPath, "Data");
            ServersPath = Path.Combine(MainPath, "Servers");
            ConfigPath = Path.Combine(DataPath, "Config.config");
            LanguagesPath = Path.Combine(MainPath, "Languages");
            PluginsPath = Path.Combine(MainPath, "Plugins");
            VersionsListPath = Path.Combine(DataPath, "VersionsList.data");
        }

        public string ObjectToJson<T>(T e)
        {
            return EncodeBase64(JsonConvert.SerializeObject(e, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            }));  
        }

        public T JsonToObject<T>(string data) 
        {
            return JsonConvert.DeserializeObject<T>(DecodeBase64(data));
        }

        public string EncodeBase64(string value)
        {
            var valueBytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(valueBytes);
        }

        public string DecodeBase64(string value)
        {
            var valueBytes = Convert.FromBase64String(value);
            return Encoding.UTF8.GetString(valueBytes);
        }

        public void ReverseComboBoxList<T>(ComboBox.ObjectCollection list)
        {
            if (list == null || list.Count <= 1)
            {
                return;
            }

            T value = (T)list[0];
            list.RemoveAt(0);

            ReverseComboBoxList<T>(list);

            list.Add(value);
        }

        public string GetServerType(string type)
        {
            return type.Replace("+", "Plus");
        }

        public string GetJarType(string originalType)
        {
            string type = string.Empty;

            string newType = originalType.Replace("+", "Plus");

            switch (newType)
            {
                case "CraftBukkit":
                    type = "bukkit";
                    break;
                case "PufferfishPlus":
                    type = "pufferfishplus";
                    break;
                case "PufferfishPlus (Purpur)":
                    type = "pufferfishpluspurpur";
                    break;
                default:
                    type = newType.ToLower();
                    break;
            }

            return type;
        }
    }
}
