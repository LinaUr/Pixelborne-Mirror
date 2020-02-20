using System.Collections.Generic;
using UnityEngine;

// This class contains the Multiplayer game mode logic.
public class Singleplayer : ScriptableObject, IGame
{
    private int m_currentStageIndex;
    private HashSet<GameObject> m_entitiesThatRequestedDisableEntityCollision = new HashSet<GameObject>();

    private static Singleplayer m_instance = null;


    public static Singleplayer Instance
    {
        get
        {
            // A ScriptableObject should not be instanciated directly,
            // so we use CreateInstance instead.
            return m_instance == null ? CreateInstance<Singleplayer>() : m_instance;
        }
    }

    public void Start()
    {
        GameMediator.Instance.ActiveGame = this;
        GameMediator.Instance.CurrentMode = Mode.Singleplayer;
        //ImageManager.Instance.ImageHolder = m_sceneImageHolder;
        //ImageManager.Instance.IsFirstLoad = true;

        // Activate DriveMusicManager.
        DriveMusicManager.Instance.Go();

        ResetGame();
        PrepareGame();
    }

    public void ResetGame()
    {
        m_currentStageIndex = 0;
    }

    public void PrepareGame()
    {
        bool isStageExistent = SceneChanger.LoadSingleplayerStageAsActiveScene(m_currentStageIndex);
        if (!isStageExistent)
        {
            SceneChanger.SetWinningScreenAsActiveScene();
            ResetGame();
        }
    }

    public void PlayerDied(GameObject player)
    {
        SceneChanger.LoadSellingScreenAdditive();
    }

    public void DisableEntityCollision(GameObject callingEntity, int layer1, int layer2)
    {
        m_entitiesThatRequestedDisableEntityCollision.Remove(callingEntity);
        if (m_entitiesThatRequestedDisableEntityCollision.Count == 0)
        {
            Physics2D.IgnoreLayerCollision(layer1, layer2, false);
        }
    }

    public void EnableEntityCollision(GameObject callingEntity, int layer1, int layer2)
    {
        m_entitiesThatRequestedDisableEntityCollision.Add(callingEntity);
        Physics2D.IgnoreLayerCollision(layer1, layer2, true);
    }

    public void EndOfStageIsReached()
    {
        m_currentStageIndex++;
        PrepareGame();
    }
}
