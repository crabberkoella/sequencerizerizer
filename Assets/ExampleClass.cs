using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Basic demonstration of a music system that uses PlayScheduled to preload and sample-accurately
// stitch two AudioClips in an alternating fashion.  The code assumes that the music pieces are
// each 16 bars (4 beats / bar) at a tempo of 140 beats per minute.
// To make it stitch arbitrary clips just replace the line
//   nextEventTime += (60.0 / bpm) * numBeatsPerSegment
// by
//   nextEventTime += clips[flip].length;

[RequireComponent(typeof(AudioSource))]
public class ExampleClass : MonoBehaviour
{
    public float bpm = 140.0f;
    public float numBeatsPerSegment = 16;

    private double nextEventTime;
    private AudioSource[] audioSources;

    double thirtysecondTime = 1f / 8f;

    double normalizedTime = 0f;
    double maxTime = 16f; // 64 16th notes that are each 1/4 second
    double normalizedTimeStart = 0f;

    public GameObject audioSourcesParent;

    void Start()
    {
        audioSources = audioSourcesParent.GetComponentsInChildren<AudioSource>();

        double startTime = AudioSettings.dspTime + 5.0f;

        for (int i = 0; i < 128; i++)
        {
            notes.Add(new TmpNote(startTime + 3f + (thirtysecondTime * i)));
        }

    }

    List<TmpNote> notes = new List<TmpNote>();

    void Update()
    {

        double time = AudioSettings.dspTime;

        normalizedTime = time % maxTime;
        normalizedTimeStart = time - normalizedTime;

        //Debug.Log(normalizedTimeStart);

        for (int i = 0; i < notes.Count; i++)
        {
            if (time + 2f > notes[i].nextPlayTime)
            {
                audioSources[i].PlayScheduled(notes[i].nextPlayTime);
                notes[i].nextPlayTime += maxTime;
            }

        }
    }

    class TmpNote
    {
        public TmpNote(double nextPlayTime_)
        {
            nextPlayTime = nextPlayTime_;
        }

        public double nextPlayTime;
    }
}