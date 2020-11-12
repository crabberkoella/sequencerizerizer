using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeKeeper : MonoBehaviour
{

    public static bool sixteenthPlayedThisFrame;
    public static int sixteenthCounter = 0;
    public static int beatCounter = 0; // may not need to be static in the end--may not even need beatCounter at all
    public static int roundCounter = 0;
    public static int notePlayed; // basically changing sixteenthCounter and beatCounter to a 0 - 63 number
    int numberOfRounds = 1;

    double lastSixteenthTime;
    
    public static bool mute;
    public static bool selectingActiveRounds;

    float sixteenthLength = 1f / 4f;

    public PlayerController playerController;
    AudioSource audioSource;

    public RectTransform activeRoundSelector;

    Ring activeRing;

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

                beatCounter += 1;
                if (beatCounter == 4)
                {
                    beatCounter = 0;
                    roundCounter += 1;

                    if (roundCounter == numberOfRounds)
                    {
                        roundCounter = 0;
                    }
                }
            }

            notePlayed = sixteenthCounter + (beatCounter * 16);
        }

        if (Input.GetKeyUp(KeyCode.M))
        {
            mute = !mute;
        }

        if(selectingActiveRounds)
        {
            if(Input.GetKeyUp(KeyCode.Return))
            {
                selectingActiveRounds = false;

                List<int> newRoundsActive = new List<int>();

                for(int i = 0; i < activeRoundSelector.childCount; i++)
                {
                    if(activeRoundSelector.GetChild(i).GetComponent<Image>().color.a > 0.6f)
                    {
                        newRoundsActive.Add(i);
                    }
                }

                activeRing.roundsActive = newRoundsActive;

                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;

                activeRoundSelector.gameObject.SetActive(false);
            }
        }

        if(Input.GetKeyUp(KeyCode.KeypadPlus))
        {
            numberOfRounds += 1;

            GameObject newRoundElement = Instantiate(activeRoundSelector.GetChild(0).gameObject);
            RectTransform newRoundRect = newRoundElement.GetComponent<RectTransform>();

            newRoundRect.SetParent(activeRoundSelector);
            newRoundRect.SetAsLastSibling();

            newRoundRect.position = activeRoundSelector.GetChild(activeRoundSelector.childCount - 2).GetComponent<RectTransform>().position;
            newRoundRect.position += new Vector3(138f, 0f, 0f);
        }

        if (Input.GetKeyUp(KeyCode.BackQuote))
        {

            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity, 1 << 10))
            {

                activeRing = hit.transform.GetComponentInParent<Ring>();

                selectingActiveRounds = true;

                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                activeRoundSelector.gameObject.SetActive(true);

                for (int i = 0; i < activeRoundSelector.childCount; i++)
                {
                    if (playerController.activeRing.roundsActive.Contains(i))
                    {
                        activeRoundSelector.GetChild(i).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                    }
                    else
                    {
                        activeRoundSelector.GetChild(i).GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f);
                    }
                }
            }

        }

    }

    public void RoundSelectionClicked(Transform indexClicked)
    {
        Image buttonImage = indexClicked.GetComponent<Image>();

        if(buttonImage.color.a > 0.6f)
        {
            buttonImage.color = new Color(1f, 1f, 1f, 0.5f);
        } else
        {
            buttonImage.color = new Color(1f, 1f, 1f, 1f);
        }

    }
}