using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundHelper
{
    public static void PlayMusic(string soundName, bool loop = true, System.Action cbEnd = null)
    {
        SoundController.instance?.PlaySoundMusic(soundName, loop, cbEnd);
    }
    public static void PlayEffect(string soundName)
    {
        SoundController.instance?.PlaySoundEffect(soundName);
    }

    /// <summary>
    ///  Chi su dung PlayEffectVer2 khi am thanh can Stop, Pause , UnPause
    /// </summary>
    public static void PlayEffectVer2(string soundName)
    {
        SoundController.instance?.PlaySoundEffectVer2(soundName);
    } 
    public static void StopEffect(string soundName)
    {
        SoundController.instance?.StopSoundEffect(soundName);
    }
    public static void PauseEffect(string soundName)
    {
        SoundController.instance?.PauseSoundEffect(soundName);
    } 
    public static void UnPauseEffect(string soundName)
    {
        SoundController.instance?.UnPauseSoundEffect(soundName);
    }
}
