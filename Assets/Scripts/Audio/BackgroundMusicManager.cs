using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Networking;
using System.IO;

// NOTE:
// Unity is not Thread safe, so they decided to make it impossible 
// to call their API from another Thread by adding a mechanism to 
// throw an exception when its API is used from another Thread.
// (see https://stackoverflow.com/questions/41330771/use-unity-api-from-another-thread-or-call-a-function-in-the-main-thread )
// That is why I cannot run ChangeBackgroundAudio() in a new Task
// (or anything inside it really), because then an Exception like 
// "Xxx() can only be called from the main thread" gets thrown.

// This class searches for MP3-files in the user folder of the current user,
// assignes them randomly to an Audio Source component in the scene and plays them.
public class BackgroundMusicManager : MonoBehaviour
{
    // TODO: Comments anpassen
    // TODO: Comments auch in NAudioPlayer anpassen
    // TODO: optimieren, wann WAVs geladen werden
    // TODO: WAVs im Hintergrund austauschen
    // TODO: elapsed Time überarbeiten --> wann der nächste Titel geladen wird.
    // TODO: IDEE: wenn Titel/Requests genommen wurden, die aus den listen Schmeißen und was anderes Nachladen.
    private static BackgroundMusicManager m_instance = null;
    private AudioSource m_audioPlayer;
    private List<string> m_audioPaths = new List<string>();
    private List<byte[]> m_audioDataStore = new List<byte[]>();
    private List<WAV> m_wavStore = new List<WAV>();
    private bool m_isLoadingPaths = true;
    private bool m_isLoadingRequests = true;
    private AudioClip m_audioClip = null;
    private float m_elapsedTime = 0;

    public static BackgroundMusicManager Instance
    {
        get
        {
            // We have to make use of AddComponent because this class derives 
            // from MonoBehaviour.
            if (m_instance == null)
            {
                GameObject go = new GameObject();
                m_instance = go.AddComponent<BackgroundMusicManager>();
            }
            return m_instance;
        }
        private set { }
    }

    public GameObject AudioHolder{ get; set; }

    void Awake()
    {
        LoadAllPaths();
        StartCoroutine(SetNewAudioClip());
    }

    // This method searches for images on the computer and stores their paths.
    private async void LoadAllPaths()
    {
        m_isLoadingPaths = true;
        await Task.Run(() =>
        {
            string userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            m_audioPaths = Toolkit.GetFiles(userPath, new List<string>() { "mp3" });

            // Gitlab Issue #48
            // Load images from the entire user folder.
            //m_imagePaths = await Task.Run(() => Toolkit.GetFiles(userPath, new List<string>(){ "jpg", "jpeg", "png" }));

            m_isLoadingPaths = false;
        });

        if (m_audioPaths.Count > 0)
        {
            StartCoroutine(StoreAllAudioRequests());
        }
    }

    // This coroutine loads 2 found audios and stores their requests.
    // NOTE: It can currently only handle MP3 files.
    private IEnumerator StoreAllAudioRequests()
    {
        m_isLoadingRequests = true;
        for (int i = 0; i < 2; i++)
        {
            UnityWebRequest audioRequest = UnityWebRequestTexture.GetTexture("file://" + m_audioPaths[i]);
            // Wait until its loaded.
            yield return audioRequest.SendWebRequest();

            m_audioDataStore.Add(audioRequest.downloadHandler.data);
        }
        m_isLoadingRequests = false;
    }

    // This method converts the requested MP3 data to a WAV AudioClip;
    private void StoreRequestedAudios()
    {
        m_audioDataStore.ForEach(audioData =>
        {
            var wav = NAudioPlayer.FromMp3Data(audioData);
            wav.Name = m_audioPaths[m_audioDataStore.IndexOf(audioData)];
            m_wavStore.Add(wav);
        });
    } 

    async void Update()
    {
        // If we would pass StoreRequestedAudios as a callback function into StoreAllAudioRequests we would 
        // not be able to execute it on a new thread. We instead would have to make use of coroutines in 
        // StoreRequestedAudios which is quite hard to implement to stop the game from pausing when converting 
        // the requested data to WAV, because we are using an external library in there.
        if (!m_isLoadingRequests && m_wavStore.Count <= 0)
        {
            await Task.Run(StoreRequestedAudios);
        }
        

        m_elapsedTime += Time.deltaTime;

        if (m_audioClip == null)
            return;

        if (m_elapsedTime >= m_audioClip.length)
        {
            m_elapsedTime -= m_audioClip.length;
            StartCoroutine(SetNewAudioClip());
        }
    }

    // This coroutine picks a random file from the AudioStore, 
    // assigns it to the audioPlayer and plays it.
    private IEnumerator SetNewAudioClip()
    {
        // Wait until search for paths finished.
        while (m_isLoadingPaths)
        {
            yield return null;
        }

        if (m_audioPaths.Count > 0)
        {
             // Wait until at least one AudioClip has been stored.
            while (m_wavStore.Count <= 0)
            {
                yield return null;
            }

            // Grab needed amount but random images from the ImageStore.
            int index = UnityEngine.Random.Range(0, m_wavStore.Count);
            var wav = m_wavStore[index];

            AudioClip audioClip = AudioClip.Create(wav.Name, wav.SampleCount, 1, wav.Frequency, false);
            audioClip.SetData(wav.LeftChannel, 0);
            m_audioClip = audioClip;

            if (m_audioPlayer == null)
            {
                m_audioPlayer = AudioHolder.GetComponent<AudioSource>();
            }
            m_audioPlayer.clip = m_audioClip;
            m_audioPlayer.Play();
        }
        else
        {
            Debug.Log("No MP3-files were found. Cannot play any background audio.");
        }
    }

}