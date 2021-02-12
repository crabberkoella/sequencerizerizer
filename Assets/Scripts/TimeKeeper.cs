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

    double lastThirtysecondTime;
    double lastSixteenthTime;
    
    public static bool mute;
    public static bool selectingActiveRounds;

    float thirtysecondLength = 1f / 16f;
    float sixteenthLength = 1f / 8f;

    public PlayerInteractionController playerController;
    AudioSource audioSource;

    private void Start()
    {
        lastSixteenthTime = AudioSettings.dspTime;
        lastThirtysecondTime = AudioSettings.dspTime;
        CreateRound();
    }
    
    void Update()
    {

        thirtysecondPlayedThisFrame = false;
        sixteenthPlayedThisFrame = false;


        double time = AudioSettings.dspTime;
        
        if(time - lastThirtysecondTime > thirtysecondLength)//lastSixteenthTime > sixteenthLength)
        {
            thirtysecondCounter += 1;
            lastThirtysecondTime = time;

            thirtysecondPlayedThisFrame = true;

            if (thirtysecondCounter == 128)
            {
                thirtysecondCounter = 0;

                roundCounter += 1;

                if (roundCounter >= numberOfRounds)
                {
                    roundCounter = 0;
                }

            }
        }

        if (Input.GetKeyUp(KeyCode.M))
        {
            mute = !mute;
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

    }

    public void SetRounds(int n)
    {
        /*
        numberOfRounds = 1;

        while(activeRoundSelector.childCount > 1)
        {
            Destroy(activeRoundSelector.GetChild(-1).gameObject);
        }

        for(int i = 1; i < n; i++)
        {
            CreateRound();
        }
        */
    }
}