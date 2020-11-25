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
        /// See <see cref="GetMidiOutputDeviceCapabilities"/> for information.
        /// </summary>
        [DllImport("winmm.dll", SetLastError = true)]
        private static extern Result midiOutGetDevCaps(IntPtr deviceID, ref MidiOutputDeviceCapabilities deviceCapabilities, int deviceCapabilitiesSize);

        /// <summary>
        /// See <see cref="OpenMidiOutputDevice"/> for information.
        /// </summary>
        [DllImport("winmm.dll", SetLastError = true)]
        private static extern Result midiOutOpen(ref MidiDeviceHandle deviceHandle, int deviceID, MidiOutputDelegate callback, IntPtr callbackInstance, MidiOpenFlags flags);

        /// <summary>
        /// See <see cref="CloseMidiOutputDevice"/> for information.
        /// </summary>
        [DllImport("winmm.dll", SetLastError = true)]
        private static extern Result midiOutClose(MidiDeviceHandle deviceHandle);

        /// <summary>
        /// See <see cref="ResetMidiOutputDevice"/> for information.
        /// </summary>
        [DllImport("winmm.dll", SetLastError = true)]
        private static extern Result midiOutReset(MidiDeviceHandle deviceHandle);

        /// <summary>
        /// See <see cref="SendShortMessage"/> for information.
        /// </summary>
        [DllImport("winmm.dll", SetLastError = true)]
        private static extern Result midiOutShortMsg(MidiDeviceHandle deviceHandle, int message);

        /// <summary>
        /// See <see cref="SendLongMessage"/> for information.
        /// </summary>
        [DllImport("winmm.dll", SetLastError = true)]
        private static extern Result midiOutLongMsg(MidiDeviceHandle deviceHandle, IntPtr midiHeader, int midiHeaderSize);

        /// <summary>
        /// See <see cref="PrepareMidiOutputHeader"/> for information.
        /// </summary>
        [DllImport("winmm.dll", SetLastError = true)]
        private static extern Result midiOutPrepareHeader(MidiDeviceHandle deviceHandle, IntPtr midiHeader, int midiHeaderSize);

        /// <summary>
        /// See <see cref="UnprepareMidiOutputHeader"/> for information.
        /// </summary>
        [DllImport("winmm.dll", SetLastError = true)]
        private static extern Result midiOutUnprepareHeader(MidiDeviceHandle deviceHandle, IntPtr midiHeader, int midiHeaderSize);

        /// <summary>
        /// See <see cref="GetMidiOutputDeviceErrorText"/> for information.
        /// </summary>
        [DllImport("winmm.dll", SetLastError = true)]
        private static extern Result midiOutGetErrorText(Result result, StringBuilder stringBuilder, int stringBuilderCapacity);

        /// <summary>
        /// See <see cref="MidiInputDeviceCount"/> for information.
        /// </summary>
        [DllImport("winmm.dll", SetLastError = true)]
        private static extern int midiInGetNumDevs();

        /// <summary>
        /// See <see cref="GetMidiInputDeviceCapabilities"/> for information.
        /// </summary>
        [DllImport("winmm.dll", SetLastError = true)]
        private static extern Result midiInGetDevCaps(IntPtr deviceID, ref MidiInputDeviceCapabilities deviceCapabilities, int deviceCapabilitiesSize);

        /// <summary>
        /// See <see cref="OpenMidiInputDevice"/> for information.
        /// </summary>
        [DllImport("winmm.dll", SetLastError = true)]
        private static extern Result midiInOpen(ref MidiDeviceHandle deviceHandle, int deviceID, MidiInputDelegate callback, IntPtr callbackInstance, MidiOpenFlags flags);

        /// <summary>
        /// See <see cref="CloseMidiInputDevice"/> for information.
        /// </summary>
        [DllImport("winmm.dll", SetLastError = true)]
        private static extern Result midiInClose(MidiDeviceHandle deviceHandle);

        /// <summary>
        /// See <see cref="ResetMidiInputDevice"/> for information.
        /// </summary>
        [DllImport("winmm.dll", SetLastError = true)]
        private static extern Result midiInReset(MidiDeviceHandle deviceHandle);

        /// <summary>
        /// See <see cref="StartMidiInputDevice"/> for information.
        /// </summary>
        [DllImport("winmm.dll", SetLastError = true)]
        private static extern Result midiInStart(MidiDeviceHandle deviceHandle);

        /// <summary>
        /// See <see cref="StopMidiInputDevice"/> for information.
        /// </summary>
        [DllImport("winmm.dll", SetLastError = true)]
        private static extern Result midiInStop(MidiDeviceHandle deviceHandle);

        /// <summary>
        /// See <see cref="AddMidiInputBuffer"/> for information.
        /// </summary>
        [DllImport("winmm.dll", SetLastError = true)]
        private static extern Result midiInAddBuffer(MidiDeviceHandle deviceHandle, IntPtr midiHeader, int midiHeaderSize);

        /// <summary>
        /// See <see cref="PrepareMidiInputHeader"/> for information.
        /// </summary>
        [DllImport("winmm.dll", SetLastError = true)]
        private static extern Result midiInPrepareHeader(MidiDeviceHandle deviceHandle, IntPtr midiHeader, int midiHeaderSize);

        /// <summary>
        /// See <see cref="UnprepareMidiInputHeader"/> for information.
        /// </summary>
        [DllImport("winmm.dll", SetLastError = true)]
        private static extern Result midiInUnprepareHeader(MidiDeviceHandle deviceHandle, IntPtr midiHeader, int midiHeaderSize);

        /// <summary>
        /// See <see cref="GetMidiInputDeviceErrorText"/> for information.
        /// </summary>
        [DllImport("winmm.dll", SetLastError = true)]
        private static extern Result midiInGetErrorText(Result result, StringBuilder stringBuilder, int stringBuilderCapacity);

        /// <summary>
        /// See <see cref="ConnectMidiDevices"/> for information.
        /// </summary>
        [DllImport("winmm.dll")]
        private static extern Result midiConnect(MidiDeviceHandle handleA, MidiDeviceHandle handleB, IntPtr reserved);

        /// <summary>
        /// See <see cref="DisconnectMidiDevices"/> for information.
        /// </summary>
        [DllImport("winmm.dll")]
        private static extern Result midiDisconnect(MidiDeviceHandle handleA, MidiDeviceHandle handleB, IntPtr reserved);

        #endregion

        #region Constants

        /// <summary>
        /// The maximum lenght of a device name field in the <see cref="MidiInputDeviceCapabilities"/> and <see cref="MidiOutputDeviceCapabilities"/> structures.
        /// </summary>
        public const int MAX_DEVICE_NAME_LENGTH = 32;

        /// <summary>
        /// The maximum length of error text returned by the <see cref=""/> and <see cref=""/> functions.
        /// </summary>
        public const int MAX_ERROR_TEXT_LENGTH = 256;

        #endregion

        #region Delegates

        /// <summary>
        /// Delegate providing the signature for the callback function handling outgoing MIDI messages.
        /// </summary>
        /// <param name="deviceHandle">A <see cref="MidiDeviceHandle"/> to the MIDI output device to associate with the callback function.</param>
        /// <param name="message">A <see cref="MidiOutputMessage"/> received from the MIDI output device.</param>
        /// <param name="instance">Instance data supplied by the <see cref="OpenMidiOutputDevice"/> method.</param>
        /// <param name="messageParameterA">Dependent on the received <see cref="MidiOutputMessage"/>.</param>
        /// <param name="messageParameterB">Dependent on the received <see cref="MidiOutputMessage"/>.</param>
        public delegate void MidiOutputDelegate(MidiDeviceHandle deviceHandle, MidiOutputMessage message, IntPtr instance, IntPtr messageParameterA, IntPtr messageParameterB);

        /// <summary>
        /// Delegate providing the signature for the callback function handling incoming MIDI messages.
        /// </summary>
        /// <param name="deviceHandle">A <see cref="MidiDeviceHandle"/> to the MIDI input device to associate with the callback function.</param>
        /// <param name="message">A <see cref="MidiInputMessage"/> received from the MIDI input device.</param>
        /// <param name="instance">Instance data supplied by the <see cref="OpenMidiInputDevice"/> method.</param>
        /// <param name="messageParameterA">Dependent on the received <see cref="MidiInputMessage"/>.</param>
        /// <param name="messageParameterB">Dependent on the received <see cref="MidiInputMessage"/>.</param>
        public delegate void MidiInputDelegate(MidiDeviceHandle deviceHandle, MidiInputMessage message, IntPtr instance, IntPtr messageParameterA, IntPtr messageParameterB);

        #endregion

        #region Enumerations

        /// <summary>
        /// Defines the possible return codes of Windows multimedia system API calls.
        /// </summary>
        public enum Result : int
        {
            // Multimedia system general return codes
            /// <summary>
            /// The operation was succesul.
            /// </summary>
            MULTIMEDIA_SYSTEM_ERROR_NO_ERROR            = 0,  // Operation was succesful

            /// <summary>
            /// An unspecified error occured.
            /// </summary>
            MULTIMEDIA_SYSTEM_ERROR_ERROR               = 1,  // Unspecified error

            /// <summary>
            /// The specified device ID is out of range.
            /// </summary>
            MULTIMEDIA_SYSTEM_ERROR_BAD_DEVICE_ID       = 2,  // An out of range device ID was specified
            MULTIMEDIA_SYSTEM_ERROR_NOT_ENABLED         = 3,  // Driver not enabled

            /// <summary>
            /// The device is alread opened.
            /// </summary>
            MULTIMEDIA_SYSTEM_ERROR_ALLOCATED           = 4,  // The device is already open and is not available

            /// <summary>
            /// The specified device handle is invalid.
            /// </summary>
            MULTIMEDIA_SYSTEM_ERROR_INVALID_HANDLE      = 5,  // An invalid device handle was specified, or the device is closed and the handle is no longer valid.
            MULTIMEDIA_SYSTEM_ERROR_NO_DRIVER           = 6,  // No device driver is present for this device

            /// <summary>
            /// The system is unable to allocate or lock memory.
            /// </summary>
            MULTIMEDIA_SYSTEM_ERROR_NO_MEM              = 7,  // Driver memory allocation error
            MULTIMEDIA_SYSTEM_ERROR_NOT_SUPPORTED       = 8,  // Unsupported function
            MULTIMEDIA_SYSTEM_ERROR_BAD_ERROR_NUMBER    = 9,  // Error value out of range
            /// <summary>
            /// The flags specified are invalid.
            /// </summary>
            MULTIMEDIA_SYSTEM_ERROR_INVALID_FLAG        = 10, // Invalid flag passed
            /// <summary>
            /// The specified pointer or structure is invalid.
            /// </summary>
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
            /// <summary>
            /// The buffer pointed to by the MIDI header has not been prepared. 
            /// </summary>
            MIDI_ERROR_UNPREPARED                       = 64, // Header not prepared
            /// <summary>
            /// Buffers are still in the queue.
            /// </summary>
            MIDI_ERROR_STILL_PLAYING                    = 65, // Attempt to close device while still playing
            MIDI_ERROR_NO_MAP                           = 66, // No configured instruments

            /// <summary>
            /// The hardware is busy with other data. 
            /// </summary>
            MIDI_ERROR_NOT_READY                        = 67, // Hardware is busy
            /// <summary>
            /// No MIDI port was found, only occurs when the mapper is opened.
            /// </summary>
            MIDI_ERROR_NO_DEVICE                        = 68, // Port no longer connected
            MIDI_ERROR_INVALID_SETUP                    = 69, // Invalid memory initialization file
            /// <summary>
            /// The application sent a message without a status byte to a stream handle. 
            /// </summary>
            MIDI_ERROR_BAD_OPEN_MODE                    = 70, // Operation unsupported open mode
            MIDI_ERROR_DONT_CONTINUE                    = 71, // Thru device stops a message
            MIDI_ERROR_LAST_ERROR                       = 71  // Last error
        }

        /// <summary>
        /// Defines the possible results for the <see cref="MidiOutputDeviceCapabilities.DeviceType"/> field.
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

        /// <summary>
        /// Defines the possible flags to pass to the <see cref="OpenMidiOutputDevice"/> and <see cref="OpenMidiInputDevice"/> methods.
        /// </summary>
        private enum MidiOpenFlags : int
        {
            CALLBACK_NULL       = 0x00000, // No callback mechanism
            CALLBACK_IO_STATUS  = 0x00020, // The callback in combination with CALLBACK_FUNCTION enables MIDI_INPUT_MESSAGE_MORE_DATA to be sent to the callback function
            CALLBACK_WINDOW     = 0x10000, // The callback parameter is a window handle
            CALLBACK_THREAD     = 0x20000, // The callback parameter is a thread identifier
            CALLBACK_FUNCTION   = 0x30000, // The callback parameter is a callback function address
            CALLBACK_EVENT      = 0x50000  // The callback parameter is an event handle, for output only
        }

         /// <summary>
        /// Defines the possible messages sent to the MIDI output device's callback function.
        /// </summary>
        public enum MidiOutputMessage : int
        {
            MIDI_OUTPUT_MESSAGE_OPEN    = 0x3C7, // MIDI Output device is opened
            MIDI_OUTPUT_MESSAGE_CLOSE   = 0x3C8, // MIDI Output device is closed
            MIDI_OUTPUT_MESSAGE_DONE    = 0x3C9  // System exclusive or stream buffer has been played an returned to the application
        }

        /// <summary>
        /// Defines the possible message sent to the MIDI input device's callback function.
        /// </summary>
        public enum MidiInputMessage : int
        {
            MIDI_INPUT_MESSAGE_OPEN         = 0x3C1, // MIDI Input device is opened
            MIDI_INPUT_MESSAGE_CLOSE        = 0x3C2, // MIDI Input device is closed
            MIDI_INPUT_MESSAGE_DATA         = 0x3C3, // MIDI Short message reseived
            MIDI_INPUT_MESSAGE_LONG_DATA    = 0x3C4, // MIDI Long message received
            MIDI_INPUT_MESSAGE_ERROR        = 0x3C5, // Invalid MIDI message received
            MIDI_INPUT_MESSAGE_LONG_ERROR   = 0x3C6, // Invalid or incomplete system exclusive message received
            MIDI_INPUT_MESSAGE_MORE_DATA    = 0x3CC  // Application is not processing data fast enough
        }

        /// <summary>
        /// Defines the possible flags for the <see cref="MidiHeader.Flags"/> field.
        /// </summary>
        public enum MidiHeaderFlags : int
        {
            MIDI_HEADER_DONE        = 1, // Set by device driver to indicate it is finished with the buffer and is returning to the application
            MIDI_HEADER_PREPARED    = 2, // Set by Windows to indicate that the buffer has been prepared
            MIDI_HEADER_ENQUEUED    = 4, // Set by Windows to indicate that the buffer is queued for playback
            MIDI_HEADER_STREAM      = 8  // Set to indicate that the buffer is a stream buffer
        }

        #endregion

        #region Structures

        /// <summary>
        /// Defines a handle to a MIDI device.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MidiDeviceHandle
        {
            public IntPtr Handle;
        }

        /// <summary>
        /// Defines a structure to store an outgoing system exclusive message or stream buffer.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct MidiHeader
        {
            public IntPtr Data;           // Pointer to MIDI data
            public int BufferSize;        // Size of the buffer
            public int BytesRecorded;     // Actual amount of data in the buffer
            public IntPtr UserData;       // Custom user data
            public MidiHeaderFlags Flags; // Flags providing information about the buffer
            public IntPtr Next;           // Reserved
            public IntPtr Reserved;       // Reserved
            public int Offset;            // Offset into the buffer when a callback is performed, enables to determine which event caused the callback

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public int[] ReservedArray;   // Reserved
        }

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
        internal static int MidiOutputDeviceCount()
        {
            return midiOutGetNumDevs();
        }

        /// <summary>
        /// Counts the number of available MIDI input devices in the system.
        /// </summary>
        /// <returns>An <see cref="int"/> representing the number of available MIDI input devices in the system.</returns>
        internal static int MidiInputDeviceCount()
        {
            return midiInGetNumDevs();
        }

        /// <summary>
        /// Queries the specified MIDI output device to determine its capabilities.
        /// </summary>
        /// <param name="deviceID">An <see cref="int"/> representing the MIDI output device's ID.</param>
        /// <param name="deviceCapabilities">A reference to the <see cref="MidiOutputDeviceCapabilities"/> structure to store the MIDI output device's capabilities.</param>
        /// <returns>A <see cref="Result"/> value containing the result of the API call.</returns>
        internal static Result GetMidiOutputDeviceCapabilities(int deviceID, ref MidiOutputDeviceCapabilities deviceCapabilities)
        {
            return midiOutGetDevCaps((IntPtr)deviceID, ref deviceCapabilities, Marshal.SizeOf(typeof(MidiOutputDeviceCapabilities)));
        }

        /// <summary>
        /// Queries the specified MIDI input device to determine its capabilities.
        /// </summary>
        /// <param name="deviceID">An <see cref="int"/> representing the MIDI input device's ID.</param>
        /// <param name="deviceCapabilities">A reference to the <see cref="MidiInputDeviceCapabilities"/> structure to store the MIDI input device's capabilities.</param>
        /// <returns>A <see cref="Result"/> value containing the result of the API call.</returns>
        internal static Result GetMidiInputDeviceCapabilities(int deviceID, ref MidiInputDeviceCapabilities deviceCapabilities)
        {
            return midiInGetDevCaps((IntPtr)deviceID, ref deviceCapabilities, Marshal.SizeOf(typeof(MidiInputDeviceCapabilities)));
        }

        /// <summary>
        /// Opens the specified MIDI output device for sending MIDI data.
        /// </summary>
        /// <param name="deviceHandle">A <see cref="MidiDeviceHandle"/> to store the obtained device handle.</param>
        /// <param name="deviceID">An <see cref="int"/> representing the MIDI output device's ID.</param>
        /// <param name="callback">A <see cref="MidiOutputDelegate"/> specifying the callback function to process MIDI messages.</param>
        /// <param name="callbackInstance">An <see cref="IntPtr"/> specifying user instance data passed to the callback function.</param>
        /// <returns>A <see cref="Result"/> value containing the result of the API call.</returns>
        internal static Result OpenMidiOutputDevice(ref MidiDeviceHandle deviceHandle, int deviceID, MidiOutputDelegate callback, IntPtr callbackInstance)
        {
            return midiOutOpen(ref deviceHandle, deviceID, callback, callbackInstance, callback == null ? MidiOpenFlags.CALLBACK_NULL : MidiOpenFlags.CALLBACK_FUNCTION);
        }

        /// <summary>
        /// Opens the specified MIDI input device for receiving MIDI data.
        /// </summary>
        /// <param name="deviceHandle">A <see cref="MidiDeviceHandle"/> to store the obtained device handle.</param>
        /// <param name="deviceID">An <see cref="int"/> representing the MIDI input device's ID.</param>
        /// <param name="callback">A <see cref="MidiInputDelegate"/> specifying the callback function to process MIDI messages.</param>
        /// <param name="callbackInstance">An <see cref="IntPtr"/> specifying user instance data passed to the callback function.</param>
        /// <returns>A <see cref="Result"/> value containing the result of the API call.</returns>
        internal static Result OpenMidiInputDevice(ref MidiDeviceHandle deviceHandle, int deviceID, MidiInputDelegate callback, IntPtr callbackInstance)
        {
            return midiInOpen(ref deviceHandle, deviceID, callback, callbackInstance, callback == null ? MidiOpenFlags.CALLBACK_NULL : MidiOpenFlags.CALLBACK_FUNCTION | MidiOpenFlags.CALLBACK_IO_STATUS);
        }

        /// <summary>
        /// Closes the specified MIDI output device.
        /// </summary>
        /// <param name="deviceHandle">A <see cref="MidiDeviceHandle"/> referencing the MIDI output device to close.</param>
        /// <returns>A <see cref="Result"/> value containing the result of the API call.</returns>
        internal static Result CloseMidiOutputDevice(MidiDeviceHandle deviceHandle)
        {
            return midiOutClose(deviceHandle);
        }

        /// <summary>
        /// Closes the specified MIDI input device.
        /// </summary>
        /// <param name="deviceHandle">A <see cref="MidiDeviceHandle"/> referencing the MIDI input device to close.</param>
        /// <returns>A <see cref="MultiMediaResult"/> value containing the result of the API call.</returns>
        internal static Result CloseMidiInputDevice(MidiDeviceHandle deviceHandle)
        {
            return midiInClose(deviceHandle);
        }

        /// <summary>
        /// Resets the specified MIDI output device, turning off all notes on all MIDI channels.
        /// </summary>
        /// <param name="deviceHandle">A <see cref="MidiDeviceHandle"/> referencing the MIDI output device to reset.</param>
        /// <returns>A <see cref="Result"/> value containing the result of the API call.</returns>
        internal static Result ResetMidiOutputDevice(MidiDeviceHandle deviceHandle)
        {
            return midiOutReset(deviceHandle);
        }

        /// <summary>
        /// Resets the specified MIDI input device, stopping input on all MIDI channels.
        /// </summary>
        /// <param name="deviceHandle">A <see cref="MidiDeviceHandle"/> referencing the MIDI input device to reset.</param>
        /// <returns>A <see cref="Result"/> value containing the result of the API call.</returns>
        internal static Result ResetMidiInputDevice(MidiDeviceHandle deviceHandle)
        {
            return midiInReset(deviceHandle);
        }

        /// <summary>
        /// Method to retreive a textual description for a MIDI output device error identified by the specified <see cref="Result"/> error code.
        /// </summary>
        /// <param name="errorCode">A <see cref="Result"/> value specifying the error code.</param>
        /// <param name="buffer">A <see cref="StringBuilder"/> object to function as buffer to be filled with the textual description.</param>
        /// <returns>A <see cref="Result"/> value containing the result of the API call.</returns>
        internal static Result GetMidiOutputDeviceErrorText(Result errorCode, StringBuilder buffer)
        {
            return midiOutGetErrorText(errorCode, buffer, buffer.Capacity);
        }

        /// <summary>
        /// Method to retreive a textual description for a MIDI input device error identified by the specified <see cref="Result"/> error code.
        /// </summary>
        /// <param name="errorCode">A <see cref="Result"/> value specifying the error code.</param>
        /// <param name="buffer">A <see cref="StringBuilder"/> object to function as buffer to be filled with the textual description.</param>
        /// <returns>A <see cref="Result"/> value containing the result of the API call.</returns>
        internal static Result GetMidiInputDeviceErrorText(Result errorCode, StringBuilder buffer)
        {
            return midiInGetErrorText(errorCode, buffer, buffer.Capacity);
        }

        /// <summary>
        /// Method to start receiving MIDI data on a MIDI input device.
        /// </summary>
        /// <param name="deviceHandle">A <see cref="MidiDeviceHandle"/> referencing the MIDI input device to start receiving.</param>
        /// <returns>A <see cref="Result"/> value containing the result of the API call.</returns>
        internal static Result StartMidiInputDevice(MidiDeviceHandle deviceHandle)
        {
            return midiInStart(deviceHandle);
        }

        /// <summary>
        /// Method to stop receiving MIDI data on a MIDI input device.
        /// </summary>
        /// <param name="deviceHandle">A <see cref="MidiDeviceHandle"/> referencing the MIDI input device to stop receiving.</param>
        /// <returns>A <see cref="Result"/> value containing the result of the API call.</returns>
        internal static Result StopMidiInputDevice(MidiDeviceHandle deviceHandle)
        {
            return midiInStop(deviceHandle);
        }

        /// <summary>
        /// Connects a MIDI input device to a MIDI thru or output device or connects a MIDI thru device to a MIDI output device.
        /// </summary>
        /// <param name="inputMidiDeviceHandle">A <see cref="MidiDeviceHandle"/> referencing the MIDI input or thru device to connect.</param>
        /// <param name="outputMidiDeviceHandle">A <see cref="MidiDeviceHandle"/> referencing the MIDI output or thru device to connect.</param>
        /// <returns>A <see cref="Result"/> value containing the result of the API call.</returns>
        /// <remarks><i>For MIDI thru devices, a handle must be obtained by a calling the <see cref="OpenMidiOutputDevice"/> method.</i></remarks>
        public static Result ConnectMidiDevices(MidiDeviceHandle inputMidiDeviceHandle, MidiDeviceHandle outputMidiDeviceHandle)
        {
            return midiConnect(inputMidiDeviceHandle, outputMidiDeviceHandle, IntPtr.Zero);
        }

        /// <summary>
        /// Disconnects a MIDI input device from a MIDI thru or output device or disconnects a MIDI thru device from a MIDI output device.
        /// </summary>
        /// <param name="inputMidiDeviceHandle">An <see cref="MidiDeviceHandle"/> referencing the MIDI input or thru device to disconnect.</param>
        /// <param name="outputMidiDeviceHandle">An <see cref="MidiDeviceHandle"/> referencing the MIDI output or thru device to disconnect.</param>
        /// <returns>A <see cref="MultiMediaResult"/> value containing the result of the API call.</returns>
        public static Result DisconnectMidiDevices(MidiDeviceHandle inputMidiDeviceHandle, MidiDeviceHandle outputMidiDeviceHandle)
        {
            return midiDisconnect(inputMidiDeviceHandle, outputMidiDeviceHandle, IntPtr.Zero);
        }

        /// <summary>
        /// Sends a short MIDI message to the specified <see cref="MidiOutputDevice"/>.
        /// </summary>
        /// <param name="deviceHandle">A <see cref="MidiDeviceHandle"/> referencing the MIDI output device to send the short MIDI message to.</param>
        /// <param name="message">An <see cref="int"/> containting the MIDI message to send.</param>
        /// <returns>A <see cref="Result"/> value containing the result of the API call.</returns>
        internal static Result SendShortMessage(MidiDeviceHandle deviceHandle, int message)
        {
            return midiOutShortMsg(deviceHandle, message);
        }

        /// <summary>
        /// Sends a long MIDI message to the specified <see cref="MidiOutputDevice"/>.
        /// </summary>
        /// <param name="deviceHandle">A <see cref="MidiDeviceHandle"/> referencing the MIDI output device to send the short MIDI message to.</param>
        /// <param name="midiHeader">A <see cref="IntPtr"/> referencing the <see cref="MidiHeader"/> structure containing the data to send.</param>
        /// <param name="midiHeaderSize">An <see cref="int"/> specifying the size in bytes of the <see cref="MidiHeader"/> structure.</param>
        /// <returns>A <see cref="Result"/> value containing the result of the API call.</returns>
        internal static Result SendLongMessage(MidiDeviceHandle deviceHandle, IntPtr midiHeader, int midiHeaderSize)
        {
            return midiOutLongMsg(deviceHandle, midiHeader, midiHeaderSize);
        }

        /// <summary>
        /// Prepares a MIDI system exclusive or stream buffer for output.
        /// </summary>
        /// <param name="deviceHandle">A <see cref="MidiDeviceHandle"/> referencing the MIDI output device to prepare the buffer for.</param>
        /// <param name="midiHeader">A <see cref="IntPtr"/> referencing the <see cref="MidiHeader"/> structure that identifies the buffer to be prepared.</param>
        /// <param name="midiHeaderSize">An <see cref="int"/> specifying the size in bytes of the <see cref="MidiHeader"/> structure.</param>
        /// <returns>A <see cref="Result"/> value containing the result of the API call.</returns>
        internal static Result PrepareMidiOutputHeader(MidiDeviceHandle deviceHandle, IntPtr midiHeader, int midiHeaderSize)
        {
            return midiOutPrepareHeader(deviceHandle, midiHeader, midiHeaderSize);
        }

        /// <summary>
        /// Cleans the preparation performed by the <see cref="PrepareMidiOutputHeader"/> method.
        /// </summary>
        /// <param name="deviceHandle">A <see cref="MidiDeviceHandle"/> referencing the MIDI output device to unprepare the buffer for.</param>
        /// <param name="midiHeader">A <see cref="IntPtr"/> referencing the <see cref="MidiHeader"/> structure that identifies the buffer to be unprepared.</param>
        /// <param name="midiHeaderSize">An <see cref="int"/> specifying the size in bytes of the <see cref="MidiHeader"/> structure.</param>
        /// <returns>A <see cref="Result"/> value containing the result of the API call.</returns>
        internal static Result UnprepareMidiOutputHeader(MidiDeviceHandle deviceHandle, IntPtr midiHeader, int midiHeaderSize)
        {
            return midiOutUnprepareHeader(deviceHandle, midiHeader, midiHeaderSize);
        }

        /// <summary>
        /// Adds a buffer to the MIDI input device in order to receive long MIDI messages.
        /// </summary>
        /// <param name="deviceHandle">A <see cref="MidiDeviceHandle"/> referencing the MIDI input device to add the buffer to.</param>
        /// <param name="midiHeader">A <see cref="IntPtr"/> referencing the <see cref="MidiHeader"/> structure that identifies the buffer to add.</param>
        /// <param name="midiHeaderSize">An <see cref="int"/> specifying the size in bytes of the <see cref="MidiHeader"/> structure.</param>
        /// <returns>A <see cref="Result"/> value containing the result of the API call.</returns>
        internal static Result AddMidiInputBuffer(MidiDeviceHandle deviceHandle, IntPtr midiHeader, int midiHeaderSize)
        {
            return midiInAddBuffer(deviceHandle, midiHeader, midiHeaderSize);
        }

        /// <summary>
        /// Prepares a MIDI system exclusive or stream buffer for input.
        /// </summary>
        /// <param name="deviceHandle">A <see cref="MidiDeviceHandle"/> referencing the MIDI input device to prepare the buffer for.</param>
        /// <param name="midiHeader">A <see cref="IntPtr"/> referencing the <see cref="MidiHeader"/> structure that identifies the buffer to be prepared.</param>
        /// <param name="midiHeaderSize">An <see cref="int"/> specifying the size in bytes of the <see cref="MidiHeader"/> structure.</param>
        /// <returns>A <see cref="Result"/> value containing the result of the API call.</returns>
        internal static Result PrepareMidiInputHeader(MidiDeviceHandle deviceHandle, IntPtr midiHeader, int midiHeaderSize)
        {
            return midiInPrepareHeader(deviceHandle, midiHeader, midiHeaderSize);
        }

        /// <summary>
        /// Cleans the preparation performed by the <see cref="PrepareMidiInputHeader"/> method.
        /// </summary>
        /// <param name="deviceHandle">A <see cref="MidiDeviceHandle"/> referencing the MIDI input device to unprepare the buffer for.</param>
        /// <param name="midiHeader">A <see cref="IntPtr"/> referencing the <see cref="MidiHeader"/> structure that identifies the buffer to be unprepared.</param>
        /// <param name="midiHeaderSize">An <see cref="int"/> specifying the size in bytes of the <see cref="MidiHeader"/> structure.</param>
        /// <returns>A <see cref="Result"/> value containing the result of the API call.</returns>
        internal static Result UnprepareMidiInputHeader(MidiDeviceHandle deviceHandle, IntPtr midiHeader, int midiHeaderSize)
        {
            return midiInUnprepareHeader(deviceHandle, midiHeader, midiHeaderSize);
        }

        #endregion
    }
}
