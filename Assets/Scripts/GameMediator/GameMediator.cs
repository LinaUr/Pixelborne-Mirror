using UnityEngine;
using System;
using Assets.Scripts.Tools;

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
            return (m_instance == null) ? new GameMediator() : m_instance;
        }
    }

    // Every scene need to have a corresponding script that set the mode
    public Mode CurrentMode { get; set; }
    public IGame ActiveGame { get; set; }
    public ICamera ActiveCamera { get; set; }

    public GameMediator()
    {
        m_instance = this;
    }

    public void HandleDeath(GameObject diedObject)
    {
        if (CurrentMode != Mode.MainMenu)
        {
            PlayerDied(diedObject);
        }
        else
        {
            throw new Exception($"Mode is: {CurrentMode} which is an invalid state for an object to die!");
        }
    }

    public void PlayerDied(GameObject player)
    {
        ActiveGame.LockPlayerInput(true);
        m_lastDiedPlayer = player;
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
        ActiveGame.LockPlayerInput(false);
    }

    public void GameHasFinished()
    {
        SceneChanger.SetMainMenuAsActiveScene();
    }
}
