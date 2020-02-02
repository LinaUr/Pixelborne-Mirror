using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleplayer : MonoBehaviour, IGame
{
    private GameObject m_player;

    private void Awake() 
    {
        m_player = GameObject.FindWithTag("Player_1");
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
    public void DisableEntityCollision(GameObject callingEntity, int layer1, int layer2)
    {

    }
    public void EnableEntityCollision(GameObject callingEntity, int layer1, int layer2)
    {

    }

    public GameObject[] GetActivePlayers(){
        return new GameObject[]{m_player};
    }
}
