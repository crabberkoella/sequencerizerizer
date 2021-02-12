using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingRoundPiePiece : InteractableObject
{

    RingRound ringRoundOwner;
    int piePieceID; // 0 - 8
    public bool active = false;

    public override void PrimaryInteractUp(PlayerInteractionController player = null)
    {
        base.PrimaryInteractUp(player);

        ringRoundOwner.PieClicked(piePieceID);

        //SetActive(!active);
    }

    public void Initialize(int piePieceIDIn, RingRound ringRoundOwnerIn)
    {
        ringRoundOwner = ringRoundOwnerIn;
        piePieceID = piePieceIDIn;

        //GetComponent<MeshRenderer>().material.SetFloat("_Strength", (active ? 1f : 0f));
    }

    public void SetActive(bool activeIn)
    {
        active = activeIn;
        GetComponent<MeshRenderer>().material.SetFloat("_Strength", (active ? 1f : 0f));
    }


}
