using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System.IO;
using System;

public class RecordAudio : MonoBehaviour
{

    // Microphone input
    public AudioClip _microphone_clip;
    public AudioMixerGroup mixerGroupMicrophone;
    public int RecordDuration = 10; // in seconds
    private string _selectedDevice;
    private bool _recording = false;
    private float _time_left_recording = 0.0f;
    private static string _AUDIO_RECORD_DIR = "records";


    // Start is called before the first frame update
    void Start()
    {
        if(Microphone.devices.Length > 0) 
        {
            _selectedDevice = Microphone.devices[0].ToString();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(_time_left_recording > 0){
            _time_left_recording -= Time.fixedDeltaTime;
            if(_time_left_recording <= 0 ){
                SaveRecording();
            }
        }
    }

    public bool MicrophoneAviable(){
        return string.IsNullOrEmpty(_selectedDevice);
    }

    public void StartRecording(){
        _microphone_clip = Microphone.Start(_selectedDevice, false, RecordDuration, 44100);
        _time_left_recording = ((float) RecordDuration) * 1.1f; // puffer
    }

    private void SaveRecording(){
        string filename = "sound_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + ".wav";
        var filepath = Path.Combine(Application.dataPath, _AUDIO_RECORD_DIR);
        filepath = Path.Combine(filepath, filename);
        
        SavWav.Save(filepath, _microphone_clip);
    }
}
