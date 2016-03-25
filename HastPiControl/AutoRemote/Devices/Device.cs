// ***********************************************************************
// Assembly         : HastPiControl
// Author           : Jon Benson
// Created          : 20-03-2016
// 
// Last Modified By : Jon Benson
// Last Modified On : 26-03-2016
// ***********************************************************************

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierWordIsNotInDictionary
namespace HastPiControl.AutoRemote.Devices
{
    using System;
    using System.Collections.Generic;

    /// <summary>Class Device.</summary>
    public class Device
    {
        /// <summary>The known device list</summary>
        internal static readonly List<string> KnownDeviceList = new List<string>();

        /// <summary>Personal key for the device. Check here for more details: http://joaoapps.com/autoremote/personal/</summary>
        /// <value>The key.</value>
        public String key { get; set; }

        /// <summary>Name of the device</summary>
        /// <value>The name.</value>
        public String name { get; set; }

        /// <summary>Device's Public IP</summary>
        /// <value>The publicip.</value>
        public String publicip { get; set; }

        /// <summary>Device's Local IP</summary>
        /// <value>The localip.</value>
        public String localip { get; set; }

        /// <summary>Device's Port</summary>
        /// <value>The port.</value>
        public String port { get; set; }
    }
}