using UnityEngine;

// This interface must be implemented by Singleplayer and Multiplayer class. 
public interface IGame
{
    void ResetGame();
    void PrepareGame();
    void PlayerDied(GameObject player);
    void LockPlayerInput(bool isLocked);
    void EnableEntityCollision(GameObject callingEntity);
    void DisableEntityCollision(GameObject callingEntity);
    GameObject[] GetActivePlayers();
}
