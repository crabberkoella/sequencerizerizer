using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{

    public Ring ownerRing;
    public int noteID; // where it lands in the Ring
    public string instrumentName;

    public AudioClip audioClip;
    public float key = 10f;
    public float clipLength;

    float noteLength; // just for convenience, to not have to type AllInstruments.noteLengths all the time

    public AudioSource audioSource;

    float lowestBrightness = 0.2f;

    public void NoteStupidConstructor(Ring ring, int id, float start, string instrument)
    {
        ownerRing = ring;
        noteID = id;
        key = start; // everything is set up for the key to be where in the sound file the note is played
        instrumentName = instrument;
        noteLength = AllInstruments.noteLengths[instrumentName];

        audioClip = AllInstruments.instruments[instrument];

        clipLength = audioClip.length;

        audioSource = GetComponent<AudioSource>();
        audioSource.time = key;
        audioSource.clip = audioClip;

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
            audioSource.PlayScheduled(AudioSettings.dspTime + (immediate ? 0f : 2.0f) + (offset ? 0.125f : 0f));
            audioSource.SetScheduledEndTime(AudioSettings.dspTime + 3.9f + (immediate ? 0f : 2.0f) + (offset ? 0.125f : 0f));
        }

        yield return new WaitForSeconds((immediate ? 0f : 2f));
        float playTime = noteLength - 0.05f; // to avoid the very last frame of the sound file, in case it plays the start of the next key (probably should find a better solution eventually)
        float playTimer = playTime;
        while (playTimer > 0f)
        {
            float progress = playTimer / playTime;
            float b = lowestBrightness + (progress * (1f - lowestBrightness));
            GetComponent<MeshRenderer>().material.SetColor("_Brightness", new Color(b, b, b, 1f));

            playTimer -= Time.deltaTime;

            yield return new WaitForEndOfFrame();

        }

        GetComponent<MeshRenderer>().material.SetColor("_Brightness", new Color(lowestBrightness, lowestBrightness, lowestBrightness, 1f));
    }

    public void DeleteNote()
    {
        ownerRing.placedNotes.Remove(noteID);
        Destroy(gameObject);
    }

    public void IncrementNote()
    {
        if(Input.GetKey(KeyCode.RightShift))
        {
            key += noteLength * 4f;
        }
        else
        {
            key += noteLength;
        }
        

        if(key >= clipLength)
        {
            key = key - clipLength;
        }

        audioSource.time = key;
    }
    public void DecrementNote()
    {
        if (Input.GetKey(KeyCode.RightShift))
        {
            key -= noteLength * 4f;
        }
        else
        {
            key -= noteLength;
        }
            

        if (key < 0f)
        {
            key = clipLength + key; // because key will be negative, so it'll put it towards the end of the clip
        }

        audioSource.time = key;
    }
}
