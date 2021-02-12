using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedControllerSlider : InteractableObject
{

    public Transform slider;
    RingSpeedController ringSpeedController;

    private void Start()
    {
        ringSpeedController = GetComponentInParent<RingSpeedController>();
    }

    public override void PrimaryInteractDown(PlayerInteractionController player = null)
    {
        base.PrimaryInteractDown(player);
        MoveSlider();
    }

    public override void PrimaryInteract(PlayerInteractionController player = null)
    {
        base.PrimaryInteract(player);
        MoveSlider();
    }

    public override void PrimaryInteractUp(PlayerInteractionController player = null)
    {
        base.PrimaryInteractUp(player);
        MoveToNearestMark();
        ReportSpeed();
    }


    void MoveSlider()
    {
        RaycastHit hit = new RaycastHit();
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity, 1 << 16)) // 16 == sliderHolder
        {
            Vector3 outPos = transform.localPosition;
            float yPos = ringSpeedController.transform.InverseTransformPoint(hit.point).y;

            yPos = Mathf.Min(1f, yPos);
            yPos = Mathf.Max(-0.5f, yPos);

            outPos.y = yPos;
            transform.localPosition = outPos;
        }
    }

    void MoveToNearestMark()
    {
        float currentYPos = transform.localPosition.y;

        currentYPos = (float)Mathf.Round(currentYPos * 2) / 2f;//Math.Round(value * 2, MidpointRounding.AwayFromZero) / 2

        transform.localPosition = new Vector3(transform.localPosition.x, currentYPos, transform.localPosition.z);
    }

    void ReportSpeed()
    {
        float outSpeed = transform.localPosition.y + 0.5f;

        outSpeed *= 2f;

        ringSpeedController.SetOwnerRingSpeed(Mathf.RoundToInt(outSpeed));
    }

}
