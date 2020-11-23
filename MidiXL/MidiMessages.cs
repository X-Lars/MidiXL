using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MidiXL
{
    #region MIDI Message Base Classes

    /// <summary>
    /// Base class for all types of MIDI messages.
    /// </summary>
    public abstract class MidiMessage
    {

    }
        
    /// <summary>
    /// Defines a MIDI short message.
    /// </summary>
    public class ShortMessage : MidiMessage
    {
        #region Constructor

        /// <summary>
        /// Creates and initializes a <see cref="ShortMessage"/>, for use inside the <see cref="MidiInputDevice.Callback"/> method.
        /// </summary>
        /// <param name="messageParameterA">An <see cref="IntPtr"/> to store a reference to the first message parameter.</param>
        /// <param name="messageParameterB">An <see cref="IntPtr"/> to store a reference to the second message parameter.</param>
        internal ShortMessage(IntPtr messageParameterA, IntPtr messageParameterB)
        {
            // Extract message parameter A
            this.Status = ((int)messageParameterA & 0xF0);
            this.DataA  = ((int)messageParameterA & 0xFF00) >> 8;
            this.DataB  = ((int)messageParameterA & 0xFF0000) >> 16;

            // Extract message parameter B
            this.TimeStamp = (int)messageParameterB;

            // Set the parameter references
            this.ParameterA = messageParameterA;
            this.ParameterB = messageParameterB;
        }

        /// <summary>
        /// Creates and initializes a <see cref="ShortMessage"/>, for manual creation of MIDI short messages.
        /// </summary>
        /// <param name="status">An <see cref="int"/> specifying the MIDI short message type.</param>
        /// <param name="dataA">An <see cref="int"/> providing the first data part of the message.</param>
        /// <param name="dataB">An <see cref="int"/> providing the second data part of the message.</param>
        internal ShortMessage(int status, int dataA, int dataB)
        {
            this.Status = status;
            this.DataA = dataA;
            this.DataB = dataB;

            this.TimeStamp = 0;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a reference to the first message parameter of the MIDI short message.
        /// </summary>
        internal IntPtr ParameterA { get; private set; }

        /// <summary>
        /// Gets a reference to the second message parameter of the MIDI short message.
        /// </summary>
        internal IntPtr ParameterB { get; private set; }

        /// <summary>
        /// Gets the status byte of the MIDI short message.
        /// </summary>
        internal int Status { get; private set; }

        /// <summary>
        /// Gets the first data part of the MIDI short message.
        /// </summary>
        internal int DataA { get; private set; }

        /// <summary>
        /// Gets the second data part of the MIDI short message.
        /// </summary>
        internal int DataB { get; private set; }

        /// <summary>
        /// Gets the time the MIDI short message received since the MIDI input device started.
        /// </summary>
        internal int TimeStamp { get; private set; }

        /// <summary>
        /// Gets the raw short message data.
        /// </summary>
        public virtual int Message
        {
            get { return (this.Status | (this.DataA << 8) | (this.DataB << 16)); }
        }

        #endregion
    }

    /// <summary>
    /// Defines a MIDI long message.
    /// </summary>
    public class LongMessage : MidiMessage
    {
        #region Fields

        /// <summary>
        /// Stores a reference to the MIDI header.
        /// </summary>
        private IntPtr _MidiHeaderPointer;

        /// <summary>
        /// Structure to store received MIDI long messages.
        /// </summary>
        private API.MidiHeader _MidiHeader;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates and initializes a <see cref="LongMessage"/>, for use inside the <see cref="MidiInputDevice.Callback"/> method.
        /// </summary>
        /// <param name="messageParameterA">An <see cref="IntPtr"/> to store a reference to the first message parameter.</param>
        /// <param name="messageParameterB">An <see cref="IntPtr"/> to store a reference to the second message parameter.</param>
        internal LongMessage(IntPtr messageParameterA, IntPtr messageParameterB)
        {
            // Set the pointer to the MIDI header structure (Converted from UIntPtr to IntPtr)
            _MidiHeaderPointer = unchecked((IntPtr)(long)(ulong)messageParameterA);

            // Create a managed copy of the MIDI header
            _MidiHeader = (API.MidiHeader)Marshal.PtrToStructure(_MidiHeaderPointer, typeof(API.MidiHeader));

            // Create the data array with the size of the long message
            this.Data = new byte[_MidiHeader.BytesRecorded];

            // Fill the data array by looping through the MIDI header data
            for (int i = 0; i < _MidiHeader.BytesRecorded; i++)
            {
                this.Data[i] = Marshal.ReadByte(_MidiHeader.Data, i);
            }

            // Extract the status byte
            this.Status = this.Data[0];

            // Extract message parameter B
            this.TimeStamp = (int)messageParameterB;

            // Set the parameter references
            this.ParameterA = messageParameterA;
            this.ParameterB = messageParameterB;
        }

        /// <summary>
        /// Internal constructor for conversion of <see cref="LongMessage"/> derived types inside the <see cref="MidiInputDevice.Callback"/> method.
        /// </summary>
        /// <param name="message">The <see cref="LongMessage"/> to create and initialize the derived type from.</param>
        internal LongMessage(LongMessage message) : this(message.ParameterA, message.ParameterB) { }

        /// <summary>
        /// Creates and initializes a <see cref="LongMessage"/>, for manual creation of MIDI long messages.
        /// </summary>
        /// <param name="data">A <see cref="byte[]"/> array containing the raw message data.</param>
        internal LongMessage(byte[] data)
        {
            this.Data = data;
            this.Status = data[0];
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a reference to the first message parameter of the MIDI long message.
        /// </summary>
        internal IntPtr ParameterA { get; private set; }

        /// <summary>
        /// Gets a reference to the second message parameter of the MIDI long message.
        /// </summary>
        internal IntPtr ParameterB { get; private set; }

        /// <summary>
        /// Gets the status byte of the MIDI long message.
        /// </summary>
        internal int Status { get; private set; }

        /// <summary>
        /// Gets the time the MIDI long message received since the MIDI input device started.
        /// </summary>
        public int TimeStamp { get; private set; }

        /// <summary>
        /// Gets the data of the MIDI long message.
        /// </summary>
        public byte[] Data { get; private set; }

        #endregion
    }

    #endregion

    #region MIDI Message Implementations

    /// <summary>
    /// Defines a MIDI channel message, extends a <see cref="ShortMessage"/> with a channel property.
    /// </summary>
    public class ChannelMessage : ShortMessage
    {
        #region Constructor

        /// <summary>
        /// Creates and initializes a <see cref="ChannelMessage"/>, for use inside the <see cref="MidiInputDevice.Callback"/> method.
        /// </summary>
        /// <param name="messageParameterA">An <see cref="IntPtr"/> to store a reference to the first message parameter.</param>
        /// <param name="messageParameterB">An <see cref="IntPtr"/> to store a reference to the second message parameter.</param>
        internal ChannelMessage(IntPtr messageParameterA, IntPtr messageParameterB) : base(messageParameterA, messageParameterB)
        {
            // Extract the channel from message parameter A
            this.Channel = (MidiChannels)((int)messageParameterA & 0x0F);
        }

        /// <summary>
        /// Creates and initializes a <see cref="ChannelMessage"/> from the specified <see cref="ShortMessage"/>.
        /// </summary>
        /// <param name="message">A <see cref="ShortMessage"/> to initialize the <see cref="ChannelMessage"/> from.</param>
        internal ChannelMessage(ShortMessage message) : this(message.ParameterA, message.ParameterB) { }

        /// <summary>
        /// Creates and initializes a <see cref="ChannelMessage"/> from the specified parameters.
        /// </summary>
        /// <param name="type">A <see cref="ChannelMessageTypes"/> specifying the type of message.</param>
        /// <param name="channel">A <see cref="MidiChannels"/> specifying the MIDI channel associated with the message.</param>
        /// <param name="dataA">An <see cref="int"/> specifying the first data part of the message.</param>
        /// <param name="dataB">An <see cref="int"/> specifying the second data part of the message.</param>
        public ChannelMessage(ChannelMessageTypes type, MidiChannels channel, int dataA, int dataB) : base((int)type, dataA, dataB)
        {
            this.Channel = channel;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the MIDI channel over which the message was received.
        /// </summary>
        public MidiChannels Channel { get; protected set; }

        /// <summary>
        /// Gets the type of MIDI channel message.
        /// </summary>
        public ChannelMessageTypes Type
        {
            get { return (ChannelMessageTypes)Status; }
        }

        #endregion
    }

    /// <summary>
    /// Defines a MIDI system common message.
    /// </summary>
    public class SystemCommonMessage : ShortMessage
    {
        #region Constructor

        /// <summary>
        /// Creates and initializes a <see cref="SystemCommonMessage"/>, for use inside the <see cref="MidiInputDevice.Callback"/> method.
        /// </summary>
        /// <param name="messageParameterA">An <see cref="IntPtr"/> to store a reference to the first message parameter.</param>
        /// <param name="messageParameterB">An <see cref="IntPtr"/> to store a reference to the second message parameter.</param>
        internal protected SystemCommonMessage(IntPtr messageParameterA, IntPtr messageParameterB) : base(messageParameterA, messageParameterB) { }

        /// <summary>
        /// Creates and initializes a <see cref="SystemCommonMessage"/> from the specified <see cref="ShortMessage"/>.
        /// </summary>
        /// <param name="message">A <see cref="ShortMessage"/> to initialize the <see cref="SystemCommonMessage"/> from.</param>
        internal protected SystemCommonMessage(ShortMessage message) : this(message.ParameterA, message.ParameterB) { }

        /// <summary>
        /// Creates and initializes a <see cref="ChannelMessage"/> from the specified parameters.
        /// </summary>
        /// <param name="type">A <see cref="SystemCommonMessageTypes"/> specifying the type of message.</param>
        /// <param name="dataA">An <see cref="int"/> specifying the first data part of the message.</param>
        /// <param name="dataB">An <see cref="int"/> specifying the second data part of the message.</param>
        internal protected SystemCommonMessage(SystemCommonMessageTypes type, int dataA, int dataB) : base((int)type, dataA, dataB) { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the type of MIDI system common message.
        /// </summary>
        public SystemCommonMessageTypes Type
        {
            get { return (SystemCommonMessageTypes)Status; }
        }

        #endregion
    }

    /// <summary>
    /// Defines a MIDI system realtime message.
    /// </summary>
    public class SystemRealtimeMessage : ShortMessage
    {
        #region Constructor

        /// <summary>
        /// Creates and initializes a <see cref="SystemRealtimeMessage"/>, for use inside the <see cref="MidiInputDevice.Callback"/> method.
        /// </summary>
        /// <param name="messageParameterA">An <see cref="IntPtr"/> to store a reference to the first message parameter.</param>
        /// <param name="messageParameterB">An <see cref="IntPtr"/> to store a reference to the second message parameter.</param>
        internal protected SystemRealtimeMessage(IntPtr messageParameterA, IntPtr messageParameterB) : base(messageParameterA, messageParameterB) { }

        /// <summary>
        /// Creates and initializes a <see cref="SystemRealtimeMessage"/> from the specified <see cref="ShortMessage"/>.
        /// </summary>
        /// <param name="message">A <see cref="ShortMessage"/> to initialize the <see cref="SystemRealtimeMessage"/> from.</param>
        internal protected SystemRealtimeMessage(ShortMessage message) : this(message.ParameterA, message.ParameterB) { }

        /// <summary>
        /// Creates and initializes a <see cref="SystemRealtimeMessage"/> from the specified parameters.
        /// </summary>
        /// <param name="type">A <see cref="SystemRealtimeMessageTypes"/> specifying the type of message.</param>
        /// <param name="dataA">An <see cref="int"/> specifying the first data part of the message.</param>
        /// <param name="dataB">An <see cref="int"/> specifying the second data part of the message.</param>
        internal protected SystemRealtimeMessage(SystemRealtimeMessageTypes type, int dataA, int dataB) : base((int)type, dataA, dataB) { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the type of MIDI system realtime message.
        /// </summary>
        public SystemRealtimeMessageTypes Type
        {
            get { return (SystemRealtimeMessageTypes)Status; }
        }

        #endregion
    }

    #endregion

    #region MIDI Message Public Classes

    /// <summary>
    /// Defines a note on MIDI message, extends a <see cref="ChannelMessage"/> with note and velocity properties.
    /// </summary>
    public sealed class NoteOnMessage : ChannelMessage
    {
        #region Constructor

        /// <summary>
        /// Creates and initializes a <see cref="NoteOnMessage"/> from the specified parameters.
        /// </summary>
        /// <param name="channel">A <see cref="MidiChannels"/> to associate with the message.</param>
        /// <param name="note">An <see cref="int"/> specifying the note.</param>
        /// <param name="velocity">An <see cref="int"/> specifying the velocity of the note.</param>
        public NoteOnMessage(MidiChannels channel, int note, int velocity) : base(ChannelMessageTypes.NoteOn, channel, note, velocity) { }

        /// <summary>
        /// Creates and initializes a <see cref="NoteOnMessage"/> from the specified <see cref="ShortMessage"/>.
        /// </summary>
        /// <param name="message">A <see cref="ShortMessage"/> to initialize the <see cref="NoteOnMessage"/> from.</param>
        internal NoteOnMessage(ShortMessage message) : base(message) { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the MIDI note.
        /// </summary>
        public int Note
        {
            get { return DataA; }
        }

        /// <summary>
        /// Gets the velocity of the MIDI note.
        /// </summary>
        public int Velocity
        {
            get { return DataB; }
        }

        #endregion
    }

    /// <summary>
    /// Defines a note off MIDI message, extends a <see cref="ChannelMessage"/> with note and velocity properties.
    /// </summary>
    public sealed class NoteOffMessage : ChannelMessage
    {
        #region Constructor

        /// <summary>
        /// Creates and initializes a <see cref="NoteOffMessage"/> from the specified parameters.
        /// </summary>
        /// <param name="channel">A <see cref="MidiChannels"/> to associate with the message.</param>
        /// <param name="note">An <see cref="int"/> specifying the note.</param>
        /// <param name="velocity">An <see cref="int"/> specifying the velocity of the note.</param>
        public NoteOffMessage(MidiChannels channel, int note, int velocity) : base(ChannelMessageTypes.NoteOff, channel, note, velocity) { }

        /// <summary>
        /// Creates and initializes a <see cref="NoteOffMessage"/> from the specified <see cref="ShortMessage"/>.
        /// </summary>
        /// <param name="message">A <see cref="ShortMessage"/> to initialize the <see cref="NoteOffMessage"/> from.</param>
        internal NoteOffMessage(ShortMessage message) : base(message) { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the MIDI note.
        /// </summary>
        public int Note
        {
            get { return DataA; }
        }

        /// <summary>
        /// Gets the velocity of the MIDI note.
        /// </summary>
        public int Velocity
        {
            get { return DataB; }
        }

        #endregion
    }

    /// <summary>
    /// Defines a control change MIDI message, extends a <see cref="ChannelMessage"/> with controller and value properties.
    /// </summary>
    public sealed class ControlChangeMessage : ChannelMessage
    {
        #region Constructor

        /// <summary>
        /// Creates and initializes a <see cref="ControlChangeMessage"/> from the specified parameters.
        /// </summary>
        /// <param name="channel">A <see cref="MidiChannels"/> to associate with the message.</param>
        /// <param name="controller">An <see cref="int"/> specifying the controller number.</param>
        /// <param name="value">An <see cref="int"/> specifying the controller value.</param>
        public ControlChangeMessage(MidiChannels channel, int controller, int value) : base(ChannelMessageTypes.ControlChange, channel, controller, value) { }

        /// <summary>
        /// Creates and initializes a <see cref="ControlChangeMessage"/> from the specified <see cref="ShortMessage"/>.
        /// </summary>
        /// <param name="message">The <see cref="ShortMessage"/> to initialize the <see cref="ControlChangeMessage"/> from.</param>
        internal ControlChangeMessage(ShortMessage message) : base(message) { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the controller number.
        /// </summary>
        public int Controller
        {
            get { return this.DataA; }
        }

        /// <summary>
        /// Gets the value of the controller.
        /// </summary>
        public int Value
        {
            get { return this.DataB; }
        }


        #endregion
    }

    /// <summary>
    /// Defines a program change MIDI message, extends a <see cref="ChannelMessage"/> with a value property.
    /// </summary>
    public sealed class ProgramChangeMessage : ChannelMessage
    {
        #region Constructor

        /// <summary>
        /// Creates and initializes a <see cref="ControlChangeMessage"/> from the specified parameters.
        /// </summary>
        /// <param name="channel">A <see cref="MidiChannels"/> to associate with the message.</param>
        /// <param name="value">An <see cref="int"/> specifying the controller value.</param>
        public ProgramChangeMessage(MidiChannels channel, int value) : base(ChannelMessageTypes.ProgramChange, channel, value, 0) { }

        /// <summary>
        /// Creates and initializes a <see cref="ControlChangeMessage"/> from the specified <see cref="ShortMessage"/>.
        /// </summary>
        /// <param name="message">The <see cref="ShortMessage"/> to initialize the <see cref="ControlChangeMessage"/> from.</param>
        internal ProgramChangeMessage(ShortMessage message) : base(message) { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the value of the program change.
        /// </summary>
        public int Value
        {
            get { return this.DataA; }
        }

        #endregion
    }

    /// <summary>
    /// Defines a program change MIDI message, extends a <see cref="ChannelMessage"/> with a pitch property.
    /// </summary>
    public sealed class PitchBendMessage : ChannelMessage
    {
        #region Constructor

        /// <summary>
        /// Creates and initializes a <see cref="PitchBendMessage"/> from the specified parameters.
        /// </summary>
        /// <param name="channel">A <see cref="MidiChannels"/> to associate with the message.</param>
        /// <param name="pitch">An <see cref="int"/> specifying the pitch value.</param>
        public PitchBendMessage(MidiChannels channel, int pitch) : base(ChannelMessageTypes.ProgramChange, channel, (pitch + 0x2000) & 0x7F, (pitch + 0x2000) >> 7) { }

        /// <summary>
        /// Creates and initializes a <see cref="PitchBendMessage"/> from the specified <see cref="ShortMessage"/>.
        /// </summary>
        /// <param name="message">The <see cref="ShortMessage"/> to initialize the <see cref="PitchBendMessage"/> from.</param>
        internal PitchBendMessage(ShortMessage message) : base(message) { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the pitch value.
        /// </summary>
        public int Pitch
        {
            get 
            {
                // Convert MIDI data to integer range of -8192 to +8191
                return (DataA | (DataB << 7)) - 0x2000;
            }
        }

        #endregion
    }

    /// <summary>
    /// Defines a program change MIDI message, extends a <see cref="ChannelMessage"/> with note and pressure properties.
    /// </summary>
    public sealed class KeyAfterTouchMessage : ChannelMessage
    {
        #region Constructor

        /// <summary>
        /// Creates and initializes a <see cref="KeyAfterTouchMessage"/> from the specified parameters.
        /// </summary>
        /// <param name="channel">A <see cref="MidiChannels"/> to associate with the message.</param>
        /// <param name="pitch">An <see cref="int"/> specifying the pitch value.</param>
        public KeyAfterTouchMessage(MidiChannels channel, int note, int pressure) : base(ChannelMessageTypes.ProgramChange, channel, note, pressure) { }

        /// <summary>
        /// Creates and initializes a <see cref="KeyAfterTouchMessage"/> from the specified <see cref="ShortMessage"/>.
        /// </summary>
        /// <param name="message">The <see cref="ShortMessage"/> to initialize the <see cref="KeyAfterTouchMessage"/> from.</param>
        internal KeyAfterTouchMessage(ShortMessage message) : base(message) { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the note value.
        /// </summary>
        public int Note
        {
            get { return DataA; }
        }

        /// <summary>
        /// Gets the pressure value.
        /// </summary>
        public int Pressure
        {
            get { return DataB; }
        }

        #endregion
    }

    /// <summary>
    /// Defines a program change MIDI message, extends a <see cref="ChannelMessage"/> with a pressure property.
    /// </summary>
    public sealed class ChannelAfterTouchMessage : ChannelMessage
    {
        #region Constructor

        /// <summary>
        /// Creates and initializes a <see cref="ChannelAfterTouchMessage"/> from the specified parameters.
        /// </summary>
        /// <param name="channel">A <see cref="MidiChannels"/> to associate with the message.</param>
        /// <param name="pitch">An <see cref="int"/> specifying the pitch value.</param>
        public ChannelAfterTouchMessage(MidiChannels channel, int pressure) : base(ChannelMessageTypes.ProgramChange, channel, pressure, 0) { }

        /// <summary>
        /// Creates and initializes a <see cref="ChannelAfterTouchMessage"/> from the specified <see cref="ShortMessage"/>.
        /// </summary>
        /// <param name="message">The <see cref="ShortMessage"/> to initialize the <see cref="ChannelAfterTouchMessage"/> from.</param>
        internal ChannelAfterTouchMessage(ShortMessage message) : base(message) { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the pressure value.
        /// </summary>
        public int Pressure
        {
            get { return DataA; }
        }

        #endregion
    }

    /// <summary>
    /// Defines a system exclusive MIDI message.
    /// </summary>
    public sealed class SystemExclusiveMessage : LongMessage
    {
        #region Constructor

        /// <summary>
        /// Creates and initializes a <see cref="SystemExclusiveMessage"/> from the specified data.
        /// </summary>
        /// <param name="data">A <see cref="byte[]"/> array containting the raw system exclusive data.</param>
        public SystemExclusiveMessage(byte[] data) : base(data) { }

        /// <summary>
        /// Creates and initializes a <see cref="SystemExclusiveMessage"/> from the specified <see cref="LongMessage"/>.
        /// </summary>
        /// <param name="message">The <see cref="LongMessage"/> to initilize the <see cref="SystemExclusiveMessage"/> from.</param>
        internal SystemExclusiveMessage(LongMessage message) : base(message) { }

        #endregion
    }

    /// <summary>
    /// Defines a MIDI universal system exlusive non real time message.
    /// </summary>
    public sealed class UniversalNonRealTimeMessage : LongMessage
    {
        #region Constructor

        /// <summary>
        /// Creates and initializes a <see cref="UniversalNonRealTimeMessage"/> from the specified data.
        /// </summary>
        /// <param name="data">A <see cref="byte[]"/> containting the raw universal non real time data.</param>
        public UniversalNonRealTimeMessage(byte[] data) : base(data) { }

        /// <summary>
        /// Creates and initializes a <see cref="LongMessage"/> from the specified <see cref="UniversalNonRealTimeMessage"/>.
        /// </summary>
        /// <param name="message">A <see cref="LongMessage"/> to initilize the <see cref="UniversalNonRealTimeMessage"/> from.</param>
        internal UniversalNonRealTimeMessage(LongMessage message) : base(message) { }

        #endregion
    }

    /// <summary>
    /// Defines a MIDI universal system exclusive real time message.
    /// </summary>
    public sealed class UniversalRealTimeMessage : LongMessage
    {
        #region Constructor

        /// <summary>
        /// Creates and initializes a <see cref="UniversalRealTimeMessage"/> and initializes it with the provided data.
        /// </summary>
        /// <param name="data">A <see cref="byte[]"/> containting the raw universal real time data.</param>
        public UniversalRealTimeMessage(byte[] data) : base(data) { }

        /// <summary>
        /// Creates and initializes a <see cref="LongMessage"/> from the specified <see cref="UniversalRealTimeMessage"/>.
        /// </summary>
        /// <param name="message">A <see cref="LongMessage"/> to initilize the <see cref="UniversalRealTimeMessage"/> from.</param>
        internal UniversalRealTimeMessage(LongMessage message) : base(message) { }

        #endregion
    }

    #endregion
}
