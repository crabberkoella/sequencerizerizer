using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring : MonoBehaviour
{

    public Dictionary<int, Note> placedNotes = new Dictionary<int, Note>();

    public Note notePrefab;
    public bool offset = false;

    public List<int> roundsActive = new List<int>();    

    void Update()
    {
        if (roundsActive.Contains(TimeKeeper.roundCounter) == false) // make this smarter xD TO DO
        {
            return;
        }

        if (TimeKeeper.sixteenthPlayedThisFrame && placedNotes.ContainsKey(TimeKeeper.notePlayed))
        {
            placedNotes[TimeKeeper.notePlayed].PlayNote(offset);            
        }
    }

    public void ToggleOffset()
    {
        offset = !offset;
        transform.rotation = (offset ? Quaternion.Euler(0f, 5.625f/2f, 180f) : Quaternion.identity);
    }

    public void CreateNote(int noteID, Vector3 position, InstrumentOption instrumentOption)
    {

        Note newNote = Instantiate(notePrefab, transform, true);
        newNote.transform.position = position;

        newNote.NoteStupidConstructor(this, instrumentOption.key, noteID, instrumentOption.instrumentName);

        placedNotes[noteID] = newNote;
    }

    public void CreateNoteFromSave(int noteID, Vector3 localRingPos, int noteKey, string instrumentName)
    {
        Note newNote = Instantiate(notePrefab, transform, true);
        newNote.transform.localPosition = localRingPos;

        newNote.NoteStupidConstructor(this, noteKey, noteID, instrumentName);

        placedNotes[noteID] = newNote;
    }

}
