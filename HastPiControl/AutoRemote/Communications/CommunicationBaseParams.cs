// ***********************************************************************
// Assembly         : HastPiControl
// Author           : Jon Benson
// Created          : 26-03-2016
// 
// Last Modified By : Jon Benson
// Last Modified On : 26-03-2016
// ***********************************************************************

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierWordIsNotInDictionary
namespace HastPiControl.AutoRemote.Communications
{
    /// <summary>Class CommunicationBaseParams.</summary>
    public class CommunicationBaseParams
    {
        /// <summary>Gets the sender.</summary>
        public string sender { get; set; } = ConstantsThatShouldBeVariables.ID;

        /// <summary>Gets or sets the type.</summary>
        public string type { get; set; }
    }
}