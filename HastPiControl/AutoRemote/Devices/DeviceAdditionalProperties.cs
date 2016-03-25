// ***********************************************************************
// Assembly         : HastPiControl
// Author           : Jon Benson
// Created          : 20-03-2016
// 
// Last Modified By : Jon Benson
// Last Modified On : 26-03-2016
// ***********************************************************************

// ReSharper disable InconsistentNaming
namespace HastPiControl.AutoRemote.Devices
{
    using System;

    /// <summary>Class DeviceAdditionalProperties.</summary>
    public class DeviceAdditionalProperties
    {
        /// <summary>The icon URL. Has to be a publicly accessible url of a png or jpg image</summary>
        public String iconUrl { get; } = Constants.ICON_URL;

        /// <summary>The plugin type. Something like 'windows plugin' or 'mac plugin'</summary>
        public String type { get; } = Constants.PLUGIN_TYPE;

        /// <summary>Whether or not this plugin can receive files via HTTP PUT</summary>
        public Boolean canReceiveFiles { get; } = Constants.CAN_RECEIVE_FILES;
    }
}