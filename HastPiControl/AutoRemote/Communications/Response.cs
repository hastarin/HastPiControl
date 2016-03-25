// ***********************************************************************
// Assembly         : HastPiControl
// Author           : Jon Benson
// Created          : 20-03-2016
// 
// Last Modified By : Jon Benson
// Last Modified On : 26-03-2016
// ***********************************************************************

// ReSharper disable InconsistentNaming
namespace HastPiControl.AutoRemote.Communications
{
    using System;

    /// <summary>Class Response.</summary>
    /// <seealso cref="HastPiControl.AutoRemote.Communications.Communication" />
    public class Response : Communication
    {
        //If the request resulted in error, set the error here
        /// <summary>Gets or sets the response error.</summary>
        public String responseError { get; set; }

        /// <summary>Gets the GCM endpoint.</summary>
        protected override string GetGCMEndpoint()
        {
            return "sendresponse";
        }
    }
}