using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiXL
{
    /// <summary>
    /// 
    /// </summary>
    public class MidiInputDevice : MidiDevice
    {
        #region Fields

        /// <summary>
        /// Structure to store the MIDI input device's capabilities.
        /// </summary>
        private API.MidiInputDeviceCapabilities _Capabilities;

        /// <summary>
        /// Array to store the MIDI input devices installed in the system.
        /// </summary>
        private static MidiInputDevice[] _DeviceList = null;

        /// <summary>
        /// Reference to the callback function used by the <see cref="API.OpenMidiInputDevice"/> method.
        /// </summary>
        private API.MidiInputDelegate _Callback;

        /// <summary>
        /// Buffer for receiving long MIDI messages.
        /// </summary>
        private List<IntPtr> _Buffer = new List<IntPtr>();

        /// <summary>
        /// Lock for thread safety.
        /// </summary>
        private static readonly object _Lock = new object();

        #endregion

        #region Constructor

        /// <summary>
        /// Private contstructor to create and initialize a new <see cref="MidiInputDevice"/>, only invoked by the <see cref="Initialize"/> method.
        /// </summary>
        /// <param name="deviceID">An <see cref="int"/> representing the unique indexed <see cref="MidiInputDevice"/>'s ID.</param>
        /// <param name="capabilities">An <see cref="API.MidiInputDeviceCapabilities"/> structure to store the <see cref="MidiInputDevice"/>'s capabilities.</param>
        private MidiInputDevice(int deviceID, API.MidiInputDeviceCapabilities capabilities) : base(deviceID, capabilities)
        {
            _Capabilities = capabilities;
            _Callback = Callback;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a list installed MIDI input devices installed in the system.
        /// </summary>
        public static ReadOnlyCollection<MidiInputDevice> DevicesList
        {
            get
            {
                lock (_Lock)
                {
                    Initialize();

                    return new ReadOnlyCollection<MidiInputDevice>(_DeviceList);
                }
            }
        }

        /// <summary>
        /// Gets whether the MIDI input device is openend or closed.
        /// </summary>
        public bool IsOpen { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the internal list of installed MIDI input devices.
        /// </summary>
        /// <exception cref="MidiInputDeviceException">Raises error #2: MULTIMEDIA_SYSTEM_ERROR_BAD_DEVICE_ID, the specified device ID is out of range.</exception>
        /// <exception cref="MidiInputDeviceException">Raises error #6: MULTIMEDIA_SYSTEM_ERROR_NO_DRIVER, the driver is not installed.</exception>
        /// <exception cref="MidiInputDeviceException">Raises error #7: MULTIMEDIA_SYSTEM_ERROR_NO_MEM, the system is unable to allocate or lock memory.</exception>
        /// <exception cref="MidiInputDeviceException">Raises error #11: MULTIMEDIA_SYSTEM_ERROR_INVALID_PARAMETER, the specified pointer or structure is invalid.</exception>
        private static void Initialize()
        {
            _DeviceList = new MidiInputDevice[API.MidiInputDeviceCount()];

            for (int deviceID = 0; deviceID < _DeviceList.Length; deviceID++)
            {
                API.MidiInputDeviceCapabilities capabilities = new API.MidiInputDeviceCapabilities();

                InvalidateResult(API.GetMidiInputDeviceCapabilities(deviceID, ref capabilities));

                _DeviceList[deviceID] = new MidiInputDevice(deviceID, capabilities);
            }
        }

        /// <summary>
        /// Opens the MIDI input device for receiving MIDI messages.
        /// </summary>
        /// <exception cref="MidiInputDeviceException">Raises error #2: MULTIMEDIA_SYSTEM_ERROR_BAD_DEVICE_ID, the specified device ID is out of range.</exception>
        /// <exception cref="MidiInputDeviceException">Raises error #4: MULTIMEDIA_SYSTEM_ERROR_ALLOCATED when the device is alread opened.</exception>
        /// <exception cref="MidiInputDeviceException">Raises error #7: MULTIMEDIA_SYSTEM_ERROR_NO_MEM, the system is unable to allocate or lock memory.</exception>
        /// <exception cref="MidiInputDeviceException">Raises error #10: MULTIMEDIA_SYSTEM_ERROR_INVALID_FLAG, the flags specified are invalid.</exception>
        /// <exception cref="MidiInputDeviceException">Raises error #11: MULTIMEDIA_SYSTEM_ERROR_INVALID_PARAMETER, the specified pointer or structure is invalid.</exception>
        public void Open()
        {
            InvalidateResult(API.OpenMidiInputDevice(ref _Handle, this.ID, _Callback, IntPtr.Zero));
        }

        /// <summary>
        /// Closes the MIDI input device.
        /// </summary>
        /// <exception cref="MidiInputDeviceException">Raises error #5: MULTIMEDIA_SYSTEM_ERROR_INVALID_HANDLE, the specified device handle is invalid.</exception>
        /// <exception cref="MidiInputDeviceException">Raises error #7: MULTIMEDIA_SYSTEM_ERROR_NO_MEM, the system is unable to allocate or lock memory.</exception>
        /// <exception cref="MidiInputDeviceException">Raises error #65: MIDI_ERROR_STILL_PLAYING, buffers are still in the queue.</exception>
        public void Close()
        {
            InvalidateResult(API.CloseMidiInputDevice(_Handle));
        }

        /// <summary>
        /// Turns off all notes on all MIDI channels.
        /// </summary>
        /// <exception cref="MidiInputDeviceException">Raises error #5: MULTIMEDIA_SYSTEM_ERROR_INVALID_HANDLE, the specified device handle is invalid.</exception>
        public void Reset()
        {
            InvalidateResult(API.ResetMidiInputDevice(_Handle));
        }

        /// <summary>
        /// Starts listening for MIDI messages on the MIDI input device.
        /// </summary>
        /// <exception cref="MidiInputDeviceException">Raises error #5: MULTIMEDIA_SYSTEM_ERROR_INVALID_HANDLE, the specified device handle is invalid.</exception>
        public void Start()
        {
            // TODO: Buffering for long MIDI messages

            InvalidateResult(API.StartMidiInputDevice(_Handle));
        }

        /// <summary>
        /// Stops listening for MIDI messages on the MIDI input device.
        /// </summary>
        /// <exception cref="MidiInputDeviceException">Raises error #5: MULTIMEDIA_SYSTEM_ERROR_INVALID_HANDLE, the specified device handle is invalid.</exception>
        public void Stop()
        {
            InvalidateResult(API.StopMidiInputDevice(_Handle));
        }

        /// <summary>
        /// Connects a MIDI output or thru device to the MIDI input device.
        /// Received <see cref="API.MidiInputMessage"/>s are sent through to the connected MIDI output or thru device.
        /// </summary>
        /// <param name="device">A <see cref="MidiOutputDevice"/> or MIDI thru device to connect to.</param>
        /// <exception cref="MidiInputDeviceException">Raises error #5: MULTIMEDIA_SYSTEM_ERROR_INVALID_HANDLE, the specified device handle is invalid.</exception>
        /// <exception cref="MidiInputDeviceException">Raises error #67: MIDI_ERROR_NOT_READY, the specified input device is already connected to an output device.</exception>
        /// <remarks>For MIDI thru devices, a handle must be obtained by a calling the <see cref="API.OpenMidiOutputDevice"/> method.</remarks>
        public void Connect(MidiOutputDevice device)
        {
            InvalidateResult(API.ConnectMidiDevices(_Handle, device._Handle));
        }

        /// <summary>
        /// Disconnects a MIDI output or thru device from the MIDI input device.
        /// </summary>
        /// <param name="device">A <see cref="MidiOutputDevice"/> or MIDI thru device to disconnect from.</param>
        /// <exception cref="MidiInputDeviceException">Raises error #5: MULTIMEDIA_SYSTEM_ERROR_INVALID_HANDLE, the specified device handle is invalid.</exception>
        public void Disconnect(MidiOutputDevice device)
        {
            InvalidateResult(API.DisconnectMidiDevices(_Handle, device._Handle));
        }

        /// <summary>
        /// Callback function to be called by the system.
        /// </summary>
        /// <param name="handle">An <see cref="API.MidiDeviceHandle"/> to the MIDI input device to associate with the callback function.</param>
        /// <param name="message">An <see cref="API.MidiInputMessage"/> containing the message.</param>
        /// <param name="instance">An <see cref="IntPtr"/> to the instance data supplied by the <see cref="API.OpenMidiInputDevice"/> function.</param>
        /// <param name="messageParameterA">An <see cref="IntPtr"/> to the first message parameter.</param>
        /// <param name="messageParameterB">An <see cref="IntPtr"/> to the second message parameter.</param>
        private void Callback(API.MidiDeviceHandle handle, API.MidiInputMessage message, IntPtr instance, IntPtr messageParameterA, IntPtr messageParameterB)
        {
            if (message == API.MidiInputMessage.MIDI_INPUT_MESSAGE_OPEN)
            {
                this.IsOpen = true;
            }
            else if (message == API.MidiInputMessage.MIDI_INPUT_MESSAGE_CLOSE)
            {
                this.IsOpen = false;
            }

            // TODO: MIDI Messages, changes order of if statements for performance or use switch statement?
        }

        /// <summary>
        /// Invalidates the result returned by API calls for errors and throws a <see cref="MidiInputDeviceException"/> if an error occured.
        /// </summary>
        /// <param name="result">An <see cref="API.Result"/> value containing the result of the API call.</param>
        private static void InvalidateResult(API.Result result)
        {
            if (result != API.Result.MULTIMEDIA_SYSTEM_ERROR_NO_ERROR)
            {
                throw new MidiInputDeviceException(result);
            }
        }

        #endregion
    }

    /// <summary>
    /// Devines a MIDI input device exception that is thrown when an error occurs with a <see cref="MidiInputDevice"/>.
    /// </summary>
    public class MidiInputDeviceException : MidiDeviceException
    {
        #region Fields

        /// <summary>
        /// A buffer to be filled with the error message received by the <see cref="API.GetMidiInputDeviceErrorText"/> method.
        /// </summary>
        private StringBuilder _Buffer = new StringBuilder(API.MAX_ERROR_TEXT_LENGTH);

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new <see cref="MidiInputDeviceException"/> with the specified error code.
        /// </summary>
        /// <param name="errorCode">A <see cref="API.Result"/> value specifying the error code.</param>
        public MidiInputDeviceException(API.Result errorCode) : base(errorCode)
        {
            API.GetMidiInputDeviceErrorText(errorCode, _Buffer);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the message describing the exception.
        /// </summary>
        public override string Message
        {
            get { return $"MIDI Input Device Error #{ErrorCode}\n{_Buffer}"; }
        }

        #endregion
    }
}
