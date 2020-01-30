using UnityEngine;

// This interface must be implemented by Singleplayer and Multiplayer class. 
public interface IGame
{
    void ResetGame();
    void PrepareGame();
    void PlayerDied(GameObject player);
    void EnableEntityCollision(GameObject callingEntity, int Layer1, int layer2);
    void DisableEntityCollision(GameObject callingEntity, int Layer1, int layer2);
}
