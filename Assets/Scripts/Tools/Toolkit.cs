using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class contains various miscellaneous utility methods for other classes.
public static class Toolkit
{
    // This method returns the current time in the unix format.
    public static int CurrentTime()
    {
        return ((int)(System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds);
    }

    // This method returns the number of milliseconds that passed since the start of the current day.
    public static int CurrentTimeMillisecondsToday()
    {
        return ((int)(System.DateTime.UtcNow.Subtract(System.DateTime.Today)).TotalMilliseconds);
    }
}
