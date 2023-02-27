using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginTester
{
    public class Configuration
    {
        public string JavaPath { get; set; }
        public int SelectedVersion { get; set; }
        public int SelectedServerType { get; set; }
        public string JavaArgument { get; set; }
    }
}
