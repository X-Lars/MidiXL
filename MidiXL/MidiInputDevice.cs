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
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a list installed MIDI input devices installed in the system.
        /// </summary>
        public static IReadOnlyCollection<MidiInputDevice> DevicesList
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

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the internal list of installed MIDI input devices.
        /// </summary>
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
