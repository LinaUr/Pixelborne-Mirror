using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class contains the Singleplayer game mode logic.
public class Singleplayer : ScriptableObject, IGame
{
    private HashSet<GameObject> m_entitiesThatRequestedDisableEntityCollision = new HashSet<GameObject>();
    private int m_currentStageIndex = m_START_STAGE_INDEX;
    private int m_enemyLayer;
    private PlayerMovement m_playerMovement;
    private Vector2 m_playerRevivePosition;
    private static Singleplayer s_instance = null;

    private const int m_START_STAGE_INDEX = 0;

    public CameraSingleplayer Camera { get; set; }
    public float PriceToPay { get; set; }
    public GameObject Player { get; set; } = null;
    public List<GameObject> ActiveEnemies { get; set; } = new List<GameObject>();

    public static Singleplayer Instance
    {
        get
        {
            // A ScriptableObject should not be instanciated directly,
            // so we use CreateInstance instead.
            if (s_instance == null) 
            {
                s_instance = CreateInstance<Singleplayer>();
                s_instance.m_enemyLayer = LayerMask.NameToLayer("Enemy");
            }
            return s_instance;
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

    private Singleplayer()
    {
        s_instance = this;
    }

    public void Go()
    {
        Game.Current = Instance;
        Game.Mode = Mode.Singleplayer;

        SellingScreen.GetImportantFiles();

        PrepareStage();
    }

    public string GetWinner()
    {
        return $"You";
    }

    public void RegisterPlayer(GameObject player)
    {
        if (Player == null)
        {
            Player = player;
            m_playerMovement = player.GetComponent<PlayerMovement>();
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

    public void RevivePlayer()
    {
        m_playerMovement.SetPositionForRevive(m_playerRevivePosition);
        m_playerMovement.ResetEntityActions();
    }

    public void LockPlayerInput(bool isLocked)
    {
        m_playerMovement.IsInputLocked = isLocked;
        foreach(GameObject enemy in ActiveEnemies)
        {
            enemy.GetComponent<EnemyAttackAndMovement>().IsInputLocked = isLocked;
        }
    }

    public void HandleDeath(GameObject entity)
    {
        //Game.Freeze();
        //StartCoroutine(WaitForDocuments());
        //Debug.Log(SellingScreen.s_isLoadingPaths);
        if (entity == Player/* && !SellingScreen.s_isLoadingPaths*/)
        {
            //Game.Unfreeze();
            m_playerRevivePosition = m_playerMovement.RevivePosition;
            SceneChanger.LoadSellingScreenAdditive();
        }
        else 
        {
            throw new ArgumentException($"Expected player as argument but got: {entity}");
        }
    }

    /*private IEnumerator WaitForDocuments()
    {
        yield return new WaitUntil(() => SellingScreen.s_isLoadingPaths == false);
    }*/

    public void ResetGame()
    {
        m_currentStageIndex = m_START_STAGE_INDEX;
    }

    public void PrepareStage()
    {
        bool isStageExistent = SceneChanger.LoadSingleplayerStageAsActiveScene(m_currentStageIndex);
        if (!isStageExistent)
        {
            Game.Finish();
            ResetGame();
            return;
        }

        // Activate DriveMusicManager.
        DriveMusicManager.Instance.Go();
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
            Physics2D.IgnoreLayerCollision(Player.layer, m_enemyLayer, false);
        }
    }

    public void DisableEntityCollision(GameObject callingEntity)
    {
        m_entitiesThatRequestedDisableEntityCollision.Add(callingEntity);
        Physics2D.IgnoreLayerCollision(Player.layer, m_enemyLayer, true);
    }

    public void SwapHudSymbol(GameObject gameObject, Sprite sprite)
    {
        Camera.SwapHudSymbol(gameObject, sprite);
    }
}
