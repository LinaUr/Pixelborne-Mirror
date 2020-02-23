using System;
using System.Collections.Generic;
using UnityEngine;

// This class contains the Multiplayer game mode logic.
public class Singleplayer : ScriptableObject, IGame
{
    private int m_currentStageIndex;
    private HashSet<GameObject> m_entitiesThatRequestedDisableEntityCollision = new HashSet<GameObject>();

    private GameObject m_player;
    private static Singleplayer m_instance = null;

    public bool IsPlayerDead { get; set; }
    public List<GameObject> ActiveEnemies { get; set; } = new List<GameObject>();
    public float PriceToPay { get; set; }
    public CameraSingleplayer Camera { get; set; }

    public static Singleplayer Instance
    {
        get
        {
            // A ScriptableObject should not be instanciated directly,
            // so we use CreateInstance instead.
            //return m_instance == null ? CreateInstance<Singleplayer>() : m_instance;

            // The following code is for quicker testing and debugging in single singleplayer stages,
            // so you don't always have to start playing from the MainMenu.
            // TODO: remove later!
            // --start--
            m_instance = m_instance == null ? CreateInstance<Singleplayer>() : m_instance;
            //GameMode.Instance.Current = Mode.Singleplayer;
            return m_instance;
            // --end--
        }
    }

    public Singleplayer()
    {
        m_instance = this;
    }

    public void Go()
    {
        //GameMediator.Instance.ActiveGame = Instance;
        //GameMediator.Instance.CurrentMode = Mode.Singleplayer;
        //ImageManager.Instance.ImageHolder = m_sceneImageHolder;
        //ImageManager.Instance.IsFirstLoad = true;

        // Activate DriveMusicManager.
        DriveMusicManager.Instance.Go();

        ResetGame();
        PrepareGame();
    }

    public void RegisterPlayer(GameObject player)
    {
        if (m_player)
        {
            m_player = player;
        }
        else
        {
            throw new Exception($"Error: Object \"{player.name}\" can not be registered. Player has already been assigned.");
        }
    }

    public void UnegisterPlayer(GameObject player)
    {
        m_player = null;
    }

    public void ResetGame()
    {
        m_currentStageIndex = 0;
    }

    public void PrepareGame()
    {
        if (IsPlayerDead)
        {
            GameMediator.Instance.SetGameToStage(0);
            IsPlayerDead = false;
        }
        else
        {
            bool isStageExistent = SceneChanger.LoadSingleplayerStageAsActiveScene(m_currentStageIndex);

            if (!isStageExistent)
            {
                SceneChanger.SetWinningScreenAsActiveScene();
                ResetGame();
            }
        }
    }

    public void PlayerDied(GameObject player)
    {
        IsPlayerDead = true;
        SceneChanger.LoadSellingScreenAdditive();
    }

    public void DisableEntityCollision(GameObject callingEntity)
    {
        m_entitiesThatRequestedDisableEntityCollision.Remove(callingEntity);
        if (m_entitiesThatRequestedDisableEntityCollision.Count == 0)
        {
            // TODO: 2nd Layer is Enemy
            Physics2D.IgnoreLayerCollision(m_player.layer, m_player.layer, false);
        }
    }

    public void EnableEntityCollision(GameObject callingEntity)
    {
        m_entitiesThatRequestedDisableEntityCollision.Add(callingEntity);
        // TODO: 2nd Layer is Enemy
        Physics2D.IgnoreLayerCollision(m_player.layer, m_player.layer, true);
    }

    public void ReachedEndOfStage()
    {
        m_currentStageIndex++;
        PrepareGame();
    }
}
