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

    // This method returns the time of the animation that is identified by the provided parameter string name.
    public static float GetAnimationLength( Animator animator, string name)
    {
        float time = 0;
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        for(int i = 0; i < ac.animationClips.Length; ++i)
        {
            if(ac.animationClips[i].name == name) 
            {
                time = ac.animationClips[i].length;
            }
        }
        return time;
    }


}
