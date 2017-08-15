using InvertedTomato.LightWeightSerialization;
using Newtonsoft.Json;
using ProtoBuf;
using System.Runtime.Serialization;

namespace Comparison {
    [DataContract]
    [ProtoContract]
    public class Segment {
        /// <summary>
        /// Determines how to compute mapping to verse ID.
        /// </summary>
        /// <remarks>
        /// 0 = continuation of last verse in sequence
        /// 1 = entire verse on it's own
        /// N = number of entire verses
        /// </remarks>
        [LightWeightProperty(0)]
        [JsonProperty("m")]
        [DataMember(Order = 0)]
        [ProtoMember(1)]
        public byte Mode { get; set; }

        [LightWeightProperty(1)]
        [JsonProperty("c")]
        [DataMember(Order = 1)]
        [ProtoMember(2)]
        public string Content { get; set; }
    }
}
