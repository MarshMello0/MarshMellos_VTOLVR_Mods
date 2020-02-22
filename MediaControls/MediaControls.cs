using System;
using System.Runtime.InteropServices;
using HarmonyLib;
using UnityEngine;
using System.Reflection;
/*
* Thanks to Carlos Delgado for this post
* https://ourcodeworld.com/articles/read/128/how-to-play-pause-music-or-go-to-next-and-previous-track-from-windows-using-c-valid-for-all-windows-music-players
*/
public class MediaControls : VTOLMOD
{
    public enum MediaKey { PlayPause,NextTrack,PreviousTrack};
    public const int KEYEVENTF_EXTENTEDKEY = 1;
    public const int KEYEVENTF_KEYUP = 0;
    public const int VK_MEDIA_NEXT_TRACK = 0xB0;// code to jump to next track
    public const int VK_MEDIA_PLAY_PAUSE = 0xB3;// code to play or pause a song
    public const int VK_MEDIA_PREV_TRACK = 0xB1;// code to jump to prev track

    [DllImport("user32.dll")]
    public static extern void keybd_event(byte virtualKey, byte scanCode, uint flags, IntPtr extraInfo);
    private static readonly string harmonyID = "marsh.mediacontrols";

    private void Start()
    {
        Harmony instance = new Harmony(harmonyID);
        instance.PatchAll(Assembly.GetExecutingAssembly());
    }

    public static void PressButton(MediaKey key)
    {
        switch (key)
        {
            case MediaKey.PlayPause:
                Debug.Log("Play/Pause Pressed");
                keybd_event(VK_MEDIA_PLAY_PAUSE, 0, KEYEVENTF_EXTENTEDKEY, IntPtr.Zero);
                break;
            case MediaKey.NextTrack:
                Debug.Log("Next Track Pressed");
                keybd_event(VK_MEDIA_NEXT_TRACK, 0, KEYEVENTF_EXTENTEDKEY, IntPtr.Zero);
                break;
            case MediaKey.PreviousTrack:
                Debug.Log("Previous Track Pressed");
                keybd_event(VK_MEDIA_PREV_TRACK, 0, KEYEVENTF_EXTENTEDKEY, IntPtr.Zero);
                break;
        }
    }
}
/*
 * If anyone knows harmony, it would be create if these could be in one class.
 */

[HarmonyPatch(typeof(CockpitRadio))]
[HarmonyPatch("PlayButton")]
public class Patch0
{
    public static bool Prefix()
    {
        MediaControls.PressButton(MediaControls.MediaKey.PlayPause);
        return false;
    }
}

[HarmonyPatch(typeof(CockpitRadio))]
[HarmonyPatch("NextSong")]
public class Patch1
{
    public static bool Prefix()
    {
        MediaControls.PressButton(MediaControls.MediaKey.NextTrack);
        return false;
    }
}

[HarmonyPatch(typeof(CockpitRadio))]
[HarmonyPatch("PrevSong")]
public class Patch2
{
    public static bool Prefix()
    {
        MediaControls.PressButton(MediaControls.MediaKey.PreviousTrack);
        return false;
    }
}