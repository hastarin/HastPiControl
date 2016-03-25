// ***********************************************************************
// Assembly         : HastPiControl
// Author           : Jon Benson
// Created          : 20-03-2016
// 
// Last Modified By : Jon Benson
// Last Modified On : 26-03-2016
// ***********************************************************************

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierWordIsNotInDictionary
namespace HastPiControl.AutoRemote.Communications
{
    using System;

    /// <summary>Class Request.</summary>
    /// <seealso cref="HastPiControl.AutoRemote.Communications.Communication" />
    public class Request : Communication
    {
        /// <summary>Gets or sets the TTL.</summary>
        /// <value>The TTL.</value>
        public int ttl { get; set; }

        /// <summary>Gets or sets the collapsekey.</summary>
        /// <value>The collapsekey.</value>
        public String collapsekey { get; set; }

        /// <summary>
        ///     Executes the request. Is different for every type of request. The default is to just respond with the response
        ///     ResponseNoAction
        /// </summary>
        /// <returns>Response.</returns>
        public virtual Response ExecuteRequest()
        {
            return new ResponseNoAction();
        }

        /// <summary>Gets the GCM endpoint.</summary>
        /// <returns>String.</returns>
        protected override string GetGCMEndpoint()
        {
            return "sendrequest";
        }
    }
}