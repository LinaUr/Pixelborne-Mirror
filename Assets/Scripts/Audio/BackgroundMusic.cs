using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Networking;

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
        m_playlist = await Task.Run(() => Toolkit.GetFiles(userPath, "*.mp3"));

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

    // This coroutine picks a random file from the playlist, converts the
    // MP3 file to WAV, assigns it to the audioPlayer and plays it.
    // NOTE: It can currently only handle mp3 files.
    private IEnumerator ChangeBackgroundAudio()
    {
        int index = UnityEngine.Random.Range(0, m_playlist.Count);

        if (m_playlist.Count != 0)
        {
            UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip("file://" + m_playlist[index], AudioType.MPEG);
            yield return request.SendWebRequest();

            // Apparently Unity does not allow the use of DownloadHandlerAudioClip.audioClip
            // when the audiofile is MP3 format (it returns null). So we make use of the NAudio library.
            m_audioClip = NAudioPlayer.FromMp3Data(request.downloadHandler.data);
            m_audioClip.name = m_playlist[index];
            m_audioPlayer.clip = m_audioClip;
            m_audioPlayer.Play();
        }
        else
        {
            Debug.Log("No MP3-files were found. Cannot play any background audio.");
        }
    }
}