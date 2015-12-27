// ***********************************************************************
// Assembly         : Hastarin.Devices.MCP23S17
// Author           : Jon Benson
// Created          : 28-11-2015
// 
// Last Modified By : Jon Benson
// Last Modified On : 28-11-2015
// ***********************************************************************

namespace Hastarin.Devices
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;

    using Windows.Devices.Enumeration;
    using Windows.Devices.Spi;

    /// <summary>Class to handle interaction with the MCP23S17 chip for the PiFace Digital 2 board.</summary>
    /// <remarks>
    ///     All credit for the original code should go to Peter Oakes (https://microsoft.hackster.io/en-US/peteroakes).
    ///     Portions of the documentation have been taken from the datasheet
    ///     (http://ww1.microchip.com/downloads/en/DeviceDoc/21952b.pdf) for the chip.
    /// </remarks>
    public class MCP23S17
    {
        /// <summary>Enumeration of the <see cref="MCP23S17" /> registers</summary>
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public enum Register : byte
        {
            /// <summary>I/O Direction A - controls the direction of the data I/O.</summary>
            /// <remarks>
            ///     When a bit is set, the corresponding pin becomes an input. When a bit is clear, the corresponding pin becomes
            ///     an output.
            /// </remarks>
            IODIRA = 0x00,

            /// <summary>I/O Direction B - controls the direction of the data I/O.</summary>
            /// <remarks>
            ///     When a bit is set, the corresponding pin becomes an input. When a bit is clear, the corresponding pin becomes
            ///     an output.
            /// </remarks>
            IODIRB = 0x01,

            /// <summary>Input Polarity A - allows the user to configure the polarity on the corresponding GPIO port bits.</summary>
            /// <remarks>If a bit is set, the corresponding GPIO register bit will reflect the inverted value on the pin.</remarks>
            IPOLA = 0x02,

            /// <summary>Input Polarity B - allows the user to configure the polarity on the corresponding GPIO port bits.</summary>
            /// <remarks>If a bit is set, the corresponding GPIO register bit will reflect the inverted value on the pin.</remarks>
            IPOLB = 0x03,

            /// <summary>Interrupt on change control A - controls the interrupt-on-change feature for each pin.</summary>
            /// <remarks>
            ///     If a bit is set, the corresponding pin is enabled for interrupt-on-change. The DEFVAL and INTCON registers
            ///     must also be configured if any pins are enabled for interrupt-on-change.
            /// </remarks>
            GPINTENA = 0x04,

            /// <summary>Interrupt on change control B - controls the interrupt-on-change feature for each pin.</summary>
            /// <remarks>
            ///     If a bit is set, the corresponding pin is enabled for interrupt-on-change. The DEFVAL and INTCON registers
            ///     must also be configured if any pins are enabled for interrupt-on-change.
            /// </remarks>
            GPINTENB = 0x05,

            /// <summary>Default compare for interrupt on change A</summary>
            /// <remarks>
            ///     The default comparison value is configured in the DEFVAL register. If enabled (via GPINTEN and INTCON) to
            ///     compare against the DEFVAL register, an opposite value on the associated pin will cause an interrupt to occur.
            /// </remarks>
            DEFVALA = 0x06,

            /// <summary>Default compare for interrupt on change B</summary>
            /// <remarks>
            ///     The default comparison value is configured in the DEFVAL register. If enabled (via GPINTEN and INTCON) to
            ///     compare against the DEFVAL register, an opposite value on the associated pin will cause an interrupt to occur.
            /// </remarks>
            DEFVALB = 0x06,

            /// <summary>Interrupt Control A - controls how the associated pin value is compared for the interrupt-on-change feature.</summary>
            /// <remarks>
            ///     If a bit is set, the corresponding I/O pin is compared against the associated bit in the DEFVAL register. If a
            ///     bit value is clear, the corresponding I/O pin is compared against the previous value.
            /// </remarks>
            INTCONA = 0x08,

            /// <summary>Interrupt Control B - controls how the associated pin value is compared for the interrupt-on-change feature.</summary>
            /// <remarks>
            ///     If a bit is set, the corresponding I/O pin is compared against the associated bit in the DEFVAL register. If a
            ///     bit value is clear, the corresponding I/O pin is compared against the previous value.
            /// </remarks>
            INTCONB = 0x09,

            /// <summary>I/O Expander Configuration A - contains several bits for configuring the device.</summary>
            /// <remarks>See page 16/17 of the datasheet (http://ww1.microchip.com/downloads/en/DeviceDoc/21952b.pdf) for details.</remarks>
            IOCONA = 0x0A,

            /// <summary>I/O Expander Configuration B - contains several bits for configuring the device.</summary>
            /// <remarks>See page 16/17 of the datasheet (http://ww1.microchip.com/downloads/en/DeviceDoc/21952b.pdf) for details.</remarks>
            IOCONB = 0x0B,

            /// <summary>Pull Up resistor configuration register A - controls the pull-up resistors for the port pins.</summary>
            /// <remarks>
            ///     If a bit is set and the corresponding pin is configured as an input, the corresponding port pin is internally
            ///     pulled up with a 100 kΩ resistor.
            /// </remarks>
            GPPUA = 0x0C,

            /// <summary>Pull Up resistor configuration B - controls the pull-up resistors for the port pins.</summary>
            /// <remarks>
            ///     If a bit is set and the corresponding pin is configured as an input, the corresponding port pin is internally
            ///     pulled up with a 100 kΩ resistor.
            /// </remarks>
            GPPUB = 0x0D,

            /// <summary>
            ///     Interrupt Flag A - reflects the interrupt condition on the port pins of any pin that is enabled for interrupts
            ///     via the GPINTEN register.
            /// </summary>
            /// <remarks>
            ///     A ‘set’ bit indicates that the associated pin caused the interrupt. This register is ‘read-only’. Writes to
            ///     this register will be ignored.
            /// </remarks>
            INTFA = 0x0E,

            /// <summary>
            ///     Interrupt Flag B - reflects the interrupt condition on the port pins of any pin that is enabled for interrupts
            ///     via the GPINTEN register.
            /// </summary>
            /// <remarks>
            ///     A ‘set’ bit indicates that the associated pin caused the interrupt. This register is ‘read-only’. Writes to
            ///     this register will be ignored.
            /// </remarks>
            INTFB = 0x0F,

            /// <summary>Interrupt Capture A - captures the GPIO port value at the time the interrupt occurred.</summary>
            /// <remarks>
            ///     The register is ‘read only’ and is updated only when an interrupt occurs. The register will remain unchanged
            ///     until the interrupt is cleared via a read of INTCAP or GPIO.
            /// </remarks>
            INTCAPA = 0x10,

            /// <summary>Interrupt Capture B - captures the GPIO port value at the time the interrupt occurred.</summary>
            /// <remarks>
            ///     The register is ‘read only’ and is updated only when an interrupt occurs. The register will remain unchanged
            ///     until the interrupt is cleared via a read of INTCAP or GPIO.
            /// </remarks>
            INTCAPB = 0x11,

            /// <summary>GPIO A - reflects the value on the port.</summary>
            /// <remarks>Reading from this register reads the port. Writing to this register modifies the Output Latch (OLAT) register.</remarks>
            GPIOA = 0x12,

            /// <summary>GPIO B - reflects the value on the port.</summary>
            /// <remarks>Reading from this register reads the port. Writing to this register modifies the Output Latch (OLAT) register.</remarks>
            GPIOB = 0x13,

            /// <summary>Output Latch A - provides access to the output latches.</summary>
            /// <remarks>
            ///     A read from this register results in a read of the OLAT and not the port itself. A write to this register
            ///     modifies the output latches that modifies the pins configured as outputs.
            /// </remarks>
            OLATA = 0x14,

            /// <summary>Output Latch B - provides access to the output latches.</summary>
            /// <remarks>
            ///     A read from this register results in a read of the OLAT and not the port itself. A write to this register
            ///     modifies the output latches that modifies the pins configured as outputs.
            /// </remarks>
            OLATB = 0x15
        }

        private const string SpiControllerName = "SPI0";

        private const int SpiChipSelectLine = 0;

        /// <summary>Represents an On state</summary>
        public const byte On = 1;

        /// <summary>Represents an Off state</summary>
        public const byte Off = 0;

        /// <summary>Represents the Output state.</summary>
        public const byte Output = 0;

        /// <summary>Represents the Input state.</summary>
        public const byte Input = 1;

        private const byte Address = 0x00; // offset address if hardware addressing is on and is 0 - 7 (A0 - A2) 

        private const byte BaseAddW = 0x40; // MCP23S17 Write base address

        private const byte BaseAddR = 0x41; // MCP23S17 Read Base Address

        /// <summary>IOCON register for MCP23S17, x08 enables hardware address so sent address must match hardware pins A0-A2</summary>
        // ReSharper disable once InconsistentNaming
        private const byte HAEN = 0x08;

        /*RaspBerry Pi2  Parameters*/

        /// <summary>default Pinmode for the MXP23S17 set to inputs</summary>
        private static ushort pinMode = 0XFFFF;

        /// <summary>default pullups for the MXP23S17 set to weak pullup</summary>
        private static ushort pullUpMode = 0XFFFF;

        /// <summary>Holds output data</summary>
        private static readonly byte[] ReadBuffer3 = new byte[3];

        /// <summary>Holds output data</summary>
        private static readonly byte[] ReadBuffer4 = new byte[4];

        /// <summary>Register, then 16 bit value</summary>
        private static readonly byte[] WriteBuffer3 = new byte[3];

        /// <summary>Register, then 16 bit value</summary>
        private static readonly byte[] WriteBuffer4 = new byte[4];

        private static SpiDevice spi0;

        /// <summary>The state of the pins</summary>
        public static ushort PinState { get; set; }

        /// <summary>The inversion mode used for each pin</summary>
        public static ushort InversionMode { get; set; }

        /// <summary>Initialize the chip based on <see cref="pinMode" />
        /// </summary>
        public static void Initialize()
        {
            WriteRegister8(Register.IOCONA, HAEN); // enable the hardware address incase there is more than one chip
            WriteRegister16(Register.IODIRA, pinMode); // Set the default or current pin mode
        }

        /// <summary>Initialize the underlying SPI Device to interact with the chip on the Raspberry Pi 2.</summary>
        /// <remarks><see cref="SpiConnectionSettings.ClockFrequency" /> is set to 10MHz to match the chip.</remarks>
        /// <remarks>Line 0 maps to physical pin number 24 on the Rpi2, line 1 to pin 26</remarks>
        public static async Task InitializeSpiDevice()
        {
            var settings = new SpiConnectionSettings(SpiChipSelectLine)
                               {
                                   ClockFrequency = 1000000,
                                   Mode = SpiMode.Mode0
                               };
            var spiAqs = SpiDevice.GetDeviceSelector(SpiControllerName);
            var deviceInfo = await DeviceInformation.FindAllAsync(spiAqs);
            if (deviceInfo != null && deviceInfo.Any())
            {
                spi0 = await SpiDevice.FromIdAsync(deviceInfo[0].Id, settings);
            }
        }

        /// <summary>Read the value of a given pin</summary>
        /// <param name="pin">The ping to read</param>
        /// <returns>Either <see cref="On" />/<see cref="Off" />.</returns>
        public static ushort ReadPin(byte pin)
        {
            if (pin > 15)
            {
                return 0x00; // If the pin value is not valid (1-16) return, do nothing and return
            }
            ushort value = ReadRegister16(); // Initialize a variable to hold the read values to be returned
            ushort pinmask = (ushort)(1 << pin); // Initialize a variable to hold the read values to be returned
            return ((value & pinmask) > 0) ? On : Off;
            // Call the word reading function, extract HIGH/LOW information from the requested pin
        }

        /// <summary>Read the values of <see cref="Register.GPIOA" />
        /// </summary>
        /// <returns>The constructed word</returns>
        public static ushort ReadRegister16()
        {
            WriteBuffer4[0] = (BaseAddR | (Address << 1));
            WriteBuffer4[1] = (byte)Register.GPIOA;
            WriteBuffer4[2] = 0;
            WriteBuffer4[3] = 0;
            spi0.TransferFullDuplex(WriteBuffer4, ReadBuffer4);
            return ConvertToUnsignedShort(ReadBuffer4); // Return the constructed word, the format is 0x(register value)
        }

        /// <summary>Reads a <see langword="byte" /> from the given <paramref name="register" />
        /// </summary>
        /// <param name="register">The register to read.</param>
        /// <returns>The byte read from the register.</returns>
        public static byte ReadRegister8(byte register)
        {
            // This function will read a single register, and return it
            WriteBuffer3[0] = (BaseAddR | (Address << 1)); // Send the MCP23S17 opcode, chip address, and read bit
            WriteBuffer3[1] = register;
            spi0.TransferFullDuplex(WriteBuffer3, ReadBuffer3);
            return ReadBuffer4[2];
            // convertToInt(readBuffer);                             // Return the constructed word, the format is 0x(register value)
        }

        /// <summary>Set the inversion of input polarity a pin at a time.</summary>
        /// <param name="pin">The pin to set</param>
        /// <param name="mode">The mode to set</param>
        public static void SetInversionMode(byte pin, byte mode)
        {
            if (pin > 15)
            {
                return;
            }
            if (mode == On)
            {
                InversionMode |= (ushort)(1 << (pin - 1));
            }
            else
            {
                InversionMode &= (ushort)(~(1 << (pin - 1)));
            }
            WriteRegister16(Register.IPOLA, InversionMode);
        }

        /// <summary>Set the inversion of input polarity for all pins.</summary>
        /// <param name="mode">The mode to set</param>
        public static void SetInversionMode(ushort mode)
        {
            WriteRegister16(Register.IPOLA, mode);
            InversionMode = mode;
        }

        /// <summary>Sets the given <paramref name="pin" /> to the given <paramref name="mode" />.</summary>
        /// <param name="pin">The pin to set.</param>
        /// <param name="mode">The mode to set.</param>
        /// <remarks>Any value other than <see cref="Input" /> will be treated as <see cref="Output" /></remarks>
        public static void SetPinMode(byte pin, byte mode)
        {
            if (pin > 15)
            {
                return; // only a 16bit port so do a bounds check, it cant be less than zero as this is a byte value
            }
            if (mode == Input)
            {
                pinMode |= (ushort)(1 << (pin)); // update the pinMode register with new direction
            }
            else
            {
                pinMode &= (ushort)(~(1 << (pin))); // update the pinMode register with new direction
            }
            WriteRegister16(Register.IODIRA, pinMode);
            // Call the generic word writer with start register and the mode cache
        }

        /// <summary>Sets all pins to the given <paramref name="mode" />.</summary>
        /// <param name="mode">The mode to set.</param>
        /// <remarks>Any value other than <see cref="Input" /> will be treated as <see cref="Output" /></remarks>
        public static void SetPinMode(ushort mode)
        {
            WriteRegister16(Register.IODIRA, mode);
            pinMode = mode;
        }

        /// <summary>Set the pull up mode</summary>
        /// <param name="pin">The pin to set</param>
        /// <param name="mode">The mode to set.  Anything other than <see cref="On" /> is considered <see cref="Off" />.</param>
        public static void SetPullUpMode(byte pin, byte mode)
        {
            if (pin > 15)
            {
                return;
            }
            if (mode == On)
            {
                pullUpMode |= (ushort)(1 << (pin));
            }
            else
            {
                pullUpMode &= (ushort)(~(1 << (pin)));
            }
            WriteRegister16(Register.GPPUA, pullUpMode);
        }

        /// <summary>Set the pull up mode</summary>
        /// <param name="mode">The mode to set.  Anything other than <see cref="On" /> is considered <see cref="Off" />.</param>
        public static void SetPullUpMode(ushort mode)
        {
            WriteRegister16(Register.GPPUA, mode);
            pullUpMode = mode;
        }

        /// <summary>Write a value to the pin and record it's state in PinState</summary>
        /// <param name="pin">The pin to write to</param>
        /// <param name="value">The value to write</param>
        public static void WritePin(byte pin, byte value)
        {
            if (pin > 15)
            {
                return;
            }
            if (value > 1)
            {
                return;
            }
            if (value == 1)
            {
                PinState |= (ushort)(1 << pin);
            }
            else
            {
                PinState &= (ushort)(~(1 << pin));
            }
            WriteRegister16(Register.GPIOA, PinState);
        }

        /// <summary>Write the given <paramref name="value" /> to the given register</summary>
        /// <param name="register">The byte address of the register to write to</param>
        /// <param name="value">The value to write</param>
        public static void WriteRegister16(byte register, ushort value)
        {
            WriteBuffer4[0] = (BaseAddW | (Address << 1));
            WriteBuffer4[1] = register;
            WriteBuffer4[2] = (byte)(value >> 8);
            WriteBuffer4[3] = (byte)(value & 0XFF);
            spi0.Write(WriteBuffer4);
        }

        /// <summary>Writes the supplied <paramref name="word" /> to the given <paramref name="register" />
        /// </summary>
        /// <param name="register">The register to write to.</param>
        /// <param name="word">The word to write</param>
        public static void WriteRegister16(Register register, ushort word)
        {
            WriteRegister16((byte)register, word);
        }

        /// <summary>Write the given <paramref name="value" /> to the given register</summary>
        /// <param name="register">The byte address of the register to write to</param>
        /// <param name="value">The value to write</param>
        public static void WriteRegister8(byte register, byte value)
        {
            // Direct port manipulation speeds taking Slave Select LOW before SPI action
            WriteBuffer3[0] = (BaseAddW | (Address << 1));
            WriteBuffer3[1] = register;
            WriteBuffer3[2] = value;
            spi0.Write(WriteBuffer3);
        }

        /// <summary>Writes the supplied <paramref name="value" /> to the given <paramref name="register" />
        /// </summary>
        /// <param name="register">The register to write to.</param>
        /// <param name="value">The value to write.</param>
        public static void WriteRegister8(Register register, byte value)
        {
            WriteRegister8((byte)register, value);
        }

        /// <summary>Write a <paramref name="value" /> to the device record it's state in <see cref="PinState" />.</summary>
        /// <param name="value">The value to write</param>
        public static void WriteWord(ushort value)
        {
            WriteRegister16(Register.GPIOA, value);
            PinState = value;
        }

        private static ushort ConvertToUnsignedShort(byte[] data)
        {
            // byte[0] = command, byte[1] register, byte[2] = data high, byte[3] = data low
            ushort result = (ushort)(data[2] & 0xFF);
            result <<= 8;
            result += data[3];
            return result;
        }
    }
}