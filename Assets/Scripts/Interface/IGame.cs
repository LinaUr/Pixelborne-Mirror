using UnityEngine;

// This interface must be implemented by Singleplayer and Multiplayer class. 
public interface IGame
{
    void RegisterPlayer(GameObject player);
    void UnegisterPlayer(GameObject player);
    void LockPlayerInput(bool isLocked);
    void HandleDeath(GameObject entity, bool isDeadByDeathZone);
    void PrepareStage();
    void DisableEntityCollision(GameObject callingEntity);
    void EnableEntityCollision(GameObject callingEntity);
    void SwapHudSymbol(GameObject gameObject, Sprite sprite);

    string GetWinner();
}
