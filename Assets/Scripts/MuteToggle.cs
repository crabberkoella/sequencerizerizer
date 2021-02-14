using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuteToggle : InteractableObject
{
    public override void PrimaryInteractUp(PlayerInteractionController player = null)
    {
        base.PrimaryInteractUp(player);

        TimeKeeper.mute = !TimeKeeper.mute;

        GetComponent<MeshRenderer>().material.SetFloat("_Strength", (TimeKeeper.mute ? 0f : 1f));
    }
}
