﻿using System;
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
        /// See <see cref=""/> for information.
        /// </summary>
        [DllImport("winmm.dll", SetLastError = true)]
        private static extern Result midiOutGetErrorText(Result result, StringBuilder stringBuilder, int stringBuilderCapacity);

        /// <summary>
        /// See <see cref="MidiInputDeviceCount"/> for information.
        /// </summary>
        [DllImport("winmm.dll", SetLastError = true)]
        private static extern int midiInGetNumDevs();

        /// <summary>
        /// See <see cref="GetMidiInputDeviceCapabilities(int, ref MidiInputDeviceCapabilities)"/> for information.
        /// </summary>
        [DllImport("winmm.dll", SetLastError = true)]
        private static extern Result midiInGetDevCaps(IntPtr deviceID, ref MidiInputDeviceCapabilities deviceCapabilities, int deviceCapabilitiesSize);

        /// <summary>
        /// See <see cref=""/> for information.
        /// </summary>
        [DllImport("winmm.dll", SetLastError = true)]
        private static extern Result midiInGetErrorText(Result result, StringBuilder stringBuilder, int stringBuilderCapacity);
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
        /// <param name="message">A <see cref="MidiOutputMessage"/> received from the <see cref="MidiOutputDevice"/>.</param>
        /// <param name="instance">Instance data supplied by the <see cref="OpenMidiOutputDevice"/> method.</param>
        /// <param name="messageParameterA">Dependent on the received <see cref="MidiOutputMessage"/>.</param>
        /// <param name="messageParameterB">Dependent on the received <see cref="MidiOutputMessage"/>.</param>
        public delegate void MidiOutputDelegate(MidiDeviceHandle deviceHandle, MidiOutputMessage message, IntPtr instance, IntPtr messageParameterA, IntPtr messageParameterB);

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
        /// Defines the possible flags to pass to the <see cref="OpenMidiOutputDevice"/> and <see cref="OpenMidiInput"/> methods.
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
        /// Defines the the possible messages sent to the MIDI output device's callback function.
        /// </summary>
        public enum MidiOutputMessage : int
        {
            MIDI_OUTPUT_MESSAGE_OPEN    = 0x3C7, // MIDI Output device is opened
            MIDI_OUTPUT_MESSAGE_CLOSE   = 0x3C8, // MIDI Output device is closed
            MIDI_OUTPUT_MESSAGE_DONE    = 0x3C9  // System exclusive or stream buffer has been played an returned to the application
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
        /// Closes the specified MIDI output device.
        /// </summary>
        /// <param name="deviceHandle">A <see cref="MidiDeviceHandle"/> referencing the MIDI output device to close.</param>
        /// <returns>A <see cref="Result"/> value containing the result of the API call.</returns>
        internal static Result CloseMidiOutputDevice(MidiDeviceHandle deviceHandle)
        {
            return midiOutClose(deviceHandle);
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
        #endregion
    }
}
