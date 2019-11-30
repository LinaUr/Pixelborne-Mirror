using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tempKillScript : MonoBehaviour
{

    [SerializeField]
    protected int player; //1=Player1, other values=Player2


    [SerializeField]
    protected int kill; //Value other than 0 will kill the player once

    [SerializeField]
    protected bool isAlive;

    [SerializeField]
    protected int showHealth;


    protected PlayerHealth playerRef;
    // Start is called before the first frame update
    void Start()
    {
        //kill = 0;
        if(player == 1)
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
        showHealth = playerRef.currentHealth;
        if (kill != 0)
        {
            kill = 0;
            playerRef.currentHealth = 0;
        }
        isAlive = playerRef.isAlive;
    }
}
