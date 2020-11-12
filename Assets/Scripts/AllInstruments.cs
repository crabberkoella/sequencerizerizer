using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllInstruments : MonoBehaviour
{
    public static Dictionary<string, AudioClip> instruments = new Dictionary<string, AudioClip>();
    public List<AudioClip> allStupidAudioClips = new List<AudioClip>();

    private void Start()
    {
        foreach(AudioClip audioClip in allStupidAudioClips)
        {
            instruments[audioClip.name] = audioClip;
        }

    }

    public static Dictionary<string, Color> instrumentColors = new Dictionary<string, Color>()
    {
        {"CrateDigger", new Color (1f, 1f, 1f) },
        {"SoCal", new Color (0.128f, 0.111f, 0.168f)},
        {"DarkMalletLayers", new Color (1f, 1f, 1f)},
        {"GlassMarimbaHard", new Color (0.22f, 0.74f, 0.265f) },
        {"Vibraphone", new Color (0.22f, 0.74f, 0.265f) },        
        {"NeverendingRiser", new Color (1f, 1f, 1f) },
        {"AcousticGuitar", new Color (1f, 1f, 1f) },
        {"HardRock", new Color (1f, 1f, 1f)},
        {"GrandPianoAndPad", new Color (1f, 1f, 1f) },
        {"StudioTrumpet1", new Color (1f, 1f, 1f) },
        {"SugarHill", new Color (1f, 1f, 1f)},
        {"ChurchChoir", new Color (1f, 1f, 1f) }
    };

    public static Dictionary<string, float> instrumentPaletteLevels = new Dictionary<string, float>()
    {
        {"CrateDigger", 0f},
        {"SoCal", 0f},
        {"DarkMalletLayers", 10f},
        {"GlassMarimbaHard", 10f},
        {"Vibraphone", 10f },
        {"NeverendingRiser", 10f},
        {"AcousticGuitar", 10f },
        {"HardRock", 10f},
        {"GrandPianoAndPad", 10f },
        {"StudioTrumpet1", 10f },
        {"SugarHill", 10f},
        {"ChurchChoir", 10f }
    };

    public static Dictionary<string, int> instrumentStartingNotes = new Dictionary<string, int>()
    {
        {"CrateDigger", 0 },
        {"SoCal", 0 },
        {"DarkMalletLayers", 0 },
        {"GlassMarimbaHard", 0 },
        {"Vibraphone", 0 },
        {"NeverendingRiser", 0 },
        {"AcousticGuitar", 0 },
        {"HardRock", 0 },
        {"GrandPianoAndPad", 0 },
        {"StudioTrumpet1", 4 },
        {"SugarHill", 4 },
        {"ChurchChoir", 4}
    };

    public static List<string> instrumentNoteIDToName = new List<string>() { "C", "C#" , "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"};
}
