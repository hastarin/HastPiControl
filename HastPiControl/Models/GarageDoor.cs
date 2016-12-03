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
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using GalaSoft.MvvmLight;

    /// <summary>Wrapper for the garage door connected to a Raspberry Pi 2</summary>
    public class GarageDoor : ViewModelBase
    {
        private readonly GpioPinViewModel openInput;

        private readonly GpioPinViewModel partialOpenInput;

        private readonly PiFaceDigital2ViewModel piFaceDigital2ViewModel;

        private readonly GpioPinViewModel pushButtonRelay;

        /// <summary>Initializes a new instance of the <see cref="GarageDoor" /> class.</summary>
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
        }

        /// <summary>Gets a value inidcating if the door is open.</summary>
        public bool IsOpen => this.openInput.IsOn;

        /// <summary>Gets a value that indicates if the door is partially open.</summary>
        public bool IsPartiallyOpen => this.partialOpenInput.IsOn;

        /// <summary>Gets a value indicating if the door is closed.</summary>
        public bool IsClosed => this.openInput.IsOn == false && this.IsPartiallyOpen == false;

        /// <summary>Close the garage door</summary>
        public async Task Close()
        {
            while (this.openInput.IsOn || this.partialOpenInput.IsOn)
            {
                await this.PushButton();
                await Task.Delay(15000);
            }
        }

        /// <summary>Opens the garage door</summary>
        public async Task Open()
        {
            if (this.IsPartiallyOpen || this.IsClosed)
            {
                await this.PushButton();
            }
        }

        /// <summary>Partially opens the garage door</summary>
        public async Task PartialOpen()
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
        public async Task PushButton()
        {
            this.pushButtonRelay.IsOn = true;
            await Task.Delay(500);
            this.pushButtonRelay.IsOn = false;
        }
    }
}