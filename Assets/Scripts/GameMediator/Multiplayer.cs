using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// This class contains the Multiplayer game mode logic.
public class Multiplayer : MonoBehaviour, IGame
{
    private int m_currentStageIndex;
    private HashSet<GameObject> m_entitiesThatRequestedDisableEntityCollision = new HashSet<GameObject>();

    private const int m_AMOUNT_OF_STAGES = 3;
    private static int PLAYER_1_LAYER;
    private static int PLAYER_2_LAYER;

    // The GameMediator gets prepared for this game mode.
    // This should be done on awake for safety reasons.
    void Awake()
    {
        GameMediator.Instance.ActiveGame = this;
        GameMediator.Instance.CurrentMode = Mode.Multiplayer;
        PLAYER_1_LAYER = LayerMask.NameToLayer("Player_1");
        PLAYER_2_LAYER = LayerMask.NameToLayer("Player_2");
    }

    void Start()
    {
        ResetGame();
        PrepareGame();
    }

    public void ResetGame()
    {
        m_currentStageIndex = m_AMOUNT_OF_STAGES / 2;
    }

    public void PrepareGame()
    {
        GameMediator.Instance.SetGameToStage(m_currentStageIndex);
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
            List<GameObject> players = GameMediator.Instance.ActivePlayers;
            if(players.Count > 2)
            {
                throw new Exception("ERROR: More than two players registered, cannot decide who has won.");
            }

            GameObject winningPlayer = player == players.First() ? players.Last() : players.First();
            // Reset the game to avoid OutOfRangeException with m_currentStageIndex.
            ResetGame();

            Debug.Log($"{winningPlayer.name} has won the game!");
            GameMediator.Instance.GameHasFinished();
        }
    }

    public void EnableEntityCollision(GameObject callingEntity)
    {
        m_entitiesThatRequestedDisableEntityCollision.Add(callingEntity);
        Physics2D.IgnoreLayerCollision(PLAYER_1_LAYER, PLAYER_2_LAYER, true);
    }
    public void DisableEntityCollision(GameObject callingEntity)
    {
        m_entitiesThatRequestedDisableEntityCollision.Remove(callingEntity);
        if (m_entitiesThatRequestedDisableEntityCollision.Count == 0)
        {
            Physics2D.IgnoreLayerCollision(PLAYER_1_LAYER, PLAYER_2_LAYER, false);
        }
    }
}
