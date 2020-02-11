using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public static float GetAnimationLength(Animator animator, string name)
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
    // directory and all subdirectories. Additionally, it writes and saves 
    // the found file paths in an extra file only if this file does not exist 
    // to prevent duplicates after restart of the game (source:
    // https://docs.microsoft.com/en-us/previous-versions/aa735748(v=vs.71)?redirectedfrom=MSDN).
    // Access to certain paths can be denied, so using Directory.GetFiles() could cause exceptions.
    // Therefore, implementing recursion ourselves is the best way to avoid those exceptions.
    // See https://social.msdn.microsoft.com/Forums/vstudio/en-US/ae61e5a6-97f9-4eaa-9f1a-856541c6dcce/directorygetfiles-gives-me-access-denied?forum=csharpgeneral
    public static List<string> GetFiles(string root, List<string> fileExtensions)
    {
        List<string> fileList = new List<string>();

        Stack<string> pending = new Stack<string>();
        pending.Push(root);

        string logFile;
        if (fileExtensions.Contains("mp3"))
        {
            logFile = "AudioFilePaths.txt";
        } else
        {
            logFile = "ImageFilePaths.txt";
        }
        string logFilePath = Path.Combine(Directory.GetCurrentDirectory(), logFile);

        using (StreamWriter writer = new StreamWriter(logFilePath))
        {
            while (pending.Count != 0)
            {
                string currentPath = pending.Pop();
                string[] next = null;

                try
                {
                    next = Directory.GetFiles(currentPath, "*.*", SearchOption.TopDirectoryOnly)
                                    .Where(fileName => fileExtensions.Any(extension =>
                                            fileName.ToLower().EndsWith($".{extension}"))).ToArray();
                }
                catch { }

                if (next != null && next.Length != 0)
                {
                    foreach (string file in next)
                    {
                        fileList.Add(file);
                        writer.WriteLine(file);
                    }
                }
                
                try
                {
                    next = Directory.GetDirectories(currentPath);
                    foreach (string subdir in next) pending.Push(subdir);
                }
                catch { }
            }
        }
        return fileList;
    }

    // This method logs a message with a given writer.
    // See: https://docs.microsoft.com/de-de/dotnet/standard/io/how-to-open-and-append-to-a-log-file
    public static void Log(string logMessage, TextWriter writer)
    {
        writer.Write("\r\nLog Entry : ");
        writer.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
        writer.WriteLine($"  :{logMessage}");
        writer.WriteLine("-------------------------------");
    }
}
