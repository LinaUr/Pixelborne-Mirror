using UnityEngine;

// This interface must be implemented by Singleplayer and Multiplayer class. 
public interface IGame
{
    void RegisterPlayer(GameObject player);
    void UnregisterPlayer(GameObject player);
    void LockPlayerInput(bool isLocked);
    void HandleDeath(GameObject entity);
    void PrepareStage();
    void DisableEntityCollision(GameObject callingEntity);
    void EnableEntityCollision(GameObject callingEntity);
    void SwapHudSymbol(GameObject gameObject, Sprite sprite);

    string GetWinner();
}
