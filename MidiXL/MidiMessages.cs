using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiXL
{
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
        /// Creates and initializes a <see cref="ShortMessage"/> for use inside the <see cref="MidiInputDevice.Callback"/> method.
        /// </summary>
        /// <param name="messageParameterA">An <see cref="IntPtr"/> to store a reference to the first message parameter.</param>
        /// <param name="messageParameterB">An <see cref="IntPtr"/> to store a reference to the second message parameter.</param>
        internal ShortMessage(IntPtr messageParameterA, IntPtr messageParameterB)
        {
            // Decode message parameter A
            this.Status = ((int)messageParameterA & 0xF0);
            this.DataA  = ((int)messageParameterA & 0xFF00) >> 8;
            this.DataB  = ((int)messageParameterA & 0xFF0000) >> 16;

            // Decode message parameter B
            this.TimeStamp = (int)messageParameterB;
        }

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
}
