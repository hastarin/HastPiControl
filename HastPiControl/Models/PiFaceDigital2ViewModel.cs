// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PiFaceDigital2ViewModel.cs" company="NA">
//   Jon Benson
// </copyright>
// <summary>
//   Class PiFaceDigital2ViewModel.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HastPiControl.Models
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Security.Authentication;
    using System.Text;
    using System.Threading.Tasks;

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

    using MQTTnet;
    using MQTTnet.Client.Options;
    using MQTTnet.Diagnostics;
    using MQTTnet.Extensions.ManagedClient;

    using Windows.ApplicationModel.Resources;
    using Windows.Devices.AllJoyn;
    using Windows.Networking;
    using Windows.UI.Xaml;

    using MQTTnet.Client.Connecting;

    /// <summary>Class PiFaceDigital2ViewModel.</summary>
    public class PiFaceDigital2ViewModel : ViewModelBase, IDisposable
    {
        /// <summary>MQTT Topic for publishing the status of this app.</summary>
        /// <remarks>Eg. Connected/Published/Disconnected/Error</remarks>
        private const string GarageDoorStatusTopic = "hass/cover/garage/status";

        private const long MinimumInterval = 2000;

        /// <summary>Name to use for the input for the main door reed switch</summary>
        public static string GarageDoorOpen = "GarageDoorOpen";

        /// <summary>Name to use for the input for the door partial open reed switch</summary>
        public static string GarageDoorPartialOpen = "GarageDoorPartialOpen";

        /// <summary>Name to use for the output controlling the push button relay</summary>
        public static string GarageDoorPushButtonRelay = "GarageDoorPushButton";

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

        private readonly IManagedMqttClient mqttClient;

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
                Console.WriteLine(
                    @"You may need to downgrade Microsoft.Rest.ClientRuntime to 2.3.2   See https://github.com/Azure/autorest/issues/1542 for more information.");
            }

            // ReSharper disable once CommentTypo
            // NOTE: These resources come from Strings/en-US/Resources.resw which you'll need to provide/edit to match
            var resource = ResourceLoader.GetForCurrentView();
            var mqttHost = resource.GetString("MqttHost"); // Host MUST match certificate name if using TLS

            if (!string.IsNullOrWhiteSpace(mqttHost))
            {
                var tlsOptions = new MqttClientOptionsBuilderTlsParameters
                                     {
                                         UseTls = true, SslProtocol = SslProtocols.Tls12
                                     };

                var willMessage = new MqttApplicationMessageBuilder().WithTopic("hass/cover/garage/availability")
                    .WithAtLeastOnceQoS().WithRetainFlag().WithPayload("offline").Build();

                // Setup and start a managed MQTT client.
                var options = new ManagedMqttClientOptionsBuilder().WithAutoReconnectDelay(TimeSpan.FromSeconds(30))
                    .WithClientOptions(
                        new MqttClientOptionsBuilder().WithClientId("garage-pi").WithTcpServer(mqttHost).WithWillMessage(willMessage)
                            .WithCleanSession().WithCredentials(mqttUserName, mqttPassword).WithTls(tlsOptions).Build())
                    .Build();

                this.mqttClient = new MqttFactory().CreateManagedMqttClient();
                MqttNetGlobalLogger.LogMessagePublished += (s, e) =>
                    {
                        var trace =
                            $">> [{e.LogMessage.Timestamp:O}] [{e.LogMessage.ThreadId}] [{e.LogMessage.Source}] [{e.LogMessage.Level}]: {e.LogMessage.Message}";
                        if (e.LogMessage.Exception != null)
                        {
                            trace += Environment.NewLine + e.LogMessage.Exception;
                        }

                        Debug.WriteLine(trace);
                    };

                this.mqttClient.ConnectedHandler = new MqttClientConnectedHandlerDelegate(
                    async e =>
                        {
                            await this.mqttClient.SubscribeAsync(
                                new MqttTopicFilterBuilder().WithAtMostOnceQoS().WithTopic("hass/cover/garage/set")
                                    .Build());
                            await this.mqttClient.PublishAsync(
                                new MqttApplicationMessageBuilder().WithAtLeastOnceQoS()
                                    .WithTopic("hass/cover/garage/availability").WithPayload("online").WithRetainFlag()
                                    .Build());
                            var configMessage = new MqttApplicationMessageBuilder().WithAtLeastOnceQoS().WithTopic("hass/cover/garage/config")
                                .WithPayload(@"{""name"":""Garage Cover"",""uniq_id"":""hassgarage2103029"",""dev_cla"":""garage"",""avty_t"":""hass/cover/garage/availability"",""cmd_t"":""hass/cover/garage/set"",""stat_t"":""hass/cover/garage/state"",""ret"":false}")
                                .WithRetainFlag().Build();
                            await this.mqttClient.PublishAsync(configMessage);
                            this.MqttPublish();
                        });

                this.garageDoor.PropertyChanged += delegate(object sender, PropertyChangedEventArgs args)
                    {
                        if (args.PropertyName == "State")
                        {
                            this.MqttPublish();
                        }
                    };
                this.mqttClient.StartAsync(options);
                this.mqttClient.UseApplicationMessageReceivedHandler(e =>
                        {
                            Debug.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
                            Debug.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                            var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload).ToUpperInvariant();
                            Debug.WriteLine($"+ Payload = {payload}");
                            Debug.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
                            Debug.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
                            Debug.WriteLine(string.Empty);

                            switch (payload)
                            {
                                case "OPEN":
                                    DispatcherHelper.CheckBeginInvokeOnUI(() => this.garageDoor.Open());
                                    break;
                                case "CLOSE":
                                    DispatcherHelper.CheckBeginInvokeOnUI(() => this.garageDoor.Close());
                                    break;
                                case "STOP":
                                    DispatcherHelper.CheckBeginInvokeOnUI(() => this.garageDoor.Stop());
                                    break;
                            }
                        });
            }

            this.HookEvents();
            this.debounceTimer.Start();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="PiFaceDigital2ViewModel"/> class.
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
            get => this.inputs;
            set => this.Set(ref this.inputs, value);
        }

        /// <summary>Gets or sets the outputs.</summary>
        /// <value>The outputs.</value>
        public ObservableCollection<GpioPinViewModel> Outputs
        {
            get => this.outputs;
            set => this.Set(ref this.outputs, value);
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Initializes the pi face.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task InitializePiFace()
        {
            await MCP23S17.InitializeSpiDevice();
            MCP23S17.Initialize();

            // ReSharper disable CommentTypo
            MCP23S17.SetPinMode(0x00FF); // 0x0000 = all outputs, 0xffff=all inputs, 0x00FF is PIFace Default
            MCP23S17.SetPullUpMode(0x00FF); // 0x0000 = no pullups, 0xffff=all pullups, 0x00FF is PIFace Default
            MCP23S17.WriteWord(0x0000); // 0x0000 = no pullups, 0xffff=all pullups, 0x00FF is PIFace Default

            // ReSharper restore CommentTypo

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
                this.mqttClient.Dispose();
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

            // Instantiate Device (device to send stuff to). In a proper app this device should have been created with a received RequestSendRegistration
            string personalKey = resource.GetString("AutoRemoteKey");

            // see how to get it here http://joaoapps.com/autoremote/personal
            var localIp = resource.GetString("LocalDeviceIp");
            var device = new Device { localip = localIp, port = "1817", key = personalKey };
            return device;
        }

        private static void RegisterMyselfOnOtherDevice()
        {
            var device = GetAutoRemoteLocalDevice();

            // Instantiate Registration Request
            RequestSendRegistration request = new RequestSendRegistration();

            // Send registration request
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
            // ReSharper disable once CommentTypo
            // NOTE: These come from Strings/en-US/Resources.resw which you'll need to provide/edit to match
            var resource = ResourceLoader.GetForCurrentView();
            var newUri = resource.GetString("AutoRemoteNotificationUri");

            // ReSharper disable once StringLiteralTypo
            this.autoRemoteUri = newUri.Replace("[AUTOREMOTEKEY]", resource.GetString("AutoRemoteKey"));
            autoRemotePassword = resource.GetString("AutoRemotePassword");
            makerKey = resource.GetString("IFTTTMakerSecretKey");
            adafruitIoKey = resource.GetString("AdafruitIoKey");
            adafruitIoFeedName = resource.GetString("AdafruitIoFeedName");
            mqttUserName = resource.GetString("MqttUserName");
            mqttPassword = resource.GetString("MqttPassword");
        }

        private void MqttPublish()
        {
            if (this.mqttClient?.IsConnected == true)
            {
                try
                {
                    var message = new MqttApplicationMessageBuilder().WithTopic(GarageDoorStatusTopic)
                        .WithPayload("Publish at " + DateTime.Now).WithAtMostOnceQoS().WithRetainFlag().Build();
                    this.mqttClient.PublishAsync(message);
                    message = new MqttApplicationMessageBuilder().WithTopic("hass/cover/garage/state").WithPayload(this.garageDoor.State)
                        .WithAtMostOnceQoS().WithRetainFlag().Build();
                    this.mqttClient.PublishAsync(message);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
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

            this.IfttMakerSendEvent(state);

            this.AdafruitIoCreateData();

            this.AutoRemoteSend(device, state);
        }

        private string SetState()
        {
            var state = "Closed";
            if (this.adafruitIo != null)
            {
                this.adafruitIo.Data.Value = "0";
            }

            if (this.garageDoor.IsOpen)
            {
                if (this.adafruitIo != null)
                {
                    this.adafruitIo.Data.Value = "100";
                }

                state = "Open";
            }

            if (this.garageDoor.IsPartiallyOpen)
            {
                if (this.adafruitIo != null)
                {
                    this.adafruitIo.Data.Value = "5";
                }

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