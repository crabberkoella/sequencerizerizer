using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundAdder : InteractableObject
{

    public TimeKeeper timeKeeper;

    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public override void PrimaryInteractDown(PlayerInteractionController player = null)
    {
        base.PrimaryInteractDown(player);
    }

    public override void PrimaryInteractUp(PlayerInteractionController player = null)
    {
        base.PrimaryInteractUp(player);

        timeKeeper.CreateRound();

        audioSource.Play();
    }

}
