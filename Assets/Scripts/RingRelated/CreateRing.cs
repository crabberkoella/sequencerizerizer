using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateRing : InteractableObject
{

    PlayerInteractionController playerInteractionController;

    private void Start()
    {
        playerInteractionController = GameObject.FindObjectOfType<PlayerInteractionController>();
    }
    public override void PrimaryInteractUp(PlayerInteractionController player = null)
    {
        base.PrimaryInteractUp(player);

        playerInteractionController.CreateRing();
    }
}
