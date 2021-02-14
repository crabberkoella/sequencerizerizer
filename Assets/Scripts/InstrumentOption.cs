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

    public override void PrimaryInteractUp(PlayerInteractionController player = null)
    {
        base.PrimaryInteractUp(player);

        if (transform.GetComponentInParent<InstrumentPalette>().activeInstrumentOption == this)
        {
            IncrementNote();
        }

        ClickedOn();
    }

    public override void SecondaryInteractUp(PlayerInteractionController player = null)
    {
        base.SecondaryInteractUp(player);

        if (transform.GetComponentInParent<InstrumentPalette>().activeInstrumentOption == this)
        {
            DecrementNote();
        }

        ClickedOn();
    }

    public void ClickedOn()
    {
        transform.parent.GetComponentInParent<InstrumentPalette>().OptionClicked(this);
        PlayNote();
    }
}
