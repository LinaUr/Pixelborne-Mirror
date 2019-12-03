using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

public class BackgroundMusic : MonoBehaviour
{
    private AudioSource audioPlayer;
    private AudioClip audio;
    private List<string> playlist;
    private float elapsedTime;

    void Start()
    {
        audioPlayer = GetComponent<AudioSource>();

        var userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        playlist = GetFiles(userPath, "*.mp3").ToList();

        elapsedTime = 0;
        audio = null;
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

    IEnumerator ChangeBackgroundAudio()
    {
        int index = UnityEngine.Random.Range(0, playlist.Count);

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

    // This method returns all file paths for files with a certain ending.
    // Access to certain paths or folders can be denied and so using Directory.GetFiles() could cause exceptions.
    // Therefore implementing recursion ourselves is the best way to avoid those exceptions.
    private IEnumerable<string> GetFiles(string root, string fileEnding)
    {
        //Methode asynchron in neuem Task/Thread ausführen!
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
                foreach (var file in next) yield return file;
            try
            {
                next = Directory.GetDirectories(path);
                foreach (var subdir in next) pending.Push(subdir);
            }
            catch { }
        }
    }
}
