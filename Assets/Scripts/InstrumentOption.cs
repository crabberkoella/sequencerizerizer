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
    public float key; // when in the audio file to start playing
    public Text keyLabel;
    public Text noteLabel;

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
        key += AllInstruments.noteLengths[instrumentName];
        float clipLength = audioClip.length;

        if (key >= clipLength)
        {
            key = key - clipLength;
        }
    }

    public void DecrementNote()
    {
        key -= AllInstruments.noteLengths[instrumentName];
        float clipLength = audioClip.length;

        if (key < 0f)
        {
            key = clipLength + key; // because key will be negative (see comment in Note)
        }
    }

    public void ClickedOn()
    {
        keyLabel.text = ((int)(key/ AllInstruments.noteLengths[instrumentName])).ToString();
        transform.parent.GetComponentInParent<InstrumentPalette>().OptionClicked(this);
    }
}
