using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class InstrumentPalette : MonoBehaviour
{
    public AllInstruments allInstruments;

    public InstrumentOption instrumentOptionPrefab;

    public InstrumentOption activeInstrumentOption;

    public Transform instrumentOptionHolder;

    public NoteData activeInstrumentOptionData;

    // for displaying the options
    Vector3 instrumentOptionStartPos = new Vector3(0.0742f, -0.0026f, 0.0758f); // where in the holder we put the first option
    float instrumentOptionHeight = .03f; // should get this programmatically, later
    float instrumentOptionMargin = .00043f;

    int maxRows = 3;

    void Start()
    {

        CreatePalette();

    }

    void Update()
    {
        
    }

    public void OptionClicked(InstrumentOption instrument)
    {
        activeInstrumentOption = instrument;
        activeInstrumentOptionData = instrument.noteData;
    }

    public void Toggle()
    {
        Animator animator = GetComponent<Animator>();
        animator.SetBool("open", !animator.GetBool("open"));
    }

    void CreatePalette()
    {
        float xPos = instrumentOptionStartPos.x;
        float zPos = instrumentOptionStartPos.z; // it's Z :shrug:

        int tmpcounter = 0;
        foreach (AudioClip audioClip in allInstruments.allStupidAudioClips)
        {

            InstrumentOption instrumentOptionPrefabClone = Instantiate(instrumentOptionPrefab);
            instrumentOptionPrefabClone.transform.SetParent(instrumentOptionHolder);

            instrumentOptionPrefabClone.transform.localPosition = new Vector3(xPos, instrumentOptionStartPos.y, zPos);
            instrumentOptionPrefabClone.transform.localRotation = Quaternion.Euler(Vector3.zero);

            if (tmpcounter == maxRows - 1)
            {
                zPos = instrumentOptionStartPos.z;
                xPos -= instrumentOptionHeight; // height because it's square (for now)
            }
            else
            {
                zPos -= instrumentOptionHeight + instrumentOptionMargin;
            }
            

            InstrumentOption newInstrumentOption = instrumentOptionPrefabClone.GetComponent<InstrumentOption>();
            NoteData noteData = new NoteData(audioClip.name, AllInstruments.instrumentStartingNotes[audioClip.name]);

            newInstrumentOption.Initialize(noteData, -1); // -1 is noteID, but instrumentOptions don't use it

            instrumentOptionPrefabClone.gameObject.GetComponent<MeshRenderer>().material.SetTexture("_EmissionMap", allInstruments.instrumentNoteTextures[audioClip.name]);

            tmpcounter++;
        }

    }
}
