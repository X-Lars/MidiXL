﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiXL
{
    /// <summary>
    /// Class providing MIDI event arguments specific to <see cref="NoteOnMessage"/>s.
    /// </summary>
    public class NoteOnMessageEventArgs
    {
        #region Constructor

        /// <summary>
        /// Creates and initializes a <see cref="NoteOnMessageEventArgs"/> with the specified message.
        /// </summary>
        /// <param name="message">The <see cref="NoteOnMessage"/> that raised the event.</param>
        public NoteOnMessageEventArgs(NoteOnMessage message)
        {
            this.Message = message;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the message that raised the event.
        /// </summary>
        public NoteOnMessage Message { get; }

        #endregion
    }

    /// <summary>
    /// Class providing MIDI event arguments specific to <see cref="NoteOffMessage"/>s.
    /// </summary>
    public class NoteOffMessageEventArgs
    {
        #region Constructor

        /// <summary>
        /// Creates and initializes a <see cref="NoteOffMessageEventArgs"/> with the specified message.
        /// </summary>
        /// <param name="message">The <see cref="NoteOffMessage"/> that raised the event.</param>
        public NoteOffMessageEventArgs(NoteOffMessage message)
        {
            this.Message = message;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the message that raised the event.
        /// </summary>
        public NoteOffMessage Message { get; }

        #endregion
    }

    /// <summary>
    /// Class providing MIDI event arguments specific to <see cref="ControlChangeMessage"/>s.
    /// </summary>
    public class ControlChangeMessageEventArgs
    {
        #region Constructor

        /// <summary>
        /// Creates and initializes a <see cref="ControlChangeMessageEventArgs"/> with the specified message.
        /// </summary>
        /// <param name="message"></param>
        public ControlChangeMessageEventArgs(ControlChangeMessage message)
        {
            this.Message = message;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the message that raised the event.
        /// </summary>
        public ControlChangeMessage Message { get; }

        #endregion
    }

    /// <summary>
    /// Class providing MIDI event arguments specific to <see cref="ProgramChangeMessage"/>s.
    /// </summary>
    public class ProgramChangeMessageEventArgs
    {
        #region Constructor

        /// <summary>
        /// Creates and initializes a <see cref="ProgramChangeMessageEventArgs"/> with the specified message.
        /// </summary>
        /// <param name="message"></param>
        public ProgramChangeMessageEventArgs(ProgramChangeMessage message)
        {
            this.Message = message;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the message that raised the event.
        /// </summary>
        public ProgramChangeMessage Message { get; }

        #endregion
    }

    /// <summary>
    /// Class providing MIDI event arguments specific to <see cref="PitchBendMessage"/>
    /// </summary>
    public class PitchBendMessageEventArgs
    {
        #region Constructor

        /// <summary>
        /// Creates and initializes a <see cref="PitchBendMessageEventArgs"/> with the specified message.
        /// </summary>
        /// <param name="message"></param>
        public PitchBendMessageEventArgs(PitchBendMessage message)
        {
            this.Message = message;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the message that raised the event.
        /// </summary>
        public PitchBendMessage Message { get; }

        #endregion
    }

    /// <summary>
    /// Class providing MIDI event arguments specific to <see cref="KeyAfterTouchMessage"/>
    /// </summary>
    public class KeyAfterTouchMessageEventArgs
    {
        #region Constructor

        /// <summary>
        /// Creates and initializes a <see cref="KeyAfterTouchMessageEventArgs"/> with the specified message.
        /// </summary>
        /// <param name="message"></param>
        public KeyAfterTouchMessageEventArgs(KeyAfterTouchMessage message)
        {
            this.Message = message;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the message that raised the event.
        /// </summary>
        public KeyAfterTouchMessage Message { get; }

        #endregion
    }

    /// <summary>
    /// Class providing MIDI event arguments specific to <see cref="ChannelAfterTouchMessage"/>
    /// </summary>
    public class ChannelAfterTouchMessageEventArgs
    {
        #region Constructor

        /// <summary>
        /// Creates and initializes a <see cref="ChannelAfterTouchMessageEventArgs"/> with the specified message.
        /// </summary>
        /// <param name="message"></param>
        public ChannelAfterTouchMessageEventArgs(ChannelAfterTouchMessage message)
        {
            this.Message = message;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the message that raised the event.
        /// </summary>
        public ChannelAfterTouchMessage Message { get; }

        #endregion
    }
}
