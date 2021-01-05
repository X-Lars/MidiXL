using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MidiXL
{
    /// <summary>
    /// Common base class for MIDI input and output devices.
    /// </summary>
    public abstract class MidiDevice
    {
        #region Fields

        /// <summary>
        /// A handle to reference the <see cref="MidiDevice"/> in API calls.
        /// </summary>
        internal protected API.MidiDeviceHandle _Handle;

        /// <summary>
        /// Stores connected MIDI devices.
        /// </summary>
        internal protected List<API.MidiDeviceHandle> _Connections = new List<API.MidiDeviceHandle>();

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new <see cref="MidiDevice"/> initialized from the specified <see cref="API.MidiOutputDeviceCapabilities"/>.
        /// </summary>
        /// <param name="deviceID">An <see cref="int"/> representing the ID of the device.</param>
        /// <param name="capabilities">An <see cref="API.MidiOutputDeviceCapabilities"/> containing the MIDI device's capabilities.</param>
        internal protected MidiDevice(int deviceID, API.MidiOutputDeviceCapabilities capabilities)
        {
            this.ID = deviceID;
            this.Name = capabilities.DeviceName;
            this.Type = capabilities.DeviceType;
            this.Handle = IntPtr.Zero;
        }

        /// <summary>
        /// Creates a new <see cref="MidiDevice"/> initialized from the specified <see cref="API.MidiInputDeviceCapabilities"/>.
        /// </summary>
        /// <param name="deviceID">An <see cref="int"/> representing the ID of the device.</param>
        /// <param name="capabilities">An <see cref="API.MidiInputDeviceCapabilities"/> containing the MIDI device's capabilities.</param>
        internal protected MidiDevice(int deviceID, API.MidiInputDeviceCapabilities capabilities)
        {
            this.ID = deviceID;
            this.Name = capabilities.DeviceName;
            this.Type = API.MidiDeviceType.MIDI_DEVICETYPE_MIDI_PORT;
            this.Handle = IntPtr.Zero;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the handle of the device.
        /// </summary>
        protected IntPtr Handle 
        { 
            get { return _Handle.Handle; }
            set { _Handle.Handle = value; }
        }

        /// <summary>
        /// Gets the ID of the device.
        /// </summary>
        public int ID { get; }

        /// <summary>
        /// Gets the name of the device.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the type of device.
        /// </summary>
        public API.MidiDeviceType Type { get; }

        /// <summary>
        /// Gets whether the MIDI device is openend or closed.
        /// </summary>
        protected bool IsOpen { get; set; } = false;

        /// <summary>
        /// Gets wheter the MIDI device is connected to another MIDI device.
        /// </summary>
        protected bool IsConnected { get; set; } = false;

        #endregion
    }

    /// <summary>
    /// Common base class for MIDI input and output device exceptions.
    /// </summary>
    public abstract class MidiDeviceException : ApplicationException
    {
        #region Constructor

        /// <summary>
        /// Creates a new <see cref="MidiDeviceException"/> with the specified error code.
        /// </summary>
        /// <param name="errorCode">An <see cref="API.Result"/> value specifying the error code.</param>
        public MidiDeviceException(API.Result errorCode)
        {
            this.ErrorCode = (int)errorCode;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the error code associated with the device exception.
        /// </summary>
        public int ErrorCode { get; }

        #endregion
    }
}
