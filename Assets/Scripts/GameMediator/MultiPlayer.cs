using System.Collections.Generic;
using UnityEngine;

// This class contains the Multiplayer game mode logic.
public class MultiPlayer : MediatableMonoBehavior
{
    [SerializeField]
    private CameraMover m_cameraMover;
    [SerializeField]
    // Transforms from outer left to outer right stage.
    private Transform m_cameraPositionsTransform;
    [SerializeField]
    private GameObject m_player1;
    [SerializeField]
    private GameObject m_player2;

    // Positions from outer left to outer right stage.
    private IList<Vector2> m_cameraPositions;
    private PlayerMovement m_player1Movement;
    private PlayerMovement m_player2Movement;
    private int m_currentCameraPositionIndex;
    private const int m_PLAYER_DISTANCE_TO_CENTER_X = 7;
    private const int m_PLAYER_DISTANCE_TO_CENTER_Y = 0;

    void Start()
    {
        m_player1Movement = m_player1.GetComponent<PlayerMovement>();
        m_player2Movement = m_player2.GetComponent<PlayerMovement>();

        m_cameraPositions = new List<Vector2>();
        foreach (Transform positionsTransform in m_cameraPositionsTransform)
        {
            m_cameraPositions.Add(positionsTransform.position);
        }

        ResetMultiplayer();
    }

    public void PlayerDied(GameObject player)
    {
        if (player == m_player1)
        {
            m_currentCameraPositionIndex--;
        }
        else if (player == m_player2)
        {
            m_currentCameraPositionIndex++;
        }
        else
        {
            Debug.Log("ERROR no player was given!");
        }
        TestForWin(player);
    }

    private void TestForWin(GameObject player)
    {
        if (m_currentCameraPositionIndex < 0 ||
            m_currentCameraPositionIndex >= m_cameraPositions.Count)
        {
            GameObject winningPlayer = player == m_player1 ? m_player2 : m_player1;
            gameMediator.TriggerWin(winningPlayer);
            StopMultiplayer();
        }
    }

    public void SetPlayerPositions()
    {
        Vector2 cameraPosition = m_cameraPositions[m_currentCameraPositionIndex];
        Vector2 spawnDistance = new Vector2(m_PLAYER_DISTANCE_TO_CENTER_X, m_PLAYER_DISTANCE_TO_CENTER_Y);
        m_player1Movement.SetPosition(cameraPosition - spawnDistance);
        m_player2Movement.SetPosition(cameraPosition + spawnDistance);
    }

    public void ResetPlayersActions()
    {
        m_player1Movement.ResetPlayerActions();
        m_player2Movement.ResetPlayerActions();
    }

    public void SetCameraPosition()
    {
        Vector2 newPosition = m_cameraPositions[m_currentCameraPositionIndex];
        m_cameraMover.MoveCamera(newPosition.x, newPosition.y);
    }

    public void StartMultiplayer()
    {
        ResetMultiplayer();
        SetPlayerPositions();
        SetCameraPosition();
    }

    public void ResetMultiplayer()
    {
        m_currentCameraPositionIndex = m_cameraPositions.Count / 2;
    }

    public void StopMultiplayer()
    {
        ResetMultiplayer();
        m_player1 = null;
        m_player2 = null;
        m_cameraMover = null;
    }
}
