using System;
using UnityEngine;

// This class contains the Multiplayer game mode logic.
public class Multiplayer : MonoBehaviour, IGame
{
    [SerializeField]
    private CameraMultiplayer m_cameraMultiplayer;
    [SerializeField]
    private GameObject m_player1;
    [SerializeField]
    private GameObject m_player2;

    private PlayerMovement m_player1Movement;
    private PlayerMovement m_player2Movement;
    // Multiplayer stages are indexed from far left to far right as they are in the scene.
    private int m_currentStageIndex;
    private const int m_PLAYER_DISTANCE_TO_CENTER_X = 7;
    private const int m_PLAYER_DISTANCE_TO_CENTER_Y = 0;

    // The GameMediator gets prepared for this game mode.
    // This should be done on awake for safety reasons.
    void Awake()
    {
        GameMediator.Instance.ActiveGame = this;
        GameMediator.Instance.ActiveCamera = m_cameraMultiplayer;
        GameMediator.Instance.CurrentMode = Mode.Multiplayer;
    }

    void Start()
    {
        m_player1Movement = m_player1.GetComponent<PlayerMovement>();
        m_player2Movement = m_player2.GetComponent<PlayerMovement>();

        ResetGame();
        PrepareGame();
    }

    public void ResetGame()
    {
        m_currentStageIndex = m_cameraMultiplayer.Positions.Count / 2;
    }

    public void PrepareGame()
    {
        // Since the player positions are based on the camera position we have to set the camera first.
        SetCameraPosition();
        SetPlayerPositions(m_cameraMultiplayer.transform.position);
        ResetPlayersActions();
    }

    private void SetCameraPosition()
    {
        m_cameraMultiplayer.MoveCamera(m_currentStageIndex);
    }

    private void SetPlayerPositions(Vector2 centerPosition)
    {
        Vector2 spawnDistance = new Vector2(m_PLAYER_DISTANCE_TO_CENTER_X, m_PLAYER_DISTANCE_TO_CENTER_Y);
        m_player1Movement.SetPosition(centerPosition - spawnDistance);
        m_player2Movement.SetPosition(centerPosition + spawnDistance);
    }

    private void ResetPlayersActions()
    {
        m_player1Movement.ResetPlayerActions();
        m_player2Movement.ResetPlayerActions();
    }

    public void PlayerDied(GameObject player)
    {
        if (player == m_player1)
        {
            m_currentStageIndex--;
        }
        else if (player == m_player2)
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
            m_currentStageIndex >= m_cameraMultiplayer.Positions.Count)
        {
            GameObject winningPlayer = player == m_player1 ? m_player2 : m_player1;

            Debug.Log($"{winningPlayer.name} has won the game!");
            GameMediator.Instance.GameHasFinished();
        }
    }

    public void LockPlayerInput(bool isLocked)
    {
        m_player1Movement.InputIsLocked = isLocked;
        m_player2Movement.InputIsLocked = isLocked;
    }
}
