using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class InstrumentOption : MonoBehaviour, IPointerClickHandler
{

    public UnityEvent leftClick;
    public UnityEvent middleClick;
    public UnityEvent rightClick;

    public string instrumentName;
    public AudioClip audioClip;

    public Text keyLabel;
    public Text noteLabel;

    public int key;

    public int highestKey; // for convenience; we use it to know when we should go back to the beginning if we increment the note past this
    // start is G

    public float audioClipStartTime;
    public float pitchShiftAmount;

    public void OnPointerClick(PointerEventData eventData)
    {        
        if (eventData.button == PointerEventData.InputButton.Left)
            leftClick.Invoke();
        else if (eventData.button == PointerEventData.InputButton.Middle)
            middleClick.Invoke();
        else if (eventData.button == PointerEventData.InputButton.Right)
            rightClick.Invoke();
    }

    // Start is called before the first frame update
    void Start()
    {
        leftClick.AddListener(LeftClick);
        rightClick.AddListener(RightClick);
    }

    void LeftClick()
    {
        if(transform.GetComponentInParent<InstrumentPalette>().activeInstrumentOption == this)
        {
            IncrementNote();
        }        

        ClickedOn();
    }

    void RightClick()
    {
        if (transform.GetComponentInParent<InstrumentPalette>().activeInstrumentOption == this)
        {
            DecrementNote();
        }        

        ClickedOn();
    }

    public void IncrementNote()
    {
        if (Input.GetKey(KeyCode.RightShift))
        {
            key += 4;
        }
        else
        {
            key += 1;
        }


        if (key >= highestKey)
        {
            key = key - highestKey;
        }

        CalculateClipStartTimeAndPitchShift();
    }
    public void DecrementNote()
    {
        if (Input.GetKey(KeyCode.RightShift))
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

    public void CalculateClipStartTimeAndPitchShift() // specific enough?
    {
        int octave = key / 12;

        audioClipStartTime = (float)octave * AllInstruments.noteLengths[instrumentName];

        pitchShiftAmount = (float)(key % 12) - 5f;
    }

    public void ClickedOn()
    {
        keyLabel.text = (key).ToString();
        transform.parent.GetComponentInParent<InstrumentPalette>().OptionClicked(this);
    }
}
