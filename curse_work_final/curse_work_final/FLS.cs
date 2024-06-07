using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace curse_work_final
{
    public class FLR
    {
        [JsonProperty("flrID")]
        public string FlrId { get; set; }

        [JsonProperty("beginTime")]
        public string BeginTime { get; set; }

        [JsonProperty("peakTime")]
        public string PeakTime { get; set; }

        [JsonProperty("endTime")]
        public string EndTime { get; set; }

        [JsonProperty("classType")]
        public string ClassType { get; set; }

        [JsonProperty("sourceLocation")]
        public string SourceLocation { get; set; }

        [JsonProperty("activeRegionNum")]
        public int ActiveRegionNum { get; set; }

        [JsonProperty("link")]
        public Uri Link { get; set; }
    }
}
