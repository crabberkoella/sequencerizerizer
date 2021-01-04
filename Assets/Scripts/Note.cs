using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{

    public Ring ownerRing;
    public int noteID;
    public string instrumentName;

    public AudioClip audioClip;
    public float startTime = 10f;
    public float clipLength;

    public AudioSource audioSource;

    float lowestBrightness = 0.2f;

    public void NoteStupidConstructor(Ring ring, int id, float start, string instrument)
    {
        ownerRing = ring;
        noteID = id;
        startTime = start;
        instrumentName = instrument;

        audioClip = AllInstruments.instruments[instrument];

        clipLength = audioClip.length;

        audioSource = GetComponent<AudioSource>();
        audioSource.time = start;
        audioSource.clip = audioClip;

        PlayNote(false, true);
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.time = startTime;

        GetComponent<MeshRenderer>().material.SetColor("_Color", AllInstruments.instrumentColors[instrumentName]);


    }
    
    void Update()
    {
        
    }

    public void PlayNote(bool offset, bool immediate = false)
    {
        StartCoroutine(PlayNote_(offset, immediate));
    }

    float playTime = 3.9f;
    IEnumerator PlayNote_(bool offset, bool immediate)
    {
        if(!TimeKeeper.mute || immediate)
        {
            audioSource.PlayScheduled(AudioSettings.dspTime + (immediate ? 0f : 2.0f) + (offset ? 0.125f : 0f));
            audioSource.SetScheduledEndTime(AudioSettings.dspTime + 3.9f + (immediate ? 0f : 2.0f) + (offset ? 0.125f : 0f));
        }

        yield return new WaitForSeconds((immediate ? 0f : 2f));
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
        //audioSource.Stop();

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
            startTime += 16f;
        }
        else
        {
            startTime += 4f;
        }
        

        if(startTime >= clipLength)
        {
            startTime = clipLength - startTime;
        }

        audioSource.time = startTime;
    }
    public void DecrementNote()
    {
        if (Input.GetKey(KeyCode.RightShift))
        {
            startTime -= 16f;
        }
        else
        {
            startTime -= 4f;
        }
            

        if (startTime < 0f)
        {
            startTime = clipLength + startTime;
        }

        audioSource.time = startTime;
    }
}
