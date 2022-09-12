using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace fileui.Models
{
    public class FileTag
    {
        public string TagSetName { get; set; } = "";
        public string TagName { get; set; }
        public bool Enabled { get; set; }
        public int Order { get; set; }
        [JsonIgnore]
        public Button TagButton { get; set; }
        [JsonIgnore]
        public Button TagButtonLinker { get; set; }
    }
}
