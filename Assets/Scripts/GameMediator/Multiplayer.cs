using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

// This class contains the Multiplayer game mode logic.
public class Multiplayer : ScriptableObject, IGame
{
    private GameObject m_deadPlayer;
    private HashSet<GameObject> m_entitiesThatRequestedDisableEntityCollision = new HashSet<GameObject>();
    private int m_currentStageIndex = m_START_STAGE_INDEX;
    private int m_winnerIndex;
    private List<GameObject> m_players = new List<GameObject>();
    private static Multiplayer s_instance;

    private const int m_AMOUNT_OF_STAGES = 5;
    private const int m_START_STAGE_INDEX = m_AMOUNT_OF_STAGES / 2;

    public CameraMultiplayer Camera { get; set; }

    public static Multiplayer Instance
    {
        get
        {
            // A ScriptableObject should not be instanciated directly,
            // so we use CreateInstance instead.
            return s_instance == null ? CreateInstance<Multiplayer>() : s_instance;
        }
    }

    // This is for testing and debugging single stages quicker without having to start from the MainMenu.
    // TODO: Remove later.
    public int DEBUG_currentStageIndex
    {
        set
        {
            m_currentStageIndex = value;
        }
    }

    public Multiplayer()
    {
        s_instance = this;
    }

    public async void Go()
    {
        Game.Current = this;
        Game.Mode = GameMode.Multiplayer;

        SceneChanger.SetMultiplayerAsActiveScene();

        // Activate DriveMusicManager.
        DriveMusicManager.Instance.Go();

        CancellationTokenSource cts = new CancellationTokenSource();
        cts.CancelAfter(5000);
        await Task.Run(() =>
        {
            // Wait until both players have been registered.
            while (m_players.Count < 2)
            {
                if (cts.Token.IsCancellationRequested)
                {
                    throw new Exception("Error: Player did not register at multiplayer within time.");
                }
            };
        }, cts.Token);

        // If the Task returns when the application has been quit the reference of this is null 
        // which can throw an error if we do not check on this.
        if (this != null)
        {
            PrepareStage();
            LockPlayerInput(false);
        }
    }

    public string GetWinner()
    {
        return $"Player {m_winnerIndex}";
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

    public void UnregisterPlayer(GameObject player)
    {
        m_players.Remove(player);
    }

    public void LockPlayerInput(bool isLocked)
    {
        m_players.ForEach(player =>
        {
            var playerMovement = player.GetComponent<PlayerMovement>();
            playerMovement.IsInputLocked = isLocked;
            playerMovement.ResetEntityAnimations();
        });
    }

    public void HandleDeath(GameObject player)
    {
        LockPlayerInput(true);
        m_deadPlayer = player;
        Camera.FadeOut();
    }

    // This method prepares the game after a camera fade out before fading in again.
    public void FadedOut()
    {
        PlayerDied(m_deadPlayer);
        PrepareStage();
        Camera.FadeIn();
        m_deadPlayer = null;
    }

    public void FadedIn()
    {
        LockPlayerInput(false);
    }

    private void ResetGame()
    {
        m_currentStageIndex = m_START_STAGE_INDEX;
    }

    public void PrepareStage()
    {
        ImageManager.Instance.SetNewSceneImages();
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
        if (m_currentStageIndex < 0 || m_currentStageIndex >= m_AMOUNT_OF_STAGES)
        {
            List<GameObject> players = m_players;
            if (players.Count > 2)
            {
                throw new Exception("ERROR: More than two players registered, cannot decide who has won.");
            }

            GameObject winningPlayer = player == players.First() ? players.Last() : players.First();
            m_winnerIndex = winningPlayer.GetComponent<PlayerMovement>().Index;
            Game.Finish();

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
        Camera.SwapHudSymbol(gameObject, sprite);
    }
}
