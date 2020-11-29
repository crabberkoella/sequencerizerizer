using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class LordOfTheRings : MonoBehaviour
{
    // saving and loading rings

    public PlayerController playerController;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.F1))
        {
            SaveFile("");
        }

        if (Input.GetKeyUp(KeyCode.F2))
        {
            SaveFile("1");
        }

        if (Input.GetKeyUp(KeyCode.F3))
        {
            SaveFile("2");
        }

        if (Input.GetKeyUp(KeyCode.F4))
        {
            SaveFile("3");
        }

        if (Input.GetKeyUp(KeyCode.F5))
        {
            LoadFile("");
        }

        if (Input.GetKeyUp(KeyCode.F6))
        {
            LoadFile("1");
        }

        if (Input.GetKeyUp(KeyCode.F7))
        {
            LoadFile("2");
        }

        if (Input.GetKeyUp(KeyCode.F8))
        {
            LoadFile("3");
        }
    }

    public void SaveFile(string number)
    {
        string destination = Application.persistentDataPath + "/save" + number + ".dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);

        // we just round up all the rings and record each note's info and a little bit about the ring (offset and rounds active)
        List<List<int>> noteIDs = new List<List<int>>();
        List<List<float>> noteLevels = new List<List<float>>();
        List<List<string>> noteInstruments = new List<List<string>>();
        List<bool> offsets = new List<bool>();
        List<List<int>> roundsActive = new List<List<int>>();

        List<List<float>> xPositions = new List<List<float>>();
        List<List<float>> zPositions = new List<List<float>>();


        foreach (Ring ring in Object.FindObjectsOfType<Ring>())
        {

            List<int> ringNoteIDs = new List<int>();
            List<float> ringNoteLevels = new List<float>();
            List<string> ringNoteInstruments = new List<string>();

            bool ringOffset = ring.offset;

            List<float> xPosses = new List<float>();
            List<float> zPosses = new List<float>();

            foreach (int key in ring.placedNotes.Keys)
            {
                Note note = ring.placedNotes[key];

                ringNoteIDs.Add(key);
                ringNoteLevels.Add(note.startTime); // level and startTime are interchangeable, which can be confusing but it's really convenient when playing the AudioClip
                ringNoteInstruments.Add(note.instrumentName);

                xPosses.Add(note.transform.localPosition.x);
                zPosses.Add(note.transform.localPosition.z);
            }

            noteIDs.Add(ringNoteIDs);
            noteLevels.Add(ringNoteLevels);
            noteInstruments.Add(ringNoteInstruments);
            offsets.Add(ringOffset);
            roundsActive.Add(ring.roundsActive);

            xPositions.Add(xPosses);
            zPositions.Add(zPosses);
        }

        noteIDs.Reverse();
        noteLevels.Reverse();
        noteInstruments.Reverse();
        offsets.Reverse();

        xPositions.Reverse();
        zPositions.Reverse();

        GameData data = new GameData(noteIDs, noteLevels, noteInstruments, offsets, xPositions, zPositions, roundsActive);
        BinaryFormatter bf = new BinaryFormatter();

        bf.Serialize(file, data);
        file.Close();
    }



    public void LoadFile(string number)
    {
        string destination = Application.persistentDataPath + "/save" + number + ".dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
        {
            Debug.LogError("File not found");
            return;
        }

        BinaryFormatter bf = new BinaryFormatter();
        GameData data = (GameData)bf.Deserialize(file);
        file.Close();

        foreach (Ring o in Object.FindObjectsOfType<Ring>())
        {
            Destroy(o.gameObject);
        }

        playerController.LoadNew();

        for (int i = 0; i < data.noteIDs.Count; i++)
        {

            Ring newRing = playerController.CreateRing();

            for (int j = 0; j < data.noteIDs[i].Count; j++)
            {
                Vector3 pos = new Vector3(data.xPositions[i][j], 0f, data.zPositions[i][j]);
                newRing.CreateNoteFromSave(data.noteIDs[i][j], pos, data.noteLevels[i][j], data.noteInstruments[i][j]);
            }

            if (data.offsets[i])
            {
                newRing.ToggleOffset();
            }

            newRing.roundsActive = data.roundsActive[i];

        }


    }
}

[System.Serializable]
public class GameData
{
    public List<List<int>> noteIDs;
    public List<List<float>> xPositions;
    public List<List<float>> zPositions;
    public List<List<float>> noteLevels;
    public List<List<string>> noteInstruments;
    public List<bool> offsets;
    public List<List<int>> roundsActive;

    public GameData(List<List<int>> noteIDs_, List<List<float>> noteLevels_, List<List<string>> noteInstruments_, List<bool> offsets_, List<List<float>> xPositions_, List<List<float>> zPositions_, List<List<int>> roundsActive_)
    {
        noteIDs = noteIDs_;
        noteLevels = noteLevels_;
        noteInstruments = noteInstruments_;
        offsets = offsets_;
        roundsActive = roundsActive_;

        xPositions = xPositions_;
        zPositions = zPositions_;
    }
}
