// ***********************************************************************
// Assembly         : HastPiControl
// Author           : Jon Benson
// Created          : 14-11-2015
// 
// Last Modified By : Jon Benson
// Last Modified On : 20-03-2016
// ***********************************************************************
// <copyright file="PiFaceDigital2ViewModel.cs" company="Champion Data">
//     Copyright (c) Champion Data. All rights reserved.
// </copyright>
// ***********************************************************************

namespace HastPiControl.Models
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using Windows.ApplicationModel.Resources;
    using Windows.Devices.AllJoyn;
    using Windows.UI.Core;
    using Windows.UI.Xaml;

    using AutoRemotePlugin.AutoRemote.Devices;

    using com.hastarin.GarageDoor;

    using GalaSoft.MvvmLight;

    using Hastarin.Devices;

    using HastPiControl.AutoRemote;
    using HastPiControl.AutoRemote.Communications;

    /// <summary>Class PiFaceDigital2ViewModel.</summary>
    public class PiFaceDigital2ViewModel : ViewModelBase, IDisposable
    {
        private const long MinimumInterval = 2000;

        /// <summary>Name to use for the input for the main door reed switch</summary>
        public static string GarageDoorOpen = "GarageDoorOpen";

        /// <summary>Name to use for the input for the door partial open reed switch</summary>
        public static string GarageDoorPartialOpen = "GarageDoorPartialOpen";

        /// <summary>Name to use for the output controlling the push button relay</summary>
        public static string GarageDoorPushButtonRelay = "GarageDoorPushButton";

        private static GarageDoorProducer doorProducer;

        private static string autoRemotePassword;

        private readonly string autoRemoteUri;

        private readonly DispatcherTimer debounceTimer = new DispatcherTimer();

        private readonly CoreDispatcher dispatcher;

        private readonly GarageDoor garageDoor;

        private readonly Stopwatch minimumIntervalStopwatch = new Stopwatch();

        private bool disposed;

        private ObservableCollection<GpioPinViewModel> inputs = new ObservableCollection<GpioPinViewModel>();

        private ObservableCollection<GpioPinViewModel> outputs = new ObservableCollection<GpioPinViewModel>();

        private DispatcherTimer timer;

        /// <summary>Initializes a new instance of the ViewModelBase class.</summary>
        public PiFaceDigital2ViewModel()
        {
            this.dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;

            for (int i = 0; i < 8; i++)
            {
                this.Inputs.Add(new GpioPinViewModel((byte)i) { Id = i, Name = "Input " + i });
                var j = i + 8;
                this.Outputs.Add(new GpioPinViewModel((byte)j, true) { Id = i, Name = "Output " + i });
            }
            var resource = ResourceLoader.GetForCurrentView();
            var newUri = resource.GetString("AutoRemoteNotificationUri");
            this.autoRemoteUri = newUri.Replace("[AUTOREMOTEKEY]", resource.GetString("AutoRemoteKey"));
            autoRemotePassword = resource.GetString("AutoRemotePassword");
            var attachment = new AllJoynBusAttachment();
            attachment.AboutData.DefaultDescription = "PiFaceDigital 2 test application";
            attachment.AboutData.ModelNumber = "PiFaceDigital2";

            // ReSharper disable once ExceptionNotDocumented
            this.debounceTimer.Interval = TimeSpan.FromMilliseconds(MinimumInterval);
            this.debounceTimer.Tick += this.DebounceTimerOnTick;

            var doorOpenSwitch = this.Inputs.First(input => input.Id == 6);
            doorOpenSwitch.PropertyChanged += this.OnPropertyChanged;
            doorOpenSwitch.Name = GarageDoorOpen;
            var doorPartialOpenSwitch = this.Inputs.First(input => input.Id == 7);
            doorPartialOpenSwitch.Name = GarageDoorPartialOpen;
            doorPartialOpenSwitch.PropertyChanged += this.OnPropertyChanged;
            var pb = this.Outputs.First(o => o.Id == 0);
            pb.Name = GarageDoorPushButtonRelay;

            this.garageDoor = new GarageDoor(this);
            doorProducer = new GarageDoorProducer(attachment) { Service = new GarageDoorService(this.garageDoor) };
            doorProducer.Start();
            var autoRemoteHttpServer = new AutoRemoteHttpServer(ConstantsThatShouldBeVariables.PORT);
            autoRemoteHttpServer.RequestReceived += this.AutoRemoteHttpServerOnRequestReceived;
            RegisterMyselfOnOtherDevice();
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
            this.SendStatus();
        }

        private void DebounceTimerOnTick(object sender, object o)
        {
            ((DispatcherTimer)sender).Stop();
            this.SendStatus();
        }

        private void AutoRemoteHttpServerOnRequestReceived(Request request)
        {
            if (request.communication_base_params.type != "Message")
            {
                return;
            }
            var m = request as Message;
            if (m == null || m.password != autoRemotePassword)
            {
                return;
            }
            this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.ProcessMessage(m));
        }

        private void ProcessMessage(Message m)
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
                default:
                    this.SendStatus();
                    break;
            }
        }

        private void SendStatus()
        {
            if (string.IsNullOrWhiteSpace(this.autoRemoteUri))
            {
                return;
            }
            var sw = this.minimumIntervalStopwatch;
            if (sw.IsRunning && sw.ElapsedMilliseconds < MinimumInterval)
            {
                return;
            }
            sw.Restart();
            var device = GetAutoRemoteLocalDevice();
            var state = "Closed";
            if (this.garageDoor.IsOpen)
            {
                state = "Open";
            }
            if (this.garageDoor.IsPartiallyOpen)
            {
                state = "Partially Open";
            }
            var m = new Message { message = state + "=:=GarageDoor" };
            m.Send(device);
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

        private void TimerOnTick(object sender, object e)
        {
            this.UpdateValuesFromPi();
        }

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

        private static void RegisterMyselfOnOtherDevice()
        {
            var device = GetAutoRemoteLocalDevice();

            //Instantiate Registration Request
            RequestSendRegistration request = new RequestSendRegistration();

            //Send registration request
            request.Send(device);
        }

        private static Device GetAutoRemoteLocalDevice()
        {
            Device device;
            var resource = ResourceLoader.GetForCurrentView();
            //Instantiate Device (device to send stuff to). In a proper app this device should have been created with a received RequestSendRegistration
            String personalKey = resource.GetString("AutoRemoteKey");
            //see how to get it here http://joaoapps.com/autoremote/personal

            var localip = resource.GetString("LocalDeviceIp");
            device = new Device { localip = localip, port = "1817", key = personalKey };
            return device;
        }

        /// <summary>
        ///     Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by
        ///     garbage collection.
        /// </summary>
        ~PiFaceDigital2ViewModel()
        {
            this.Dispose(false);
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
            }

            // release any unmanaged objects
            // set the object references to null
            doorProducer.Stop();
            doorProducer = null;

            this.disposed = true;
        }
    }
}