using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
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
