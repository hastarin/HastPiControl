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
namespace HastPiControl.AutoRemote.Communications
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Windows.Networking;
    using Windows.Networking.Connectivity;

    using HastPiControl.AutoRemote.Devices;

    /// <summary>Class RequestSendRegistration.</summary>
    /// <seealso cref="HastPiControl.AutoRemote.Communications.Request" />
    public class RequestSendRegistration : Request
    {
        /// <summary>Initializes a new instance of the <see cref="RequestSendRegistration" /> class.</summary>
        public RequestSendRegistration()
        {
            //Any unique Id.
            this.id = ConstantsThatShouldBeVariables.ID;
            //Name you want to appear in AutoRemote
            this.name = ConstantsThatShouldBeVariables.NAME;
            //This is always "plugin"
            this.type = Constants.DEVICE_TYPE;
            //Your local IP. Should be dynamically gotten if possible
            this.localip = NetworkInformation.GetHostNames().First(h => h.Type == HostNameType.Ipv4).RawName;
            //Public IP. Should be dynamically gotten if possible
            this.publicip = null;
            //Any port you want. Should make it user configurable
            this.port = ConstantsThatShouldBeVariables.PORT.ToString();
            //Always set to true
            this.haswifi = true;
            //Instantiate the additional device properties that are needed for plugins
            this.additional = new DeviceAdditionalProperties();
        }

        /// <summary>Gets or sets the identifier.</summary>
        public String id { get; set; }

        /// <summary>Gets or sets the name.</summary>
        public String name { get; set; }

        /// <summary>Gets or sets the type.</summary>
        public String type { get; set; }

        /// <summary>Gets or sets the localip.</summary>
        public String localip { get; set; }

        /// <summary>Gets or sets the publicip.</summary>
        public String publicip { get; set; }

        /// <summary>Gets or sets the port.</summary>
        public String port { get; set; }

        /// <summary>Gets or sets the haswifi.</summary>
        public Boolean haswifi { get; set; }

        /// <summary>Gets or sets the additional.</summary>
        public DeviceAdditionalProperties additional { get; set; }

        /// <summary>
        ///     Executes the request. Is different for every type of request. The default is to just respond with the response
        ///     ResponseNoAction
        /// </summary>
        public override Response ExecuteRequest()
        {
            var baseResponse = base.ExecuteRequest();
            if (Device.KnownDeviceList.Contains(this.id))
            {
                return baseResponse;
            }

            var request = new RequestSendRegistration();

            request.Send(
                new Device
                    {
                        key = this.id,
                        localip = this.localip,
                        port = this.port,
                        name = this.name,
                        publicip = this.publicip
                    });

            Device.KnownDeviceList.Add(this.id);

            return baseResponse;
        }
    }
}