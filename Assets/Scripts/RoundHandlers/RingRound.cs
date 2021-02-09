using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingRound : InteractableObject
{

    public int roundNumber = 0;
    public Ring ownerRing;

    bool active = true;

    public override void PrimaryInteractDown(PlayerInteractionController player = null)
    {
        base.PrimaryInteractDown(player);
    }

    public override void PrimaryInteractUp(PlayerInteractionController player = null)
    {
        base.PrimaryInteractUp(player);

        Toggle();
    }

    public void SetActive(bool activeIn)
    {
        if(active != activeIn)
        {
            Toggle();
        }
    }

    void Toggle()
    {
        active = !active;

        if(active)
        {
            List<int> ownerRingRoundsActive = ownerRing.GetRoundsActive();

            if (!ownerRingRoundsActive.Contains(roundNumber))
            {
                ownerRingRoundsActive.Add(roundNumber);
            }

            ownerRing.SetRoundsActive(ownerRingRoundsActive);
        } else
        {
            List<int> ownerRingRoundsActive = ownerRing.GetRoundsActive();

            if (ownerRingRoundsActive.Contains(roundNumber))
            {
                ownerRingRoundsActive.Remove(roundNumber);
            }

            ownerRing.SetRoundsActive(ownerRingRoundsActive);
            
        }

        // change shader to reflect
        GetComponent<MeshRenderer>().material.SetFloat("_Strength", (active ? 1f : 0f));

    }
}
