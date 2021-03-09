﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring : MonoBehaviour
{

    public Dictionary<int, Note> placedNotes = new Dictionary<int, Note>(); // Dictionary, not a List, because you could have a note at slot 2 and 4 and everything else is empty
    public Note notePrefab; // it's unfortunate using a public parameter like this, but there isn't a great alternative for now
    public RingRound ringRoundPrefab;
    public RingSpeedController ringSpeedControllerPrefab;

    public RingSpeedController ringSpeedController;

    public int speed = 2;

    // genius, or the dumbest thing ever built?
    public List<List<bool>> speedZeroRoundsActive;
    public List<List<bool>> speedOneRoundsActive;
    public List<List<bool>> speedTwoRoundsActive;
    public List<List<bool>> speedThreeRoundsActive;

    void Update()
    {

        int notePlayed = -100;
        int thirtySecond = TimeKeeper.thirtysecondCounter;

        if(TimeKeeper.mute || TimeKeeper.noRoundsUnmuted) { return; }

        switch (speed)
        {
            case 0:
                if (thirtySecond % 8 == 0)
                {
                    notePlayed = thirtySecond / 8;
                }

                if (TimeKeeper.thirtysecondPlayedThisFrame && placedNotes.ContainsKey(notePlayed) && speedZeroRoundsActive[TimeKeeper.roundCounter][0]) // notePlayed == 16th counter (old)
                {
                    placedNotes[notePlayed].PlayNote();
                }

                break;
            case 1:
                if (thirtySecond % 4 == 0)
                {
                    notePlayed = (thirtySecond % 64) / 4;
                }

                if (TimeKeeper.thirtysecondPlayedThisFrame && placedNotes.ContainsKey(notePlayed) && speedOneRoundsActive[TimeKeeper.roundCounter][thirtySecond / 64]) // notePlayed == 16th counter (old)
                {
                    placedNotes[notePlayed].PlayNote();
                }
                break;
            case 2:
                if (thirtySecond % 2 == 0) // to prevent 16th's getting played twice, because of the way int's divide by 2
                {                    
                    notePlayed = (thirtySecond % 32) / 2;
                }

                if (TimeKeeper.thirtysecondPlayedThisFrame && placedNotes.ContainsKey(notePlayed) && speedTwoRoundsActive[TimeKeeper.roundCounter][thirtySecond / 32]) // notePlayed == 16th counter (old)
                {
                    placedNotes[notePlayed].PlayNote();
                }
                break;
            case 3:
                notePlayed = thirtySecond % 16;

                if (TimeKeeper.thirtysecondPlayedThisFrame && placedNotes.ContainsKey(notePlayed) && speedThreeRoundsActive[TimeKeeper.roundCounter][thirtySecond / 16]) // notePlayed == 16th counter (old)
                {
                    placedNotes[notePlayed].PlayNote();
                }
                break;
        }
    }

    public void PieClicked(int round, int microRound) // when rightClicked we'll toggle all of them
    {
        switch (speed)
        {
            case 0:
                speedZeroRoundsActive[round][0] = !speedZeroRoundsActive[round][0];
                break;
            case 1:
                speedOneRoundsActive[round][microRound / 4] = !speedOneRoundsActive[round][microRound / 4];
                break;
            case 2:
                speedTwoRoundsActive[round][microRound / 2] = !speedTwoRoundsActive[round][microRound / 2];
                break;
            case 3:
                speedThreeRoundsActive[round][microRound] = !speedThreeRoundsActive[round][microRound];
                break;
        }

        SetSpeed(speed); // to update the piePieces
    }

    public void PieRightClicked(int round, bool activeIn)
    {
        switch (speed)
        {
            case 0:
                speedZeroRoundsActive[round][0] = activeIn;
                break;
            case 1:
                for (int i = 0; i < 2; i++)
                {
                    speedOneRoundsActive[round][i] = activeIn;
                }
                
                break;
            case 2:
                for (int i = 0; i < 4; i++)
                {
                    speedTwoRoundsActive[round][i] = activeIn;
                }
                break;
            case 3:
                for (int i = 0; i < 8; i++)
                {
                    speedThreeRoundsActive[round][i] = activeIn;
                }
                break;
        }

        SetSpeed(speed);
    }

    private void Start()
    {
        ringSpeedController = Instantiate(ringSpeedControllerPrefab);
        ringSpeedController.Initialize(this);

        ringSpeedController.transform.parent = transform;
        ringSpeedController.transform.SetAsLastSibling(); // not for any particular reason
    }

    List<RingRound> ringRounds = new List<RingRound>();

    public void SetSpeed(int speedIn)
    {
        speed = speedIn;

        switch (speed)
        {
            case 0:
                for (int i = 0; i < ringRounds.Count; i++)
                {
                    foreach(RingRoundPiePiece piePiece in ringRounds[i].ringRoundPiePieces)
                    {
                        piePiece.SetActive(speedZeroRoundsActive[i][0]);
                    }
                }
                break;
            case 1:
                for (int i = 0; i < ringRounds.Count; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        ringRounds[i].ringRoundPiePieces[j].SetActive(speedOneRoundsActive[i][j / 4]);
                    }
                }
                break;
            case 2:
                for (int i = 0; i < ringRounds.Count; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        ringRounds[i].ringRoundPiePieces[j].SetActive(speedTwoRoundsActive[i][j / 2]);
                    }
                }
                break;
            case 3:
                for (int i = 0; i < ringRounds.Count; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        ringRounds[i].ringRoundPiePieces[j].SetActive(speedThreeRoundsActive[i][j]);
                    }
                }
                break;
        }
    }

    public RingRound NewRoundAdded(int roundNumber, bool activeIn = false)
    {
        RingRound newRingRound = Instantiate(ringRoundPrefab);

        newRingRound.ownerRing = this;

        newRingRound.transform.parent = transform;
        newRingRound.transform.SetAsLastSibling();

        newRingRound.transform.localEulerAngles = new Vector3(0f, 20f + (roundNumber * 6f), 0f);
        newRingRound.transform.localPosition = new Vector3(0f, .8f, 0f);

        newRingRound.roundNumber = roundNumber;
        
        if (speedZeroRoundsActive == null)
        {
            speedZeroRoundsActive = new List<List<bool>>();
            speedOneRoundsActive = new List<List<bool>>();
            speedTwoRoundsActive = new List<List<bool>>();
            speedThreeRoundsActive = new List<List<bool>>();
        }

        speedZeroRoundsActive.Add(new List<bool>() { activeIn });
        speedOneRoundsActive.Add(new List<bool>() { activeIn, activeIn });
        speedTwoRoundsActive.Add(new List<bool>() { activeIn, activeIn, activeIn, activeIn });
        speedThreeRoundsActive.Add(new List<bool>() { activeIn, activeIn, activeIn, activeIn, activeIn, activeIn, activeIn, activeIn });

        newRingRound.SetActive(activeIn);
        ringRounds.Add(newRingRound);

        return newRingRound;
    }

    public void RoundRemoved(int roundNumberRemoved)
    {
        RingRound roundToDestroy = null;

        foreach(RingRound ringRound in ringRounds)
        {
            if(ringRound.roundNumber < roundNumberRemoved)
            {
                continue;
            }else if (ringRound.roundNumber > roundNumberRemoved)
            {
                ringRound.roundNumber -= 1;
                ringRound.transform.localEulerAngles = new Vector3(0f, 20f + (ringRound.roundNumber * 6f), 0f);
            }
            else
            {
                roundToDestroy = ringRound;
                
            }

        }
        ringRounds.Remove(roundToDestroy);

        speedZeroRoundsActive.RemoveAt(roundNumberRemoved);
        speedOneRoundsActive.RemoveAt(roundNumberRemoved);
        speedTwoRoundsActive.RemoveAt(roundNumberRemoved);
        speedThreeRoundsActive.RemoveAt(roundNumberRemoved);

        Destroy(roundToDestroy.gameObject);

    }

    public void CreateNote(NoteData instrumentOptionData, int noteID, Transform location)
    {

        Note newNote = Instantiate(notePrefab, transform, true);
        newNote.transform.position = location.position;
        newNote.transform.rotation = location.rotation;
        
        newNote.Initialize(instrumentOptionData, noteID);

        placedNotes[noteID] = newNote;

    }

    public void RemoveNote(Note note)
    {
        placedNotes.Remove(note.noteID);
    }
    

}