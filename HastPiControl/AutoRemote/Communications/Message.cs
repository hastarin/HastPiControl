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
    /// <summary>Class Message.</summary>
    /// <seealso cref="HastPiControl.AutoRemote.Communications.Request" />
    public class Message : Request
    {
        /// <summary>Message text</summary>
        public string message { get; set; }

        /// <summary>Optional password. Shouldn't allow incoming messages unless this matches with a user defined password.</summary>
        public string password { get; set; }

        /// <summary>Optional Files array.</summary>
        public string[] files { get; set; }
    }
}