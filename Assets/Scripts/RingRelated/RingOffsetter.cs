using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingOffsetter : InteractableObject
{
    public int offsetDirection;

    public override void PrimaryInteractUp(PlayerInteractionController player = null)
    {
        base.PrimaryInteractUp(player);

        //GameObject.FindObjectOfType<PlayerInteractionController>().DestroyRing(GetComponentInParent<RingSpeedController>().ownerRing);

        GetComponentInParent<RingSpeedController>().OffsetNotes(offsetDirection);
    }
}
