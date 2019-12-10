using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using System.Threading.Tasks;


/** NOTE: 
 * [..don't know where to exactly put it yet]
 * 
 * Unity is not Thread safe, so they decided to make it impossible 
 * to call their API from another Thread by adding a mechanism to 
 * throw an exception when its API is used from another Thread.
 * (see https://stackoverflow.com/questions/41330771/use-unity-api-from-another-thread-or-call-a-function-in-the-main-thread )
 * 
 * That's why I can't run ChangeBackgroundAudio() in a new Task
 * (or anything inside it really), 
 * because the an Exception like "Xxx() can only be called from the main thread".
 */

public class BackgroundMusic : MonoBehaviour
{
    private AudioSource audioPlayer;
    private AudioClip audio = null;
    private List<string> playlist = new List<string>(); // contains paths to mp3 files on drive
    private float elapsedTime = 0;

    async void Start()
    {
        audioPlayer = GetComponent<AudioSource>();

        // Path to windows user folder of current user regardless of the username.
        var userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        
        // Running GetFiles() asynchronously in new task prevents game from stopping/pausing
        // when starting a new game.
        playlist = await Task.Run(() => GetFiles(userPath, "*.mp3"));

        StartCoroutine(ChangeBackgroundAudio());
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        if(audio == null)
            return;

        if (elapsedTime >= audio.length)
        {
            elapsedTime -= audio.length;
            StartCoroutine(ChangeBackgroundAudio());
        }

    }


    /// <summary>This method picks a random file from the playlist, converts the
    /// mp3 file to wav, assigns it to the audioPlayer and plays it.
    /// </summary>
    /// NOTE: It can currently only handle mp3 files.
    IEnumerator ChangeBackgroundAudio()
    {
        int index = UnityEngine.Random.Range(0, playlist.Count);

        // TODO: prevent game from stopping/lagging somewhere in this method when changing audio
        if (playlist.Count != 0)
        {
            WWW request = new WWW("file://" + playlist[index]);
            while (!request.isDone)
                yield return 0;

            audio = NAudioPlayer.FromMp3Data(request.bytes);
            audio.name = playlist[index];
            audioPlayer.clip = audio;
            audioPlayer.Play();
        }
        else
        {
            Debug.Log("No mp3-files were found. Can't play any background audio.");
        }
    }

    /// <summary>This method returns all file paths for files with a certain <paramref name="fileEnding"/>
    /// in the <paramref name="root"/> directory and all subdirectories.
    /// Access to certain paths or folders can be denied and so using Directory.GetFiles() could cause exceptions.
    /// Therefore implementing recursion ourselves is the best way to avoid those exceptions.
    /// <see cref="https://social.msdn.microsoft.com/Forums/vstudio/en-US/ae61e5a6-97f9-4eaa-9f1a-856541c6dcce/directorygetfiles-gives-me-access-denied?forum=csharpgeneral"/>
    /// </summary>
    /// <param name="root">The root directory.</param>
    /// <param name="fileEnding">The file ending.</param>
    private List<string> GetFiles(string root, string fileEnding)
    {
        var fileList = new List<string>();

        Stack<string> pending = new Stack<string>();
        pending.Push(root);
        while (pending.Count != 0)
        {
            var path = pending.Pop();
            string[] next = null;
            try
            {
                next = Directory.GetFiles(path, fileEnding);
            }
            catch { }
            if (next != null && next.Length != 0)
                foreach (var file in next) fileList.Add(file);
            try
            {
                next = Directory.GetDirectories(path);
                foreach (var subdir in next) pending.Push(subdir);
            }
            catch { }
        }
        return fileList;
    }
}
