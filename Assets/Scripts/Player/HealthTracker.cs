﻿using UnityEngine;
using TMPro;
using System;

// This class manages the display of the health of a player via a TextMeshProUGUI.
public class HealthTracker : MonoBehaviour
{
    [SerializeField]
    private EntityHealth m_playerHealth;

    private TextMeshProUGUI m_text;

    void Start()
    {
        m_text = gameObject.GetComponent<TextMeshProUGUI>();
    }

    
    void Update()
    {
        int health = m_playerHealth.GetHealth();
        if (m_playerHealth.isDead)
        {
            health = 0;
        }
        m_text.SetText($"{health}");

        Color32 color;
        if (health >= Math.Ceiling((double)m_playerHealth.maxHealth * 3 / 4))
        {
            color = new Color32(0, 200, 0, 255);
        }
        else if (health >= Math.Ceiling((double)m_playerHealth.maxHealth * 1 / 4))
        {
            color = new Color32(255, 255, 0, 255);
        }
        else if (health > 0)
        {
            color = new Color32(255, 140, 0, 255);
        }
        else if (health == 0)
        {
            color = new Color32(255, 0, 0, 255);
        }
        else
        {
            color = new Color32(255, 0, 0, 255);
        }
        m_text.color = color;
    }
}
