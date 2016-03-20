// ***********************************************************************
// Assembly         : HastPiControl
// Author           : Jon Benson
// Created          : 05-01-2016
// 
// Last Modified By : Jon Benson
// Last Modified On : 05-01-2016
// ***********************************************************************
// <copyright file="GarageDoorService.cs" company="Champion Data">
//     Copyright (c) Champion Data. All rights reserved.
// </copyright>
// ***********************************************************************

namespace HastPiControl
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Windows.Devices.AllJoyn;
    using Windows.Foundation;
    using Windows.UI.Core;

    using com.hastarin.GarageDoor;

    using HastPiControl.Models;

    /// <summary>
    /// My first garage door AllJoyn service
    /// </summary>
    public class GarageDoorService : IGarageDoorService
    {
        /// <summary>
        /// Represents the garage door state
        /// </summary>
        public GarageDoor GarageDoor { get; set; }

        private CoreDispatcher dispatcher;

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public GarageDoorService(GarageDoor garageDoor)
        {
            this.GarageDoor = garageDoor;
            this.dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
        }

        public IAsyncOperation<GarageDoorOpenResult> OpenAsync(AllJoynMessageInfo info, bool partialOpen)
        {
            var task = new Task<GarageDoorOpenResult>(
                () =>
                    {
                        if (!partialOpen)
                        {
                            this.dispatcher.RunAsync( CoreDispatcherPriority.Normal, () => this.GarageDoor.PushButton());
                        }
                        else
                        {
                            this.dispatcher.RunAsync( CoreDispatcherPriority.Normal, () => this.GarageDoor.PartialOpen());
                        }
                        return GarageDoorOpenResult.CreateSuccessResult();
                    });
            task.Start();
            return task.AsAsyncOperation();
        }

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

        public IAsyncOperation<GarageDoorGetIsOpenResult> GetIsOpenAsync(AllJoynMessageInfo info)
        {
            var task = new Task<GarageDoorGetIsOpenResult>(
                () =>
                    {
                        return GarageDoorGetIsOpenResult.CreateSuccessResult(this.GarageDoor.IsOpen);
                    });
            task.Start();
            return task.AsAsyncOperation();
        }

        public IAsyncOperation<GarageDoorGetIsPartiallyOpenResult> GetIsPartiallyOpenAsync(AllJoynMessageInfo info)
        {
            var task = new Task<GarageDoorGetIsPartiallyOpenResult>(
                () =>
                    {
                        return GarageDoorGetIsPartiallyOpenResult.CreateSuccessResult(this.GarageDoor.IsPartiallyOpen);
                    });
            task.Start();
            return task.AsAsyncOperation();
        }
    }
}