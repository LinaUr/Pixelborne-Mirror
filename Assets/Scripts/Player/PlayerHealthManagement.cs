using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthManagement : MonoBehaviour
{

    //Updates player health

    [SerializeField]
    protected int player; //1=Player1, other values=Player2
    [SerializeField]
    protected bool showIsAlive;
    [SerializeField]
    protected int showCurrentrHealth;

    protected PlayerHealth playerRef;
    // Start is called before the first frame update
    void Start()
    {
        if (player == 1)
        {
            playerRef = PlayerHealthController.player1Health;
        }
        else
        {
            playerRef = PlayerHealthController.player2Health;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Check if player has died since last frame, add function with despawn, ... later
        if (playerRef.currentHealth <= 0 && playerRef.isAlive)
        {
            playerRef.isAlive = false;
            playerRef.timeLastDeath = Toolkit.currentTime();
        }
        //Revive player after enough time has elapsed, add function with respawn, ... later
        if (!playerRef.isAlive)
        {
            if ((Toolkit.currentTime() - playerRef.timeLastDeath) >= playerRef.respawnTime)
            {
                playerRef.currentHealth = playerRef.maxHealth;
                playerRef.isAlive = true;
            }
        }

        showIsAlive = playerRef.isAlive;
        showCurrentrHealth = playerRef.currentHealth;
    }
}
