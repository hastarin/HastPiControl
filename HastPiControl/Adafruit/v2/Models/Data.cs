// Code generated by Microsoft (R) AutoRest Code Generator 0.16.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace HastPiControl.Adafruit.v2.Models
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Microsoft.Rest;
    using Microsoft.Rest.Serialization;

    public partial class Data
    {
        /// <summary>
        /// Initializes a new instance of the Data class.
        /// </summary>
        public Data() { }

        /// <summary>
        /// Initializes a new instance of the Data class.
        /// </summary>
        public Data(string value = default(string), string lat = default(string), string lon = default(string), string ele = default(string), double? epoch = default(double?))
        {
            Value = value;
            Lat = lat;
            Lon = lon;
            Ele = ele;
            Epoch = epoch;
        }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "lat")]
        public string Lat { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "lon")]
        public string Lon { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "ele")]
        public string Ele { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "epoch")]
        public double? Epoch { get; set; }

    }
}
