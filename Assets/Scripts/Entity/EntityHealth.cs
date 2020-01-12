using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class manages the the Health of an entity.
public class EntityHealth : MonoBehaviour
{
    public int m_maxHealth;
    public int CurrentHealth { get; private set;} 

    public bool isAlive 
    {
        get { return CurrentHealth > 0; }
    }
    public bool isDead 
    {
        get { return CurrentHealth <= 0; }
    }

    void Start()
    {
        Revive();
    }

    void Update()
    {
        
    }

    // This method revives the entity by resetting its m_currentHealth.
    public void Revive()
    {
        CurrentHealth = m_maxHealth;
    }

    // This method deals damage to the entity by reducing it's m_currentHealth.
    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        // ensure that hp cannot be negative
        CurrentHealth = CurrentHealth < 0 ? 0 : CurrentHealth;
    }
}
