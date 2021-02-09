using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring : MonoBehaviour
{

    public Dictionary<int, Note> placedNotes = new Dictionary<int, Note>(); // Dictionary, not a List, because you could have a note at slot 2 and 4 and everything else is empty
    public Note notePrefab; // it's unfortunate using a public parameter like this, but there isn't a great alternative for now
    public RingRound ringRoundPrefab;

    bool offset = false;

    List<int> roundsActive = new List<int>();    

    void Update()
    {
        // we're asking a couple of Contains() every Update to check if a Note should be played, but it keeps things orderly even if it's not hyper-efficient
        if (roundsActive.Contains(TimeKeeper.roundCounter) == false)
        {
            return;
        }

        if (TimeKeeper.sixteenthPlayedThisFrame && placedNotes.ContainsKey(TimeKeeper.notePlayed))
        {
            placedNotes[TimeKeeper.notePlayed].PlayNote(offset);            
        }
    }

    private void Start()
    {

    }

    public RingRound NewRoundAdded(int roundNumber, bool activeIn = false)
    {
        RingRound newRingRound = Instantiate(ringRoundPrefab);

        newRingRound.ownerRing = this;

        newRingRound.transform.parent = transform;
        newRingRound.transform.SetAsLastSibling();

        newRingRound.transform.localEulerAngles = new Vector3(0f, 12f + ((roundNumber - 1f) * 6f), 0f);
        newRingRound.transform.localPosition = new Vector3(0f, .8f, 0f);

        newRingRound.roundNumber = roundNumber;

        newRingRound.SetActive(activeIn);

        return newRingRound;
    }

    public void CreateNote(InstrumentOption instrumentOption, int noteID, Transform location)
    {

        Note newNote = Instantiate(notePrefab, transform, true);
        newNote.transform.position = location.position;
        newNote.transform.rotation = location.rotation;

        newNote.Initialize(instrumentOption.noteData, noteID);

        placedNotes[noteID] = newNote;

        newNote.deleteNoteDelegate += RemoveNote;
    }
    /*
    public void CreateNoteFromSave(int noteID, Vector3 localRingPos, int noteKey, string instrumentName)
    {
        Note newNote = Instantiate(notePrefab, transform, true);
        newNote.transform.localPosition = localRingPos;

        newNote.NoteStupidConstructor(this, noteKey, noteID, instrumentName);

        placedNotes[noteID] = newNote;
    }
    */

    public void ToggleOffset()
    {
        offset = !offset;
        transform.rotation = (offset ? Quaternion.Euler(0f, 5.625f/2f, 180f) : Quaternion.identity); // TO DO make compatible with rotating rings
    }

    public bool IsOffset()
    {
        return offset;
    }

    public List<int> GetRoundsActive()
    {
        return roundsActive;
    }

    public void SetRoundsActive(List<int> roundsActiveIn)
    {
        roundsActive = roundsActiveIn;
        /*
        RingRound[] ringRounds = transform.GetComponentsInChildren<RingRound>(); // TO DO --> very sloppy, here

        for (int i = 0; i < ringRounds.Length; i++)
        {
            Debug.Log(ringRounds[i].transform.GetSiblingIndex());
            if(!roundsActive.Contains(i))
            {
                ringRounds[i].PrimaryInteractUp();
            }
        }
        */
    }

    public void RemoveNote(Note note)
    {
        placedNotes.Remove(note.noteID);
    }
    

}
