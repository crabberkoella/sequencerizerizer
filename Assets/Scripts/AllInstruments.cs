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
            instruments[audioClip.name] = audioClip;
        }

    }

    public static Dictionary<string, Color> instrumentColors = new Dictionary<string, Color>()
    {
        {"PlasticKeys", new Color (1f, 0f, 0f) },
        {"80sSineSynth", new Color (0f, 0f, 1f) },
        {"DangerBass", new Color (.5f, .25f, .33f) },
        {"ClassicFunkBoogieBass", new Color (.75f, .5f, 0f) },
        {"StudioStomps", new Color (.25f, .5f, 1f) },
        {"ElectronicCowbell", new Color (.8f, 8f, .4f) },
        {"PawnShopClaps", new Color (.5f, .8f, .0f) }
    };

    // how long a note is for a given instrument
    public static Dictionary<string, float> noteLengths = new Dictionary<string, float>()
    {
        {"PlasticKeys", 1f },
        {"80sSineSynth", 1f },
        {"ClassicFunkBoogieBass", 2f },
        {"StudioStomps", 1f },
        {"ElectronicCowbell", 1f },
        {"PawnShopClaps", 1f }
    };

    // we could calculate it at runtime, but this'll make things much cleaner if we just manually record it, here
    public static Dictionary<string, int> numberOfOctaves = new Dictionary<string, int>()
    {
        {"PlasticKeys", 2 },
        {"80sSineSynth", 2 },
        {"ClassicFunkBoogieBass", 3 },
        {"StudioStomps", 1 },
        {"ElectronicCowbell", 1 },
        {"PawnShopClaps", 1 }
    };

    // this is useful to hone in the starting key, since some have a pretty wide range but the lower extreme is too low to be said 'default'
    public static Dictionary<string, int> instrumentStartingNotes = new Dictionary<string, int>()
    {
        {"PlasticKeys", 6 },
        {"80sSineSynth", 6 },
        {"ClassicFunkBoogieBass", 18 },
        {"StudioStomps", 6 },
        {"ElectronicCowbell", 6 },
        {"PawnShopClaps", 6 }
    };

    public static List<string> instrumentNoteIDToName = new List<string>() { "C", "C#" , "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"};
}
