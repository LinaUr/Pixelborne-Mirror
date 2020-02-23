using UnityEngine;
using System;
using System.Collections.Generic;

// This class serves as a mediator between various components of the game.
// It is a Singleton.
public class GameMediator : ScriptableObject
{
    private GameObject m_lastDiedPlayer;
    private static GameMediator m_instance = null;

    public static GameMediator Instance
    {
        get
        {
            // A ScriptableObject should not be instanciated directly,
            // so we use CreateInstance instead.
            return m_instance == null ? CreateInstance<GameMediator>() : m_instance;
        }
        private set { }
    }

    //public int WinnerIndex { get; set; }
    //public float PriceToPay { get; set; }
    //// Every scene needs to have a corresponding script that sets the CurrentMode.
    //public Mode CurrentMode { get; set; }
    //public IGame ActiveGame { get; set; }
    //public ICamera ActiveCamera { get; set; }
    //public List<GameObject> ActivePlayers { get; set; } = new List<GameObject>();
    //public List<GameObject> ActiveEnemies { get; set; } = new List<GameObject>();

    public GameMediator()
    {
        m_instance = this;
    }

    public void SetGameToStage(int stageIndex)
    {
        ActiveCamera.SetPosition(stageIndex);
        ActivePlayers.ForEach(player =>
        {
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            playerMovement.SetPosition(stageIndex);
            playerMovement.ResetEntityActions();
        });
    }

    public void PauseGame()
    {
        SceneChanger.LoadPauseMenuAdditive();
    }

    public void FreezeGame()
    {
        Time.timeScale = 0;
        LockPlayerInput(true);
    }

    public void UnfreezeGame()
    {
        Time.timeScale = 1;
        LockPlayerInput(false);
    }

    public void HandleDeath(GameObject diedObject)
    {
        // TODO: remove at end of project! Just for quicker testing and debugging reasonst of singleplayer stages!
        ActiveGame = ActiveGame == null ? Singleplayer.Instance : ActiveGame;

        if (CurrentMode != Mode.MainMenu)
        {
            PlayerDied(diedObject);
        }
        else
        {
            throw new Exception($"Mode is: {CurrentMode} which is an invalid state for an object to die!");
        }
    }

    public void PlayerDied(GameObject deadPlayer)
    {
        LockPlayerInput(true);
        m_lastDiedPlayer = deadPlayer;
        ActiveCamera.FadeOut();
    }

    // This methods prepares the game after a camera fade out before fading in again.
    public void FadedOut()
    {
        ActiveGame.PlayerDied(m_lastDiedPlayer);
        if (CurrentMode != Mode.MainMenu)
        {
            ActiveGame.PrepareGame();
            ActiveCamera.FadeIn();
        }
        m_lastDiedPlayer = null;
    }

    public void FadedIn()
    {
        LockPlayerInput(false);
    }

    //public void GameHasFinished()
    //{
    //    SceneChanger.SetWinningScreenAsActiveScene();
    //}

    public void SwapHudSymbol(GameObject gameObject, Sprite sprite)
    {
        ActiveCamera.SwapHudSymbol(gameObject, sprite);
    }

    //public void DisableEntityCollision(GameObject callingEntity)
    //{
    //    if (CurrentMode == Mode.Multiplayer)
    //    {
    //        ActiveGame.DisableEntityCollision(callingEntity, ActivePlayers[0].layer, ActivePlayers[1].layer);
    //    } 
    //}

    //public void EnableEntityCollision(GameObject callingEntity)
    //{
    //    if (CurrentMode == Mode.Multiplayer)
    //    {
    //        ActiveGame.EnableEntityCollision(callingEntity, ActivePlayers[0].layer, ActivePlayers[1].layer);
    //    }
    //}

    public void LockPlayerInput(bool isLocked)
    {
        ActivePlayers.ForEach(player => player.GetComponent<PlayerMovement>().IsInputLocked = isLocked);
    }
}
