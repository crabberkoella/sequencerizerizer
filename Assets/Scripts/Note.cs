using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : InteractableObject
{

    public delegate void DeleteNoteDelegate(Note note);
    public DeleteNoteDelegate deleteNoteDelegate;

    protected AudioSource audioSource;

    public int noteID; // where it lands in the Ring (0-15)
                       // it's not in noteData because noteData could change but this placed note would maintain the same ID and location within a Ring
    public NoteData noteData;

    protected int highestKey; // for convenience; we use it to know when we should go back to the beginning if we increment the note past this and calculate it on note creation
    // start is G

    // these are here, not with NoteData, as they're specific to the audioSource
    protected float audioClipStartTime;
    protected float pitchShiftAmount;

    // info for colors our sound affects
    protected float lowestBrightness = 0.2f;    
    public Color outColor = new Color(); // these two are public because the SoundToColorController
    public float loudness = 0;           // uses them to add up the final color value of Rings et al

    // when changing Note pitch by clicking and dragging
    float mouseDeltaY;
    int pitchStart;

    private void Start()
    {

    }

    private void Update()
    {
        /*
        if(!audioSource.isPlaying)
        {
            int octave = noteData.pitch / 12;

            audioClipStartTime = (float)octave * AllInstruments.noteLengths[noteData.instrumentName];

            audioSource.time = audioClipStartTime;
        }
        */
    }

    public void Initialize(NoteData noteDataIn, int noteIDIn)
    {
         
        noteData = noteDataIn;
        noteID = noteIDIn;

        highestKey = (AllInstruments.numberOfOctaves[noteData.instrumentName] * 12) - 1; // '- 1' because, of course, we start at zero

        outColor = AllInstruments.instrumentColors[noteData.instrumentName];

        audioSource = GetComponent<AudioSource>();

        GetComponent<MeshRenderer>().material.SetColor("_Color", AllInstruments.instrumentColors[noteData.instrumentName]);

        if(Time.realtimeSinceStartup > 5f) // cheating for now; all the Palette's InstrumentOptions play on startup, otherwise
        {
            PlayNote();
        }
        
    }

    public override void PrimaryInteractDown(PlayerInteractionController player = null)
    {
        base.PrimaryInteractDown(player);

        mouseDeltaY = 0f;
        pitchStart = noteData.pitch;
    }

    public override void PrimaryInteract(PlayerInteractionController player = null)
    {
        base.PrimaryInteract(player);

        mouseDeltaY += Input.GetAxis("Mouse Y") * 2f;

        int pitchOut = pitchStart + Mathf.FloorToInt(mouseDeltaY);

        if (pitchOut >= highestKey)
        {
            pitchOut = highestKey;
        }

        pitchOut = Mathf.Max(0, pitchOut);

        if (pitchOut != noteData.pitch)
        {
            noteData.pitch = pitchOut;


            PlayNote();
        }
    }

    public override void PrimaryInteractUp(PlayerInteractionController player = null)
    {
        base.PrimaryInteractUp(player);
    }

    public override void SecondaryInteractUp(PlayerInteractionController player = null)
    {
        base.SecondaryInteractUp(player);
    }

    public void PlayNote()
    {
        StartCoroutine(PlayNote_());
    }

    protected IEnumerator PlayNote_()
    {        

        audioSource.clip = AllInstruments.instruments[noteData.instrumentName];

        int octave = noteData.pitch / 12;

        audioClipStartTime = (float)octave * AllInstruments.noteLengths[noteData.instrumentName];

        pitchShiftAmount = (float)(noteData.pitch % 12) - 5f; // % 12 == number of pitches/notes in an octave (including sharps/flats) and - 5 == roughly the middle of an octave

        audioSource.time = audioClipStartTime;
        audioSource.pitch = Mathf.Pow(1.05946f, pitchShiftAmount);


        float endTime = AllInstruments.noteLengths[noteData.instrumentName] - (AllInstruments.noteLengths[noteData.instrumentName] * .07f * (pitchShiftAmount > 0 ? pitchShiftAmount : 1f));

        audioSource.PlayScheduled(AudioSettings.dspTime);
        audioSource.SetScheduledEndTime(AudioSettings.dspTime + endTime);

        loudness = 0f;
        SoundToColorControl.playingNotes.Add(this);
        

        float playTime = AllInstruments.noteLengths[noteData.instrumentName] - 0.1f; // to avoid the very last frame of the sound file, in case it plays the start of the next key (probably should find a better solution eventually)
        float playTimer = playTime;
        while (playTimer > 0f)
        {
            float progress = playTimer / playTime;

            float l = GetLoudness();

            loudness = l; // (l > 0.05f ? l * 2.5f : 0f);

            
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
        GetComponentInParent<Ring>().RemoveNote(this);

        Destroy(gameObject);
    }

    public void IncrementNote()
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            noteData.pitch += 4;
        }
        else
        {
            noteData.pitch += 1;
        }
        

        if(noteData.pitch > highestKey)
        {
            noteData.pitch = noteData.pitch - highestKey - 1;
        }

    }

    public void DecrementNote()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            noteData.pitch -= 4;
        }
        else
        {
            noteData.pitch -= 1;
        }
            

        if (noteData.pitch < 0f)
        {
            noteData.pitch = highestKey + noteData.pitch + 1; // because noteData.pitch will be negative, so it'll put it towards the end of the range
        }

    }

    public float updateStep = 0.1f;
    public int sampleDataLength = 128;

    private float clipLoudness;
    private float[] clipSampleData;

    // Use this for initialization
    void Awake()
    {
        clipSampleData = new float[sampleDataLength];
    }


    float GetLoudness()
    {

        audioSource.clip.GetData(clipSampleData, audioSource.timeSamples); // I read 1024 samples, which is about 80 ms on a 44khz stereo clip, beginning at the current sample position of the clip.

        clipLoudness = 0f;

        foreach (var sample in clipSampleData)
        {
            clipLoudness += Mathf.Abs(sample);
        }

        clipLoudness /= sampleDataLength; // clipLoudness is what you are looking for
        

        return clipLoudness;

    }


}
