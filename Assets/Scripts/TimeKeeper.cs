using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeKeeper : MonoBehaviour
{

    public RoundRep roundRepPrefab;

    public static bool sixteenthPlayedThisFrame;
    public static int sixteenthCounter = 0;
    public static int beatCounter = 0; // may not need to be static in the end--may not even need beatCounter at all
    public static int roundCounter = 0;
    public static int notePlayed; // basically changing sixteenthCounter and beatCounter to a 0 - 63 number
    public static int numberOfRounds = 1;

    double lastSixteenthTime;
    
    public static bool mute;
    public static bool selectingActiveRounds;

    float sixteenthLength = 1f / 8f;

    public PlayerInteractionController playerController;
    AudioSource audioSource;

    private void Start()
    {
        lastSixteenthTime = AudioSettings.dspTime;
    }
    
    void Update()
    {
        // letting the player reset the time to 0, so you don't have to wait for the whole thing to go around
        if (Input.GetKeyUp(KeyCode.Home))
        {
            sixteenthCounter = -1;
            beatCounter = 0;
        }

        sixteenthPlayedThisFrame = false;

        double time = AudioSettings.dspTime;
        
        if(time - lastSixteenthTime > sixteenthLength)
        {
            sixteenthCounter += 1;
            lastSixteenthTime = time;

            sixteenthPlayedThisFrame = true;

            if (sixteenthCounter == 16)
            {
                sixteenthCounter = 0;

                roundCounter += 1;

                if (roundCounter == numberOfRounds)
                {
                    roundCounter = 0;
                }

            }

            notePlayed = sixteenthCounter;// + (beatCounter * 16);
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

        newRoundRep.transform.parent = this.transform;

        newRoundRep.transform.localEulerAngles = new Vector3(0f, 30f + ((numberOfRounds - 1f) * 20f), 0f);

        foreach(Ring ring in playerController.rings)
        {
            ring.NewRoundAdded(numberOfRounds - 1);
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