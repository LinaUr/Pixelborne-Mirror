﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

// This class contains the Multiplayer game mode logic.
public class Multiplayer : ScriptableObject, IGame
{
    private int m_currentStageIndex;
    private List<GameObject> m_players = new List<GameObject>();
    private HashSet<GameObject> m_entitiesThatRequestedDisableEntityCollision = new HashSet<GameObject>();
    private static Multiplayer m_instance;
    private GameObject m_deadPlayer;
    private int m_winnerIndex;

    private const int m_AMOUNT_OF_STAGES = 5;

    public CameraMultiplayer Camera { get; set; }

    //public static Multiplayer Instance
    //{
    //    get
    //    {
    //        // We have to make use of AddComponent because this class derives 
    //        // from MonoBehaviour.
    //        if (m_instance == null)
    //        {
    //            GameObject go = new GameObject();
    //            m_instance = go.AddComponent<Multiplayer>();
    //        }
    //        return m_instance;
    //    }
    //}

    public static Multiplayer Instance
    {
        get
        {
            // A ScriptableObject should not be instanciated directly,
            // so we use CreateInstance instead.
            return m_instance == null ? CreateInstance<Multiplayer>() : m_instance;
        }
    }

    public Multiplayer()
    {
        m_instance = this;
    }

    // This should be done on awake for safety reasons.
    public async void Go()
    {
        Game.Current = this;

        SceneChanger.SetMultiplayerAsActiveScene();

        //ImageManager.Instance.ImageHolder = m_sceneImageHolder;
        //ImageManager.Instance.IsFirstLoad = true;
        // Activate DriveMusicManager.
        //DriveMusicManager.Instance.Go();

        await Task.Run(() =>
        {
            // Wait until both players have been registered.
            while (m_players.Count < 2) { };
        });

        ResetGame();
        PrepareGame();
        LockPlayerInput(false);
    }

    public void RegisterPlayer(GameObject player)
    {
        if (m_players.Count < 2)
        {
            m_players.Add(player);
        }
        else
        {
            throw new Exception($"Error: Object \"{player.name}\" can not be registered. 2 players have already been assigned.");
        }
    }

    public void UnegisterPlayer(GameObject player)
    {
        m_players.Remove(player);
    }

    public void LockPlayerInput(bool isLocked)
    {
        m_players.ForEach(player => player.GetComponent<PlayerMovement>().IsInputLocked = isLocked);
    }

    public void HandleDeath(GameObject player)
    {
        LockPlayerInput(true);
        m_deadPlayer = player;
        Camera.FadeOut();
    }

    // This methods prepares the game after a camera fade out before fading in again.
    public void FadedOut()
    {
        PlayerDied(m_deadPlayer);
        PrepareGame();
        Camera.FadeIn();
        m_deadPlayer = null;
    }

    public void FadedIn()
    {
        LockPlayerInput(false);
    }

    public void ResetGame()
    {
        m_currentStageIndex = m_AMOUNT_OF_STAGES / 2;
    }

    public void PrepareGame()
    {
        //ImageManager.Instance.SetNewSceneImages();
        SetGameToStage(m_currentStageIndex);
    }

    public void PlayerDied(GameObject player)
    {
        int playerIndex = player.GetComponent<PlayerMovement>().Index;
        if (playerIndex == 1)
        {
            m_currentStageIndex--;
        }
        else if (playerIndex == 2)
        {
            m_currentStageIndex++;
        }
        else
        {
            throw new Exception("ERROR no player was given!");
        }
        CheckHasWonGame(player);
    }

    // This methods checks and reacts ot one player successfully winning the Multiplayer game.
    private void CheckHasWonGame(GameObject player)
    {
        if (m_currentStageIndex < 0 ||
            m_currentStageIndex >= m_AMOUNT_OF_STAGES)
        {
            List<GameObject> players = m_players;
            if(players.Count > 2)
            {
                throw new Exception("ERROR: More than two players registered, cannot decide who has won.");
            }

            GameObject winningPlayer = player == players.First() ? players.Last() : players.First();
            m_winnerIndex = winningPlayer.GetComponent<PlayerMovement>().Index;
            Game.HasFinished();

            // Reset the game to avoid OutOfRangeException with m_currentStageIndex.
            ResetGame();
        }
    }

    public void EnableEntityCollision(GameObject callingEntity)
    {
        m_entitiesThatRequestedDisableEntityCollision.Remove(callingEntity);
        if (m_entitiesThatRequestedDisableEntityCollision.Count == 0)
        {
            Physics2D.IgnoreLayerCollision(m_players[0].layer, m_players[1].layer, false);
        }
    }

    public void DisableEntityCollision(GameObject callingEntity)
    {
        m_entitiesThatRequestedDisableEntityCollision.Add(callingEntity);
        Physics2D.IgnoreLayerCollision(m_players[0].layer, m_players[1].layer, true);
    }

    public void SetGameToStage(int stageIndex)
    {
        Camera.SetPosition(stageIndex);
        m_players.ForEach(player =>
        {
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            playerMovement.SetPosition(stageIndex);
            playerMovement.ResetEntityActions();
        });
    }

    public void SwapHudSymbol(GameObject gameObject, Sprite sprite)
    {
        //Camera.SwapHudSymbol(gameObject, sprite);
    }

    public string GetWinner()
    {
        return $"Player {m_winnerIndex}";
    }
}
