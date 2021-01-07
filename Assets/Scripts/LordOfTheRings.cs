using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class LordOfTheRings : MonoBehaviour
{
    // saving and loading rings

    public PlayerController playerController;
    public TimeKeeper timeKeeper; // I think this is awful

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
        List<List<int>> noteKeys = new List<List<int>>();
        List<List<string>> noteInstruments = new List<List<string>>();
        List<bool> offsets = new List<bool>();
        List<List<int>> roundsActive = new List<List<int>>();

        List<List<float>> xPositions = new List<List<float>>();
        List<List<float>> zPositions = new List<List<float>>();


        foreach (Ring ring in Object.FindObjectsOfType<Ring>())
        {

            List<int> ringNoteIDs = new List<int>();
            List<int> ringNoteKeys = new List<int>();
            List<string> ringNoteInstruments = new List<string>();

            bool ringOffset = ring.offset;

            List<float> xPosses = new List<float>();
            List<float> zPosses = new List<float>();

            foreach (int key in ring.placedNotes.Keys)
            {
                Note note = ring.placedNotes[key];

                ringNoteIDs.Add(key);
                ringNoteKeys.Add(note.key); // key and startTime are interchangeable, which can be confusing but it's really convenient when playing the AudioClip
                ringNoteInstruments.Add(note.instrumentName);

                xPosses.Add(note.transform.localPosition.x);
                zPosses.Add(note.transform.localPosition.z);
            }

            noteIDs.Add(ringNoteIDs);
            noteKeys.Add(ringNoteKeys);
            noteInstruments.Add(ringNoteInstruments);
            offsets.Add(ringOffset);
            roundsActive.Add(ring.roundsActive);

            xPositions.Add(xPosses);
            zPositions.Add(zPosses);
        }

        noteIDs.Reverse();
        noteKeys.Reverse();
        noteInstruments.Reverse();
        offsets.Reverse();
        roundsActive.Reverse();

        xPositions.Reverse();
        zPositions.Reverse();

        GameData data = new GameData(noteIDs, noteKeys, noteInstruments, offsets, xPositions, zPositions, roundsActive);
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

        int highestRound = 0;

        for (int i = 0; i < data.noteIDs.Count; i++)
        {

            Ring newRing = playerController.CreateRing();

            for (int j = 0; j < data.noteIDs[i].Count; j++)
            {
                Vector3 pos = new Vector3(data.xPositions[i][j], 0f, data.zPositions[i][j]);
                newRing.CreateNoteFromSave(data.noteIDs[i][j], pos, data.noteKeys[i][j], data.noteInstruments[i][j]);
            }

            if (data.offsets[i])
            {
                newRing.ToggleOffset();
            }

            newRing.roundsActive = data.roundsActive[i];
            foreach(int r in data.roundsActive[i])
            {
                if (r > highestRound)
                {
                    highestRound = r;
                }
            }
        }

        timeKeeper.SetRounds(highestRound + 1);
    }
}

[System.Serializable]
public class GameData
{
    public List<List<int>> noteIDs;
    public List<List<float>> xPositions;
    public List<List<float>> zPositions;
    public List<List<int>> noteKeys;
    public List<List<string>> noteInstruments;
    public List<bool> offsets;
    public List<List<int>> roundsActive;

    public GameData(List<List<int>> noteIDs_, List<List<int>> noteKeys_, List<List<string>> noteInstruments_, List<bool> offsets_, List<List<float>> xPositions_, List<List<float>> zPositions_, List<List<int>> roundsActive_)
    {
        noteIDs = noteIDs_;
        noteKeys = noteKeys_;
        noteInstruments = noteInstruments_;
        offsets = offsets_;
        roundsActive = roundsActive_;

        xPositions = xPositions_;
        zPositions = zPositions_;
    }
}
