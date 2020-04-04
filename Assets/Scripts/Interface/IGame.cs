using UnityEngine;

// This interface must be implemented by Singleplayer and Multiplayer class. 
/// <summary></summary>
public interface IGame
{
    /// <summary>Gets the winner.</summary>
    /// <returns></returns>
    string GetWinner();

    /// <summary>Registers the player.</summary>
    /// <param name="player">The player.</param>
    void RegisterPlayer(GameObject player);
    /// <summary>Unregisters the player.</summary>
    /// <param name="player">The player.</param>
    void UnregisterPlayer(GameObject player);
    /// <summary>Locks the player input.</summary>
    /// <param name="isLocked">if set to <c>true</c> [is locked].</param>
    void LockPlayerInput(bool isLocked);
    /// <summary>Handles the death.</summary>
    /// <param name="entity">The entity.</param>
    void HandleDeath(GameObject entity);
    /// <summary>Prepares the stage.</summary>
    void PrepareStage();
    /// <summary>Disables the entity collision.</summary>
    /// <param name="callingEntity">The calling entity.</param>
    void DisableEntityCollision(GameObject callingEntity);
    /// <summary>Enables the entity collision.</summary>
    /// <param name="callingEntity">The calling entity.</param>
    void EnableEntityCollision(GameObject callingEntity);
    /// <summary>Swaps the hud symbol.</summary>
    /// <param name="gameObject">The game object.</param>
    /// <param name="sprite">The sprite.</param>
    void SwapHudSymbol(GameObject gameObject, Sprite sprite);
}
