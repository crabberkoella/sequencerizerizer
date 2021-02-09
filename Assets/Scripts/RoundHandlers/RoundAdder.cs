using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundAdder : InteractableObject
{

    public TimeKeeper timeKeeper;

    public override void PrimaryInteractDown(PlayerInteractionController player = null)
    {
        base.PrimaryInteractDown(player);
    }

    public override void PrimaryInteractUp(PlayerInteractionController player = null)
    {
        base.PrimaryInteractUp(player);

        timeKeeper.CreateRound();
    }

}
