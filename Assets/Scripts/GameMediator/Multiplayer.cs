using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// This class contains the Multiplayer game mode logic.
public class Multiplayer : MonoBehaviour, IGame
{
    //[SerializeField]
    //private const int m_amountOfStages = 5;
    //[SerializeField]
    //private GameObject m_sceneImageHolder;


    private int m_currentStageIndex;
    private List<GameObject> m_players = new List<GameObject>();
    private HashSet<GameObject> m_entitiesThatRequestedDisableEntityCollision = new HashSet<GameObject>();
    private static Multiplayer m_instance;

    private const int m_AMOUNT_OF_STAGES = 5;

    public CameraMultiplayer Camera { get; set; }
    public int WinnerIndex { get; set; }

    public static Multiplayer Instance
    {
        get
        {
            // We have to make use of AddComponent because this class derives 
            // from MonoBehaviour.
            if (m_instance == null)
            {
                GameObject go = new GameObject();
                m_instance = go.AddComponent<Multiplayer>();
            }
            return m_instance;
        }
    }


    // The GameMediator gets prepared for this game mode.
    // This should be done on awake for safety reasons.
    void Awake()
    {
        //GameMediator.Instance.ActiveGame = this;
        //GameMediator.Instance.CurrentMode = Mode.Multiplayer;
        //ImageManager.Instance.ImageHolder = m_sceneImageHolder;
        ImageManager.Instance.IsFirstLoad = true;
        // Activate DriveMusicManager.
        DriveMusicManager.Instance.Go();
    }

    void Start()
    {
        ResetGame();
        PrepareGame();
        GameMediator.Instance.LockPlayerInput(false);
    }

    public void RegisterPlayer(GameObject player)
    {
        if (m_players.Count <= 2)
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
            //List<GameObject> players = GameMediator.Instance.ActivePlayers;
            List<GameObject> players = m_players;
            if(players.Count > 2)
            {
                throw new Exception("ERROR: More than two players registered, cannot decide who has won.");
            }

            GameObject winningPlayer = player == players.First() ? players.Last() : players.First();
            //GameMediator.Instance.WinnerIndex = winningPlayer.GetComponent<PlayerMovement>().Index;
            WinnerIndex = winningPlayer.GetComponent<PlayerMovement>().Index;
            GameMediator.Instance.GameHasFinished();
            GameMediator.Instance.GameHasFinished();

            // Reset the game to avoid OutOfRangeException with m_currentStageIndex.
            ResetGame();
        }
    }

    //public void EnableEntityCollision(GameObject callingEntity, int layer1, int layer2)
    public void EnableEntityCollision(GameObject callingEntity)
    {
        m_entitiesThatRequestedDisableEntityCollision.Remove(callingEntity);
        if (m_entitiesThatRequestedDisableEntityCollision.Count == 0)
        {
            //Physics2D.IgnoreLayerCollision(layer1, layer2, false);
            Physics2D.IgnoreLayerCollision(m_players[0].layer, m_players[1].layer, false);
        }
    }

    //public void DisableEntityCollision(GameObject callingEntity, int layer1, int layer2)
    public void DisableEntityCollision(GameObject callingEntity)
    {
        m_entitiesThatRequestedDisableEntityCollision.Add(callingEntity);
        //Physics2D.IgnoreLayerCollision(layer1, layer2, true);
        Physics2D.IgnoreLayerCollision(m_players[0].layer, m_players[1].layer, true);
    }
}
