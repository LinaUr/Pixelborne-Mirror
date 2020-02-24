using System;
using System.Collections.Generic;
using UnityEngine;

// This class contains the Multiplayer game mode logic.
public class Singleplayer : ScriptableObject, IGame
{
    private int m_currentStageIndex;
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

    public Singleplayer()
    {
        m_instance = this;
    }

    public void Go()
    {
        Game.Current = Instance;
        //ImageManager.Instance.ImageHolder = m_sceneImageHolder;
        //ImageManager.Instance.IsFirstLoad = true;

        // Activate DriveMusicManager.
        //DriveMusicManager.Instance.Go();

        ResetGame();
        PrepareGame();
    }

    public void RegisterPlayer(GameObject player)
    {
        if (Player == null)
        {
            Player = player;
        }
        else
        {
            throw new Exception($"Error: Object \"{player.name}\" can not be registered. Player has already been assigned.");
        }
    }

    public void UnegisterPlayer(GameObject player)
    {
        Player = null;
    }

    public void LockPlayerInput(bool isLocked)
    {
        Player.GetComponent<PlayerMovement>().IsInputLocked = isLocked;
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

    public void ResetGame()
    {
        m_currentStageIndex = 0;
    }

    public void PrepareGame()
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
                Game.HasFinished();
                ResetGame();
            }
        }
    }

    private void ResetCurrentStage()
    {
        //Camera.SetPosition(stageIndex);
        PlayerMovement playerMovement = Player.GetComponent<PlayerMovement>();
        // Set player position to start point of stage.
        playerMovement.SetPosition(0);
        playerMovement.ResetEntityActions();
    }

    private void EnemyDied()
    {

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

    public void ReachedEndOfStage()
    {
        m_currentStageIndex++;
        PrepareGame();
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
