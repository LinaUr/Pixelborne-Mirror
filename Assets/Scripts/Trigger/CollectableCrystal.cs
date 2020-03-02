﻿using UnityEngine;

public class CollectableCrystal : MonoBehaviour
{
    private AudioSource m_audioPlayer;
    private bool m_isCollected = false;

    private void Start()
    {
        m_audioPlayer = gameObject.GetComponent<AudioSource>();
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
        if (!m_isCollected && collider.gameObject == Singleplayer.Instance.Player)
        {
                m_isCollected = true;
                m_audioPlayer.Play();
        }
    }
}
