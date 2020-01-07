using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityHealth : MonoBehaviour
{
    public int maxHealth;
    private int currentHealth;

    public bool isAlive {
        get { return currentHealth > 0; }
    }
    public bool isDead {
        get { return currentHealth <= 0; }
    }

    // Start is called before the first frame update
    void Start()
    {
        Revive();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Revive(){
        currentHealth = maxHealth;
    }

    public void takeDamage(int damage){
        currentHealth -= damage;
        // ensure that hp cannot be negative
        currentHealth = currentHealth < 0 ? 0 : currentHealth;
    }
}
