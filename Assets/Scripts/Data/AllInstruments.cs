using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllInstruments : MonoBehaviour
{

    public static Dictionary<string, AudioClip> instruments = new Dictionary<string, AudioClip>();

    // Unity's way of streaming assets just isn't working, so we're going to handle this manually
    public List<AudioClip> allStupidAudioClips = new List<AudioClip>();

    // this might not be the best way to do this
    public Texture noteObjectDrum;
    public Texture noteObjectClap;
    public Texture noteObjectKeys;
    public Texture noteObjectBell;
    public Texture noteObjectWave;
    public Texture noteObjectOcean;

    private void Start()
    {
        foreach(AudioClip audioClip in allStupidAudioClips) // temporarily set the intrument group here (depending on which list we use)
        {
            instruments[audioClip.name] = audioClip;
        }

        instrumentNoteTextures = new Dictionary<string, Texture>()
        {
            {"PlasticKeys", noteObjectKeys },
            {"80sSineSynth", noteObjectKeys },
            {"ClassicFunkBoogieBass", noteObjectOcean },
            {"StudioStomps", noteObjectDrum },
            {"ElectronicCowbell", noteObjectBell },
            {"PawnShopClaps", noteObjectClap },
            {"ClassicClean", noteObjectWave },
            {"BoompBapSub", noteObjectClap },
            {"BoompBapKick2", noteObjectDrum },
            {"Cellos", noteObjectWave },
            {"Marimba", noteObjectDrum }
        };

    }

    public static Dictionary<string, Color> instrumentColors = new Dictionary<string, Color>()
    {
        {"PlasticKeys", new Color (0f, 0f, 1f) },
        {"80sSineSynth", new Color (0.75f, 0f, 1f) },
        {"ClassicFunkBoogieBass", new Color (.75f, .5f, 0.5f) },
        {"StudioStomps", new Color (1f, 1f, 0f) },
        {"ElectronicCowbell", new Color (1f, 0.5f, .4f) },
        {"PawnShopClaps", new Color (1f, .8f, 1f) },
        {"ClassicClean", new Color (.75f, .3f, 1f) },
        {"BoompBapSub", new Color (0.65f, .8f, 0.65f) },
        {"BoompBapKick2", new Color (.75f, .3f, .45f) },
        {"Cellos", new Color (.9f, .9f, .9f) },
        {"Marimba", new Color (1f, .7f, .3f) }
    };

    // how long a note is for a given instrument
    public static Dictionary<string, float> noteLengths = new Dictionary<string, float>()
    {
        {"PlasticKeys", 1f },
        {"80sSineSynth", 1f },
        {"ClassicFunkBoogieBass", 2f },
        {"StudioStomps", 1f },
        {"ElectronicCowbell", 1f },
        {"PawnShopClaps", 1f },
        {"ClassicClean", 2f },
        {"BoompBapSub", 2f },
        {"BoompBapKick2", 1f },
        {"Cellos", 4f },
        {"Marimba", 4f }
    };

    // we could calculate it at runtime, but this'll make things much cleaner if we just manually record it, here
    public static Dictionary<string, int> numberOfOctaves = new Dictionary<string, int>()
    {
        {"PlasticKeys", 2 },
        {"80sSineSynth", 2 },
        {"ClassicFunkBoogieBass", 3 },
        {"StudioStomps", 1 },
        {"ElectronicCowbell", 1 },
        {"PawnShopClaps", 1 },
        {"ClassicClean", 3 },
        {"BoompBapSub", 1 },
        {"BoompBapKick2", 1 },
        {"Cellos", 3 },
        {"Marimba", 3 }
    };

    // this is useful to hone in the starting key, since some have a pretty wide range but the lower extreme is too low to be said 'default'
    public static Dictionary<string, int> instrumentStartingNotes = new Dictionary<string, int>()
    {
        {"PlasticKeys", 6 },
        {"80sSineSynth", 6 },
        {"ClassicFunkBoogieBass", 18 },
        {"StudioStomps", 6 },
        {"ElectronicCowbell", 6 },
        {"PawnShopClaps", 6 },
        {"ClassicClean", 18 },
        {"BoompBapSub", 6 },
        {"BoompBapKick2", 6 },
        {"Cellos", 6 },
        {"Marimba", 6 }
    };

    public Dictionary<string, Texture> instrumentNoteTextures;

    public static List<string> instrumentNoteIDToName = new List<string>() { "G", "G#", "A", "A#", "B", "C", "C#", "D", "D#", "E", "F", "F#" };
}
