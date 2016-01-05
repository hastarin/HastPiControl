// ***********************************************************************
// Assembly         : HastPiControl
// Author           : Jon Benson
// Created          : 14-11-2015
// 
// Last Modified By : Jon Benson
// Last Modified On : 14-11-2015
// ***********************************************************************

namespace HastPiControl.Models
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Windows.Devices.AllJoyn;
    using Windows.UI.Xaml;

    using com.hastarin.GarageDoor;

    using GalaSoft.MvvmLight;

    using Hastarin.Devices;

    /// <summary>
    /// Class PiFaceDigital2ViewModel.
    /// </summary>
    public class PiFaceDigital2ViewModel : ViewModelBase, IDisposable
    {
        /// <summary>
        /// Name to use for the input for the main door reed switch
        /// </summary>
        public static string GarageDoorOpen = "GarageDoorOpen";

        /// <summary>
        /// Name to use for the input for the door partial open reed switch
        /// </summary>
        public static string GarageDoorPartialOpen = "GarageDoorPartialOpen";

        /// <summary>
        /// Name to use for the output controlling the push button relay
        /// </summary>
        public static string GarageDoorPushButtonRelay = "GarageDoorPushButton";

        private ObservableCollection<GpioPinViewModel> inputs = new ObservableCollection<GpioPinViewModel>();

        private ObservableCollection<GpioPinViewModel> outputs = new ObservableCollection<GpioPinViewModel>();

        private DispatcherTimer timer;

        private static GarageDoorProducer doorProducer;

        /// <summary>
        /// Initializes a new instance of the ViewModelBase class.
        /// </summary>
        public PiFaceDigital2ViewModel()
        {
            for (int i = 0; i < 8; i++)
            {
                this.Inputs.Add(new GpioPinViewModel((byte)i) { Id = i, Name = "Input " + i });
                var j = i + 8;
                this.Outputs.Add(new GpioPinViewModel((byte)j, isOutput: true) { Id = i, Name = "Output " + i });
            }
            var attachment = new AllJoynBusAttachment();
            doorProducer = new GarageDoorProducer(attachment) { Service = new GarageDoorService(this) };
            doorProducer.Start();
            var doorOpenSwitch = this.Inputs.First(input => input.Id == 6);
            doorOpenSwitch.PropertyChanged += this.OnPropertyChanged;
            doorOpenSwitch.Name = GarageDoorOpen;
            var doorPartialOpenSwitch = this.Inputs.First(input => input.Id == 7);
            doorPartialOpenSwitch.Name = GarageDoorPartialOpen;
            doorPartialOpenSwitch.PropertyChanged += this.OnPropertyChanged;
            var pb = this.Outputs.First(o => o.Id == 0);
            pb.Name = GarageDoorPushButtonRelay;
        }

        private async void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsOn")
            {
                var input = (GpioPinViewModel)sender;
                if (input.Id == 6)
                {
                    if (((GpioPinViewModel)sender).IsOn)
                    {
                        using (var client = new HttpClient())
                        {
                            var autoRemoteMessage = new Uri("https://goo.gl/<YourURLHere>");
                            await client.GetAsync(autoRemoteMessage);
                        }
                    }
                    doorProducer.EmitIsOpenChanged();
                }
                else
                {
                    doorProducer.EmitIsPartiallyOpenChanged();
                }
            }
        }

        public async void PushButton()
        {
            var output = this.Outputs.SingleOrDefault(o => o.Name == PiFaceDigital2ViewModel.GarageDoorPushButtonRelay);

            if (output != null)
            {
                output.IsOn = true;

                await Task.Delay(1000);

                output.IsOn = false;
            }
        }

        /// <summary>
        /// Initializes the pi face.
        /// </summary>
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

        private void TimerOnTick(object sender, object e)
        {
            var result = MCP23S17.ReadRegister16();    // do something with the values
            for (int i = 0; i < 8; i++)
            {
                var bitIsOn = (result & (1 << i)) != 0;
                this.Inputs.First(input => input.Id == i).IsOn = !bitIsOn;
                bitIsOn = (result & (1 << (i+8))) != 0;
                this.Outputs.First(input => input.Id == i).IsOn = bitIsOn;
            }
        }

        /// <summary>
        /// Gets or sets the inputs.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the outputs.
        /// </summary>
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

        private bool disposed;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.
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