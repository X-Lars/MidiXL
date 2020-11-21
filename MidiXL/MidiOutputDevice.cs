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
        /// Lock for thread safety.
        /// </summary>
        private static readonly object _Lock = new object();

        #endregion

        #region Constructor

        /// <summary>
        /// Private contstructor to create and initialize a new <see cref="MidiOutputDevice"/>, only invoked by the <see cref="Initialize"/> method.
        /// </summary>
        /// <param name="deviceID">An <see cref="int"/> representing the unique indexed <see cref="MidiOutputDevice"/>'s ID.</param>
        /// <param name="capabilities">An <see cref="API.MidiOutputDeviceCapabilities"/> structure to store the <see cref="MidiOutputDevice"/>'s capabilities.</param>
        private MidiOutputDevice(int deviceID, API.MidiOutputDeviceCapabilities capabilities) : base(deviceID, capabilities)
        {
            _Capabilities = capabilities;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a list installed MIDI output devices installed in the system.
        /// </summary>
        public static IReadOnlyCollection<MidiOutputDevice> DevicesList
        {
            get 
            { 
                lock(_Lock)
                {
                    Initialize();

                    return new ReadOnlyCollection<MidiOutputDevice>(_DeviceList);
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the internal list of installed MIDI output devices.
        /// </summary>
        private static void Initialize()
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
            get { return $"MIDI Output Device Error #{ErrorCode}: {_Buffer}"; }
        }

        #endregion
    }
}
