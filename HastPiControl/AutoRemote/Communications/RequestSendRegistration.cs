namespace HastPiControl.AutoRemote.Communications
{
    using System;
    using System.Linq;

    using Windows.Networking;
    using Windows.Networking.Connectivity;

    using AutoRemotePlugin.AutoRemote;
    using AutoRemotePlugin.AutoRemote.Devices;

    public class RequestSendRegistration : Request
    {
        public String id { get; set; }
        public String name { get; set; }
        public String type { get; set; }
        public String localip { get; set; }
        public String publicip { get; set; }
        public String port { get; set; }
        public Boolean haswifi { get; set; }
        public DeviceAdditionalProperties additional { get; set; }

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

    }
}
