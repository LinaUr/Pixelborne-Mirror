using UnityEngine;

// This interface must be implemented by Singleplayer and Multiplayer class. 
public interface IGame
{
    void RegisterPlayer(GameObject player);
    void UnegisterPlayer(GameObject player);
    void ResetGame();
    void PrepareGame();
    void PlayerDied(GameObject player);
    void DisableEntityCollision(GameObject callingEntity);
    void EnableEntityCollision(GameObject callingEntity);
}
