using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DataverseFunc1
{
    public class StartOn
    {
        public string type { get; set; }
        public string format { get; set; }
        public string value { get; set; }
    }

    public class EndOn
    {
        public string type { get; set; }
        public string format { get; set; }
        public string value { get; set; }
    }

    public class Properties
    {
        public StartOn StartOn { get; set; }
        public EndOn EndOn { get; set; }
    }

    public class Input
    {
        [JsonProperty("$schema")]
        public string Schema { get; set; }
        public string type { get; set; }
        public Properties properties { get; set; }
        public List<string> required { get; set; }
    }
}
