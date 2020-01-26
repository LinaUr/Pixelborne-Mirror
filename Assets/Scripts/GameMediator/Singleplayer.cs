using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleplayer : MonoBehaviour, IGame
{
    private GameObject m_player;

    private void Awake() 
    {
        // TODO refactore
        m_player = GameObject.FindWithTag("Player1");
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void ResetGame()
    {

    }
    public void PrepareGame()
    {

    }
    public void PlayerDied(GameObject player)
    {

    }
    public void LockPlayerInput(bool isLocked)
    {

    }
    public void EnableEntityCollision(GameObject callingEntity)
    {

    }
    public void DisableEntityCollision(GameObject callingEntity)
    {

    }

    public GameObject[] GetActivePlayers(){
        return new GameObject[]{m_player};
    }
}
