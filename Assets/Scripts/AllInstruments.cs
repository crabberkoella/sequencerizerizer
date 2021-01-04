using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllInstruments : MonoBehaviour
{

    public static Dictionary<string, AudioClip> instruments = new Dictionary<string, AudioClip>();

    // Unity's way of streaming assets just isn't working, so we're going to handle this manually
    public List<AudioClip> allStupidAudioClips = new List<AudioClip>();

    private void Start()
    {
        foreach(AudioClip audioClip in allStupidAudioClips) // temporarily set the intrument group here (depending on which list we use)
        {
            Debug.Log(audioClip.name);
            instruments[audioClip.name] = audioClip;
        }

    }

    public static Dictionary<string, Color> instrumentColors = new Dictionary<string, Color>()
    {
        {"BoxySynthBass", new Color (1f, 1f, 1f) },
    };

    // how long a note is for a given instrument
    public static Dictionary<string, float> noteLengths = new Dictionary<string, float>()
    {
        {"BoxySynthBass", 4f }
    };

    // this is useful to hone in the starting noteLevel, since some have a pretty wide range but the lower extreme is too low to be said 'default'
    public static Dictionary<string, int> instrumentStartingNotes = new Dictionary<string, int>()
    {
        {"BoxySynthBass", 0}
    };

    public static List<string> instrumentNoteIDToName = new List<string>() { "C", "C#" , "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"};
}
