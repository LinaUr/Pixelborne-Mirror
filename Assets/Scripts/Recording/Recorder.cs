﻿using UnityEngine;

namespace Assets.Scripts.Recording
{
    // This class is used to manage the recording of the mircrophone and taking photos with the webcam.
    public class Recorder : MonoBehaviour
    {
        private RecordAudio m_audioRecorder;
        private WebcamPhoto m_photoRecorder;

        private void Start()
        {
            m_audioRecorder = gameObject.AddComponent<RecordAudio>();
            m_photoRecorder = gameObject.AddComponent<WebcamPhoto>();
        }

        public void Record()
        {
            m_audioRecorder.Record();
            m_photoRecorder.Record();
        }
    }
}