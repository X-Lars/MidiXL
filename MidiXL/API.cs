using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MidiXL
{
    /// <summary>
    /// 
    /// </summary>
    public static class API
    {
        #region Windows Multimedia API

        #region DLL Imports

        /// <summary>
        /// See <see cref="MidiOutputDeviceCount"/> for information.
        /// </summary>
        [DllImport("winmm.dll", SetLastError = true)]
        private static extern int midiOutGetNumDevs();

        /// <summary>
        /// See <see cref="GetMidiOutputDeviceCapabilities(int, ref MidiOutputDeviceCapabilities)"/> for information.
        /// </summary>
        [DllImport("winmm.dll", SetLastError = true)]
        private static extern Result midiOutGetDevCaps(IntPtr deviceID, ref MidiOutputDeviceCapabilities deviceCapabilities, int deviceCapabilitiesSize);

        /// <summary>
        /// See <see cref="MidiInputDeviceCount"/> for information.
        /// </summary>
        [DllImport("winmm.dll", SetLastError = true)]
        private static extern int midiInGetNumDevs();

        /// <summary>
        /// See <see cref=""/>
        /// </summary>
        [DllImport("winmm.dll", SetLastError = true)]
        private static extern Result midiInGetDevCaps(IntPtr deviceID, ref MidiInputDeviceCapabilities deviceCapabilities, int deviceCapabilitiesSize);


        #endregion

        #region Constants

        /// <summary>
        /// The maximum lenght of a device name field of the <see cref="MidiInputDeviceCapabilities"/> and <see cref="MidiOutputDeviceCapabilities"/> structures.
        /// </summary>
        public const int MAX_DEVICE_NAME_LENGTH = 32;

        #endregion

        #region Enumerations

        /// <summary>
        /// Defines the possible return codes of Windows multimedia system API calls.
        /// </summary>
        public enum Result : int
        {
            // Multimedia system general return codes
            MULTIMEDIA_SYSTEM_ERROR_NO_ERROR            = 0,  // Operation was succesful
            MULTIMEDIA_SYSTEM_ERROR_ERROR               = 1,  // Unspecified error
            MULTIMEDIA_SYSTEM_ERROR_BAD_DEVICE_ID       = 2,  // An out of range device ID was specified
            MULTIMEDIA_SYSTEM_ERROR_NOT_ENABLED         = 3,  // Driver not enabled
            MULTIMEDIA_SYSTEM_ERROR_ALLOCATED           = 4,  // The device is already open and is not available
            MULTIMEDIA_SYSTEM_ERROR_INVALID_HANDLE      = 5,  // An invalid device handle was specified, or the device is closed and the handle is no longer valid.
            MULTIMEDIA_SYSTEM_ERROR_NO_DRIVER           = 6,  // No device driver is present for this device
            MULTIMEDIA_SYSTEM_ERROR_NO_MEM              = 7,  // Driver memory allocation error
            MULTIMEDIA_SYSTEM_ERROR_NOT_SUPPORTED       = 8,  // Unsupported function
            MULTIMEDIA_SYSTEM_ERROR_BAD_ERROR_NUMBER    = 9,  // Error value out of range
            MULTIMEDIA_SYSTEM_ERROR_INVALID_FLAG        = 10, // Invalid flag passed
            MULTIMEDIA_SYSTEM_ERROR_INVALID_PARAMETER   = 11, // Invalid parameter passed
            MULTIMEDIA_SYSTEM_ERROR_HANDLE_BUSY         = 12, // Handle being used simultaneously on another thread
            MULTIMEDIA_SYSTEM_ERROR_INVALID_ALIAS       = 13, // Specified alias not found
            MULTIMEDIA_SYSTEM_ERROR_BAD_DB              = 14, // Bad registry database
            MULTIMEDIA_SYSTEM_ERROR_KEY_NOT_FOUND       = 15, // Registry key not found
            MULTIMEDIA_SYSTEM_ERROR_READ_ERROR          = 16, // Registry read error
            MULTIMEDIA_SYSTEM_ERROR_WRITE_ERROR         = 17, // Registry write error
            MULTIMEDIA_SYSTEM_ERROR_DELETE_ERROR        = 18, // Registry delete error
            MULTIMEDIA_SYSTEM_ERROR_VALUE_NOT_FOUND     = 19, // Registry value not found
            MULTIMEDIA_SYSTEM_ERROR_NO_DRIVER_CB        = 20, // Driver does not call driver callback
            MULTIMEDIA_SYSTEM_ERROR_LAST_ERROR          = 20, // Last error
            
            // Multimedia system MIDI specific return codes
            MIDI_ERROR_UNPREPARED                       = 64, // Header not prepared
            MIDI_ERROR_STILL_PLAYING                    = 65, // Attempt to close device while still playing
            MIDI_ERROR_NO_MAP                           = 66, // No configured instruments
            MIDI_ERROR_NOT_READY                        = 67, // Hardware is busy
            MIDI_ERROR_NO_DEVICE                        = 68, // Port no longer connected
            MIDI_ERROR_INVALID_SETUP                    = 69, // Invalid memory initialization file
            MIDI_ERROR_BAD_OPEN_MODE                    = 70, // Operation unsupported open mode
            MIDI_ERROR_DONT_CONTINUE                    = 71, // Thru device stops a message
            MIDI_ERROR_LAST_ERROR                       = 71  // Last error
        }

        /// <summary>
        /// Defines the possible result for the <see cref="MidiOutputDeviceCapabilities.DeviceType"/> field.
        /// </summary>
        public enum MidiDeviceType : short
        {
            MIDI_DEVICETYPE_MIDI_PORT                    = 1, // MIDI Hardware port
            MIDI_DEVICETYPE_SYNTH                        = 2, // Synthesizer
            MIDI_DEVICETYPE_SQUARE_WAVE_SYNTH            = 3, // Square wave synthesizer
            MIDI_DEVICETYPE_FREQUENCY_MODULATION_SYNTH   = 4, // Frequency modulation synthesizer
            MIDI_DEVICETYPE_MIDI_MAPPER                  = 5, // Microsoft MIDI mapper
            MIDI_DEVICETYPE_WAVETABLE                    = 6, // Hardware wavetable synthesizer
            MIDI_DEVICETYPE_SOFTWARE_SYNTH               = 7, // Software synthesizer
        }

        /// <summary>
        /// Defines the possible flags for the <see cref="MidiOutputDeviceCapabilities.Support"/> field.
        /// </summary>
        public enum MidiSupport : int
        {
            MIDI_SUPPORT_VOLUME    = 0x0001, // Supports volume control
            MIDI_SUPPORT_LRVOLUME  = 0x0002, // Supports separate left and right volume control
            MIDI_SUPPORT_CACHE     = 0x0004, // Supports patch caching
            MIDI_SUPPORT_STREAM    = 0x0008  // Provides direct support for the midiStreamOut function
        }

        #endregion

        #region Structures

        /// <summary>
        /// Defines the structure to store MIDI output device capabilities.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MidiOutputDeviceCapabilities
        {
            private short ManufacturerID;       // The manufacturer ID
            private short ProductID;            // The product ID
            private uint  DriverVersion;        // The driver version, the low order word contains the minor version, the high order word the major version

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_DEVICE_NAME_LENGTH)]
            public string DeviceName;           // The device name

            public MidiDeviceType DeviceType;   // The device type
            public short Voices;                // The number of voices supported by an internal synthesizer
            public short ChannelMask;           // The channels that an internal synthesizer responds to
            public MidiSupport Support;         // Optional functionality supported by the device
        }

        /// <summary>
        /// Defines a structure to store MIDI input device capabilities.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MidiInputDeviceCapabilities
        {
            private short ManufacturerID;       // The manufacturer ID
            private short ProductID;            // The product ID
            private int DriverVersion;          // The driver version, the low order word contains the minor version, the high order word the major version

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_DEVICE_NAME_LENGTH)]
            public string DeviceName;           // The device name
            private int Support;                // Reserved, must be zero
        }

        #endregion


        #endregion

        #region 

        /// <summary>
        /// Counts the number of available MIDI output devices in the system.
        /// </summary>
        /// <returns>An <see cref="int"/> representing the number of available MIDI output devices in the system.</returns>
        public static int MidiOutputDeviceCount()
        {
            return midiOutGetNumDevs();
        }

        /// <summary>
        /// Counts the number of available MIDI input devices in the system.
        /// </summary>
        /// <returns>An <see cref="int"/> representing the number of available MIDI input devices in the system.</returns>
        public static int MidiInputDeviceCount()
        {
            return midiInGetNumDevs();
        }

        /// <summary>
        /// Queries the specified MIDI output device to determine its capabilities.
        /// </summary>
        /// <param name="deviceID">An <see cref="int"/> representing the MIDI output device's ID.</param>
        /// <param name="deviceCapabilities">A reference to the <see cref="MidiOutputDeviceCapabilities"/> structure to store the MIDI output device's capabilities.</param>
        /// <returns>A <see cref="Result"/> value containing the result of the API call.</returns>
        public static Result GetMidiOutputDeviceCapabilities(int deviceID, ref MidiOutputDeviceCapabilities deviceCapabilities)
        {
            return midiOutGetDevCaps((IntPtr)deviceID, ref deviceCapabilities, Marshal.SizeOf(typeof(MidiOutputDeviceCapabilities)));
        }

        /// <summary>
        /// Queries the specified MIDI input device to determine its capabilities.
        /// </summary>
        /// <param name="deviceID">An <see cref="int"/> representing the MIDI input device's ID.</param>
        /// <param name="deviceCapabilities">A reference to the <see cref="MidiInputDeviceCapabilities"/> structure to store the MIDI input device's capabilities.</param>
        /// <returns>A <see cref="Result"/> value containing the result of the API call.</returns>
        public static Result GetMidiInputDeviceCapabilities(int deviceID, ref MidiInputDeviceCapabilities deviceCapabilities)
        {
            return midiInGetDevCaps((IntPtr)deviceID, ref deviceCapabilities, Marshal.SizeOf(typeof(MidiInputDeviceCapabilities)));
        }

        #endregion
    }
}
