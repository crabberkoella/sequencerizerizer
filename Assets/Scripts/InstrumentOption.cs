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
        noteLevel += 2f;
        float clipLength = audioClip.length;

        if (noteLevel >= clipLength)
        {
            noteLevel = clipLength - noteLevel;
        }
    }

    public void DecrementNote()
    {
        noteLevel -= 2f;
        float clipLength = audioClip.length;

        if (noteLevel < 0f)
        {
            noteLevel = clipLength + noteLevel;
        }
    }

    public string instrumentName;
    public AudioClip audioClip;
    public float noteLevel; // also when in the audio file to start playing
    public Text noteLevelLabel;
    public Text noteLabel;

    public void ClickedOn()
    {
        noteLevelLabel.text = ((int)(noteLevel/2f)).ToString();
        transform.parent.GetComponentInParent<InstrumentPalette>().OptionClicked(this);
    }
}
