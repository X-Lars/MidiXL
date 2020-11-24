using System;
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
    public class MidiInputDevice : MidiDevice
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

        #endregion

        #region Properties

        /// <summary>
        /// Gets whether the MIDI input device is openend or closed.
        /// </summary>
        public bool IsOpen { get; private set; }

        #endregion

        #region Methods

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
            // Create buffer to receive long MIDI messages
            _Buffer.Add(CreateBuffer());

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
            Debug.Print($"[MidiInputDevice] Callback");
            switch(message)
            {
                case API.MidiInputMessage.MIDI_INPUT_MESSAGE_DATA:
                    // A = Packed MIDI message, B = Time in milliseconds since Start
                    ProcessShortMessage(new ShortMessage(messageParameterA, messageParameterB));
                    break;

                case API.MidiInputMessage.MIDI_INPUT_MESSAGE_LONG_DATA:
                    // A = Pointer to MIDI header, B = Time in milliseconds since Start
                    ProcessLongMessage(new LongMessage(messageParameterA, messageParameterB));
                    break;

                case API.MidiInputMessage.MIDI_INPUT_MESSAGE_MORE_DATA:
                    // A = Packed MIDI message, B = Time in milliseconds since Start
                    break;

                case API.MidiInputMessage.MIDI_INPUT_MESSAGE_ERROR:
                    // A = Invalid MIDI message, B = Time in milliseconds since Start
                    break;

                case API.MidiInputMessage.MIDI_INPUT_MESSAGE_LONG_ERROR:
                    // A = Pointer to MIDI header, B = Time in milliseconds since Start
                    break;

                case API.MidiInputMessage.MIDI_INPUT_MESSAGE_OPEN:
                    // A = Unused, B = unused
                    this.IsOpen = true;
                    break;

                case API.MidiInputMessage.MIDI_INPUT_MESSAGE_CLOSE:
                    // A = Unused, B = unused
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
            if(message.Status == (int)SystemExclusiveMessageTypes.SystemExclusive)
            {
                SystemExclusiveReceived?.Invoke(this, new SystemExclusiveMessageEventArgs(new SystemExclusiveMessage(message)));
            }
            else if (message.Status == (int)UniversalSystemExclusiveMessageTypes.UniversalRealTime)
            {
                UniversalRealTimeReceived?.Invoke(this, new UniversalRealTimeMessageEventArgs(new UniversalRealTimeMessage(message)));
            }
            else if (message.Status == (int)UniversalSystemExclusiveMessageTypes.UniversalNonRealTime)
            {
                UniversalNonRealTimeReceived?.Invoke(this, new UniversalNonRealTimeMessageEventArgs(new UniversalNonRealTimeMessage(message)));
            }

            //ReleaseBuffer(message.ParameterA);
            RecycleBuffer(message.ParameterA);
        }

        /// <summary>
        /// Creates a buffer for receiving <see cref="LongMessage"/>s.
        /// </summary>
        /// <returns>An <see cref="IntPtr"/> referencing the created buffer.</returns>
        /// <exception cref="MidiInputDeviceException">Raises error #5: MULTIMEDIA_SYSTEM_ERROR_INVALID_HANDLE, the specified device handle is invalid.</exception>
        /// <exception cref="MidiInputDeviceException">Raises error #7: MULTIMEDIA_SYSTEM_ERROR_NO_MEM, the system is unable to allocate or lock memory.</exception>
        /// <exception cref="MidiInputDeviceException">Raises error #11: MULTIMEDIA_SYSTEM_ERROR_INVALID_PARAMETER, the specified pointer or structure is invalid.</exception>
        /// <exception cref="MidiInputDeviceException">Raises error #64: MIDI_ERROR_UNPREPARED, the buffer pointed to by the MIDI header has not been prepared.</exception>
        /// <exception cref="MidiInputDeviceException">Raises error #65: MIDI_ERROR_STILL_PLAYING, the buffer pointed to by the MIDI header is still in the queue.</exception>
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
        //// <returns>An <see cref="IntPtr"/> referencing the recycled buffer.</returns>
        /// <exception cref="MidiInputDeviceException">Raises error #5: MULTIMEDIA_SYSTEM_ERROR_INVALID_HANDLE, the specified device handle is invalid.</exception>
        /// <exception cref="MidiInputDeviceException">Raises error #7: MULTIMEDIA_SYSTEM_ERROR_NO_MEM, the system is unable to allocate or lock memory.</exception>
        /// <exception cref="MidiInputDeviceException">Raises error #11: MULTIMEDIA_SYSTEM_ERROR_INVALID_PARAMETER, the specified pointer or structure is invalid.</exception>
        /// <exception cref="MidiInputDeviceException">Raises error #64: MIDI_ERROR_UNPREPARED, the buffer pointed to by the MIDI header has not been prepared.</exception>
        /// <exception cref="MidiInputDeviceException">Raises error #65: MIDI_ERROR_STILL_PLAYING, the buffer pointed to by the MIDI header is still in the queue.</exception>
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
        /// <exception cref="MidiInputDeviceException">Raises error #5: MULTIMEDIA_SYSTEM_ERROR_INVALID_HANDLE, the specified device handle is invalid.</exception>
        /// <exception cref="MidiInputDeviceException">Raises error #11: MULTIMEDIA_SYSTEM_ERROR_INVALID_PARAMETER, the specified pointer or structure is invalid.</exception>
        /// <exception cref="MidiInputDeviceException">Raises error #65: MIDI_ERROR_STILL_PLAYING, the buffer pointed to by the MIDI header is still in the queue.</exception>
        private void ReleaseBuffer(IntPtr buffer)
        {
            IntPtr headerPointer = unchecked((IntPtr)(long)(ulong)buffer);
            int headerSize = (int)Marshal.SizeOf(typeof(API.MidiHeader));

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
