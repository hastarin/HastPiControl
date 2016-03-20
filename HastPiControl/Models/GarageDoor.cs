// ***********************************************************************
// Assembly         : HastPiControl
// Author           : Jon Benson
// Created          : 20-03-2016
// 
// Last Modified By : Jon Benson
// Last Modified On : 20-03-2016
// ***********************************************************************

namespace HastPiControl.Models
{
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    /// <summary>Wrapper for the garage door connected to a Raspberry Pi 2</summary>
    public class GarageDoor : INotifyPropertyChanged
    {
        private readonly GpioPinViewModel openInput;

        private readonly GpioPinViewModel partialOpenInput;

        private readonly PiFaceDigital2ViewModel piFaceDigital2ViewModel;

        private readonly GpioPinViewModel pushButtonRelay;

        private PropertyChangedEventHandler propertyChanged;

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public GarageDoor(PiFaceDigital2ViewModel piFaceDigital2ViewModel)
        {
            this.piFaceDigital2ViewModel = piFaceDigital2ViewModel;
            this.pushButtonRelay =
                this.piFaceDigital2ViewModel.Outputs.Single(
                    o => o.Name == PiFaceDigital2ViewModel.GarageDoorPushButtonRelay);
            this.partialOpenInput =
                this.piFaceDigital2ViewModel.Inputs.Single(o => o.Name == PiFaceDigital2ViewModel.GarageDoorPartialOpen);
            this.openInput =
                this.piFaceDigital2ViewModel.Inputs.Single(o => o.Name == PiFaceDigital2ViewModel.GarageDoorOpen);

            this.openInput.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == "IsOn")
                    {
                        this.OnPropertyChanged("IsOpen");
                        this.OnPropertyChanged("IsClosed");
                    }
                };
            this.partialOpenInput.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == "IsOn")
                    {
                        this.OnPropertyChanged("IsPartiallyOpen");
                        this.OnPropertyChanged("IsClosed");
                    }
                };
        }

        /// <summary>Gets a value inidcating if the door is open.</summary>
        public bool IsOpen => this.openInput.IsOn;

        /// <summary>Gets a value that indicates if the door is partially open.</summary>
        public bool IsPartiallyOpen => this.partialOpenInput.IsOn;

        /// <summary>Gets a value indicating if the door is closed.</summary>
        public bool IsClosed => this.openInput.IsOn == false && this.IsPartiallyOpen == false;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                this.propertyChanged += value;
            }
            remove
            {
                this.propertyChanged -= value;
            }
        }

        /// <summary>Close the garage door</summary>
        public async void Close()
        {
            while (this.openInput.IsOn || this.partialOpenInput.IsOn)
            {
                this.PushButton();
                await Task.Delay(15000);
            }
        }

        /// <summary>Opens the garage door</summary>
        public void Open()
        {
            if (this.IsPartiallyOpen || this.IsClosed)
            {
                this.PushButton();
            }
        }

        /// <summary>Partially opens the garage door</summary>
        public async void PartialOpen()
        {
            var open = this.openInput;
            var partialOpen = this.partialOpenInput;
            var output = this.pushButtonRelay;

            if (open.IsOn || partialOpen.IsOn)
            {
                return;
            }
            output.IsOn = true;
            await Task.Delay(200);
            output.IsOn = false;
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            while (!partialOpen.IsOn && stopWatch.ElapsedMilliseconds < 1000)
            {
                this.piFaceDigital2ViewModel.UpdateValuesFromPi();
            }
            stopWatch.Stop();
            output.IsOn = true;
            await Task.Delay(500);
            output.IsOn = false;
        }

        /// <summary>Triggers the push button of the garage door</summary>
        public async void PushButton()
        {
            this.pushButtonRelay.IsOn = true;
            await Task.Delay(500);
            this.pushButtonRelay.IsOn = false;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler changedEventHandler = this.propertyChanged;
            if (changedEventHandler == null)
            {
                return;
            }
            changedEventHandler(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return false;
            }
            storage = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }
    }
}