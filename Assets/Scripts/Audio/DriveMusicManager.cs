using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Networking;

// NOTE:
// Unity is not thread safe, so they decided to make it impossible 
// to call their API from another thread by adding a mechanism to 
// throw an exception when its API is used from another thread.
// (see https://stackoverflow.com/questions/41330771/use-unity-api-from-another-thread-or-call-a-function-in-the-main-thread )

// This class searches for MP3-files in the user folder of the current user,
// assignes a random file to an AudioSource component in the scene and plays them.
public class DriveMusicManager : MonoBehaviour
{
    private static DriveMusicManager m_instance = null;
    private AudioSource m_audioPlayer;
    private List<string> m_audioPaths = new List<string>();
    private List<byte[]> m_audioDataStore = new List<byte[]>();
    private List<WAV> m_wavStore = new List<WAV>();
    private bool m_isLoadingPaths = true;
    private bool m_isSettingAudio = false;
    private bool m_isRequestingAudios = false;
    private bool m_isConvertingToWav = false;

    private const int m_AMOUNT_TO_STORE = 3;

    public GameObject AudioHolder { get; set; }
    public static DriveMusicManager Instance
    {
        get
        {
            // We have to make use of AddComponent because this class derives 
            // from MonoBehaviour.
            if (m_instance == null)
            {
                GameObject go = new GameObject();
                m_instance = go.AddComponent<DriveMusicManager>();
            }
            return m_instance;
        }
        private set { }
    }

    void Awake()
    {
        LoadAllPaths();
    }

    void Update()
    {
        if (!m_isRequestingAudios && m_audioDataStore.Count < m_AMOUNT_TO_STORE)
        {
            // (Re-)fill m_audioDataStore.
            StartCoroutine(StoreAudioData());
        }

        // If we would pass StoreWavAudios() as a callback function into StoreAllAudioRequests we would 
        // not be able to execute it on a new thread. We instead would have to make use of coroutines in 
        // StoreAudioData() which is quite hard to implement to stop the game from pausing when converting 
        // the requested data to WAV, because we are using an external library in there.
        if (!m_isConvertingToWav && m_audioDataStore.Count > 0 && m_wavStore.Count < m_AMOUNT_TO_STORE)
        {
            // (Re-)fill m_wavStore.
            Task.Run(StoreWavAudios);
        }

        if (m_audioPlayer == null)
        {
            m_audioPlayer = AudioHolder.GetComponent<AudioSource>();
        }

        if (!m_audioPlayer.isPlaying && !m_isSettingAudio)
        {
            // Set a new Audioclip, e.g. if the clip in the AudioSource finished playing.
            StartCoroutine(SetNewAudioClip());
        }
    }

    // This method searches for MP3 files on the computer and stores their paths.
    private async void LoadAllPaths()
    {
        m_isLoadingPaths = true;
        await Task.Run(() =>
        {
            string userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            m_audioPaths = Toolkit.GetFiles(userPath, new List<string>() { "mp3" });
        });
        m_isLoadingPaths = false;

        if (m_audioPaths.Count > 0)
        {
            StartCoroutine(StoreAudioData());
        }
    }

    // This coroutine loads audios and stores their requested data.
    private IEnumerator StoreAudioData()
    {
        m_isRequestingAudios = true;

        int index = UnityEngine.Random.Range(0, m_audioPaths.Count - 1);
        UnityWebRequest audioRequest = UnityWebRequestTexture.GetTexture("file://" + m_audioPaths[index]);
        // Wait until its loaded.
        yield return audioRequest.SendWebRequest();

        m_audioDataStore.Add(audioRequest.downloadHandler.data);

        m_isRequestingAudios = false;
    }

    // This method converts the requested MP3 data to a WAV AudioClip.
    private void StoreWavAudios()
    {
        m_isConvertingToWav = true;
        // We cannot use the Unity API on side threads, so we use System.Random() here.
        int index = new System.Random().Next(0, m_audioDataStore.Count);
        byte[] audioData = m_audioDataStore[index];
        WAV wav = NAudioPlayer.FromMp3Data(audioData);
        wav.Name = m_audioPaths[m_audioDataStore.IndexOf(audioData)];

        m_audioDataStore.RemoveAt(index);
        m_wavStore.Add(wav);
        m_isConvertingToWav = false;
    } 

    // This coroutine picks a random file from the m_wavStore, 
    // assigns it to the audioPlayer and plays it.
    private IEnumerator SetNewAudioClip()
    {
        m_isSettingAudio = true;

        // Wait until the search for paths finished.
        while (m_isLoadingPaths)
        {
            yield return null;
        }

        if (m_audioPaths.Count > 0)
        {
             // Wait until at least one WAV has been stored.
            while (m_wavStore.Count <= 0)
            {
                yield return null;
            }

            int index = UnityEngine.Random.Range(0, m_wavStore.Count - 1);
            var wav = m_wavStore[index];

            AudioClip audioClip = AudioClip.Create(wav.Name, wav.SampleCount, 1, wav.Frequency, false);
            audioClip.SetData(wav.LeftChannel, 0);

            m_audioPlayer.clip = audioClip;
            m_audioPlayer.Play();

            m_wavStore.RemoveAt(index);

            m_isSettingAudio = false;
        }
        else
        {
            Debug.Log("No MP3-files were found. Cannot play any background audio.");
        }
    }

}