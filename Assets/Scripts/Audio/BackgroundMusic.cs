﻿using System;
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

        string username = Environment.UserName;
        playlist = GetFiles($"C:/Users/{username}", "*.mp3").ToList();

        elapsedTime = 0;
        StartCoroutine(LoadAudio());
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        if(audio == null)
            return;

        if (elapsedTime >= audio.length)
        {
            elapsedTime -= audio.length;
            StartCoroutine(LoadAudio());
        }

    }

    IEnumerator LoadAudio()
    {
        int index = UnityEngine.Random.Range(0, playlist.Count);

        WWW request = new WWW("file://" + playlist[index]);
        while (!request.isDone)
            yield return 0;

        audio = NAudioPlayer.FromMp3Data(request.bytes);
        audio.name = playlist[index];

        audioPlayer.clip = audio;
        audioPlayer.Play();
    }

    // This method returns all file paths for files with a certain ending.
    // Access to certain paths or folders can be denied and so using Directory.GetFiles() could cause exceptions.
    // Therefore implementing recursion ourselves is the best way to avoid those exceptions.
    private IEnumerable<string> GetFiles(string root, string fileEnding)
    {
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
