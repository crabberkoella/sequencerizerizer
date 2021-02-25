using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundRep : InteractableObject
{

    public int roundNumber;
    public TimeKeeper timeKeeper;

    bool muted = false;

    public override void PrimaryInteractDown(PlayerInteractionController player = null)
    {
        base.PrimaryInteractDown(player);
    }

    public override void PrimaryInteractUp(PlayerInteractionController player = null)
    {
        base.PrimaryInteractUp(player);

        muted = !muted;
        timeKeeper.ToggleMutedRound(roundNumber);

        GetComponent<MeshRenderer>().material.SetFloat("_Strength", (muted ? 0.1f : 1f));

    }

    public override void SecondaryInteractDown(PlayerInteractionController player = null)
    {
        base.SecondaryInteractDown(player);
    }

    public override void SecondaryInteractUp(PlayerInteractionController player = null)
    {
        base.SecondaryInteractUp(player);
        timeKeeper.RemoveRound(roundNumber);

        Destroy(gameObject);
    }

}