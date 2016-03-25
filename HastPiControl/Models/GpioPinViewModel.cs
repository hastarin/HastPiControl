// ***********************************************************************
// Assembly         : HastPiControl
// Author           : Jon Benson
// Created          : 14-11-2015
// 
// Last Modified By : Jon Benson
// Last Modified On : 26-03-2016
// ***********************************************************************

namespace HastPiControl.Models
{
    using System;

    using GalaSoft.MvvmLight;

    using Hastarin.Devices;

    /// <summary>Class GpioPinViewModel.</summary>
    public class GpioPinViewModel : ViewModelBase
    {
        private int id;

        private bool isOn;

        private string name;

        /// <summary>Initializes a new instance of the <see cref="GpioPinViewModel" /> class.</summary>
        public GpioPinViewModel()
        {
        }

        /// <summary>Initializes a new instance of the ViewModelBase class.</summary>
        /// <param name="address">The address.</param>
        /// <param name="isOutput">if set to <c>true</c> [is output].</param>
        public GpioPinViewModel(byte address, bool isOutput = false)
        {
            this.Address = address;
            this.IsOutput = isOutput;
        }

        /// <summary>Gets the address.</summary>
        /// <value>The address.</value>
        public byte Address { get; }

        /// <summary>Gets a value indicating whether this instance represents an output.</summary>
        /// <value><c>true</c> if this instance is output; otherwise, <c>false</c>.</value>
        public bool IsOutput { get; }

        /// <summary>Gets or sets a value indicating whether this <see cref="GpioPinViewModel" /> is on.</summary>
        /// <value><c>true</c> if on; otherwise, <c>false</c>.</value>
        public bool IsOn
        {
            get
            {
                return this.isOn;
            }
            set
            {
                if (this.Set(ref this.isOn, value) && this.IsOutput)
                {
                    MCP23S17.WritePin(this.Address, Convert.ToByte(value));
                }
            }
        }

        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
        public int Id
        {
            get
            {
                return this.id;
            }
            set
            {
                this.Set(ref this.id, value);
            }
        }

        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.Set(ref this.name, value);
            }
        }
    }
}