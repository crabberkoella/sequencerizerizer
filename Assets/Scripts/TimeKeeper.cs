using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeKeeper : MonoBehaviour
{

    public RoundRep roundRepPrefab;

    public static bool thirtysecondPlayedThisFrame;
    public static bool sixteenthPlayedThisFrame;
    public static int sixteenthCounter = 0;
    public static int thirtysecondCounter = 0;
    public static int beatCounter = 0; // may not need to be static in the end--may not even need beatCounter at all
    public static int roundCounter = 0;
    public static int notePlayed; // basically changing sixteenthCounter and beatCounter to a 0 - 63 number
    public static int numberOfRounds = 0;
    //public static float roundProgress = 10f;
    float timeAtRoundStart;
    float lengthOfRound;
    public static float roundTime;

    float lastThirtysecondTime;
    
    public static bool mute;
    public static bool noRoundsUnmuted;

    float thirtysecondLength = 1f / 16f;

    public Transform timeIndicator; // for some visual feedback on where we are in playback, like the second hand on a clock

    public PlayerInteractionController playerController;

    List<int> mutedRounds = new List<int>();

    AudioSource audioSource;

    public void ToggleMutedRound(int roundNumber)
    {
        if(mutedRounds.Contains(roundNumber))
        {
            mutedRounds.Remove(roundNumber);
        }else
        {
            mutedRounds.Add(roundNumber);
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.pitch = -1f;
        audioSource.time = audioSource.clip.length * 0.95f;

        lengthOfRound = thirtysecondLength * 128f;

        timeAtRoundStart = Time.time;

        CreateRound();
    }
        
    void Update()
    {

        noRoundsUnmuted = (mutedRounds.Count == numberOfRounds ? true : false); // kinda silly, but it works for now -- actually, it's not silly at all
        thirtysecondPlayedThisFrame = false;

        if (noRoundsUnmuted) { return; }

        float time = Time.time;
        roundTime = time - timeAtRoundStart;

        if (roundTime / thirtysecondLength > thirtysecondCounter)
        {
            thirtysecondCounter++;

            thirtysecondPlayedThisFrame = true;

            if (thirtysecondCounter == 128)
            {
                thirtysecondCounter = 0;

                roundCounter += 1;

                timeAtRoundStart += lengthOfRound;

                if (roundCounter >= numberOfRounds)
                {
                    roundCounter = 0;
                }

            }
        }

        /*
        float lengthOfRound = thirtysecondLength * 128f;
        float roundProgress = (thirtysecondLength * thirtysecondCounter) + (time - lastThirtysecondTime);
        roundProgress /= lengthOfRound;

        roundProgress = Mathf.Lerp(-3f, 3f, roundProgress);

        timeIndicator.eulerAngles = new Vector3(0f, 20f + (roundCounter * 6f) + roundProgress, 0f);
        
        
        
        
        if(time - lastThirtysecondTime >= thirtysecondLength)
        {
            thirtysecondCounter += 1;

            thirtysecondPlayedThisFrame = true;

            if (thirtysecondCounter == 128)
            {
                thirtysecondCounter = 0;

                roundCounter += 1;

                timeSinceLastRound = Time.time;

                if (roundCounter >= numberOfRounds)
                {
                    roundCounter = 0;
                }

            }
        }
        */
        while (mutedRounds.Contains(roundCounter))
        {
            roundCounter += 1;
            if (roundCounter == numberOfRounds)
            {
                roundCounter = 0;
            }
        }

        if (Input.GetKeyUp(KeyCode.M))
        {
            //mute = !mute;
        }


    }

    public void CreateRound()
    {
        numberOfRounds += 1;

        RoundRep newRoundRep = Instantiate(roundRepPrefab);
        newRoundRep.roundNumber = numberOfRounds - 1;

        newRoundRep.transform.parent = this.transform;

        newRoundRep.transform.localEulerAngles = new Vector3(0f, 30f + ((numberOfRounds - 1f) * 20f), 0f);

        foreach(Ring ring in playerController.rings)
        {
            ring.NewRoundAdded(numberOfRounds - 1);
        }

        newRoundRep.timeKeeper = this;
    }

    public void RemoveRound(int roundNumberRemoved)
    {
        numberOfRounds -= 1;

        foreach(Ring ring in playerController.rings)
        {
            ring.RoundRemoved(roundNumberRemoved);            
        }

        foreach (RoundRep roundRep in GetComponentsInChildren<RoundRep>())
        {
            if(roundRep.roundNumber > roundNumberRemoved)
            {
                roundRep.roundNumber -= 1;
            }
            roundRep.transform.localEulerAngles = new Vector3(0f, 30f + (roundRep.roundNumber * 20f), 0f);
        }

        mutedRounds.Remove(roundNumberRemoved);

        for (int i = 0; i < mutedRounds.Count; i++)
        {
            if(mutedRounds[i] > roundNumberRemoved)
            {
                mutedRounds[i] = mutedRounds[i] - 1; // I don't know how C# treats lists in this case, so just gonna be explicit
            }
        }

        // take care of the very-important roundCounter
        if (roundNumberRemoved <= roundCounter)
        {
            roundCounter = Mathf.Max(0, roundCounter - 1);
        }

        audioSource.Play();

    }

}