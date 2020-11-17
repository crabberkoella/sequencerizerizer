using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locator : MonoBehaviour
{
    public bool locatorOn;
    public int lastLocation = -1;
    float fullScale;
    Material material;

    void Start()
    {
        fullScale = transform.localScale.x;
        material = transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material;
        //Color outColor = new Color(1f, 0f, 0f);
        //material.SetColor("_EmissionColor", outColor);
    }

    public void AnimateOn()
    {
        StopAllCoroutines();
        StartCoroutine(AnimateOn_());
        locatorOn = true;
    }

    public void AnimateOff()
    {
        StopAllCoroutines();
        StartCoroutine(AnimateOff_());
        locatorOn = false;
    }

    float animationTime = 0.2f;

    IEnumerator AnimateOn_()
    {
        float animationTimer = 0f;
        Color outColor;

        while(animationTimer < animationTime)
        {
            float progress = animationTimer / animationTime;
            float s = progress * fullScale;

            transform.localScale = new Vector3(s, s, s);
            outColor = new Color(progress, progress, progress);
            material.color = outColor;
            //material.SetColor("_EmissionColor", outColor);

            animationTimer += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        transform.localScale = new Vector3(fullScale, fullScale, fullScale);
        outColor = new Color(1f, 1f, 1f);
        material.color = outColor;
        //material.SetColor("_EmissionColor", outColor);
    }

    IEnumerator AnimateOff_()
    {
        float animationSpeed = 1f;

        while (transform.localScale.x > 0f)
        {
            yield return new WaitForEndOfFrame();

            float deltaScale = animationSpeed * Time.deltaTime;
            transform.localScale -= new Vector3(deltaScale, deltaScale, deltaScale);            
        }

        transform.localScale = Vector3.zero;

    }
}
