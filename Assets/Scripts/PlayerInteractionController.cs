﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteractionController : MonoBehaviour
{

    public Ring ringPrefab; // deliberately keeping public properties, here, so we don't need to make a prefab factory, which will likely need to be rebuilt for webgl anyway

    Transform cam;

    public InstrumentPalette instrumentPalette; // what might turn into the 'Toolbox'

    public GameObject menu; // TO DO offload to separate class

    public Ring activeRing; // public for the TimeKeeper to access when the player is setting its activeRounds TO DO remove!
    Note activeNote;

    public bool paused = false;

    public List<Ring> rings = new List<Ring>(); // just so we can put them in the right place when one's deleted TO DO (probably) offload this to a ringmaster

    void Start()
    {
        CreateRing();

        cam = transform.GetChild(0);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    List<int> activeRoundsToAssign = new List<int>(); // for the (very temporary) way we assign rings to which rounds to play
    void Update()
    {
        if (!paused)
        {
            if (Input.GetKeyUp(KeyCode.R))
            {
                CreateRing();
            }

            
        }
        Interaction();
        if (Input.GetKeyUp(KeyCode.Space) && !TimeKeeper.selectingActiveRounds)
        {
            paused = !paused;
            Cursor.visible = paused;
            Cursor.lockState = (paused ? CursorLockMode.None : CursorLockMode.Locked);
            //menu.SetActive(paused);
            instrumentPalette.Toggle();
        }

    }

    InteractableObject activeObject;

    // placing/tweaking notes
    void Interaction()
    {
        // kiiiinda dumb, but it works well for now -> if we're not looking at a ring cell, set it to not highlight
        for (int i = 0; i < rings.Count; i++)
        {
            rings[i].GetComponent<MeshRenderer>().material.SetFloat("_CellNumber", -10f);
        }


        RaycastHit hit = new RaycastHit();
        
        activeRing = null;

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(transform.position, cam.forward, out hit, Mathf.Infinity, 1 << 15))
            {
                activeObject = hit.transform.GetComponent<InteractableObject>();
                activeObject.PrimaryInteractDown(this);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (activeObject != null)
            {
                activeObject.PrimaryInteractUp(this);

                activeObject = null;
            }
        }
        else if(Input.GetMouseButton(0))
        {
            if(activeObject != null)
            {
                activeObject.PrimaryInteract(this);
            }
        }

        // first if we're looking at a Note
        if (Physics.Raycast(transform.position, cam.forward, out hit, Mathf.Infinity, 1 << 12)) // 12 == Note
        {
            if(Input.GetMouseButtonDown(0))
            {
                //hit.transform.GetComponent<Note>().PrimaryInteractDown(this);
                //activeObject = hit.transform.GetComponent<Note>();
            }
            
            /*
            // TO DO make compatible with InteractableObject, if possible
            activeNote = hit.transform.GetComponent<Note>();
            int n = (int)(activeNote.noteData.pitch / AllInstruments.noteLengths[activeNote.noteData.instrumentName]);
            n = n % 12;

            // TO DO change to keyboard-less interaction
            if (Input.GetKeyUp(KeyCode.UpArrow)) // move this to the note
            {
                activeNote.IncrementNote();
                activeNote.PlayNote(false, true);
            }

            if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                activeNote.DecrementNote();
                activeNote.PlayNote(false, true);
            }

            if (Input.GetKeyUp(KeyCode.Delete) || Input.GetKeyUp(KeyCode.Backspace))
            {
                activeNote.DeleteNote();
            }
            */

        } // now if we're looking at a Ring (placing a note)
        else if (Physics.Raycast(transform.position, cam.forward, out hit, Mathf.Infinity, 1<<10)) // 10 == Ring
        {

            activeRing = hit.transform.GetComponentInParent<Ring>(); // so it's always important the collision object is the child of the ring

            activeRing.GetComponent<MeshRenderer>().material.SetFloat("_CellNumber", (float)hit.collider.gameObject.transform.GetSiblingIndex());

            // drop a note if its empty
            if (Input.GetMouseButtonUp(0))
            {
                if(instrumentPalette.activeInstrumentOption != null && !paused)
                {
                    // figure out which note spot we're hitting
                    int cellNumber = hit.collider.gameObject.transform.GetSiblingIndex();

                    if (!activeRing.placedNotes.ContainsKey(cellNumber))
                    {
                        activeRing.CreateNote(instrumentPalette.activeInstrumentOption, cellNumber, hit.collider.transform);
                    }
                    
                }
                
            }

            // toggle offset -- change these to 3D buttons
            if (Input.GetKeyUp(KeyCode.O))
            {
                activeRing.ToggleOffset();
            }

            // destroy <-- TO DO: bring every ring down that's above the one destroyed
            if(Input.GetKeyUp(KeyCode.T))
            {
                rings.Remove(activeRing);
                Destroy(activeRing.gameObject);

                StartCoroutine(MoveRingsToPosition());
            }

        } // end if (looking at ring)
        else
        { // if we're not looking at anything (so far)

            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                instrumentPalette.activeInstrumentOption.IncrementNote();
                instrumentPalette.OptionClicked(instrumentPalette.activeInstrumentOption);
            }

            if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                instrumentPalette.activeInstrumentOption.DecrementNote();
                instrumentPalette.OptionClicked(instrumentPalette.activeInstrumentOption);
            }

        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Input.GetMouseButtonUp(0) && Physics.Raycast(ray, out hit, Mathf.Infinity, 1<<14))
        {
            //Debug.Log("hmm");
            //Debug.Log(hit.collider.gameObject);
            hit.collider.GetComponent<InstrumentOption>().PrimaryInteract(this);
        }
    }

    IEnumerator MoveRingsToPosition()
    {

        float animTime = 2f;
        float animTimer = 0f;

        List<Vector3> startPos = new List<Vector3>();

        foreach(Ring ring in rings)
        {
            startPos.Add(ring.transform.position);
        }
        int count = rings.Count;
        while (animTimer < animTime)
        {
            float progress = animTimer / animTime;

            for(int i = 0; i < count; i++)
            {
                rings[i].transform.position = Vector3.Slerp(startPos[i], new Vector3(0f, 1f + (2f * i), 0f), progress);
            }

            yield return new WaitForEndOfFrame();

            animTimer += Time.deltaTime;
        }

        for (int i = 0; i < rings.Count; i++)
        {
            rings[i].transform.position = Vector3.Slerp(startPos[i], new Vector3(0f, 1f + (2f * i), 0f), 1f); // in case it went from ie 0.98f to 1.02f
        }

    }


    public Ring CreateRing()
    {
        Ring newRing = Instantiate(ringPrefab);        

        rings.Add(newRing);

        newRing.transform.position = new Vector3(0f, 1f + (2f * (rings.Count - 1)), 0f);

        List<int> ringRoundsActive = new List<int>();
        for (int i = 0; i < TimeKeeper.numberOfRounds; i++)
        {            

            if (i == TimeKeeper.numberOfRounds - 1)
            {
                ringRoundsActive.Add(i);
                RingRound newRingRound = newRing.NewRoundAdded(i, true);                
            }else
            {
                RingRound newRingRound = newRing.NewRoundAdded(i, false);
            }
            
        }
        
        //ringsRoundsActive.Add(TimeKeeper.numberOfRounds - 1); // by default we'll just have it active on the last round :shrug:

        newRing.SetRoundsActive(ringRoundsActive);
        
        return newRing;
    }

    public void LoadNew()
    {
        rings.Clear();
    }

}
