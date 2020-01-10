using UnityEngine;
using System;

// This class serves as a mediator between various components of the game.
public class GameMediator : MonoBehaviour
{
    [SerializeField]
    private CameraMover m_cameraMover;
    [SerializeField]
    private PlayerMovement m_player1Movement;
    [SerializeField]
    private PlayerMovement m_player2Movement;

    private MultiPlayer m_multiPlayer;
    private GameObject m_lastDiedPlayer;
    private RecordAudio m_audioRecorder;
    private WebcamPhoto m_photoRecorder;

    private enum CurrentMode
    {
        None,
        MainMenu,
        SinglePlayer,
        MultiPlayer
    };

    private CurrentMode Mode { get; set; }

    void Start()
    {
        Mode = CurrentMode.MainMenu;
        m_audioRecorder = gameObject.AddComponent<RecordAudio>();
        m_photoRecorder = gameObject.AddComponent<WebcamPhoto>();
    }

    public void StartMultiplayer()
    {
        Mode = CurrentMode.MultiPlayer;
        m_multiPlayer = gameObject.GetComponent<MultiPlayer>();
        m_multiPlayer.StartMultiplayer();
    }

    public void StopMultiplayer()
    {
        Mode = CurrentMode.MainMenu;
        m_player1Movement = null;
        m_player2Movement = null;
        m_multiPlayer.StopMultiplayer();
    }

    public void Record()
    {
        m_audioRecorder.Record();
        m_photoRecorder.Record();
    }

    public void HandleDeath(GameObject diedObject)
    {
        switch (Mode)
        {
            case CurrentMode.MultiPlayer:
                PlayerDied(diedObject);
                break;
            case CurrentMode.None:
            default:
                throw new Exception($"Mode is: {Mode} which is an invalid state!");
        }
    }

    public void PlayerDied(GameObject player)
    {
        m_player1Movement.m_inputIsLocked = true;
        m_player2Movement.m_inputIsLocked = true;
        m_lastDiedPlayer = player;
        m_cameraMover.FadeOut();
    }

    // This methods reacts to one player successfully winning the multiplayer game. 
    public void TriggerWin(GameObject player)
    {
        Mode = CurrentMode.MainMenu;
        Debug.Log($"{player.name} has won the game!");
        Application.Quit();
    }

    // This methods prepares the Multiplayer after a camera fade out before fading in again.
    public void FadedOut()
    {
        m_multiPlayer.PlayerDied(m_lastDiedPlayer);
        if (Mode != CurrentMode.MainMenu)
        {
            m_multiPlayer.SetPlayerPositions();
            m_multiPlayer.ResetPlayersActions();
            m_multiPlayer.SetCameraPosition();
            m_cameraMover.FadeIn();
        }
        m_lastDiedPlayer = null;
    }

    // This methods allows players to accept inputs again when the camera has successfully faded in.
    public void FadedIn()
    {
        m_player1Movement.m_inputIsLocked = false;
        m_player2Movement.m_inputIsLocked = false;
    }
}
