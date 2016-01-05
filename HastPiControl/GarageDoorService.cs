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
    using Windows.UI.Xaml;

    using com.hastarin.GarageDoor;

    using HastPiControl.Models;

    public class GarageDoorService : IGarageDoorService
    {
        private readonly PiFaceDigital2ViewModel piFaceDigital2ViewModel;

        private CoreDispatcher dispatcher;

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public GarageDoorService(PiFaceDigital2ViewModel piFaceDigital2ViewModel)
        {
            this.piFaceDigital2ViewModel = piFaceDigital2ViewModel;
            this.dispatcher = Window.Current.Dispatcher;
        }

        public IAsyncOperation<GarageDoorOpenResult> OpenAsync(AllJoynMessageInfo info)
        {
            var task = new Task<GarageDoorOpenResult>(
                () =>
                    {
                        var input =
                            this.piFaceDigital2ViewModel.Inputs.SingleOrDefault(
                                i => i.Name == PiFaceDigital2ViewModel.GarageDoorOpen);
                        if (input == null)
                        {
                            return GarageDoorOpenResult.CreateFailureResult(0);
                        }
                        if (input.IsOn)
                        {
                            return GarageDoorOpenResult.CreateFailureResult(1);
                        }
                        var output =
                            this.piFaceDigital2ViewModel.Outputs.SingleOrDefault(
                                o => o.Name == PiFaceDigital2ViewModel.GarageDoorPushButtonRelay);
                        if (output == null)
                        {
                            return GarageDoorOpenResult.CreateFailureResult(0);
                        }
                        this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, this.piFaceDigital2ViewModel.PushButton);
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
                        var input =
                            this.piFaceDigital2ViewModel.Inputs.SingleOrDefault(
                                i => i.Name == PiFaceDigital2ViewModel.GarageDoorOpen);
                        if (input == null)
                        {
                            return GarageDoorCloseResult.CreateFailureResult(0);
                        }
                        if (!input.IsOn)
                        {
                            return GarageDoorCloseResult.CreateFailureResult(1);
                        }
                        var output =
                            this.piFaceDigital2ViewModel.Outputs.SingleOrDefault(
                                o => o.Name == PiFaceDigital2ViewModel.GarageDoorPushButtonRelay);
                        if (output == null)
                        {
                            return GarageDoorCloseResult.CreateFailureResult(0);
                        }
                        this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, this.piFaceDigital2ViewModel.PushButton);
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
                        var output =
                            this.piFaceDigital2ViewModel.Outputs.SingleOrDefault(
                                o => o.Name == PiFaceDigital2ViewModel.GarageDoorPushButtonRelay);
                        if (output == null)
                        {
                            return GarageDoorPushButtonResult.CreateFailureResult(0);
                        }
                        this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, this.piFaceDigital2ViewModel.PushButton);
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
                        var input =
                            this.piFaceDigital2ViewModel.Inputs.SingleOrDefault(
                                i => i.Name == PiFaceDigital2ViewModel.GarageDoorOpen);
                        if (input == null)
                        {
                            return GarageDoorGetIsOpenResult.CreateFailureResult(0);
                        }

                        return GarageDoorGetIsOpenResult.CreateSuccessResult(input.IsOn);
                    });
            task.Start();
            return task.AsAsyncOperation();
        }

        public IAsyncOperation<GarageDoorGetIsPartiallyOpenResult> GetIsPartiallyOpenAsync(AllJoynMessageInfo info)
        {
            var task = new Task<GarageDoorGetIsPartiallyOpenResult>(
                () =>
                    {
                        var input =
                            this.piFaceDigital2ViewModel.Inputs.SingleOrDefault(
                                i => i.Name == PiFaceDigital2ViewModel.GarageDoorPartialOpen);
                        if (input == null)
                        {
                            return GarageDoorGetIsPartiallyOpenResult.CreateFailureResult(0);
                        }

                        return GarageDoorGetIsPartiallyOpenResult.CreateSuccessResult(input.IsOn);
                    });
            task.Start();
            return task.AsAsyncOperation();
        }
    }
}