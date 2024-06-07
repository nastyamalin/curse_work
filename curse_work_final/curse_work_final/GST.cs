using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace curse_work_final
{
    public class GST
    {
        [JsonProperty("gstID")]
        public string GstId { get; set; }

        [JsonProperty("startTime")]
        public string StartTime { get; set; }

        [JsonProperty("allKpIndex")]
        public List<KpIndex> AllKpIndex { get; set; }

        [JsonProperty("linkedEvents")]
        public List<LinkedEvent> LinkedEvents { get; set; }

        [JsonProperty("link")]
        public Uri Link { get; set; }
    }


    public class KpIndex
    {
        [JsonProperty("observedTime")]
        public string ObservedTime { get; set; }

        [JsonProperty("kpIndex")]
        public long Index { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }
    }

    public class LinkedEvent
    {
        [JsonProperty("activityID")]
        public string ActivityId { get; set; }
    }
}
