using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reticle : MonoBehaviour
{

    float fullScale;

    void Start()
    {
        fullScale = transform.localScale.x;
    }

    public void AnimateOn()
    {
        StartCoroutine(AnimateOn_());
    }

    float animationTime = 1f;

    IEnumerator AnimateOn_()
    {
        float animationTimer = 0f;

        while(animationTimer < animationTime)
        {
            float s = (animationTimer / animationTime) * fullScale;
            transform.localScale = new Vector3(s, s, s);

            animationTimer += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        transform.localScale = new Vector3(fullScale, fullScale, fullScale);

    }
}
