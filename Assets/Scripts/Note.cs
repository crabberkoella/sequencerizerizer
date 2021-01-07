using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{

    public Ring ownerRing;
    public int noteID; // where it lands in the Ring
    public string instrumentName;

    public AudioClip audioClip;
    public int key;
    int numberOfOctaves;

    int highestKey; // for convenience; we use it to know when we should go back to the beginning if we increment the note past this
    // start is G

    float audioClipStartTime;
    float pitchShiftAmount;

    float noteLength; // just for convenience, to not have to type AllInstruments.noteLengths all the time

    public AudioSource audioSource;

    float lowestBrightness = 0.2f;

    public Color outColor = new Color();
    public float loudness = 0;

    public void NoteStupidConstructor(Ring ring, int keyIn, int id, string instrument)
    {
        ownerRing = ring;
        noteID = id;
        key = keyIn; // everything is set up for the key to be where in the sound file the note is played
        instrumentName = instrument;
        noteLength = AllInstruments.noteLengths[instrumentName];

        audioClip = AllInstruments.instruments[instrument];

        numberOfOctaves = AllInstruments.numberOfOctaves[instrument];
        highestKey = (numberOfOctaves * 12) - 1; // - 1 because, of course, we start at zero

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = audioClip;

        CalculateClipStartTimeAndPitchShift();
        outColor = AllInstruments.instrumentColors[instrumentName];

        PlayNote(false, true);
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        GetComponent<MeshRenderer>().material.SetColor("_Color", AllInstruments.instrumentColors[instrumentName]);


    }
    
    void Update()
    {
        
    }

    public void PlayNote(bool offset, bool immediate = false)
    {
        StartCoroutine(PlayNote_(offset, immediate));
    }

    IEnumerator PlayNote_(bool offset, bool immediate)
    {
        if(!TimeKeeper.mute || immediate)
        {
            float endTime = noteLength - (noteLength * .07f * (pitchShiftAmount > 0 ? pitchShiftAmount : 1f));

            audioSource.PlayScheduled(AudioSettings.dspTime + (immediate ? 0f : 2.0f) + (offset ? 0.125f : 0f));
            audioSource.SetScheduledEndTime(AudioSettings.dspTime + endTime + (immediate ? 0f : 2.0f) + (offset ? 0.125f : 0f));

        }

        yield return new WaitForSeconds((immediate ? 0f : 2f));

        if (!TimeKeeper.mute && !immediate)
        {
            loudness = 0f;
            SoundToColorControl.playingNotes.Add(this);
        }

        float playTime = noteLength - 0.1f; // to avoid the very last frame of the sound file, in case it plays the start of the next key (probably should find a better solution eventually)
        float playTimer = playTime;
        while (playTimer > 0f)
        {
            float progress = playTimer / playTime;

            if(!immediate)
            {
                if (progress <= 0.5f)
                {
                    loudness = progress / 5f;
                }
                else
                {
                    loudness = (1.0f - progress) / 5f;
                }
            }
            
            
            float b = lowestBrightness + (progress * (1f - lowestBrightness));
            GetComponent<MeshRenderer>().material.SetColor("_Brightness", new Color(b, b, b, 1f));

            playTimer -= Time.deltaTime;
            yield return new WaitForEndOfFrame();

        }

        GetComponent<MeshRenderer>().material.SetColor("_Brightness", new Color(lowestBrightness, lowestBrightness, lowestBrightness, 1f));
        SoundToColorControl.playingNotes.Remove(this);
        loudness = 0f;
    }

    public void DeleteNote()
    {
        ownerRing.placedNotes.Remove(noteID);
        Destroy(gameObject);
    }

    public void IncrementNote()
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            key += 4;
        }
        else
        {
            key += 1;
        }
        

        if(key >= highestKey)
        {
            key = key - highestKey;
        }

        CalculateClipStartTimeAndPitchShift();
    }
    public void DecrementNote()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            key -= 4;
        }
        else
        {
            key -= 1;
        }
            

        if (key < 0f)
        {
            key = highestKey + key; // because key will be negative, so it'll put it towards the end of the range
        }

        CalculateClipStartTimeAndPitchShift();
    }

    void CalculateClipStartTimeAndPitchShift() // specific enough?
    {
        int octave = key / 12;

        audioClipStartTime = (float)octave * AllInstruments.noteLengths[instrumentName];

        pitchShiftAmount = (float)(key % 12) - 5f;

        audioSource.time = audioClipStartTime;
        audioSource.pitch = Mathf.Pow(1.05946f, pitchShiftAmount);
    }


}
