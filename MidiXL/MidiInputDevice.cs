﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MidiXL
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class MidiInputDevice : MidiDevice, IDisposable
    {
        #region Fields

        /// <summary>
        /// Structure to store the MIDI input device's capabilities.
        /// </summary>
        private API.MidiInputDeviceCapabilities _Capabilities;

        /// <summary>
        /// Reference to the callback function used by the <see cref="API.OpenMidiInputDevice"/> method.
        /// </summary>
        private API.MidiInputDelegate _Callback;

        /// <summary>
        /// Buffer for receiving long MIDI messages.
        /// </summary>
        private List<IntPtr> _Buffer = new List<IntPtr>();
        private bool _IsDisposed;

        /// <summary>
        /// Lock for thread safety.
        /// </summary>
        private static readonly object _Lock = new object();

        #endregion

        #region Events

        /// <summary>
        /// Event raised when a <see cref="NoteOnMessage"/> is received.
        /// </summary>
        public event EventHandler<NoteOnMessageEventArgs> NoteOnReceived;

        /// <summary>
        /// Event raised when a <see cref="NoteOffMessage"/> is received.
        /// </summary>
        public event EventHandler<NoteOffMessageEventArgs> NoteOffReceived;

        /// <summary>
        /// Event raised when a <see cref="ControlChangeMessage"/> is received.
        /// </summary>
        public event EventHandler<ControlChangeMessageEventArgs> ControlChangeReceived;

        /// <summary>
        /// Event raised when a <see cref="ProgramChangeMessage"/> is received.
        /// </summary>
        public event EventHandler<ProgramChangeMessageEventArgs> ProgramChangeReceived;

        /// <summary>
        /// Event raised when a <see cref="PitchBendMessage"/> is received.
        /// </summary>
        public event EventHandler<PitchBendMessageEventArgs> PitchBendReceived;

        /// <summary>
        /// Event raised when a <see cref="KeyAfterTouchMessage"/> is received;
        /// </summary>
        public event EventHandler<KeyAfterTouchMessageEventArgs> KeyAfterTouchReceived;

        /// <summary>
        /// Event raised when a <see cref="ChannelAfterTouchMessage"/> is received.
        /// </summary>
        public event EventHandler<ChannelAfterTouchMessageEventArgs> ChannelAfterTouchReceived;

        /// <summary>
        /// Event raised when a <see cref="SystemExclusiveMessage"/> is received.
        /// </summary>
        public event EventHandler<SystemExclusiveMessageEventArgs> SystemExclusiveReceived;

        /// <summary>
        /// Event raised when a <see cref="UniversalRealTimeMessage"/> is received.
        /// </summary>
        public event EventHandler<UniversalRealTimeMessageEventArgs> UniversalRealTimeReceived;

        /// <summary>
        /// Event raised when a <see cref="UniversalNonRealTimeMessage"/> is received.
        /// </summary>
        public event EventHandler<UniversalNonRealTimeMessageEventArgs> UniversalNonRealTimeReceived;

        #endregion

        #region Constructor

        /// <summary>
        /// Private contstructor to create and initialize a new <see cref="MidiInputDevice"/>, only invoked by the <see cref="InitializeDeviceList"/> method.
        /// </summary>
        /// <param name="deviceID">An <see cref="int"/> representing the unique indexed <see cref="MidiInputDevice"/>'s ID.</param>
        /// <param name="capabilities">An <see cref="API.MidiInputDeviceCapabilities"/> structure to store the <see cref="MidiInputDevice"/>'s capabilities.</param>
        internal MidiInputDevice(int deviceID, API.MidiInputDeviceCapabilities capabilities) : base(deviceID, capabilities)
        {
            _Capabilities = capabilities;
            _Callback = Callback;
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~MidiInputDevice()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(false);
        }


        #endregion

        #region Properties

        /// <summary>
        /// Gets whether the MIDI input device is started.
        /// </summary>
        public bool IsReceiving { get; private set; } = false;

        #endregion

        #region Methods

        /// <summary>
        /// Opens the MIDI input device for receiving MIDI messages.
        /// </summary>
        /// <exception cref="MidiInputDeviceException">Raises <see cref="API.Result.MULTIMEDIA_SYSTEM_ERROR_BAD_DEVICE_ID"/>.</exception>
        /// <exception cref="MidiInputDeviceException">Raises <see cref="API.Result.MULTIMEDIA_SYSTEM_ERROR_ALLOCATED"/>.</exception>
        /// <exception cref="MidiInputDeviceException">Raises <see cref="API.Result.MULTIMEDIA_SYSTEM_ERROR_NO_MEM"/>.</exception>
        /// <exception cref="MidiInputDeviceException">Raises <see cref="API.Result.MULTIMEDIA_SYSTEM_ERROR_INVALID_FLAG"/>.</exception>
        /// <exception cref="MidiInputDeviceException">Raises <see cref="API.Result.MULTIMEDIA_SYSTEM_ERROR_INVALID_PARAMETER"/>.</exception>
        public void Open()
        {
            if (IsOpen)
                return;

            InvalidateResult(API.OpenMidiInputDevice(ref _Handle, this.ID, _Callback, IntPtr.Zero));

            IsOpen = true;
        }

        /// <summary>
        /// Closes the MIDI input device.
        /// </summary>
        /// <exception cref="MidiInputDeviceException">Raises <see cref="API.Result.MULTIMEDIA_SYSTEM_ERROR_INVALID_HANDLE"/>.</exception>
        /// <exception cref="MidiInputDeviceException">Raises <see cref="API.Result.MULTIMEDIA_SYSTEM_ERROR_NO_MEM"/>.</exception>
        /// <exception cref="MidiInputDeviceException">Raises <see cref="API.Result.MIDI_ERROR_STILL_PLAYING"/>.</exception>
        public void Close()
        {
            if (IsOpen)
            {
                // Return all pending input buffers to the callback function.
                if (_Buffer.Count > 0)
                    Reset();

                InvalidateResult(API.CloseMidiInputDevice(_Handle));

                IsOpen = false;
            }
        }

        /// <summary>
        /// Stops input on the MIDI input device.
        /// </summary>
        /// <exception cref="MidiInputDeviceException">Raises <see cref="API.Result.MULTIMEDIA_SYSTEM_ERROR_INVALID_HANDLE"/>.</exception>
        /// <remarks><i>Returns all pending input buffers to the <see cref="Callback"/> with the <see cref="API.MidiHeader.Flags"/> set to <see cref="API.MidiHeaderFlags.MIDI_HEADER_DONE"/>.</i></remarks>
        public void Reset()
        {
            InvalidateResult(API.ResetMidiInputDevice(_Handle));
        }

        /// <summary>
        /// Starts listening for MIDI messages on the MIDI input device.
        /// </summary>
        /// <exception cref="MidiInputDeviceException">Raises <see cref="API.Result.MULTIMEDIA_SYSTEM_ERROR_INVALID_HANDLE"/>.</exception>
        public void Start()
        {
            if (IsReceiving)
                return;

            // Create buffer to receive long MIDI messages
            _Buffer.Add(CreateBuffer());

            InvalidateResult(API.StartMidiInputDevice(_Handle));

            IsReceiving = true;
        }

        /// <summary>
        /// Stops listening for MIDI messages on the MIDI input device.
        /// </summary>
        /// <exception cref="MidiInputDeviceException">Raises <see cref="API.Result.MULTIMEDIA_SYSTEM_ERROR_INVALID_HANDLE"/>.</exception>
        public void Stop()
        {
            if(IsReceiving)
                InvalidateResult(API.StopMidiInputDevice(_Handle));
        }

        /// <summary>
        /// Connects a MIDI output or thru device to the MIDI input device.
        /// Received <see cref="API.MidiInputMessage"/>s are sent through to the connected MIDI output or thru device.
        /// </summary>
        /// <param name="device">A <see cref="MidiOutputDevice"/> or MIDI thru device to connect to.</param>
        /// <exception cref="MidiInputDeviceException">Raises <see cref="API.Result.MULTIMEDIA_SYSTEM_ERROR_INVALID_HANDLE"/>.</exception>
        /// <exception cref="MidiInputDeviceException">Raises <see cref="API.Result.MIDI_ERROR_NOT_READY"/>.</exception>
        /// <remarks><i>For MIDI thru devices, a handle must be obtained by a calling the <see cref="API.OpenMidiOutputDevice"/> method.</i></remarks>
        public void Connect(MidiOutputDevice device)
        {
            InvalidateResult(API.ConnectMidiDevices(_Handle, device._Handle));
        }

        /// <summary>
        /// Disconnects a MIDI output or thru device from the MIDI input device.
        /// </summary>
        /// <param name="device">A <see cref="MidiOutputDevice"/> or MIDI thru device to disconnect from.</param>
        /// <exception cref="MidiInputDeviceException">Raises <see cref="API.Result.MULTIMEDIA_SYSTEM_ERROR_INVALID_HANDLE"/>.</exception>
        public void Disconnect(MidiOutputDevice device)
        {
            InvalidateResult(API.DisconnectMidiDevices(_Handle, device._Handle));
        }

        /// <summary>
        /// Disconnects the MIDI input device from all connected MIDI devices.
        /// </summary>
        /// <exception cref="MidiInputDeviceException">Raises <see cref="API.Result.MULTIMEDIA_SYSTEM_ERROR_INVALID_HANDLE"/>.</exception>
        private void Disconnect()
        {
            foreach (API.MidiDeviceHandle connection in _Connections)
            {
                InvalidateResult(API.DisconnectMidiDevices(_Handle, connection));
            }
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
            switch(message)
            {
                case API.MidiInputMessage.MIDI_INPUT_MESSAGE_DATA:

                    // A = Packed MIDI message, B = Time in milliseconds since Start
                    ProcessShortMessage(new ShortMessage(messageParameterA, messageParameterB));

                    break;

                case API.MidiInputMessage.MIDI_INPUT_MESSAGE_LONG_DATA:

                    // Extracts the MIDI header for message validation
                    API.MidiHeader midiHeader = (API.MidiHeader)Marshal.PtrToStructure(messageParameterA, typeof(API.MidiHeader));
                    
                    // Prevents creation of long messages when the MIDI header buffer is empty
                    if (midiHeader.BytesRecorded == 0)
                    {
                        ReleaseBuffer(messageParameterA);
                        return;
                    }

                    // A = Pointer to MIDI header, B = Time in milliseconds since Start
                    ProcessLongMessage(new LongMessage(messageParameterA, messageParameterB));

                    break;

                case API.MidiInputMessage.MIDI_INPUT_MESSAGE_MORE_DATA:
                    Console.WriteLine("MIDI IN MORE DATA");
                    // A = Packed MIDI message, B = Time in milliseconds since Start
                    break;

                case API.MidiInputMessage.MIDI_INPUT_MESSAGE_ERROR:

                    // A = Invalid MIDI message, B = Time in milliseconds since Start
                    break;

                case API.MidiInputMessage.MIDI_INPUT_MESSAGE_LONG_ERROR:

                    // A = Pointer to MIDI header, B = Time in milliseconds since Start
                    break;

                case API.MidiInputMessage.MIDI_INPUT_MESSAGE_OPEN:

                    this.IsOpen = true;

                    break;

                case API.MidiInputMessage.MIDI_INPUT_MESSAGE_CLOSE:

                    this.IsOpen = false;

                    break;
            }
        }

        /// <summary>
        /// Processes the provided <see cref="ShortMessage"/> and raises associated events.
        /// </summary>
        /// <param name="message">A <see cref="ShortMessage"/> to process and raise events for.</param>
        private void ProcessShortMessage(ShortMessage message)
        {
            if((message.Status & 0xF0) >= (int)ChannelMessageTypes.Min && (message.Status & 0xF0) <= (int)ChannelMessageTypes.Max)
            {
                switch((ChannelMessageTypes)message.Status)
                {
                    case ChannelMessageTypes.NoteOn:
                        NoteOnReceived?.Invoke(this, new NoteOnMessageEventArgs(new NoteOnMessage(message)));
                        break;

                    case ChannelMessageTypes.NoteOff:
                        NoteOffReceived?.Invoke(this, new NoteOffMessageEventArgs(new NoteOffMessage(message)));
                        break;

                    case ChannelMessageTypes.ControlChange:
                        ControlChangeReceived?.Invoke(this, new ControlChangeMessageEventArgs(new ControlChangeMessage(message)));
                        break;

                    case ChannelMessageTypes.ProgramChange:
                        ProgramChangeReceived?.Invoke(this, new ProgramChangeMessageEventArgs(new ProgramChangeMessage(message)));
                        break;

                    case ChannelMessageTypes.PitchBend:
                        PitchBendReceived?.Invoke(this, new PitchBendMessageEventArgs(new PitchBendMessage(message)));
                        break;

                    case ChannelMessageTypes.KeyAfterTouch:
                        KeyAfterTouchReceived?.Invoke(this, new KeyAfterTouchMessageEventArgs(new KeyAfterTouchMessage(message)));
                        break;

                    case ChannelMessageTypes.ChannelAfterTouch:
                        ChannelAfterTouchReceived?.Invoke(this, new ChannelAfterTouchMessageEventArgs(new ChannelAfterTouchMessage(message)));
                        break;
                }
            }
        }

        /// <summary>
        /// Processes the provided <see cref="LongMessage"/> and raises associated events.
        /// </summary>
        /// <param name="message">A <see cref="LongMessage"/> to process and raise events for.</param>
        private void ProcessLongMessage(LongMessage message)
        {
            switch(message.ID)
            {
                case (int)UniversalSystemExclusiveMessageTypes.UniversalRealTime:
                    UniversalRealTimeReceived?.Invoke(this, new UniversalRealTimeMessageEventArgs(new UniversalRealTimeMessage(message)));
                    break;

                case (int)UniversalSystemExclusiveMessageTypes.UniversalNonRealTime:
                    UniversalNonRealTimeReceived?.Invoke(this, new UniversalNonRealTimeMessageEventArgs(new UniversalNonRealTimeMessage(message)));
                    break;

                default:
                    SystemExclusiveReceived?.Invoke(this, new SystemExclusiveMessageEventArgs(new SystemExclusiveMessage(message)));
                    break;
            }

            RecycleBuffer(message.ParameterA);
        }

        /// <summary>
        /// Creates a buffer for receiving <see cref="LongMessage"/>s.
        /// </summary>
        /// <returns>An <see cref="IntPtr"/> referencing the created buffer.</returns>
        /// <exception cref="MidiInputDeviceException">Raises <see cref="API.Result.MULTIMEDIA_SYSTEM_ERROR_INVALID_HANDLE"/>.</exception>
        /// <exception cref="MidiInputDeviceException">Raises <see cref="API.Result.MULTIMEDIA_SYSTEM_ERROR_NO_MEM"/>.</exception>
        /// <exception cref="MidiInputDeviceException">Raises <see cref="API.Result.MULTIMEDIA_SYSTEM_ERROR_INVALID_PARAMETER"/>.</exception>
        /// <exception cref="MidiInputDeviceException">Raises <see cref="API.Result.MIDI_ERROR_UNPREPARED"/>.</exception>
        /// <exception cref="MidiInputDeviceException">Raises <see cref="API.Result.MIDI_ERROR_STILL_PLAYING"/>.</exception>
        private IntPtr CreateBuffer()
        {
            IntPtr midiHeaderPointer;

            int headerSize = Marshal.SizeOf(typeof(API.MidiHeader));

            API.MidiHeader midiHeader = new API.MidiHeader();

            // TODO: Optimize buffer size
            midiHeader.Data = Marshal.AllocHGlobal(256);
            midiHeader.BufferSize = 256;
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

            InvalidateResult(API.PrepareMidiInputHeader(_Handle, midiHeaderPointer, headerSize));
            InvalidateResult(API.AddMidiInputBuffer(_Handle, midiHeaderPointer, headerSize));

            return midiHeaderPointer;
        }

        /// <summary>
        /// Recycles an existing buffer for receiving <see cref="LongMessage"/>s.
        /// </summary>
        /// <param name="buffer">An <see cref="IntPtr"/> referencing the buffer to recycle.</param>
        /// <exception cref="MidiInputDeviceException">Raises <see cref="API.Result.MULTIMEDIA_SYSTEM_ERROR_INVALID_HANDLE"/>.</exception>
        /// <exception cref="MidiInputDeviceException">Raises <see cref="API.Result.MULTIMEDIA_SYSTEM_ERROR_NO_MEM"/>.</exception>
        /// <exception cref="MidiInputDeviceException">Raises <see cref="API.Result.MULTIMEDIA_SYSTEM_ERROR_INVALID_PARAMETER"/>.</exception>
        /// <exception cref="MidiInputDeviceException">Raises <see cref="API.Result.MIDI_ERROR_UNPREPARED"/>.</exception>
        /// <exception cref="MidiInputDeviceException">Raises <see cref="API.Result.MIDI_ERROR_STILL_PLAYING"/>.</exception>
        private void RecycleBuffer(IntPtr buffer)
        {
            IntPtr headerPointer = unchecked((IntPtr)(long)(ulong)buffer);
            int headerSize = Marshal.SizeOf(typeof(API.MidiHeader));
            
            InvalidateResult(API.UnprepareMidiInputHeader(_Handle, headerPointer, headerSize));
            InvalidateResult(API.PrepareMidiInputHeader(_Handle, headerPointer, headerSize));
            InvalidateResult(API.AddMidiInputBuffer(_Handle, headerPointer, headerSize));
        }

        /// <summary>
        /// Releases an existing buffer and associated memory.
        /// </summary>
        /// <param name="buffer">An <see cref="IntPtr"/> referencing the buffer to release.</param>
        /// <exception cref="MidiInputDeviceException">Raises <see cref="API.Result.MULTIMEDIA_SYSTEM_ERROR_INVALID_HANDLE"/>.</exception>
        /// <exception cref="MidiInputDeviceException">Raises <see cref="API.Result.MULTIMEDIA_SYSTEM_ERROR_INVALID_PARAMETER"/>.</exception>
        /// <exception cref="MidiInputDeviceException">Raises <see cref="API.Result.MIDI_ERROR_STILL_PLAYING"/>.</exception>
        private void ReleaseBuffer(IntPtr buffer)
        {
            IntPtr headerPointer = unchecked((IntPtr)(long)(ulong)buffer);
            int headerSize = Marshal.SizeOf(typeof(API.MidiHeader));

            InvalidateResult(API.UnprepareMidiInputHeader(_Handle, headerPointer, headerSize));

            API.MidiHeader midiHeader = (API.MidiHeader)Marshal.PtrToStructure(headerPointer, typeof(API.MidiHeader));

            Marshal.FreeHGlobal(midiHeader.Data);
            Marshal.FreeHGlobal(headerPointer);

            _Buffer.Remove(headerPointer);
        }

        /// <summary>
        /// Invalidates the result returned by API calls for errors and throws a <see cref="MidiInputDeviceException"/> if an error occured.
        /// </summary>
        /// <param name="result">An <see cref="API.Result"/> value containing the result of the API call.</param>
        internal static void InvalidateResult(API.Result result)
        {
            if (result != API.Result.MULTIMEDIA_SYSTEM_ERROR_NO_ERROR)
            {
                throw new MidiInputDeviceException(result);
            }
        }

        private void Dispose(bool disposing)
        {
            if (!_IsDisposed)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                if(_Buffer.Count > 0)
                {
                    API.ResetMidiInputDevice(_Handle);

                    foreach(var headerPointer in _Buffer)
                    {
                        API.MidiHeader header = (API.MidiHeader)Marshal.PtrToStructure(headerPointer, typeof(API.MidiHeader));
                        API.UnprepareMidiInputHeader(_Handle, headerPointer, Marshal.SizeOf(typeof(API.MidiHeader)));
                        Marshal.FreeHGlobal(header.Data);
                        Marshal.FreeHGlobal(headerPointer);
                    }
                }

                API.CloseMidiInputDevice(_Handle);

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _IsDisposed = true;
            }
        }

       
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            GC.KeepAlive(_Callback);
            Dispose(true);
            GC.SuppressFinalize(this);
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
