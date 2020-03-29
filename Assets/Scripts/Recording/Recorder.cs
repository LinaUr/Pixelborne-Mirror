using UnityEngine;

namespace Assets.Scripts.Recording
{
    // This class is used to manage the recording of the mircrophone and taking photos with the webcam.
    // It is a singleton.
    public class Recorder : MonoBehaviour
    {
        private static Recorder m_instance = null;
        private AudioRecorder m_audioRecorder;
        private PhotoRecorder m_photoRecorder;

        public static Recorder Instance
        {
            get
            {
                // We have to make use of AddComponent because this class derives 
                // from MonoBehaviour.
                if (m_instance == null)
                {
                    GameObject go = new GameObject();
                    m_instance = go.AddComponent<Recorder>();
                    m_instance.m_audioRecorder = go.AddComponent<AudioRecorder>();
                    m_instance.m_photoRecorder = go.AddComponent<PhotoRecorder>();
                    m_instance.name = "Recorder";
                }
                return m_instance;
            }
        }

        public void Record()
        {
            m_audioRecorder.Record();
            m_photoRecorder.Record();
        }
    }
}
