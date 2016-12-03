// ***********************************************************************
// Assembly         : HastPiControl
// Author           : Jon Benson
// Created          : 05-01-2016
// 
// Last Modified By : Jon Benson
// Last Modified On : 26-03-2016
// ***********************************************************************

namespace HastPiControl
{
    using System;
    using System.Threading.Tasks;

    using Windows.Devices.AllJoyn;
    using Windows.Foundation;
    using Windows.UI.Core;

    using com.hastarin.GarageDoor;

    using HastPiControl.Models;

    /// <summary>My first garage door AllJoyn service</summary>
    public class GarageDoorService : IGarageDoorService
    {
        private readonly CoreDispatcher dispatcher;

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public GarageDoorService(GarageDoor garageDoor)
        {
            this.GarageDoor = garageDoor;
            this.dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
        }

        /// <summary>Represents the garage door state</summary>
        public GarageDoor GarageDoor { get; set; }

        /// <summary>Opens the garage door.</summary>
        /// <param name="info">The information.</param>
        /// <param name="partialOpen">if set to <c>true</c> [partial open].</param>
        public IAsyncOperation<GarageDoorOpenResult> OpenAsync(AllJoynMessageInfo info, bool partialOpen)
        {
            var task = new Task<GarageDoorOpenResult>(
                () =>
                    {
                        if (!partialOpen)
                        {
                            this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.GarageDoor.PushButton());
                        }
                        else
                        {
                            this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.GarageDoor.PartialOpen());
                        }
                        return GarageDoorOpenResult.CreateSuccessResult();
                    });
            task.Start();
            return task.AsAsyncOperation();
        }

        /// <summary>Closes the garage door.</summary>
        /// <param name="info">The information.</param>
        public IAsyncOperation<GarageDoorCloseResult> CloseAsync(AllJoynMessageInfo info)
        {
            var task = new Task<GarageDoorCloseResult>(
                () =>
                    {
                        this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.GarageDoor.Close());
                        return GarageDoorCloseResult.CreateSuccessResult();
                    });
            task.Start();
            return task.AsAsyncOperation();
        }

        /// <summary>Triggers the push button action for the garage door.</summary>
        /// <param name="info">The information.</param>
        public IAsyncOperation<GarageDoorPushButtonResult> PushButtonAsync(AllJoynMessageInfo info)
        {
            var task = new Task<GarageDoorPushButtonResult>(
                () =>
                    {
                        this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.GarageDoor.PushButton());
                        return GarageDoorPushButtonResult.CreateSuccessResult();
                    });
            task.Start();
            return task.AsAsyncOperation();
        }

        /// <summary>Determines if the garage door is open.</summary>
        /// <param name="info">The information.</param>
        public IAsyncOperation<GarageDoorGetIsOpenResult> GetIsOpenAsync(AllJoynMessageInfo info)
        {
            var task =
                new Task<GarageDoorGetIsOpenResult>(
                    () => GarageDoorGetIsOpenResult.CreateSuccessResult(this.GarageDoor.IsOpen));
            task.Start();
            return task.AsAsyncOperation();
        }

        /// <summary>Determines if the garage door is partially open.</summary>
        /// <param name="info">The information.</param>
        public IAsyncOperation<GarageDoorGetIsPartiallyOpenResult> GetIsPartiallyOpenAsync(AllJoynMessageInfo info)
        {
            var task =
                new Task<GarageDoorGetIsPartiallyOpenResult>(
                    () => GarageDoorGetIsPartiallyOpenResult.CreateSuccessResult(this.GarageDoor.IsPartiallyOpen));
            task.Start();
            return task.AsAsyncOperation();
        }
    }
}