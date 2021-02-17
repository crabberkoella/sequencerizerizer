using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundToColorControl : MonoBehaviour
{

    float loudness = 0;
    Color color = new Color();

    public GameObject floor;
    Material mat;

    public static List<Note> playingNotes = new List<Note>();

    // Start is called before the first frame update
    void Start()
    {
        mat = floor.GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        loudness = 0;
        color = new Color(0f, 0f, 0f, 1f);

        foreach(Note note in playingNotes)
        {
            loudness += note.loudness;
            color += note.outColor * loudness;
        }

        mat.SetFloat("_Loudness", loudness);
        mat.SetColor("_Color", color);

    }
}
