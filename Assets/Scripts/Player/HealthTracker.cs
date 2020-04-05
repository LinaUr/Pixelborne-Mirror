﻿using System;
using TMPro;
using UnityEngine;

/// <summary>This class manages the display of the health of a player via a TextMeshProUGUI.</summary>
public class HealthTracker : MonoBehaviour
{
    [SerializeField]
    private EntityHealth m_playerHealth;

    private TextMeshProUGUI m_text;

    void Start()
    {
        m_text = gameObject.GetComponent<TextMeshProUGUI>();
    }
    
    /// <summary>It updates the UI by requesting the player HP and changing the UI to the new value.</summary>
    void Update()
    {
        int health = m_playerHealth.CurrentHealth;
        if (m_playerHealth.IsZero)
        {
            health = 0;
        }
        m_text.SetText($"{health}");

        Color32 color;
        if (health >= Math.Ceiling((double)m_playerHealth.MaxHealth * 3 / 4))
        {
            color = new Color32(0, 200, 0, 255);
        }
        else if (health >= Math.Ceiling((double)m_playerHealth.MaxHealth * 1 / 4))
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
