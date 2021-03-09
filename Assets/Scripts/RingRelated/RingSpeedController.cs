using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingSpeedController : MonoBehaviour
{

    public Ring ownerRing;

    public void Initialize(Ring ownerRingIn)
    {
        ownerRing = ownerRingIn;

        transform.position = ownerRing.transform.position + new Vector3(-3.15f, 0f, -15.5f);
        transform.rotation = Quaternion.Euler(0f, 15f, 0f);
    }

    public void SetOwnerRingSpeed(int speed)
    {
        ownerRing.SetSpeed(speed);
    }

    public void OffsetNotes(int offsetDirection)
    {
        ownerRing.OffsetNotes(offsetDirection);
    }
}
