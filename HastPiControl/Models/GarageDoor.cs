// ***********************************************************************
// Assembly         : HastPiControl
// Author           : Jon Benson
// Created          : 20-03-2016
//
// Last Modified By : Jon Benson
// Last Modified On : 26-03-2016
// ***********************************************************************

namespace HastPiControl.Models
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using GalaSoft.MvvmLight;

    using Windows.UI.Xaml;

    /// <summary>Wrapper for the garage door connected to a Raspberry Pi 2</summary>
    public class GarageDoor : ViewModelBase
    {
        private readonly DispatcherTimer moveTimer = new DispatcherTimer();

        private readonly GpioPinViewModel openInput;

        private readonly GpioPinViewModel partialOpenInput;

        private readonly PiFaceDigital2ViewModel piFaceDigital2ViewModel;

        private readonly GpioPinViewModel pushButtonRelay;

        private string state;

        /// <summary>Initializes a new instance of the <see cref="GarageDoor" /> class.</summary>
        public GarageDoor(PiFaceDigital2ViewModel piFaceDigital2ViewModel)
        {
            this.piFaceDigital2ViewModel = piFaceDigital2ViewModel;
            this.pushButtonRelay =
                this.piFaceDigital2ViewModel.Outputs.Single(
                    o => o.Name == PiFaceDigital2ViewModel.GarageDoorPushButtonRelay);
            this.partialOpenInput =
                this.piFaceDigital2ViewModel.Inputs.Single(
                    o => o.Name == PiFaceDigital2ViewModel.GarageDoorPartialOpen);
            this.openInput =
                this.piFaceDigital2ViewModel.Inputs.Single(o => o.Name == PiFaceDigital2ViewModel.GarageDoorOpen);

            this.openInput.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == "IsOn")
                    {
                        this.RaisePropertyChanged(() => this.IsOpen);
                        this.RaisePropertyChanged(() => this.IsClosed);
                    }
                };
            this.partialOpenInput.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == "IsOn")
                    {
                        this.RaisePropertyChanged(() => this.IsPartiallyOpen);
                        this.RaisePropertyChanged(() => this.IsClosed);
                    }
                };

            this.moveTimer.Interval = TimeSpan.FromSeconds(15);
            this.moveTimer.Tick += (sender, o) =>
                {
                    this.IsMoving = false;
                    this.State = this.IsOpen ? "open" : "closed";
                };
        }

        /// <summary>Gets a value indicating if the door is closed.</summary>
        public bool IsClosed => this.openInput.IsOn == false && this.IsPartiallyOpen == false;

        public bool IsMoving { get; set; }

        /// <summary>Gets a value indicating if the door is open.</summary>
        public bool IsOpen => this.openInput.IsOn || this.partialOpenInput.IsOn;

        /// <summary>Gets a value that indicates if the door is partially open.</summary>
        public bool IsPartiallyOpen => this.partialOpenInput.IsOn;

        public string State
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.state))
                {
                    this.state = this.IsOpen ? "open" : "closed";
                }

                return this.state;
            }

            set => this.Set(ref this.state, value);
        }

        /// <summary>Close the garage door</summary>
        public async Task Close()
        {
            if ((this.IsPartiallyOpen || this.IsOpen) && !this.IsMoving)
            {
                this.State = "closing";
                await this.PushButton();
            }
        }

        /// <summary>Opens the garage door</summary>
        public async Task Open()
        {
            if ((this.IsPartiallyOpen || this.IsClosed) && !this.IsMoving)
            {
                this.State = "opening";
                await this.PushButton();
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

            this.IsMoving = true;
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
            this.IsMoving = false;
        }

        /// <summary>Triggers the push button of the garage door</summary>
        public async Task PushButton()
        {
            this.pushButtonRelay.IsOn = true;
            await Task.Delay(500);
            this.pushButtonRelay.IsOn = false;

            if (this.IsMoving)
            {
                this.IsMoving = false;
            }
            else
            {
                // TODO: Improve this?
                this.IsMoving = true;
                this.moveTimer.Start();
            }
        }

        public async Task Stop()
        {
            if (this.IsMoving)
            {
                this.pushButtonRelay.IsOn = true;
                await Task.Delay(500);
                this.pushButtonRelay.IsOn = false;
            }

            this.State = this.IsOpen ? "open" : "closed";
        }
    }
}