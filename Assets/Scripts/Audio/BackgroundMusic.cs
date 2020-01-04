using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Threading.Tasks;

// NOTE:
// Unity is not Thread safe, so they decided to make it impossible 
// to call their API from another Thread by adding a mechanism to 
// throw an exception when its API is used from another Thread.
// (see https://stackoverflow.com/questions/41330771/use-unity-api-from-another-thread-or-call-a-function-in-the-main-thread )
// That is why I cannot run ChangeBackgroundAudio() in a new Task
// (or anything inside it really), because then an Exception like 
// "Xxx() can only be called from the main thread" gets thrown.

// This class searches for mp3-files in the user folder of the current user,
// assignes them randomly to an Audio Source component in the scene and plays them.
public class BackgroundMusic : MonoBehaviour
{
    private AudioSource m_audioPlayer;
    private AudioClip m_audioClip;
    private float m_elapsedTime;

    // Playlist stores paths to mp3 files.
    private List<string> m_playlist = new List<string>(); 
    
    async void Start()
    {
        m_audioPlayer = GetComponent<AudioSource>();
        m_audioClip = null;
        m_elapsedTime = 0;

        // Path to windows user folder of current user regardless of the username.
        string userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        
        // Running GetFiles() asynchronously in new task prevents game from stopping/pausing
        // when starting a new game.
        m_playlist = await Task.Run(() => GetFiles(userPath, "*.mp3"));

        StartCoroutine(ChangeBackgroundAudio());
    }

    void Update()
    {
        m_elapsedTime += Time.deltaTime;

        if (m_audioClip == null)
            return;

        if (m_elapsedTime >= m_audioClip.length)
        {
            m_elapsedTime -= m_audioClip.length;
            StartCoroutine(ChangeBackgroundAudio());
        }
    }

    // This method picks a random file from the playlist, converts the
    // mp3 file to wav, assigns it to the audioPlayer and plays it.
    // NOTE: It can currently only handle mp3 files.
    private IEnumerator ChangeBackgroundAudio()
    {
        int index = UnityEngine.Random.Range(0, m_playlist.Count);

        if (m_playlist.Count != 0)
        {
            WWW request = new WWW("file://" + m_playlist[index]);
            while (!request.isDone)
                yield return 0;

            m_audioClip = NAudioPlayer.FromMp3Data(request.bytes);
            m_audioClip.name = m_playlist[index];
            m_audioPlayer.clip = m_audioClip;
            m_audioPlayer.Play();
        }
        else
        {
            Debug.Log("No mp3-files were found. Cannot play any background audio.");
        }
    }

    // This method returns all file paths for files with a certain fileEnding in the root
    // directory and all subdirectories.
    // Access to certain paths can be denied, so using Directory.GetFiles() could cause exceptions.
    // Therefore, implementing recursion ourselves is the best way to avoid those exceptions.
    // (see https://social.msdn.microsoft.com/Forums/vstudio/en-US/ae61e5a6-97f9-4eaa-9f1a-856541c6dcce/directorygetfiles-gives-me-access-denied?forum=csharpgeneral )
    private List<string> GetFiles(string root, string fileEnding)
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