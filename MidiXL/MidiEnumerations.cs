namespace MidiXL
{
    /// <summary>
    /// Defines constants representing MIDI message types.
    /// </summary>
    public enum MidiMessageTypes : byte
    {
        ChannelMessage,
        SystemExclusiveMessage,
        SystemCommonMessage,
        SystemRealtimeMessage,
        MetaMessage,
        ShortMessage
    }

    /// <summary>
    /// Defines constant values representing MIDI channel message type status bytes.
    /// </summary>
    public enum ChannelMessageTypes : byte
    {
        Min               = 0x80,
        NoteOff           = 0x80,
        NoteOn            = 0x90,
        KeyAfterTouch     = 0xA0,
        ControlChange     = 0xB0,
        ProgramChange     = 0xC0,
        ChannelAfterTouch = 0xD0,
        PitchBend         = 0xE0,
        Max               = 0xE0
    }

    /// <summary>
    /// Defines constant values representing MIDI system exclusive message type status bytes.
    /// </summary>
    public enum SystemExclusiveMessageTypes : byte
    {
        SystemExclusive    = 0xF0,
        SystemExclusiveEnd = 0xF7
    }

    /// <summary>
    /// Defines constant values representing MIDI universal system exclusive message type status bytes.
    /// </summary>
    public enum UniversalSystemExclusiveMessageType : byte
    {
        UniversalNonRealTime = 0x7E,
        UniversalRealTime    = 0x7F
    }

    /// <summary>
    /// Defines constant values representing MIDI system common message type status bytes.
    /// </summary>
    public enum SystemCommonMessageTypes : byte
    {
        Min          = 0xF1,
        MidiTimeCode = 0xF1,
        SongPosition = 0xF2,
        SongSelect   = 0xF3,
        TuneRequest  = 0xF6,
        Max          = 0xF6
    }

    /// <summary>
    /// Defines constant values representing MIDI system realtime message type status bytes.
    /// </summary>
    public enum SystemRealtimeMessageTypes : byte
    {
        Min           = 0xF8,
        Clock         = 0xF8,
        Tick          = 0xF9,
        Start         = 0xFA, 
        Continue      = 0xFB, 
        Stop          = 0xFC, 
        ActiveSensing = 0xFE,
        Reset         = 0xFF,
        Max           = 0xFF
    }

    /// <summary>
    /// Defines constant values representing MIDI channels.
    /// </summary>
    public enum MidiChannels : byte
    {
        Min       = 0x00,
        Channel01 = 0x00,
        Channel02 = 0x01,
        Channel03 = 0x02,
        Channel04 = 0x03,
        Channel05 = 0x04,
        Channel06 = 0x05,
        Channel07 = 0x06,
        Channel08 = 0x07,
        Channel09 = 0x08,
        Channel10 = 0x09,
        Channel11 = 0x0A,
        Channel12 = 0x0B,
        Channel13 = 0x0C,
        Channel14 = 0x0D,
        Channel15 = 0x0E,
        Channel16 = 0x0F,
        Max       = 0x0F
    }
}
