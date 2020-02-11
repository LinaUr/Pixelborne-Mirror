using UnityEngine;

public class LoopWithBlend : MonoBehaviour
{
    [SerializeField]
    private AudioSource m_audioPlayer;
    [SerializeField]
    private float m_blendLengthInSeconds;

    void Update()
    {
        if (!m_audioPlayer.isPlaying)
        {
            m_audioPlayer.time = m_blendLengthInSeconds;
            m_audioPlayer.Play();
        }
    }
}
