﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    Transform cam;

    public InstrumentPalette instrumentPalette;
    public Text noteIDText;
    public Text noteNameText;
    public Locator noteLocator;

    public GameObject menu;

    public Ring ringPrefab;    

    public Ring activeRing; // public for the TimeKeeper to access when the player is setting its activeRounds
    Note activeNote;

    public bool paused = false;

    List<Ring> rings = new List<Ring>(); // just so we can put them in the right place when one's deleted

    void Start()
    {
        cam = transform.GetChild(0);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        rings.Add(GameObject.FindObjectOfType<Ring>());
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

            Interaction();
        }

        if (Input.GetKeyUp(KeyCode.Space) && !TimeKeeper.selectingActiveRounds)
        {
            paused = !paused;
            Cursor.visible = paused;
            Cursor.lockState = (paused ? CursorLockMode.None : CursorLockMode.Locked);
            menu.SetActive(paused);
        }

    }

    // placing/tweaking notes
    void Interaction()
    {
        RaycastHit hit = new RaycastHit();
        
        activeRing = null;

        // first if we're looking at a Note
        if (Physics.Raycast(transform.position, cam.forward, out hit, Mathf.Infinity, 1 << 12)) // 12 == Note
        {
            activeNote = hit.transform.GetComponent<Note>();
            int n = (int)(activeNote.key / AllInstruments.noteLengths[activeNote.instrumentName]);
            n = n % 12;
            noteIDText.text = AllInstruments.instrumentNoteIDToName[n]; // this turns the key into an A to G#
            noteNameText.text = activeNote.instrumentName;

            if (Input.GetKeyUp(KeyCode.UpArrow))
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
                noteIDText.text = "";
            }

            if(noteLocator.locatorOn)
            {
                noteLocator.AnimateOff();
            }

        } // now if we're looking at a Ring (placing a note)
        else if (Physics.Raycast(transform.position, cam.forward, out hit, Mathf.Infinity, 1<<10)) // 10 == Ring
        {

            activeRing = hit.transform.GetComponentInParent<Ring>(); // so it's always important the collision object is the child of the ring


            // figure out which note spot we're hitting
            int cellNumber = hit.triangleIndex;
            cellNumber = Mathf.FloorToInt((float)cellNumber / 2f);

            List<Vector3> noteSpots = CalculateNoteSpots(hit);

            int spotID = 0;
            Vector3 closestSpotPos = new Vector3();
            float closestDistance = Mathf.Infinity;
            for (int i = 0; i < noteSpots.Count; i++)
            {
                float distance = (hit.point - noteSpots[i]).magnitude;
                if (distance < closestDistance)
                {
                    closestSpotPos = noteSpots[i];
                    closestDistance = distance;
                    spotID = i;
                }
            }

            noteLocator.transform.position = closestSpotPos;
            noteLocator.transform.position += (hit.normal * 0.025f);
            var normal = hit.normal;
            noteLocator.transform.rotation = Quaternion.FromToRotation(Vector3.up, normal);// * transform.rotation;


            // drop a note if its empty
            if (Input.GetMouseButtonUp(0))
            {
                if(instrumentPalette.activeInstrumentOption != null && !paused)
                {
                    CreateNote(cellNumber, spotID, closestSpotPos, instrumentPalette.activeInstrumentOption);
                }
                
            }

            // toggle offset
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

            if(noteLocator.lastLocation != (cellNumber * 4) + spotID)
            {
                noteLocator.AnimateOn();
                noteLocator.lastLocation = (cellNumber * 4) + spotID;
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

            if(noteLocator.locatorOn)
            {
                noteLocator.AnimateOff();
            }

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

    void CreateNote(int cellNumber, int spotID, Vector3 position, InstrumentOption instrumentOption)
    {
        int noteID = (cellNumber * 4) + (spotID);
        activeRing.CreateNote(noteID, position, instrumentOption);
    }


    public Ring CreateRing()
    {
        Ring newRing = Instantiate(ringPrefab);

        newRing.transform.position = new Vector3(0f, 1f + (2f * rings.Count), 0f);

        rings.Add(newRing);

        return newRing;
    }

    public void LoadNew()
    {
        rings.Clear();
    }

    // probably won't even use this, in the final version; didn't take too long, though
    List<Vector3> CalculateNoteSpots(RaycastHit hit)
    {

        List<Vector3> output = new List<Vector3>();

        MeshFilter meshFilter = hit.collider.gameObject.GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;

        var localToWorld = hit.collider.transform.localToWorldMatrix;

        int triA = hit.triangleIndex;
        int triB;
        if (triA % 2 == 1)
        {
            triB = triA;
            triA -= 1;
        }
        else
        {
            triB = triA + 1; // so that A is always the even, lower value (so we can go from 'left to right' when finding the spots)
        }

        List<int> triAVerts = new List<int>();
        List<int> triBVerts = new List<int>();

        for (int i = 0; i < 3; i++)
        {
            triAVerts.Add(mesh.triangles[triA * 3 + i]);
            triBVerts.Add(mesh.triangles[triB * 3 + i]);
        }

        // find the vert only triA has, which will be the 'bottom-left' one
        int uniqueAVert = 0;        
        for (int i = 0; i < 3; i++)
        {
            if (triBVerts.Contains(triAVerts[i]))
            {
                continue;
            }
            uniqueAVert = i;
        }

        // find the vert 'above' it
        int topAVert = 0;
        for (int i = 0; i < 3; i++)
        {
            if (i == uniqueAVert || Mathf.Abs(mesh.vertices[triAVerts[i]].y - mesh.vertices[triAVerts[uniqueAVert]].y) < 0.000001f)
            {
                continue;
            }
            topAVert = i;

        }

        Vector3 leftPosition = (mesh.vertices[triAVerts[uniqueAVert]] + mesh.vertices[triAVerts[topAVert]]) / 2f;

        // now do the same for the 'right' side

        // find the vert only triB has, which will be the 'bottom-left' one
        int uniqueBVert = 0;
        for (int i = 0; i < 3; i++)
        {
            if (triAVerts.Contains(triBVerts[i]))
            {
                continue;
            }
            uniqueBVert = i;
        }

        // find the vert 'above' it
        int topBVert = 0;
        for (int i = 0; i < 3; i++)
        {
            if (i == uniqueBVert || Mathf.Abs(mesh.vertices[triBVerts[i]].y - mesh.vertices[triBVerts[uniqueBVert]].y) < 0.000001f)
            {
                continue;
            }
            topBVert = i;
        }

        Vector3 rightPosition = (mesh.vertices[triBVerts[uniqueBVert]] + mesh.vertices[triBVerts[topBVert]]) / 2f;

        // now to world space
        leftPosition = localToWorld.MultiplyPoint3x4(leftPosition);
        rightPosition = localToWorld.MultiplyPoint3x4(rightPosition);

        output.Add(Vector3.Lerp(rightPosition, leftPosition, 0.2f)); // we switched them around, whoops
        output.Add(Vector3.Lerp(rightPosition, leftPosition, 0.4f));
        output.Add(Vector3.Lerp(rightPosition, leftPosition, 0.6f));
        output.Add(Vector3.Lerp(rightPosition, leftPosition, 0.8f));
        output.Reverse();
        return output;

    }

}
