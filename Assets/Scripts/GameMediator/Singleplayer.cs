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
        Game.Mode = Mode.Singleplayer;

        // Activate DriveMusicManager.
        DriveMusicManager.Instance.Go();

        PrepareStage();
    }

    public void RegisterPlayer(GameObject player)
    {
        if (Player == null)
        {
            Player = player;
            ImageManager.Instance.PlayerSpawnPosition = player.transform.position;
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

    public void LockPlayerInput(bool isLocked)
    {
        Player.GetComponent<PlayerMovement>().IsInputLocked = isLocked;
        foreach(GameObject enemy in ActiveEnemies)
        {
            enemy.GetComponent<EnemyAttackAndMovement>().IsInputLocked = isLocked;
        }
    }

    public void HandleDeath(GameObject entity)
    {
        if (entity == Player)
        {
            IsPlayerDead = true;
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
        PlayerMovement playerMovement = Player.GetComponent<PlayerMovement>();
        // Set player position to start point of stage.
        playerMovement.SetPosition(0);
        playerMovement.ResetEntityActions();
    }

    public void ReachedEndOfStage()
    {
        m_currentStageIndex++;
        PrepareStage();
    }

    public void EnableEntityCollision(GameObject callingEntity)
    {
        m_entitiesThatRequestedDisableEntityCollision.Remove(callingEntity);
        if (m_entitiesThatRequestedDisableEntityCollision.Count == 0)
        {
            // TODO: 2nd Layer is Enemy
            Physics2D.IgnoreLayerCollision(Player.layer, Player.layer, false);
        }
    }

    public void DisableEntityCollision(GameObject callingEntity)
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
