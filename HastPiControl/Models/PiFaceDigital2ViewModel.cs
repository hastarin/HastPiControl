// ***********************************************************************
// Assembly         : HastPiControl
// Author           : Jon Benson
// Created          : 14-11-2015
//
// Last Modified By : Jon Benson
// Last Modified On : 16-04-2017
// ***********************************************************************

namespace HastPiControl.Models
{
    using com.hastarin.GarageDoor;
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Threading;
    using Hastarin.Devices;
    using HastPiControl.Adafruit;
    using HastPiControl.Adafruit.Models;
    using HastPiControl.AutoRemote;
    using HastPiControl.AutoRemote.Communications;
    using HastPiControl.AutoRemote.Devices;
    using HastPiControl.IFTTT;
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using uPLibrary.Networking.M2Mqtt;
    using uPLibrary.Networking.M2Mqtt.Messages;
    using Windows.ApplicationModel.Resources;
    using Windows.Devices.AllJoyn;
    using Windows.Networking;
    using Windows.UI.Xaml;

    /// <summary>Class PiFaceDigital2ViewModel.</summary>
    public class PiFaceDigital2ViewModel : ViewModelBase, IDisposable
    {
        /// <summary>Name to use for the input for the main door reed switch</summary>
        public static string GarageDoorOpen = "GarageDoorOpen";

        /// <summary>Name to use for the input for the door partial open reed switch</summary>
        public static string GarageDoorPartialOpen = "GarageDoorPartialOpen";

        /// <summary>Name to use for the output controlling the push button relay</summary>
        public static string GarageDoorPushButtonRelay = "GarageDoorPushButton";

        /// <summary>MQTT Topic for publishing the status of this app.</summary>
        /// <remarks>Eg. Connected/Published/Disconnected/Error</remarks>
        private const string GarageDoorStatusTopic = "garage/door/status";

        private const long MinimumInterval = 2000;
        /// <summary>MQTT Topics used for controlling the garage door.</summary>
        private static readonly string[] MqttTopics = { "garage/door/switch", "garage/door/method" };

        private static string adafruitIoFeedName;
        private static string adafruitIoKey;
        private static string autoRemotePassword;
        private static GarageDoorProducer doorProducer;
        private static string makerKey;
        private static string mqttPassword;
        private static string mqttUserName;
        private readonly AdafruitIO adafruitIo;
        private readonly DispatcherTimer debounceTimer = new DispatcherTimer();
        private readonly Device defaultDevice;
        private readonly GarageDoor garageDoor;
        private readonly Maker makerChannel;
        private readonly MqttClient mqttClient;
        private readonly DispatcherTimer mqttDispatcherTimer;

        private AutoRemoteHttpServer autoRemoteHttpServer;

        private string autoRemoteUri;

        private bool disposed;

        private ObservableCollection<GpioPinViewModel> inputs = new ObservableCollection<GpioPinViewModel>();

        private ObservableCollection<GpioPinViewModel> outputs = new ObservableCollection<GpioPinViewModel>();

        private DispatcherTimer timer;

        /// <summary>Initializes a new instance of the <see cref="PiFaceDigital2ViewModel" /> class.</summary>
        public PiFaceDigital2ViewModel()
        {
            for (int i = 0; i < 8; i++)
            {
                this.Inputs.Add(new GpioPinViewModel((byte)i) { Id = i, Name = "Input " + i });
                var j = i + 8;
                this.Outputs.Add(new GpioPinViewModel((byte)j, true) { Id = i, Name = "Output " + i });
            }
            this.LoadResourceStrings();

            // ReSharper disable once ExceptionNotDocumented
            this.debounceTimer.Interval = TimeSpan.FromMilliseconds(MinimumInterval);
            this.debounceTimer.Tick += this.DebounceTimerOnTick;

            this.SetupInputOutputNames();
            this.garageDoor = new GarageDoor(this);

            this.SetupAllJoynBusAttachmentAndProducer();

            this.SetupAutoRemoteHttpServer();
            this.defaultDevice = GetAutoRemoteLocalDevice();
            if (!string.IsNullOrWhiteSpace(makerKey))
            {
                this.makerChannel = new Maker(makerKey);
            }
            try
            {
                if (!string.IsNullOrWhiteSpace(adafruitIoKey))
                {
                    this.adafruitIo = new AdafruitIO { Data = new Data { Value = "-1" } };
                    AdafruitIOExtensions.Key = adafruitIoKey;
                }
            }
            catch (AmbiguousMatchException)
            {
                Console.WriteLine("You may need to downgrade Microsoft.Rest.ClientRuntime to 2.3.2   See https://github.com/Azure/autorest/issues/1542 for more information.");
            }

            // NOTE: These resources come from Strings/en-US/Resources.resw which you'll need to provide/edit to match
            var resource = ResourceLoader.GetForCurrentView();
            var mqttHost = resource.GetString("MqttHost");
            var mqttPort = resource.GetString("MqttPort");

            if (!string.IsNullOrWhiteSpace(mqttHost))
            {
                // ReSharper disable once ExceptionNotDocumented
                this.mqttDispatcherTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(30) };
                this.mqttDispatcherTimer.Tick += this.MqttDispatcherTimerOnTick;

                // NOTE: I use TLS with MQTT   If you don't you will need to change this
                this.mqttClient = new MqttClient(mqttHost, int.Parse(mqttPort), true, MqttSslProtocols.TLSv1_2);
                this.mqttClient.ConnectionClosed += this.OnMqttConnectionClosed;

                // NOTE: Only uncomment the following if you're sure your MQTT server is secure as it allows control of the door
                // this.mqttClient.MqttMsgPublishReceived += this.OnMqttPublishedMessage;
                this.ConnectMqttClient();
            }

            this.HookEvents();
            this.debounceTimer.Start();
        }

        /// <summary>
        ///     Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by
        ///     garbage collection.
        /// </summary>
        ~PiFaceDigital2ViewModel()
        {
            this.Dispose(false);
        }

        /// <summary>Gets or sets the inputs.</summary>
        /// <value>The inputs.</value>
        public ObservableCollection<GpioPinViewModel> Inputs
        {
            get
            {
                return this.inputs;
            }
            set
            {
                this.Set(ref this.inputs, value);
            }
        }

        /// <summary>Gets or sets the outputs.</summary>
        /// <value>The outputs.</value>
        public ObservableCollection<GpioPinViewModel> Outputs
        {
            get
            {
                return this.outputs;
            }
            set
            {
                this.Set(ref this.outputs, value);
            }
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Initializes the pi face.</summary>
        public async Task InitializePiFace()
        {
            await MCP23S17.InitializeSpiDevice();
            MCP23S17.Initialize();
            MCP23S17.SetPinMode(0x00FF); // 0x0000 = all outputs, 0xffff=all inputs, 0x00FF is PIFace Default
            MCP23S17.SetPullUpMode(0x00FF); // 0x0000 = no pullups, 0xffff=all pullups, 0x00FF is PIFace Default
            MCP23S17.WriteWord(0x0000); // 0x0000 = no pullups, 0xffff=all pullups, 0x00FF is PIFace Default

            // ReSharper disable once ExceptionNotDocumented
            this.timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
            this.timer.Tick += this.TimerOnTick;
            this.timer.Start();
        }

        /// <summary>Updates the values from the Pi.</summary>
        internal void UpdateValuesFromPi()
        {
            var result = MCP23S17.ReadRegister16(); // do something with the values
            for (int i = 0; i < 8; i++)
            {
                var bitIsOn = (result & (1 << i)) != 0;
                this.Inputs.First(input => input.Id == i).IsOn = !bitIsOn;
                bitIsOn = (result & (1 << (i + 8))) != 0;
                this.Outputs.First(input => input.Id == i).IsOn = bitIsOn;
            }
        }

        // ReSharper disable once PublicMembersMustHaveComments
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                // free other managed objects that implement
                // IDisposable only
                this.autoRemoteHttpServer.Stop();
                this.mqttClient.MqttMsgPublishReceived -= this.OnMqttPublishedMessage;
                this.mqttClient.ConnectionClosed -= this.OnMqttConnectionClosed;
                if (this.mqttClient.IsConnected)
                {
                    this.mqttClient.Publish(GarageDoorStatusTopic, Encoding.UTF8.GetBytes("Disconnected " + DateTime.Now), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, true);
                    this.mqttClient.Disconnect();
                }
            }

            // release any unmanaged objects
            // set the object references to null
            doorProducer.Stop();
            doorProducer = null;

            this.disposed = true;
        }

        private static Device GetAutoRemoteLocalDevice()
        {
            var resource = ResourceLoader.GetForCurrentView();
            //Instantiate Device (device to send stuff to). In a proper app this device should have been created with a received RequestSendRegistration
            String personalKey = resource.GetString("AutoRemoteKey");
            //see how to get it here http://joaoapps.com/autoremote/personal

            var localip = resource.GetString("LocalDeviceIp");
            var device = new Device { localip = localip, port = "1817", key = personalKey };
            return device;
        }

        private static void RegisterMyselfOnOtherDevice()
        {
            var device = GetAutoRemoteLocalDevice();

            //Instantiate Registration Request
            RequestSendRegistration request = new RequestSendRegistration();

            //Send registration request
            request.Send(device);
        }

        private void AdafruitIoCreateData()
        {
            // Send to Adafruit IO
            if (this.adafruitIo != null)
            {
                try
                {
                    this.adafruitIo.CreateData(adafruitIoFeedName);
                }
                catch (Microsoft.Rest.HttpOperationException)
                {
                }
            }
        }

        private async void AutoRemoteHttpServerOnRequestReceived(Request request, HostName hostName)
        {
            if (request.communication_base_params.type != "Message")
            {
                return;
            }
            var m = request as Message;
            var device = new Device { key = request.sender, localip = hostName.RawName };
            if (m == null || m.password != autoRemotePassword)
            {
                if (Device.KnownDeviceList.Contains(request.sender))
                {
                    this.SendStatus(device);
                }
                return;
            }
            await DispatcherHelper.RunAsync(() => this.ProcessMessage(m, device));
        }

        private void AutoRemoteSend(Device device, string state)
        {
            // Send to AutoRemote
            if (!string.IsNullOrWhiteSpace(this.autoRemoteUri))
            {
                var m = new Message { message = state + "=:=GarageDoor" };
                m.Send(device ?? this.defaultDevice);
            }
        }

        private void ConnectMqttClient()
        {
            try
            {
                this.mqttClient.Connect(
                    mqttUserName,
                    mqttUserName,
                    mqttPassword,
                    true,
                    MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE,
                    true,
                    GarageDoorStatusTopic,
                    "Error",
                    false,
                    60);
                this.mqttClient.Subscribe(MqttTopics, new[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
                this.mqttClient.Publish(GarageDoorStatusTopic, Encoding.UTF8.GetBytes("Connected " + DateTime.Now), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                this.mqttDispatcherTimer.Start();
            }
        }

        private void DebounceTimerOnTick(object sender, object o)
        {
            ((DispatcherTimer)sender).Stop();
            this.SendStatus();
        }

        private void HookEvents()
        {
            var doorOpenSwitch = this.Inputs.First(input => input.Id == 6);
            doorOpenSwitch.PropertyChanged += this.OnPropertyChanged;
            var doorPartialOpenSwitch = this.Inputs.First(input => input.Id == 7);
            doorPartialOpenSwitch.PropertyChanged += this.OnPropertyChanged;
        }

        private void IfttMakerSendEvent(string state)
        {
            // Send to IFTTT Maker Channel
            if (this.makerChannel != null)
            {
                var eventName = "garage_door_" + state.ToLowerInvariant().Replace(' ', '_');

                this.makerChannel?.SendEvent(eventName);
            }
        }

        private void LoadResourceStrings()
        {
            // NOTE: These come from Strings/en-US/Resources.resw which you'll need to provide/edit to match
            var resource = ResourceLoader.GetForCurrentView();
            var newUri = resource.GetString("AutoRemoteNotificationUri");
            this.autoRemoteUri = newUri.Replace("[AUTOREMOTEKEY]", resource.GetString("AutoRemoteKey"));
            autoRemotePassword = resource.GetString("AutoRemotePassword");
            makerKey = resource.GetString("IFTTTMakerSecretKey");
            adafruitIoKey = resource.GetString("AdafruitIoKey");
            adafruitIoFeedName = resource.GetString("AdafruitIoFeedName");
            mqttUserName = resource.GetString("MqttUserName");
            mqttPassword = resource.GetString("MqttPassword");
        }

        private void MqttDispatcherTimerOnTick(object sender, object o)
        {
            this.mqttDispatcherTimer.Stop();
            if (this.mqttClient.IsConnected)
            {
                this.mqttClient.Disconnect();
            }
            this.ConnectMqttClient();
            if (this.mqttClient.IsConnected)
            {
                this.debounceTimer.Start();
            }
            else
            {
                this.mqttDispatcherTimer.Start();
            }
        }

        private void MqttPublish(string state)
        {
            if (this.mqttClient?.IsConnected == true)
            {
                try
                {
                    this.mqttClient.Publish(
                        GarageDoorStatusTopic,
                        Encoding.UTF8.GetBytes("Publish at " + DateTime.Now),
                        MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE,
                        true);
                    this.mqttClient.Publish(
                        "garage/door/state",
                        Encoding.UTF8.GetBytes(state),
                        MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE,
                        true);
                    this.mqttClient.Publish(
                        "garage/door/binary",
                        this.garageDoor.IsClosed ? Encoding.UTF8.GetBytes("OFF") : Encoding.UTF8.GetBytes("ON"),
                        MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE,
                        true);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    this.mqttDispatcherTimer.Start();
                }
            }
        }

        private void OnMqttConnectionClosed(object sender, EventArgs e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => this.mqttDispatcherTimer.Start());
        }

        private void OnMqttPublishedMessage(object sender, MqttMsgPublishEventArgs e)
        {
            if (e.Message.Length == 0)
            {
                return;
            }
            var message = Encoding.UTF8.GetString(e.Message).ToLowerInvariant();
            switch (e.Topic)
            {
                case @"garage/door/switch":
                    if (message == "on")
                    {
                        DispatcherHelper.CheckBeginInvokeOnUI(() => this.garageDoor.Open());
                    }
                    else if (message == "off")
                    {
                        DispatcherHelper.CheckBeginInvokeOnUI(() => this.garageDoor.Close());
                    }
                    break;

                case @"garage/door/method":
                    switch (message)
                    {
                        case "open":
                            DispatcherHelper.CheckBeginInvokeOnUI(() => this.garageDoor.Open());
                            break;

                        case "partialopen":
                            DispatcherHelper.CheckBeginInvokeOnUI(() => this.garageDoor.PartialOpen());
                            break;

                        case "close":
                            DispatcherHelper.CheckBeginInvokeOnUI(() => this.garageDoor.Close());
                            break;

                        default:
                            DispatcherHelper.CheckBeginInvokeOnUI(() => this.SendStatus());
                            break;
                    }
                    break;
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsOn")
            {
                var input = (GpioPinViewModel)sender;
                if (input.Id == 6)
                {
                    doorProducer.EmitIsOpenChanged();
                }
                else
                {
                    doorProducer.EmitIsPartiallyOpenChanged();
                }
                this.debounceTimer.Start();
            }
        }

        private void ProcessMessage(Message m, Device device)
        {
            switch (m.message.ToLowerInvariant())
            {
                case "open":
                    this.garageDoor.Open();
                    break;

                case "partialopen":
                case "partial":
                case "openpartial":
                    this.garageDoor.PartialOpen();
                    break;

                case "close":
                    this.garageDoor.Close();
                    break;
            }
            this.SendStatus(device);
        }

        private void SendStatus(Device device = null)
        {
            if (string.IsNullOrWhiteSpace(this.autoRemoteUri) && this.makerChannel == null)
            {
                return;
            }

            // Debounce signals so we don't spam AutoRemote/IFTTT Maker/etc
            if (this.debounceTimer.IsEnabled)
            {
                return;
            }

            var state = this.SetState();

            this.MqttPublish(state);

            this.IfttMakerSendEvent(state);

            this.AdafruitIoCreateData();

            this.AutoRemoteSend(device, state);
        }

        private string SetState()
        {
            var state = "Closed";
            if (this.adafruitIo != null) this.adafruitIo.Data.Value = "0";
            if (this.garageDoor.IsOpen)
            {
                if (this.adafruitIo != null) this.adafruitIo.Data.Value = "100";
                state = "Open";
            }
            if (this.garageDoor.IsPartiallyOpen)
            {
                if (this.adafruitIo != null) this.adafruitIo.Data.Value = "5";
                state = "Partially Open";
            }
            return state;
        }

        private void SetupAllJoynBusAttachmentAndProducer()
        {
            var attachment = new AllJoynBusAttachment();
            attachment.AboutData.DefaultDescription = "PiFaceDigital 2 Garage Door controller";
            attachment.AboutData.ModelNumber = "PiFaceDigital2";
            doorProducer = new GarageDoorProducer(attachment) { Service = new GarageDoorService(this.garageDoor) };
            doorProducer.Start();
        }

        private void SetupAutoRemoteHttpServer()
        {
            this.autoRemoteHttpServer = new AutoRemoteHttpServer(ConstantsThatShouldBeVariables.PORT);
            this.autoRemoteHttpServer.RequestReceived += this.AutoRemoteHttpServerOnRequestReceived;
            this.autoRemoteHttpServer.Start();
            RegisterMyselfOnOtherDevice();
        }

        private void SetupInputOutputNames()
        {
            var doorOpenSwitch = this.Inputs.First(input => input.Id == 6);
            doorOpenSwitch.Name = GarageDoorOpen;
            var doorPartialOpenSwitch = this.Inputs.First(input => input.Id == 7);
            doorPartialOpenSwitch.Name = GarageDoorPartialOpen;
            var pb = this.Outputs.First(o => o.Id == 0);
            pb.Name = GarageDoorPushButtonRelay;
        }

        private void TimerOnTick(object sender, object e)
        {
            this.UpdateValuesFromPi();
        }
    }
}