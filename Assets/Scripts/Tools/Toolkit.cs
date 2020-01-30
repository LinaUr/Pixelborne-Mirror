using System.Collections;
using System.Collections.Generic;
using System.IO;
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

    // This method returns all file paths for files with a certain fileEnding in the root
    // directory and all subdirectories.
    // Access to certain paths can be denied, so using Directory.GetFiles() could cause exceptions.
    // Therefore, implementing recursion ourselves is the best way to avoid those exceptions.
    // (see https://social.msdn.microsoft.com/Forums/vstudio/en-US/ae61e5a6-97f9-4eaa-9f1a-856541c6dcce/directorygetfiles-gives-me-access-denied?forum=csharpgeneral )
    public static List<string> GetFiles(string root, string fileEnding)
    {
        List<string> fileList = new List<string>();

        Stack<string> pending = new Stack<string>();
        pending.Push(root);
        while (pending.Count != 0)
        {
            string path = pending.Pop();
            string[] next = null;
            try
            {
                next = Directory.GetFiles(path, fileEnding);
            }
            catch { }
            if (next != null && next.Length != 0)
                foreach (string file in next) fileList.Add(file);
            try
            {
                next = Directory.GetDirectories(path);
                foreach (string subdir in next) pending.Push(subdir);
            }
            catch { }
        }
        return fileList;
    }

}
