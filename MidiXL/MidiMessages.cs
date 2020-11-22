using System;
using System.Collections.Generic;
using System.Linq;
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
        public int Status { get; private set; }

        /// <summary>
        /// Gets the first data part of the MIDI short message.
        /// </summary>
        public int DataA { get; private set; }

        /// <summary>
        /// Gets the second data part of the MIDI short message.
        /// </summary>
        public int DataB { get; private set; }

        /// <summary>
        /// Gets the time the MIDI short message received since the MIDI input device started.
        /// </summary>
        public int TimeStamp { get; private set; }
    }

    /// <summary>
    /// Defines a MIDI long message.
    /// </summary>
    public class LongMessage : MidiMessage
    {
        #region Fields

        private IntPtr _HeaderPointer;

        private API.MidiHeader _Header;

        #endregion


        public LongMessage()
        {

        }
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
        /// <param name="messageParameterA"></param>
        /// <param name="messageParameterB"></param>
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

    #endregion

    #region MIDI Message Public Classes

    /// <summary>
    /// Defines a note on MIDI message, extends a <see cref="ChannelMessage"/> with pitch and velocity properties.
    /// </summary>
    public sealed class NoteOnMessage : ChannelMessage
    {
        #region Constructor

        /// <summary>
        /// Creates and initializes a <see cref="NoteOnMessage"/> from the specified parameters.
        /// </summary>
        /// <param name="channel">A <see cref="MidiChannels"/> to associate with the message.</param>
        /// <param name="pitch">An <see cref="int"/> specifying the pitch of the note.</param>
        /// <param name="velocity">An <see cref="int"/> specifying the velocity of the note.</param>
        public NoteOnMessage(MidiChannels channel, int pitch, int velocity) : base(ChannelMessageTypes.NoteOn, channel, pitch, velocity) { }

        /// <summary>
        /// Creates and initializes a <see cref="NoteOnMessage"/> from the specified <see cref="ShortMessage"/>.
        /// </summary>
        /// <param name="message">A <see cref="ShortMessage"/> to initialize the <see cref="NoteOnMessage"/> from.</param>
        internal NoteOnMessage(ShortMessage message) : base(message) { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the pitch of the MIDI note.
        /// </summary>
        public int Pitch
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
    /// Defines a note off MIDI message, extends a <see cref="ChannelMessage"/> with pitch and velocity properties.
    /// </summary>
    public sealed class NoteOffMessage : ChannelMessage
    {
        #region Constructor

        /// <summary>
        /// Creates and initializes a <see cref="NoteOffMessage"/> from the specified parameters.
        /// </summary>
        /// <param name="channel">A <see cref="MidiChannels"/> to associate with the message.</param>
        /// <param name="pitch">An <see cref="int"/> specifying the pitch of the note.</param>
        /// <param name="velocity">An <see cref="int"/> specifying the velocity of the note.</param>
        public NoteOffMessage(MidiChannels channel, int pitch, int velocity) : base(ChannelMessageTypes.NoteOn, channel, pitch, velocity) { }

        /// <summary>
        /// Creates and initializes a <see cref="NoteOffMessage"/> from the specified <see cref="ShortMessage"/>.
        /// </summary>
        /// <param name="message">A <see cref="ShortMessage"/> to initialize the <see cref="NoteOffMessage"/> from.</param>
        internal NoteOffMessage(ShortMessage message) : base(message) { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the pitch of the MIDI note.
        /// </summary>
        public int Pitch
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

    #endregion
}
