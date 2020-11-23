using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MidiXL
{
    /// <summary>
    /// 
    /// </summary>
    public class MidiOutputDevice : MidiDevice
    {
        #region Fields

        /// <summary>
        /// Structure to store the MIDI output device's capabilities.
        /// </summary>
        private API.MidiOutputDeviceCapabilities _Capabilities;

        /// <summary>
        /// Array to store the MIDI output devices installed in the system.
        /// </summary>
        private static MidiOutputDevice[] _DeviceList = null;

        /// <summary>
        /// Reference to the callback function used by the <see cref="API.OpenMidiOutputDevice"/> method.
        /// </summary>
        private API.MidiOutputDelegate _Callback;

        /// <summary>
        /// Lock for thread safety.
        /// </summary>
        private static readonly object _Lock = new object();

        #endregion

        #region Constructor

        /// <summary>
        /// Private contstructor to create and initialize a new <see cref="MidiOutputDevice"/>, only invoked by the <see cref="InitializeDeviceList"/> method.
        /// </summary>
        /// <param name="deviceID">An <see cref="int"/> representing the unique indexed <see cref="MidiOutputDevice"/>'s ID.</param>
        /// <param name="capabilities">An <see cref="API.MidiOutputDeviceCapabilities"/> structure to store the <see cref="MidiOutputDevice"/>'s capabilities.</param>
        private MidiOutputDevice(int deviceID, API.MidiOutputDeviceCapabilities capabilities) : base(deviceID, capabilities)
        {
            _Capabilities = capabilities;
            _Callback = Callback;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a list installed MIDI output devices installed in the system.
        /// </summary>
        public static ReadOnlyCollection<MidiOutputDevice> DeviceList
        {
            get 
            { 
                lock(_Lock)
                {
                    InitializeDeviceList();

                    return new ReadOnlyCollection<MidiOutputDevice>(_DeviceList);
                }
            }
        }

        /// <summary>
        /// Gets whether the MIDI output device is openend or closed.
        /// </summary>
        public bool IsOpen { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the internal list of installed MIDI output devices.
        /// </summary>
        /// <exception cref="MidiOutputDeviceException">Raises error #2: MULTIMEDIA_SYSTEM_ERROR_BAD_DEVICE_ID, the specified device ID is out of range.</exception>
        /// <exception cref="MidiOutputDeviceException">Raises error #6: MULTIMEDIA_SYSTEM_ERROR_NO_DRIVER, the driver is not installed.</exception>
        /// <exception cref="MidiOutputDeviceException">Raises error #7: MULTIMEDIA_SYSTEM_ERROR_NO_MEM, the system is unable to allocate or lock memory.</exception>
        /// <exception cref="MidiOutputDeviceException">Raises error #11: MULTIMEDIA_SYSTEM_ERROR_INVALID_PARAMETER, the specified pointer or structure is invalid.</exception>
        private static void InitializeDeviceList()
        {
            _DeviceList = new MidiOutputDevice[API.MidiOutputDeviceCount()];

            for (int deviceID = 0; deviceID < _DeviceList.Length; deviceID++)
            {
                API.MidiOutputDeviceCapabilities capabilities = new API.MidiOutputDeviceCapabilities();

                InvalidateResult(API.GetMidiOutputDeviceCapabilities(deviceID, ref capabilities));

                _DeviceList[deviceID] = new MidiOutputDevice(deviceID, capabilities);
            }
        }

        /// <summary>
        /// Opens the MIDI output device for sending MIDI messages.
        /// </summary>
        /// <exception cref="MidiOutputDeviceException">Raises error #2: MULTIMEDIA_SYSTEM_ERROR_BAD_DEVICE_ID, the specified device ID is out of range.</exception>
        /// <exception cref="MidiOutputDeviceException">Raises error #4: MULTIMEDIA_SYSTEM_ERROR_ALLOCATED when the device is alread opened.</exception>
        /// <exception cref="MidiOutputDeviceException">Raises error #7: MULTIMEDIA_SYSTEM_ERROR_NO_MEM, the system is unable to allocate or lock memory.</exception>
        /// <exception cref="MidiOutputDeviceException">Raises error #11: MULTIMEDIA_SYSTEM_ERROR_INVALID_PARAMETER, the specified pointer or structure is invalid.</exception>
        /// <exception cref="MidiOutputDeviceException">Raises error #68: MIDI_ERROR_NO_DEVICE, no MIDI port was found, only occurs when the mapper is opened.</exception>
        public void Open()
        {
            InvalidateResult(API.OpenMidiOutputDevice(ref _Handle, this.ID, _Callback, IntPtr.Zero));
        }

        /// <summary>
        /// Closes the MIDI output device.
        /// </summary>
        /// <exception cref="MidiOutputDeviceException">Raises error #5: MULTIMEDIA_SYSTEM_ERROR_INVALID_HANDLE, the specified device handle is invalid.</exception>
        /// <exception cref="MidiOutputDeviceException">Raises error #7: MULTIMEDIA_SYSTEM_ERROR_NO_MEM, the system is unable to allocate or lock memory.</exception>
        /// <exception cref="MidiOutputDeviceException">Raises error #65: MIDI_ERROR_STILL_PLAYING, buffers are still in the queue.</exception>
        public void Close()
        {
            InvalidateResult(API.CloseMidiOutputDevice(_Handle));
        }

        /// <summary>
        /// Turns off all notes on all MIDI channels.
        /// </summary>
        /// <exception cref="MidiOutputDeviceException">Raises error #5: MULTIMEDIA_SYSTEM_ERROR_INVALID_HANDLE, the specified device handle is invalid.</exception>
        public void Reset()
        {
            InvalidateResult(API.ResetMidiOutputDevice(_Handle));
        }

        /// <summary>
        /// Connects the MIDI output device to a MIDI input or thru device.
        /// Received <see cref="API.MidiInputMessage"/>s are sent through to the connected MIDI output or thru device.
        /// </summary>
        /// <param name="device">A <see cref="MidiInputDevice"/> or MIDI thru device to connect to.</param>
        /// <exception cref="MidiOutputDeviceException">Raises error #5: MULTIMEDIA_SYSTEM_ERROR_INVALID_HANDLE, the specified device handle is invalid.</exception>
        /// <exception cref="MidiOutputDeviceException">Raises error #67: MIDI_ERROR_NOT_READY, the specified input device is already connected to an output device.</exception>
        /// <remarks>For MIDI thru devices, a handle must be obtained by a calling the <see cref="API.OpenMidiOutputDevice"/> method.</remarks>
        public void Connect(MidiInputDevice device)
        {
            InvalidateResult(API.ConnectMidiDevices(device._Handle, _Handle));
        }

        /// <summary>
        /// Disconnects the MIDI output device from a MIDI input or thru device.
        /// </summary>
        /// <param name="device">A <see cref="MidiInputDevice"/> or MIDI thru device to disconnect from.</param>
        /// <exception cref="MidiOutputDeviceException">Raises error #5: MULTIMEDIA_SYSTEM_ERROR_INVALID_HANDLE, the specified device handle is invalid.</exception>
        public void Disconnect(MidiInputDevice device)
        {
            InvalidateResult(API.DisconnectMidiDevices(device._Handle, _Handle));
        }

        /// <summary>
        /// Callback function to be called by the system.
        /// </summary>
        /// <param name="handle">An <see cref="API.MidiDeviceHandle"/> to the MIDI output device to associate with the callback function.</param>
        /// <param name="message">An <see cref="API.MidiOutputMessage"/> containing the message.</param>
        /// <param name="instance">An <see cref="IntPtr"/> to the instance data supplied by the <see cref="API.OpenMidiOutputDevice"/> function.</param>
        /// <param name="messageParameterA">An <see cref="IntPtr"/> to the first message parameter.</param>
        /// <param name="messageParameterB">An <see cref="IntPtr"/> to the second message parameter.</param>
        private void Callback(API.MidiDeviceHandle handle, API.MidiOutputMessage message, IntPtr instance, IntPtr messageParameterA, IntPtr messageParameterB)
        {
            if (message == API.MidiOutputMessage.MIDI_OUTPUT_MESSAGE_OPEN)
            {
                this.IsOpen = true;
            }
            else if (message == API.MidiOutputMessage.MIDI_OUTPUT_MESSAGE_CLOSE)
            {
                this.IsOpen = false;
            }
            else if (message == API.MidiOutputMessage.MIDI_OUTPUT_MESSAGE_DONE) { }
        }

        /// <summary>
        /// Sends a short MIDI message.
        /// </summary>
        /// <param name="message">The short MIDI message to send.</param>
        /// <exception cref="MidiOutputDeviceException">Raises error #5: MULTIMEDIA_SYSTEM_ERROR_INVALID_HANDLE, the specified device handle is invalid.</exception>
        /// <exception cref="MidiOutputDeviceException">Raises error #67: MIDI_ERROR_NOT_READY, the specified input device is already connected to an output device.</exception>
        /// <exception cref="MidiOutputDeviceException">Raises error #70: MIDI_ERROR_BAD_OPEN_MODE, the application sent a message without a status byte to a stream handle.</exception>
        public void Send(ShortMessage message)
        {
            InvalidateResult(API.SendShortMessage(_Handle, message.Message));
        }

        /// <summary>
        /// Sends a long MIDI message.
        /// </summary>
        /// <param name="message">The long MIDI message to send.</param>
        /// <exception cref="MidiOutputDeviceException">Raises error #5: MULTIMEDIA_SYSTEM_ERROR_INVALID_HANDLE, the specified device handle is invalid.</exception>
        /// <exception cref="MidiOutputDeviceException">Raises error #7: MULTIMEDIA_SYSTEM_ERROR_NO_MEM, the system is unable to allocate or lock memory.</exception>
        /// <exception cref="MidiOutputDeviceException">Raises error #11: MULTIMEDIA_SYSTEM_ERROR_INVALID_PARAMETER, the specified pointer or structure is invalid.</exception>
        /// <exception cref="MidiOutputDeviceException">Raises error #64: MIDI_ERROR_UNPREPARED, the buffer pointed to by the MIDI header has not been prepared.</exception>
        /// <exception cref="MidiOutputDeviceException">Raises error #65: MIDI_ERROR_STILL_PLAYING, the buffer pointed to by the MIDI header is still in the queue.</exception>
        /// <exception cref="MidiOutputDeviceException">Raises error #67: MIDI_ERROR_NOT_READY, the specified input device is already connected to an output device.</exception>
        public void Send(LongMessage message)
        {
            IntPtr midiHeaderPointer;

            int midiHeaderSize = Marshal.SizeOf(typeof(API.MidiHeader));

            API.MidiHeader midiHeader = new API.MidiHeader();

            midiHeader.Data = Marshal.AllocHGlobal(message.Data.Length);

            for (int i = 0; i < message.Data.Length; i++)
            {
                Marshal.WriteByte(midiHeader.Data, i, message.Data[i]);
            }

            midiHeader.BufferSize = message.Data.Length;
            midiHeader.BytesRecorded = message.Data.Length;
            midiHeader.Flags = 0;

            try
            {
                midiHeaderPointer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(API.MidiHeader)));

            }
            catch(Exception)
            {
                Marshal.FreeHGlobal(midiHeader.Data);
                throw;
            }

            try
            {
                Marshal.StructureToPtr(midiHeader, midiHeaderPointer, false);
            }
            catch(Exception)
            {
                Marshal.FreeHGlobal(midiHeader.Data);
                Marshal.FreeHGlobal(midiHeaderPointer);
                throw;
            }

            InvalidateResult(API.PrepareMidiOutputHeader(_Handle, midiHeaderPointer, midiHeaderSize));
            InvalidateResult(API.SendLongMessage(_Handle, midiHeaderPointer, midiHeaderSize));
            InvalidateResult(API.UnprepareMidiOutputHeader(_Handle, midiHeaderPointer, midiHeaderSize));

            Marshal.FreeHGlobal(midiHeader.Data);
            Marshal.FreeHGlobal(midiHeaderPointer);
        }

        /// <summary>
        /// Invalidates the result returned by API calls for errors and throws a <see cref="MidiOutputDeviceException"/> if an error occured.
        /// </summary>
        /// <param name="result">An <see cref="API.Result"/> value containing the result of the API call.</param>
        private static void InvalidateResult(API.Result result)
        {
            if(result != API.Result.MULTIMEDIA_SYSTEM_ERROR_NO_ERROR)
            {
                throw new MidiOutputDeviceException(result);
            }
        }

        #endregion
    }

    /// <summary>
    /// Devines a MIDI output device exception that is thrown when an error occurs with a <see cref="MidiOutputDevice"/>.
    /// </summary>
    public class MidiOutputDeviceException : MidiDeviceException
    {
        #region Fields

        /// <summary>
        /// A buffer to be filled with the error message received by the <see cref="API.GetMidiOutputDeviceErrorText"/> method.
        /// </summary>
        private StringBuilder _Buffer = new StringBuilder(API.MAX_ERROR_TEXT_LENGTH);

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new <see cref="MidiOutputDeviceException"/> with the specified error code.
        /// </summary>
        /// <param name="errorCode">A <see cref="API.Result"/> value specifying the error code.</param>
        public MidiOutputDeviceException(API.Result errorCode) : base(errorCode)
        {
            API.GetMidiOutputDeviceErrorText(errorCode, _Buffer);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the message describing the exception.
        /// </summary>
        public override string Message
        {
            get { return $"MIDI Output Device Error #{ErrorCode}\n{_Buffer}"; }
        }

        #endregion
    }
}
