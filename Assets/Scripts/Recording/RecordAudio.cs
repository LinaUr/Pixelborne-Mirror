using UnityEngine;
using System.IO;
using System;

public class RecordAudio : MonoBehaviour
{
    private static int m_RECORD_DURATION = 10; // in seconds
    private AudioClip m_microphone_clip;
    private string m_selectedDevice;
    private float m_time_left_recording = 0.0f;
    private static string m_AUDIO_RECORD_DIR = "records";
    private string m_filedir;

    // This method sets the microphone to the first device that has been found
    void Start()
    {
        if(Microphone.devices.Length > 0) 
        {
            m_selectedDevice = Microphone.devices[0].ToString();
            m_filedir = Path.Combine(Application.dataPath, m_AUDIO_RECORD_DIR);
            Directory.CreateDirectory(m_filedir);
        } else
        {
            Debug.Log("No microphone device found. Therefore recordings are not supported.");
        }
    }

    // This method counts down the time until the recording is over and then saves the file.
    void FixedUpdate()
    {
        if(m_time_left_recording > 0)
        {
            m_time_left_recording -= Time.fixedDeltaTime;
            if(m_time_left_recording <= 0 )
            {
                SaveRecording();
            }
        }
    }

    // This message returns if a microphone device was found in Start().
    public bool MicrophoneAviable()
    {
        return string.IsNullOrEmpty(m_selectedDevice);
    }

    // This method starts the recording.
    public void Record()
    {
        if (MicrophoneAviable()) 
        {
            m_microphone_clip = Microphone.Start(m_selectedDevice, false, m_RECORD_DURATION, 44100);
            m_time_left_recording = ((float) m_RECORD_DURATION) * 1.1f; // puffer
        }
    }

    // This method converts the recording to a Wav file and saves it on th disk.
    private void SaveRecording()
    {
        string filename = "sound_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + ".wav";
        var filepath = Path.Combine(m_filedir, filename);
        
        SavWav.Save(filepath, m_microphone_clip);
    }
}
