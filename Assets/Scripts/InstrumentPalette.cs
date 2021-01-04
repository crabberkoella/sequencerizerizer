﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class InstrumentPalette : MonoBehaviour
{
    public AllInstruments allInstruments;

    public RectTransform instrumentOptionPrefab;
    public string activeInstrument;
    public Text activeInstrumentLabel;
    public AudioClip activeInstrumentClip;
    public InstrumentOption activeInstrumentOption;

    // for displaying the options
    Vector2 instrumentOptionStartPos; // where in the screen we put the first option
    float instrumentOptionHeight = 100; // it just is, for now
    float instrumentOptionMargin = 10;
    int numberOfOptionsPerColumn;

    int maxRows = 6;
    //int maxColumns = 2;

    AudioSource audioSource;

    public RectTransform holder;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        holder = GetComponent<RectTransform>().GetChild(0).GetComponent<RectTransform>();

        CalculateScreenDetails();

        CreatePalette();

    }

    void Update()
    {
        
    }

    public void OptionClicked(InstrumentOption instrument)
    {

        activeInstrument = instrument.instrumentName;
        int note = Mathf.FloorToInt(instrument.key / AllInstruments.noteLengths[activeInstrument]) + AllInstruments.instrumentStartingNotes[instrument.instrumentName];
        note = note % 12;

        activeInstrumentLabel.text = instrument.instrumentName + " " + AllInstruments.instrumentNoteIDToName[note];

        audioSource.clip = instrument.audioClip;
        audioSource.time = instrument.key;

        activeInstrumentOption = instrument;
        activeInstrumentClip = instrument.audioClip;

        StopAllCoroutines();
        audioSource.Stop();
        StartCoroutine(PlayNote_());
    }

    IEnumerator PlayNote_()
    {
        audioSource.Play();
        
        yield return new WaitForSeconds(3.95f);

        audioSource.Stop();
    }

    void CreatePalette()
    {
        float xPos = instrumentOptionStartPos.x;
        float yPos = instrumentOptionStartPos.y;

        int tmpcounter = 0;
        foreach (AudioClip audioClip in allInstruments.allStupidAudioClips)
        {

            RectTransform instrumentOptionPrefabClone = Instantiate(instrumentOptionPrefab);

            // placement
            instrumentOptionPrefabClone.position = new Vector3(xPos, yPos, 0f);

            instrumentOptionPrefabClone.SetParent(holder);

            if(tmpcounter == maxRows)
            {
                yPos = instrumentOptionStartPos.y;
                xPos += 700f;
            }
            else
            {
                yPos -= instrumentOptionHeight + instrumentOptionMargin;
            }
            

            // properties; should probably move all this to its Start() <-- TO DO
            InstrumentOption newInstrumentOption = instrumentOptionPrefabClone.GetComponent<InstrumentOption>();
            newInstrumentOption.key = AllInstruments.instrumentStartingNotes[audioClip.name];
            newInstrumentOption.keyLabel.text = ((int)(newInstrumentOption.key / AllInstruments.noteLengths[audioClip.name])).ToString();
            newInstrumentOption.noteLabel.text = audioClip.name;
            newInstrumentOption.instrumentName = audioClip.name;
            newInstrumentOption.audioClip = audioClip;

            tmpcounter++;
        }

        holder.gameObject.SetActive(false);
    }

    void CalculateScreenDetails() // TO DO --> make something for multiple 'screens' of options, like a parent gameobject per screen
    {
        float width = (float)Screen.width;
        float height = (float)Screen.height;        

        float numberOfOptionsPerColumn_ = height - (instrumentOptionHeight * 2f); // bottom margin

        numberOfOptionsPerColumn_ = numberOfOptionsPerColumn_ / (instrumentOptionHeight + instrumentOptionMargin);
        numberOfOptionsPerColumn = Mathf.FloorToInt(numberOfOptionsPerColumn_);

        instrumentOptionStartPos = new Vector2(100f, height - 100f);

    }
}
