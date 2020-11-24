using System.Collections.ObjectModel;

namespace MidiXL
{
    /// <summary>
    /// Manages listing and selection of MIDI devices
    /// </summary>
    public static class DeviceManager
    {
        #region Fields

        /// <summary>
        /// Array to store the MIDI output devices installed in the system.
        /// </summary>
        private static MidiOutputDevice[] _MidiOutputDevices;

        /// <summary>
        /// Array to store the MIDI input devices installed in the system.
        /// </summary>
        private static MidiInputDevice[]  _MidiInputDevices;

        /// <summary>
        /// Lock for thread safety.
        /// </summary>
        private static readonly object _Lock = new object();

        #endregion

        #region Properties

        /// <summary>
        /// Gets a list of installed MIDI output devices installed in the system.
        /// </summary>
        public static ReadOnlyCollection<MidiOutputDevice> MidiOutputDevices
        {
            get
            {
                if(_MidiOutputDevices == null)
                {
                    lock(_Lock)
                    {
                        InitializeMidiOutputDeviceList();
                    }
                }

                return new ReadOnlyCollection<MidiOutputDevice>(_MidiOutputDevices);
            }
        }

        /// <summary>
        /// Gets a list of installed MIDI input devices installed in the system.
        /// </summary>
        public static ReadOnlyCollection<MidiInputDevice> MidiInputDevices
        {
            get
            {
                if(_MidiInputDevices == null)
                {
                    lock(_Lock)
                    {
                        InitializeMidiInputDeviceList();
                    }
                }

                return new ReadOnlyCollection<MidiInputDevice>(_MidiInputDevices);
            }
        }

        /// <summary>
        /// Gets a MIDI output device by ID.
        /// </summary>
        /// <param name="deviceID">An <see cref="int"/> containing the device ID.</param>
        /// <returns>A <see cref="MidiOutputDevice"/> or <see cref="null"/>.</returns>
        public static MidiOutputDevice GetMidiOutputDevice(int deviceID)
        {
            if (_MidiOutputDevices.Length == 0)
            {
                return null;
            }

            for (int i = 0; i < _MidiOutputDevices.Length; i++)
            {
                if (_MidiOutputDevices[i].ID == deviceID)
                    return _MidiOutputDevices[i];
            }

            return null;
        }

        /// <summary>
        /// Gets a MIDI output device by name.
        /// </summary>
        /// <param name="deviceName">An <see cref="string"/> containing the device name.</param>
        /// <returns>A <see cref="MidiOutputDevice"/> or <see cref="null"/>.</returns>
        public static MidiOutputDevice GetMidiOutputDevice(string deviceName)
        {
            if(_MidiOutputDevices.Length == 0)
            {
                return null;
            }

            for (int i = 0; i < _MidiOutputDevices.Length; i++)
            {
                if (_MidiOutputDevices[i].Name == deviceName)
                    return _MidiOutputDevices[i];
            }

            return null;
        }

        /// <summary>
        /// Gets a MIDI input device by ID.
        /// </summary>
        /// <param name="deviceID">An <see cref="int"/> containing the device ID.</param>
        /// <returns>A <see cref="MidiInputDevice"/> or <see cref="null"/>.</returns>
        public static MidiInputDevice GetMidiInputDevice(int deviceID)
        {
            if (_MidiInputDevices.Length == 0)
                return null;

            for (int i = 0; i < _MidiInputDevices.Length; i++)
            {
                if (_MidiInputDevices[i].ID == deviceID)
                    return _MidiInputDevices[i];
            }

            return null;
        }

        /// <summary>
        /// Gets a MIDI input device by name.
        /// </summary>
        /// <param name="deviceName">An <see cref="string"/> containing the device name.</param>
        /// <returns>A <see cref="MidiInputDevice"/> or <see cref="null"/>.</returns>
        public static MidiInputDevice GetMidiInputDevice(string deviceName)
        {
            if (_MidiInputDevices.Length == 0)
                return null;

            for (int i = 0; i < _MidiInputDevices.Length; i++)
            {
                if (_MidiInputDevices[i].Name == deviceName)
                    return _MidiInputDevices[i];
            }

            return null;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the MIDI input and MIDI output device lists.
        /// </summary>
        public static void Update()
        {
            lock(_Lock)
            {
                _MidiOutputDevices = null;
                _MidiInputDevices  = null;
            }
        }

        /// <summary>
        /// Initializes the internal list of installed MIDI output devices.
        /// </summary>
        /// <exception cref="MidiOutputDeviceException">Raises error #2: MULTIMEDIA_SYSTEM_ERROR_BAD_DEVICE_ID, the specified device ID is out of range.</exception>
        /// <exception cref="MidiOutputDeviceException">Raises error #6: MULTIMEDIA_SYSTEM_ERROR_NO_DRIVER, the driver is not installed.</exception>
        /// <exception cref="MidiOutputDeviceException">Raises error #7: MULTIMEDIA_SYSTEM_ERROR_NO_MEM, the system is unable to allocate or lock memory.</exception>
        /// <exception cref="MidiOutputDeviceException">Raises error #11: MULTIMEDIA_SYSTEM_ERROR_INVALID_PARAMETER, the specified pointer or structure is invalid.</exception>
        private static void InitializeMidiOutputDeviceList()
        {
            _MidiOutputDevices = new MidiOutputDevice[API.MidiOutputDeviceCount()];

            for (int deviceID = 0; deviceID < _MidiOutputDevices.Length; deviceID++)
            {
                API.MidiOutputDeviceCapabilities capabilities = new API.MidiOutputDeviceCapabilities();

                MidiOutputDevice.InvalidateResult(API.GetMidiOutputDeviceCapabilities(deviceID, ref capabilities));

                _MidiOutputDevices[deviceID] = new MidiOutputDevice(deviceID, capabilities);
            }
        }

        /// <summary>
        /// Initializes the internal list of installed MIDI input devices.
        /// </summary>
        /// <exception cref="MidiInputDeviceException">Raises error #2: MULTIMEDIA_SYSTEM_ERROR_BAD_DEVICE_ID, the specified device ID is out of range.</exception>
        /// <exception cref="MidiInputDeviceException">Raises error #6: MULTIMEDIA_SYSTEM_ERROR_NO_DRIVER, the driver is not installed.</exception>
        /// <exception cref="MidiInputDeviceException">Raises error #7: MULTIMEDIA_SYSTEM_ERROR_NO_MEM, the system is unable to allocate or lock memory.</exception>
        /// <exception cref="MidiInputDeviceException">Raises error #11: MULTIMEDIA_SYSTEM_ERROR_INVALID_PARAMETER, the specified pointer or structure is invalid.</exception>
        private static void InitializeMidiInputDeviceList()
        {
            _MidiInputDevices = new MidiInputDevice[API.MidiInputDeviceCount()];

            for (int deviceID = 0; deviceID < _MidiInputDevices.Length; deviceID++)
            {
                API.MidiInputDeviceCapabilities capabilities = new API.MidiInputDeviceCapabilities();

                MidiInputDevice.InvalidateResult(API.GetMidiInputDeviceCapabilities(deviceID, ref capabilities));

                _MidiInputDevices[deviceID] = new MidiInputDevice(deviceID, capabilities);
            }
        }

        #endregion
    }
}
