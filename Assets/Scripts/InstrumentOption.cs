using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class InstrumentOption : Note
{

    void Start()
    {

    }

    public override void PrimaryInteract(PlayerInteractionController player = null)
    {

        if(transform.GetComponentInParent<InstrumentPalette>().activeInstrumentOption == this)
        {
            IncrementNote();
        }        

        ClickedOn();
    }

    public override void SecondaryInteract(PlayerInteractionController player = null)
    {
        if (transform.GetComponentInParent<InstrumentPalette>().activeInstrumentOption == this)
        {
            DecrementNote();
        }

        ClickedOn();
    }

    public void ClickedOn()
    {
        transform.parent.GetComponentInParent<InstrumentPalette>().OptionClicked(this);
        PlayNote(false, true);
    }
}
