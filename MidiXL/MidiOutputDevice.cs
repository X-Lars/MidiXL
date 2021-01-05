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
    public sealed class MidiOutputDevice : MidiDevice
    {
        #region Fields

        /// <summary>
        /// Structure to store the MIDI output device's capabilities.
        /// </summary>
        private API.MidiOutputDeviceCapabilities _Capabilities;

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
        internal MidiOutputDevice(int deviceID, API.MidiOutputDeviceCapabilities capabilities) : base(deviceID, capabilities)
        {
            _Capabilities = capabilities;
            _Callback = Callback;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        /// <summary>
        /// Opens the MIDI output device for sending MIDI messages.
        /// </summary>
        /// <exception cref="MidiOutputDeviceException">Raises <see cref="API.Result.MULTIMEDIA_SYSTEM_ERROR_BAD_DEVICE_ID"/>.</exception>
        /// <exception cref="MidiOutputDeviceException">Raises <see cref="API.Result.MULTIMEDIA_SYSTEM_ERROR_ALLOCATED"/>.</exception>
        /// <exception cref="MidiOutputDeviceException">Raises <see cref="API.Result.MULTIMEDIA_SYSTEM_ERROR_NO_MEM"/>.</exception>
        /// <exception cref="MidiOutputDeviceException">Raises <see cref="API.Result.MULTIMEDIA_SYSTEM_ERROR_INVALID_PARAMETER"/>.</exception>
        /// <exception cref="MidiOutputDeviceException">Raises <see cref="API.Result.MIDI_ERROR_NO_DEVICE"/>.</exception>
        public void Open()
        {
            if (IsOpen)
                return;

            InvalidateResult(API.OpenMidiOutputDevice(ref _Handle, this.ID, _Callback, IntPtr.Zero));
        }

        /// <summary>
        /// Closes the MIDI output device.
        /// </summary>
        /// <exception cref="MidiOutputDeviceException">Raises <see cref="API.Result.MULTIMEDIA_SYSTEM_ERROR_INVALID_HANDLE"/>.</exception>
        /// <exception cref="MidiOutputDeviceException">Raises <see cref="API.Result.MULTIMEDIA_SYSTEM_ERROR_NO_MEM"/>.</exception>
        /// <exception cref="MidiOutputDeviceException">Raises <see cref="API.Result.MIDI_ERROR_STILL_PLAYING"/>.</exception>
        public void Close()
        {
            if (IsOpen)
            {
                InvalidateResult(API.CloseMidiOutputDevice(_Handle));
            }
        }

        /// <summary>
        /// Turns off all notes on all MIDI channels.
        /// </summary>
        /// <exception cref="MidiOutputDeviceException">Raises <see cref="API.Result.MULTIMEDIA_SYSTEM_ERROR_INVALID_HANDLE"/>.</exception>
        public void Reset()
        {
            if (IsOpen)
            {
                InvalidateResult(API.ResetMidiOutputDevice(_Handle));
            }
        }

        /// <summary>
        /// Connects the MIDI output device to a MIDI input or thru device.
        /// Received <see cref="API.MidiInputMessage"/>s are sent through to the connected MIDI output or thru device.
        /// </summary>
        /// <param name="device">A <see cref="MidiInputDevice"/> or MIDI thru device to connect to.</param>
        /// <exception cref="MidiOutputDeviceException">Raises <see cref="API.Result.MULTIMEDIA_SYSTEM_ERROR_INVALID_HANDLE"/>.</exception>
        /// <exception cref="MidiOutputDeviceException">Raises <see cref="API.Result.MIDI_ERROR_NOT_READY"/>.</exception>
        /// <remarks><i>For MIDI thru devices, a handle must be obtained by a calling the <see cref="API.OpenMidiOutputDevice"/> method.</i></remarks>
        public void Connect(MidiInputDevice device)
        {
            InvalidateResult(API.ConnectMidiDevices(device._Handle, _Handle));

            _Connections.Add(device._Handle);
        }

        /// <summary>
        /// Disconnects the MIDI output device from a MIDI input or thru device.
        /// </summary>
        /// <param name="device">A <see cref="MidiInputDevice"/> or MIDI thru device to disconnect from.</param>
        /// <exception cref="MidiOutputDeviceException">Raises <see cref="API.Result.MULTIMEDIA_SYSTEM_ERROR_INVALID_HANDLE"/>.</exception>
        public void Disconnect(MidiInputDevice device)
        {
            InvalidateResult(API.DisconnectMidiDevices(device._Handle, _Handle));

            _Connections.Remove(device._Handle);
        }

        /// <summary>
        /// Disconnects the MIDI output device from all connected MIDI devices.
        /// </summary>
        private void Disconnect()
        {
            if (_Connections == null)
                return;

            foreach (API.MidiDeviceHandle connection in _Connections)
            {
                InvalidateResult(API.DisconnectMidiDevices(connection, _Handle));
            }
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
            else 
            { 
                // API.MidiOutputMessage.MIDI_OUTPUT_MESSAGE_DONE
            }
            
        }

        /// <summary>
        /// Sends a short MIDI message.
        /// </summary>
        /// <param name="message">The short MIDI message to send.</param>
        /// <exception cref="MidiOutputDeviceException">Raises <see cref="API.Result.MULTIMEDIA_SYSTEM_ERROR_INVALID_HANDLE"/>.</exception>
        /// <exception cref="MidiOutputDeviceException">Raises <see cref="API.Result.MIDI_ERROR_NOT_READY"/>.</exception>
        /// <exception cref="MidiOutputDeviceException">Raises <see cref="API.Result.MIDI_ERROR_BAD_OPEN_MODE"/>.</exception>
        public void Send(ShortMessage message)
        {
            InvalidateResult(API.SendShortMessage(_Handle, message.Data));
        }

        /// <summary>
        /// Sends a long MIDI message.
        /// </summary>
        /// <param name="message">The long MIDI message to send.</param>
        /// <exception cref="MidiOutputDeviceException">Raises <see cref="API.Result.MULTIMEDIA_SYSTEM_ERROR_INVALID_HANDLE"/>.</exception>
        /// <exception cref="MidiOutputDeviceException">Raises <see cref="API.Result.MULTIMEDIA_SYSTEM_ERROR_NO_MEM"/>.</exception>
        /// <exception cref="MidiOutputDeviceException">Raises <see cref="API.Result.MULTIMEDIA_SYSTEM_ERROR_INVALID_PARAMETER"/>.</exception>
        /// <exception cref="MidiOutputDeviceException">Raises <see cref="API.Result.MIDI_ERROR_UNPREPARED"/>.</exception>
        /// <exception cref="MidiOutputDeviceException">Raises <see cref="API.Result.MIDI_ERROR_STILL_PLAYING"/>.</exception>
        /// <exception cref="MidiOutputDeviceException">Raises <see cref="API.Result.MIDI_ERROR_NOT_READY"/>.</exception>
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
            catch (Exception)
            {
                Marshal.FreeHGlobal(midiHeader.Data);
                throw;
            }

            try
            {
                Marshal.StructureToPtr(midiHeader, midiHeaderPointer, false);
            }
            catch (Exception)
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
        internal static void InvalidateResult(API.Result result)
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
