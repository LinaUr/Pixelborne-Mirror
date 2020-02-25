using UnityEngine;

public class CollectableCrystal : MonoBehaviour
{
    private AudioSource m_audioPlayer;
    private bool m_isCollected;

    private void Start()
    {
        m_audioPlayer = gameObject.GetComponent<AudioSource>();
        m_isCollected = false;
    }

    void Update()
    {
        if (m_isCollected && !m_audioPlayer.isPlaying)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!m_isCollected)
        {
            if (collider == Singleplayer.Instance.Player)
            {
                m_isCollected = true;
                m_audioPlayer.Play();
            }
        }
    }
}
