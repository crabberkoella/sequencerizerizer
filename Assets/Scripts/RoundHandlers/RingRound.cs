using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingRound : MonoBehaviour
{

    public int roundNumber = 0;
    public Ring ownerRing;

    public List<RingRoundPiePiece> ringRoundPiePieces = new List<RingRoundPiePiece>();

    public void SetActive(bool activeIn)
    {
        if(ringRoundPiePieces.Count == 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                ringRoundPiePieces.Add(transform.GetChild(i).GetComponent<RingRoundPiePiece>());
                ringRoundPiePieces[i].Initialize(i, this);
            }
        }        

        for (int i = 0; i < ringRoundPiePieces.Count; i++)
        {
            ringRoundPiePieces[i].SetActive(activeIn);
        }
    }

    public void PieClicked(int pieNumber)
    {
        ownerRing.PieClicked(roundNumber, pieNumber);
    }

}
