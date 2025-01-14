﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PortingAssistantExtensionTelemetry.Model
{
    public class MetricsBase
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public MetricsType MetricsType { get; set; }
        public string PortingAssistantExtensionVersion { get; set; }
        public string TargetFramework { get; set; }
        public string TimeStamp { get; set; }
    }
}
