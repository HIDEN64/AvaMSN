﻿namespace AvaMSN.MSNP.PresenceStatus;

public static class PresenceStatus
{
    public const string Available = "NLN";

    public const string Busy = "BSY";

    public const string Idle = "IDL";

    public const string BeRightBack = "BRB";

    public const string Away = "AWY";

    public const string OnThePhone = "PHN";

    public const string OutToLunch = "LUN";

    public const string Invisible = "HDN";

    public const string Offline = "";

    public static string GetFullName(string status) => status switch
    {
        Available => "Available",
        Busy => "Busy",
        Idle => "Idle",
        BeRightBack => "Be right back",
        Away => "Away",
        OnThePhone => "On the phone",
        OutToLunch => "Out to lunch",
        Invisible => "Invisible",
        _ => "Offline"
    };

    public static string GetShortName(string status) => status switch
    {
        "Available" => Available,
        "Busy" => Busy,
        "Idle" => Idle,
        "Be right back" => BeRightBack,
        "Away" => Away,
        "On the phone" => OnThePhone,
        "Out to lunch" => OutToLunch,
        "Invisible" => Invisible,
        _ => Offline
    };
}