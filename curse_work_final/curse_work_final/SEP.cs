using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace curse_work_final
{
    public class SEP
    {
        [JsonProperty("sepID")]
        public string SepId { get; set; }

        [JsonProperty("eventTime")]
        public string EventTime { get; set; }

        [JsonProperty("instruments")]
        public List<Instrument> Instruments { get; set; }

        [JsonProperty("link")]
        public Uri Link { get; set; }
    }

    public class Instrument
    {
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
    }
}
