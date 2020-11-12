using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring : MonoBehaviour
{

    public Dictionary<int, Note> notes = new Dictionary<int, Note>();

    public Note notePrefab;
    public bool offset = false;

    public List<int> roundsActive = new List<int>();    

    void Update()
    {
        if (roundsActive.Contains(TimeKeeper.roundCounter) == false) // make this smarter xD TO DO
        {
            return;
        }

        if (TimeKeeper.sixteenthPlayedThisFrame && notes.ContainsKey(TimeKeeper.notePlayed))
        {
            notes[TimeKeeper.notePlayed].PlayNote(offset);            
        }
    }

    public void ToggleOffset()
    {
        offset = true;
        transform.rotation = Quaternion.Euler(0f, 5.625f/2f, 180f);
    }

    public void CreateNote(int noteID, Vector3 position, InstrumentOption instrumentOption)
    {
        int key = noteID;

        Note newNote = Instantiate(notePrefab, transform, true);
        newNote.transform.position = position;

        newNote.NoteStupidConstructor(this, key, instrumentOption.noteLevel, instrumentOption.instrumentName);

        notes[key] = newNote;
    }

    public void CreateNoteFromSave(int noteID, Vector3 localRingPos, float noteLevel, string instrumentName)
    {
        Note newNote = Instantiate(notePrefab, transform, true);
        newNote.transform.localPosition = localRingPos;

        newNote.NoteStupidConstructor(this, noteID, noteLevel, instrumentName);

        notes[noteID] = newNote;
    }

}
