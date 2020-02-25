using UnityEngine;
using System.IO;
using System;
using System.Threading.Tasks;

public class RecordAudio : MonoBehaviour
{
    private static int m_RECORD_DURATION = 10; // in seconds
    private AudioClip m_microphoneClip;
    private string m_selectedDevice;
    private float m_timeLeftRecording = 0.0f;
    private static string m_AUDIO_RECORD_DIR = "records";
    private string m_filedir;

    // This method sets the microphone to the first device that has been found.
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
        if(m_timeLeftRecording > 0)
        {
            m_timeLeftRecording -= Time.fixedDeltaTime;
            if(m_timeLeftRecording <= 0 )
            {
                SaveRecording();
            }
        }
    }

    // This message returns if a microphone device was found in Start().
    public bool MicrophoneAvailable()
    {
        return !string.IsNullOrEmpty(m_selectedDevice);
    }

    // This method starts the recording.
    public void Record()
    {
        if (MicrophoneAvailable()) 
        {
            m_microphoneClip = Microphone.Start(m_selectedDevice, false, m_RECORD_DURATION, 44100);
            m_timeLeftRecording = ((float) m_RECORD_DURATION) * 1.1f; // puffer
        }
    }

    // This method converts the recording to a Wav file and saves it on the disk.
    private void SaveRecording()
    {
        DateTime now = DateTime.Now;
        string filename = $"{now.Year}-{now.Month.ToString("d2")}-{now.Day.ToString("d2")}_{now.Hour.ToString("d2")}-{now.Minute.ToString("d2")}-{now.Second.ToString("d2")}.wav";
        string filepath = Path.Combine(m_filedir, filename);

        SavWav.Save(filepath, m_microphoneClip);
    }
}
