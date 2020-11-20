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
        #region DLL Imports "winmm.dll"

        /// <summary>
        /// <see cref="MidiOutputDeviceCount"/>
        /// </summary>
        /// <returns><see cref="MidiOutputDeviceCount"/></returns>
        [DllImport("winmm.dll", SetLastError = true)]
        private static extern uint midiOutGetNumDevs();

        /// <summary>
        /// <see cref=""/>
        /// </summary>
        /// <returns><see cref=""/></returns>
        [DllImport("winmm.dll", SetLastError = true)]
        private static extern uint midiInGetNumDevs();

        #endregion

        #region 

        /// <summary>
        /// Counts the number of available MIDI output devices in the system.
        /// </summary>
        /// <returns>An <see cref="uint"/> representing the number of available MIDI output devices in the system.</returns>
        public static uint MidiOutputDeviceCount()
        {
            return midiOutGetNumDevs();
        }

        /// <summary>
        /// Counts the number of available MIDI input devices in the system.
        /// </summary>
        /// <returns>An <see cref="uint"/> representing the number of available MIDI input devices in the system.</returns>
        public static uint MidiInputDeviceCount()
        {
            return midiInGetNumDevs();
        }


        #endregion
    }
}
