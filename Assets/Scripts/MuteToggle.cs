using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuteToggle : InteractableObject
{

    AudioSource audioSource;
    float clipLength;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        clipLength = audioSource.clip.length;
    }

    public override void PrimaryInteractUp(PlayerInteractionController player = null)
    {
        base.PrimaryInteractUp(player);

        TimeKeeper.mute = !TimeKeeper.mute;

        GetComponent<MeshRenderer>().material.SetFloat("_Strength", (TimeKeeper.mute ? 0.1f : 1f));

        audioSource.time = (TimeKeeper.mute ? clipLength * 0.9f : 0f);
        audioSource.pitch = (TimeKeeper.mute ? -1f : 1f);

        audioSource.Play();
    }
}
