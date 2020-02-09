using UnityEngine;

// This class manages the health of an entity.
public class EntityHealth : MonoBehaviour
{
    [SerializeField]
    private int m_maxHealth;

    public bool Invincible { get; set; }
    public int CurrentHealth { get; private set; }

    public int MaxHealth
    {
        get
        {
            return m_maxHealth;
        }
        private set { }
    }

    public bool IsDead 
    {
        get
        { 
            return CurrentHealth <= 0;
        }
        private set { }
    }

    void Start()
    {
        Revive();
    }

    // This method revives the entity by resetting its m_currentHealth.
    public void Revive()
    {
        CurrentHealth = MaxHealth;
        Invincible = false;
    }

    // This method ensures that the entity has 0 health
    public void Die() {
        TakeDamage(m_maxHealth);
    }

    // This method deals damage to the entity by reducing its m_currentHealth.
    public void TakeDamage(int damage)
    {
        if(!Invincible)
        {
            CurrentHealth -= damage;
            // ensure that hp cannot be negative
            CurrentHealth = CurrentHealth < 0 ? 0 : CurrentHealth;
        }
    }
}
