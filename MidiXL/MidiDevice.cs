using System;
using System.Collections.Generic;

namespace MidiXL
{
    /// <summary>
    /// Common base class for MIDI input and output devices.
    /// </summary>
    public abstract class MidiDevice : IDisposable
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

        /// <summary>
        /// Gets wheter the MIDI device <see cref="Dispose"/> method has already been called.
        /// </summary>
        public bool IsDisposed { get; private set; } = false;

        #endregion

        #region IDisposable

        /// <summary>
        /// Disposes the MIDI device.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the MIDI device.
        /// </summary>
        /// <param name="isDisposing">A <see cref="bool"/> specifying the method is called from code (true) or by the runtime (false).</param>
        /// <remarks><i>If called from code managed and unmagaged resources have to be disposed else the runtime handles the managed resources and only unmanaged resource have to be disposed.</i></remarks>
        protected virtual void Dispose(bool isDisposing)
        {
            if (this.IsDisposed)
                return;

            if(isDisposing)
            {

            }

            this.IsDisposed = true;
        }

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
