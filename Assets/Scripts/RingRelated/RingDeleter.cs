using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingDeleter : InteractableObject
{
    // fairly temporary helper class

    public override void PrimaryInteractUp(PlayerInteractionController player = null)
    {
        base.PrimaryInteractUp(player);

        GameObject.FindObjectOfType<PlayerInteractionController>().DestroyRing(GetComponentInParent<RingSpeedController>().ownerRing);
    }
}
