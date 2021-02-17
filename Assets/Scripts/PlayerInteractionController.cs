using System.Collections;
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


    // for now just gonna have different sources for different sound effects
    AudioSource footstepAudioSource;
    AudioSource thrusterAudioSource;
    AudioSource snapAudioSource;

    // again, dumb, but fine until we sort out all the asset streaming
    public AudioClip footstepAudioClip;
    public AudioClip thrusterAudioClip;
    public AudioClip snapAudioClip;

    public AudioSource GetFootstepsSource()
    {
        return footstepAudioSource;
    }

    public AudioSource GetThrusterSource()
    {
        return thrusterAudioSource;
    }
    
    void Start()
    {
        CreateRing();

        cam = transform.GetChild(0);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        footstepAudioSource = gameObject.AddComponent<AudioSource>();
        footstepAudioSource.clip = footstepAudioClip;
        footstepAudioSource.loop = true;

        thrusterAudioSource = gameObject.AddComponent<AudioSource>();
        thrusterAudioSource.clip = thrusterAudioClip;
        thrusterAudioSource.loop = true;

        snapAudioSource = gameObject.AddComponent<AudioSource>();
        snapAudioSource.clip = snapAudioClip;

        footstepAudioSource.volume = 0.275f;
        thrusterAudioSource.volume = 0.275f;

    }


    void Update()
    {
        if (!paused)
        {
            
        }

        Interaction();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            paused = true;// !paused;

            Cursor.visible = paused;
            Cursor.lockState = (paused ? CursorLockMode.None : CursorLockMode.Locked);

            instrumentPalette.Toggle();

            transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetBool("point", true);            
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            GetComponent<PlayerMovement>().justUnpaused = true;
            paused = false;// !paused;

            Cursor.visible = paused;
            Cursor.lockState = (paused ? CursorLockMode.None : CursorLockMode.Locked);

            instrumentPalette.Toggle();

            transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetBool("point", false);
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

        // rings are special
        if (activeObject == null && Physics.Raycast(transform.position, cam.forward, out hit, Mathf.Infinity, 1 << 10)) // 10 == Ring; activeObject == null is to (at least temporarily) prevent placing notes when, say, changing Ring Rounds
        {            
            activeRing = hit.transform.GetComponentInParent<Ring>(); // so it's always important the collision object is the child of the ring

            activeRing.GetComponent<MeshRenderer>().material.SetFloat("_CellNumber", (float)hit.collider.gameObject.transform.GetSiblingIndex());

            // figure out which note spot we're hitting
            int cellNumber = hit.collider.gameObject.transform.GetSiblingIndex();

            // drop a note if its empty
            if (Input.GetMouseButtonUp(0))
            {
                if (!paused)
                {

                    if (!activeRing.placedNotes.ContainsKey(cellNumber) && instrumentPalette.activeInstrumentOptionData != null)
                    {
                        NoteData nd = new NoteData();
                        nd.pitch = instrumentPalette.activeInstrumentOptionData.pitch;
                        nd.instrumentName = instrumentPalette.activeInstrumentOptionData.instrumentName;

                        activeRing.CreateNote(nd, cellNumber, hit.collider.transform);
                    }
                    else if (activeRing.placedNotes.ContainsKey(cellNumber))
                    {
                        instrumentPalette.activeInstrumentOptionData = activeRing.placedNotes[cellNumber].noteData;
                        activeRing.placedNotes[cellNumber].PlayNote();
                    }

                }

            }
            else if (Input.GetMouseButtonUp(1))
            {
                if (activeRing.placedNotes.ContainsKey(cellNumber))
                {
                    activeRing.placedNotes[cellNumber].DeleteNote();
                }
            }

        } // end if (looking at ring)

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(transform.position, cam.forward, out hit, Mathf.Infinity, 1 << 15))
            {
                if(activeObject == null)
                {
                    activeObject = hit.transform.GetComponent<InteractableObject>();
                    activeObject.PrimaryInteractDown(this);

                    if(activeObject.GetComponent<Note>() != null)
                    {
                        GetComponent<PlayerMovement>().tweakingNote = true;
                    }
                }                
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (activeObject != null)
            {
                activeObject.PrimaryInteractUp(this);

                if (activeObject.GetComponent<Note>() != null)
                {
                    GetComponent<PlayerMovement>().tweakingNote = false;
                }

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

        if (Input.GetMouseButtonDown(1))
        {
            if (Physics.Raycast(transform.position, cam.forward, out hit, Mathf.Infinity, 1 << 15))
            {
                if (activeObject == null && hit.collider.gameObject.GetComponent<Note>() == null)
                {
                    activeObject = hit.transform.GetComponent<InteractableObject>();
                    activeObject.SecondaryInteractDown(this);
                }
            }
        }
        else if (Input.GetMouseButtonUp(1))
        {
            if (activeObject != null)
            {
                activeObject.SecondaryInteractUp(this);

                activeObject = null;
            }
        }
        else if (Input.GetMouseButton(1))
        {
            if (activeObject != null)
            {
                activeObject.SecondaryInteract(this);
            }
        }

        

        // also need a separate case for the InstrumentPalette
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1<<14))
        {
            if(Input.GetMouseButtonUp(0))
            {
                hit.collider.GetComponent<InteractableObject>().PrimaryInteractUp(this);
            }else if (Input.GetMouseButtonUp(1))
            {
                hit.collider.GetComponent<InteractableObject>().SecondaryInteractUp(this);
            }
            
        }
    }

    public void DestroyRing(Ring ring, bool destroyingAllForLoadGame = false)
    {
        rings.Remove(ring);

        Destroy(ring.ringSpeedController.gameObject);

        Destroy(ring.gameObject);

        if(!destroyingAllForLoadGame)
        {
            StopAllCoroutines();
            StartCoroutine(MoveRingsToPosition());
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
            }
            else
            {
                RingRound newRingRound = newRing.NewRoundAdded(i, false);
            }

        }


        if (Time.realtimeSinceStartup > 3f)
        {
            transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetTrigger("snap");
            StartCoroutine(_Snap());
        }
        

        return newRing;
    }

    // dirty, but temporary
    IEnumerator _Snap()
    {
        yield return new WaitForSeconds(0.33f);

        snapAudioSource.PlayOneShot(snapAudioClip);
    }

    public void LoadNew()
    {

        while(rings.Count > 0)
        {
            Ring farewellRing = rings[rings.Count - 1];
            //rings.Remove(farewellRing);

            DestroyRing(farewellRing, true);
        }

    }

}
