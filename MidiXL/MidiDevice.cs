using System;

namespace MidiXL
{
    /// <summary>
    /// Common base class for MIDI input and output devices.
    /// </summary>
    public abstract class MidiDevice
    {
        #region Variables

        /// <summary>
        /// A handle to reference the <see cref="MidiDevice"/> in API calls.
        /// </summary>
        private API.MidiDeviceHandle _Handle;

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
        public IntPtr Handle 
        { 
            get { return _Handle.Handle; }
            internal protected set { _Handle.Handle = value; }
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
            this.ErrorCode = errorCode;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the error code associated with the device exception.
        /// </summary>
        public API.Result ErrorCode { get; }

        #endregion
    }
}
