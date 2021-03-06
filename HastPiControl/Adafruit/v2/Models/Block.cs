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

    public partial class Block
    {
        /// <summary>
        /// Initializes a new instance of the Block class.
        /// </summary>
        public Block() { }

        /// <summary>
        /// Initializes a new instance of the Block class.
        /// </summary>
        public Block(string name = default(string), string description = default(string), string key = default(string), double? dashboardId = default(double?), string visualType = default(string), double? column = default(double?), double? row = default(double?), double? sizeX = default(double?), double? sizeY = default(double?), IList<BlockBlockFeedsItem> blockFeeds = default(IList<BlockBlockFeedsItem>), object properties = default(object))
        {
            Name = name;
            Description = description;
            Key = key;
            DashboardId = dashboardId;
            VisualType = visualType;
            Column = column;
            Row = row;
            SizeX = sizeX;
            SizeY = sizeY;
            BlockFeeds = blockFeeds;
            Properties = properties;
        }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "key")]
        public string Key { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "dashboard_id")]
        public double? DashboardId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "visual_type")]
        public string VisualType { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "column")]
        public double? Column { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "row")]
        public double? Row { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "size_x")]
        public double? SizeX { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "size_y")]
        public double? SizeY { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "block_feeds")]
        public IList<BlockBlockFeedsItem> BlockFeeds { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "properties")]
        public object Properties { get; set; }

    }
}
