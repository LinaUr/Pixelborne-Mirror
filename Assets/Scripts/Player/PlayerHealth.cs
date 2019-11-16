using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerHealth : MonoBehaviour
{

    //Object that stores and manages the health of a player

    public int maxHealth;
    public int currentHealth;
    public bool isAlive;
    public int timeLastDeath;
    public int respawnTime;


    public PlayerHealth(int maxhealth, int respawntime)
    {
        this.maxHealth = maxhealth;
        this.currentHealth = this.maxHealth;
        this.isAlive = true;
        this.timeLastDeath = 0;
        this.respawnTime = respawntime;

        UpdateCaller.OnUpdate += Update;
    }

    ~PlayerHealth()
    {
        UpdateCaller.OnUpdate -= Update;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Check if player has died since last frame, add function with despawn, ... later
        if(currentHealth <= 0 && isAlive)
        {
            isAlive = false;
            timeLastDeath = Toolkit.currentTime();
        }
        //Revive player after enough time has elapsed, add function with respawn, ... later
        if (!isAlive)
        {
            if((Toolkit.currentTime() - timeLastDeath) >= respawnTime)
            {
                currentHealth = maxHealth;
                isAlive = true;
            }
        }
        
    }
}
