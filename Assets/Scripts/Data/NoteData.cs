using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteData
{
    public string instrumentName;
    public int pitch;

    public NoteData(string nameIn = "", int pitchIn = 0)
    {
        instrumentName = nameIn;
        pitch = pitchIn;
    }
}
