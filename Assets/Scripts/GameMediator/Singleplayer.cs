using System;
using System.Collections.Generic;
using UnityEngine;

// This class contains the Singleplayer game mode logic.
public class Singleplayer : ScriptableObject, IGame
{
    private const int m_START_STAGE_INDEX = 0;

    private int m_currentStageIndex = m_START_STAGE_INDEX;
    private HashSet<GameObject> m_entitiesThatRequestedDisableEntityCollision = new HashSet<GameObject>();
    private static Singleplayer m_instance = null;
    private Vector2 m_playerRevivePosition;
    private PlayerMovement m_playerMovement;

    public bool IsPlayerDead { get; set; }
    public List<GameObject> ActiveEnemies { get; set; } = new List<GameObject>();
    public GameObject Player { get; set; } = null;
    public float PriceToPay { get; set; }
    public CameraSingleplayer Camera { get; set; }

    

    public static Singleplayer Instance
    {
        get
        {
            // A ScriptableObject should not be instanciated directly,
            // so we use CreateInstance instead.
            return m_instance == null ? CreateInstance<Singleplayer>() : m_instance;
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

    public Singleplayer()
    {
        m_instance = this;
    }

    public void Go()
    {
        Game.Current = Instance;

        // Activate DriveMusicManager.
        DriveMusicManager.Instance.Go();

        PrepareStage();
    }

    public void RegisterPlayer(GameObject player)
    {
        if (Player == null)
        {
            Player = player;
            m_playerMovement = player.GetComponent<PlayerMovement>();
        }
        else
        {
            throw new Exception($"Error: Object \"{player.name}\" can not be registered. Player has already been assigned.");
        }
    }

    public void UnregisterPlayer(GameObject player)
    {
        Player = null;
    }

    public void RevivePlayer()
    {
        IsPlayerDead = false;
        m_playerMovement.SetRevivePosition(m_playerRevivePosition);
        m_playerMovement.ResetEntityActions();
    }

    public void LockPlayerInput(bool isLocked)
    {
        m_playerMovement.IsInputLocked = isLocked;
    }

    public void HandleDeath(GameObject entity)
    {
        if (entity == Player)
        {
            IsPlayerDead = true;
            m_playerRevivePosition = m_playerMovement.RevivePosition;
            SceneChanger.LoadSellingScreenAdditive();
        }
        else if (ActiveEnemies.Contains(entity))
        {
            EnemyDied();
        }
    }

    // TODO: implement
    private void EnemyDied()
    {

    }

    private void ResetGame()
    {
        m_currentStageIndex = m_START_STAGE_INDEX;
    }

    public void PrepareStage()
    {
        if (IsPlayerDead)
        {
            ResetCurrentStage();
            IsPlayerDead = false;
        }
        else
        {
            bool isStageExistent = SceneChanger.LoadSingleplayerStageAsActiveScene(m_currentStageIndex);
            if (!isStageExistent)
            {
                Game.Finish();
                ResetGame();
            }
            else
            {
                // Activate DriveMusicManager again.
                DriveMusicManager.Instance.Go();
            }
        }
    }

    private void ResetCurrentStage()
    {
        // Set player position to start point of stage.
        m_playerMovement.SetPosition(0);
        m_playerMovement.ResetEntityActions();
    }

    public void ReachedEndOfStage()
    {
        m_currentStageIndex++;
        PrepareStage();
    }

    public void DisableEntityCollision(GameObject callingEntity)
    {
        m_entitiesThatRequestedDisableEntityCollision.Remove(callingEntity);
        if (m_entitiesThatRequestedDisableEntityCollision.Count == 0)
        {
            // TODO: 2nd Layer is Enemy
            Physics2D.IgnoreLayerCollision(Player.layer, Player.layer, false);
        }
    }

    public void EnableEntityCollision(GameObject callingEntity)
    {
        m_entitiesThatRequestedDisableEntityCollision.Add(callingEntity);
        // TODO: 2nd Layer is Enemy
        Physics2D.IgnoreLayerCollision(Player.layer, Player.layer, true);
    }

    public void SwapHudSymbol(GameObject gameObject, Sprite sprite)
    {
        Camera.SwapHudSymbol(gameObject, sprite);
    }

    public string GetWinner()
    {
        return $"You";
    }
}
