﻿using AvaMSN.MSNP.Messages;

namespace AvaMSN.MSNP.Models;

public class DisconnectedEventArgs : EventArgs
{
    public bool Requested { get; set; }
    public bool RedirectedByTheServer { get; set; }
    public string NewServerHost { get; set; } = string.Empty;
    public int NewServerPort { get; set; } = 1863;
}

public class ContactEventArgs : EventArgs
{
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}

public class PresenceEventArgs : ContactEventArgs
{
    public string Presence { get; set; } = string.Empty;
    public bool HasDisplayPicture { get; set; } = true;
    public string? DisplayPictureHash { get; set; }
}

public class PersonalMessageEventArgs : ContactEventArgs
{
    public string PersonalMessage { get; set; } = string.Empty;
}

public class MessageEventArgs : ContactEventArgs
{
    public bool InContactList { get; set; }
    public bool TypingUser { get; set; }
    public bool IsNudge { get; set; }
    public TextPlain? Message { get; set; }
}

public class SwitchboardEventArgs
{
    public Switchboard.Switchboard? Switchboard { get; set; }
}

public class DisplayPictureEventArgs : ContactEventArgs
{
    public byte[]? DisplayPicture { get; set; }
    public string? DisplayPictureHash { get; set; }
}