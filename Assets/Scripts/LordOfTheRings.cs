using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class LordOfTheRings : MonoBehaviour
{
    // saving and loading rings

    public PlayerInteractionController playerController;
    public TimeKeeper timeKeeper; // this is awful, but hard to avoid for now
    InstrumentPalette instrumentPalette;

    public Transform slotSelector;
    public Transform saveSlotHolder;

    public LoadButton slotButtonSelected;

    private void Start()
    {
        instrumentPalette = GameObject.FindObjectOfType<InstrumentPalette>();
    }

    public int saveSlotSelected = -1;

    public void SlotSelected(LoadButton saveSlotButton)
    {
        slotButtonSelected = saveSlotButton;
        saveSlotSelected = saveSlotButton.saveSlot;
    }

    public void SaveFile()
    {
        if (saveSlotSelected < 0) { return; }

        slotButtonSelected.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);

        string destination = Application.persistentDataPath + "/save" + saveSlotSelected.ToString() + ".dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);

        // we just round up all the rings and record each note's info and a little bit about the ring (offset and rounds active)
        List<List<int>> noteIDs = new List<List<int>>();
        List<List<int>> notePitches = new List<List<int>>();
        List<List<string>> noteInstruments = new List<List<string>>();
        List<int> ringSpeeds = new List<int>();

        List<List<List<bool>>> speedZeroRoundsActive = new List<List<List<bool>>>();
        List<List<List<bool>>> speedOneRoundsActive = new List<List<List<bool>>>();
        List<List<List<bool>>> speedTwoRoundsActive = new List<List<List<bool>>>();
        List<List<List<bool>>> speedThreeRoundsActive = new List<List<List<bool>>>();


        foreach (Ring ring in Object.FindObjectsOfType<Ring>())
        {

            List<int> ringNoteIDs = new List<int>();
            List<int> ringNoteKeys = new List<int>();
            List<string> ringNoteInstruments = new List<string>();

            foreach (int key in ring.placedNotes.Keys)
            {
                Note note = ring.placedNotes[key];

                ringNoteIDs.Add(key);
                ringNoteKeys.Add(note.noteData.pitch); // key and startTime are interchangeable, which can be confusing but it's really convenient when playing the AudioClip
                ringNoteInstruments.Add(note.noteData.instrumentName);                
            }

            noteIDs.Add(ringNoteIDs);
            notePitches.Add(ringNoteKeys);
            noteInstruments.Add(ringNoteInstruments);

            speedZeroRoundsActive.Add(ring.speedZeroRoundsActive);
            speedOneRoundsActive.Add(ring.speedOneRoundsActive);
            speedTwoRoundsActive.Add(ring.speedTwoRoundsActive);
            speedThreeRoundsActive.Add(ring.speedThreeRoundsActive);

            ringSpeeds.Add(ring.speed);
        }

        noteIDs.Reverse();
        notePitches.Reverse();
        noteInstruments.Reverse();

        speedZeroRoundsActive.Reverse();
        speedOneRoundsActive.Reverse();
        speedTwoRoundsActive.Reverse();
        speedThreeRoundsActive.Reverse();

        ringSpeeds.Reverse();

        GameData data = new GameData(noteIDs, notePitches, noteInstruments, ringSpeeds, speedZeroRoundsActive, speedOneRoundsActive, speedTwoRoundsActive, speedThreeRoundsActive);
        BinaryFormatter bf = new BinaryFormatter();

        bf.Serialize(file, data);
        file.Close();
    }



    public void LoadFile()
    {

        if(saveSlotSelected < 0) { return; }

        string destination = Application.persistentDataPath + "/save" + saveSlotSelected.ToString() + ".dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
        {
            Debug.Log("File not found");
            return;
        }

        BinaryFormatter bf = new BinaryFormatter();
        GameData data = (GameData)bf.Deserialize(file);
        file.Close();


        playerController.LoadNew(); // destroys all Rings

        
        // first, if for some reason someone saved an empty game
        if(data.ringSpeeds.Count == 0)
        {
            return;
        }

        // destroy all RoundReps
        RoundRep[] roundReps = GameObject.FindObjectsOfType<RoundRep>();

        foreach (RoundRep rr in roundReps)
        {
            Destroy(rr.gameObject);
        }

        TimeKeeper.numberOfRounds = 0;

        // recreate them accordingly
        int numberOfRounds = data.speedOneRoundsActive[0].Count; // grab the number of Rounds from the first Ring to get how many rounds there are

        for(int i = 0; i < numberOfRounds; i++)
        {
            timeKeeper.CreateRound();
        }
                
        // create the Rings
        for (int i = 0; i < data.ringSpeeds.Count; i++)
        {

            Ring newRing = playerController.CreateRing();

            newRing.speedZeroRoundsActive = data.speedZeroRoundsActive[i];
            newRing.speedOneRoundsActive = data.speedOneRoundsActive[i];
            newRing.speedTwoRoundsActive = data.speedTwoRoundsActive[i];
            newRing.speedThreeRoundsActive = data.speedThreeRoundsActive[i];

            newRing.SetSpeed(data.ringSpeeds[i]);

            for(int n = 0; n < data.noteIDs[i].Count; n++)
            {
                NoteData newNoteData = new NoteData();

                newNoteData.pitch = data.notePitches[i][n];
                newNoteData.instrumentName = data.noteInstruments[i][n];

                newRing.CreateNote(newNoteData, data.noteIDs[i][n], newRing.transform.GetChild(data.noteIDs[i][n]));
            }

        }

        //timeKeeper.SetRounds(numberOfRounds);
    }
}

[System.Serializable]
public class GameData
{
    public List<List<int>> noteIDs;
    public List<List<int>> notePitches;
    public List<List<string>> noteInstruments;

    public List<int> ringSpeeds;

    public List<List<List<bool>>> speedZeroRoundsActive;
    public List<List<List<bool>>> speedOneRoundsActive;
    public List<List<List<bool>>> speedTwoRoundsActive;
    public List<List<List<bool>>> speedThreeRoundsActive;

    public GameData(List<List<int>> noteIDs_, List<List<int>> notePitches_, List<List<string>> noteInstruments_, List<int> _ringSpeeds, List<List<List<bool>>> _speedZeroRoundsActive, List<List<List<bool>>> _speedOneRoundsActive, List<List<List<bool>>> _speedTwoRoundsActive, List<List<List<bool>>> _speedThreeRoundsActive)
    {
        noteIDs = noteIDs_;
        notePitches = notePitches_;
        noteInstruments = noteInstruments_;

        ringSpeeds = _ringSpeeds;

        speedZeroRoundsActive = _speedZeroRoundsActive;
        speedOneRoundsActive = _speedOneRoundsActive;
        speedTwoRoundsActive = _speedTwoRoundsActive;
        speedThreeRoundsActive = _speedThreeRoundsActive;
    }
}
