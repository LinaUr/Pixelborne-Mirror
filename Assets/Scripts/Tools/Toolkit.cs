using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public static class Toolkit
{
    //Collection of various utility functions

    //Returns current time in Unix format
    public static int currentTime()
    {
        return ((int)(System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds);
    }



}
