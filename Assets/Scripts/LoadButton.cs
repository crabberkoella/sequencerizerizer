using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LoadButton : InteractableObject
{

    public LordOfTheRings lotr;
    public int saveSlot;

    private void Start()
    {
        if(File.Exists(Application.persistentDataPath + "/save" + saveSlot.ToString() + ".dat"))
        {
            GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        }
    }

    public override void PrimaryInteractUp(PlayerInteractionController player = null)
    {
        base.PrimaryInteractUp(player);

        if(gameObject.name == "load") // lol so bad
        {
            lotr.LoadFile();
        } else if (gameObject.name == "save")
        {
            lotr.SaveFile();

            lotr.GetComponent<AudioSource>().Play();
        } else
        {
            lotr.saveSlotSelected = saveSlot;
            lotr.slotButtonSelected = this;

            lotr.slotSelector.gameObject.SetActive(true);
            lotr.slotSelector.localPosition = transform.localPosition - new Vector3(0f, 0f, 0.0225f);
        }
    }
}
