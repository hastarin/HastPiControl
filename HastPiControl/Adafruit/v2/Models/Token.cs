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

    public partial class Token
    {
        /// <summary>
        /// Initializes a new instance of the Token class.
        /// </summary>
        public Token() { }

        /// <summary>
        /// Initializes a new instance of the Token class.
        /// </summary>
        public Token(string tokenProperty = default(string))
        {
            TokenProperty = tokenProperty;
        }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "token")]
        public string TokenProperty { get; set; }

    }
}
