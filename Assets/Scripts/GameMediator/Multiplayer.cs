using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// This class contains the Multiplayer game mode logic.
public class Multiplayer : MonoBehaviour, IGame
{
    [SerializeField]
    private const int m_amountOfStages = 5;
    [SerializeField]
    private GameObject m_sceneImageHolder;

    private int m_currentStageIndex;
    private HashSet<GameObject> m_entitiesThatRequestedDisableEntityCollision = new HashSet<GameObject>();

    // The GameMediator gets prepared for this game mode.
    // This should be done on awake for safety reasons.
    void Awake()
    {
        GameMediator.Instance.ActiveGame = this;
        GameMediator.Instance.CurrentMode = Mode.Multiplayer;
        ImageManager.Instance.ImageHolder = m_sceneImageHolder;
        ImageManager.Instance.IsFirstLoad = true;
    }

    void Start()
    {
        ResetGame();
        PrepareGame();
    }

    public void ResetGame()
    {
        m_currentStageIndex = m_amountOfStages / 2;
    }

    public void PrepareGame()
    {
        ImageManager.Instance.SetNewSceneImages();
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
            m_currentStageIndex >= m_amountOfStages)
        {
            List<GameObject> players = GameMediator.Instance.ActivePlayers;
            if(players.Count > 2)
            {
                throw new Exception("ERROR: More than two players registered, cannot decide who has won.");
            }

            GameObject winningPlayer = player == players.First() ? players.Last() : players.First();
            // Reset the game to avoid OutOfRangeException with m_currentStageIndex.
            ResetGame();
            
            Toolkit.WinnerIndex = winningPlayer.name.Substring(6);
            GameMediator.Instance.GameHasFinished();
        }
    }

    public void EnableEntityCollision(GameObject callingEntity, int layer1, int layer2)
    {
        m_entitiesThatRequestedDisableEntityCollision.Remove(callingEntity);
        if (m_entitiesThatRequestedDisableEntityCollision.Count == 0)
        {
            Physics2D.IgnoreLayerCollision(layer1, layer2, false);
        }
    }

    public void DisableEntityCollision(GameObject callingEntity, int layer1, int layer2)
    {
        m_entitiesThatRequestedDisableEntityCollision.Add(callingEntity);
        Physics2D.IgnoreLayerCollision(layer1, layer2, true);
    }
}
