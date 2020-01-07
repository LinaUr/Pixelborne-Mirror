﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameMediator : MonoBehaviour
{
    // TODO: würde hier so ziemlich alles (zumindestest vieles) in public variablen übergeben und nicht selbst im code suchen.
    private GameObject _player1Object;
    private GameObject _player2Object;
    private GameObject lastDiedPlayer;
    private PlayerMovement _player1Movement;
    private PlayerMovement _player2Movement;
    private RecordAudio _audioRecorder;
    private WebcamPhoto _photoRecorder;
    private CurrentMode mode;
    private CameraMover cameraMover;
    private MultiPlayer multiPlayer;
    public enum CurrentMode { None, MainMenu, SinglePlayer, MultiPlayer };

    public CurrentMode Mode {
        get { return mode; }
        set { mode = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        mode = CurrentMode.MainMenu;
        GameObject cameraObject = GameObject.Find("Main Camera"); // TODO: lieber das Obejct in einer Public Variable übergeben
        cameraMover = cameraObject.GetComponent<CameraMover>();

        _audioRecorder = gameObject.AddComponent<RecordAudio>();
        _photoRecorder = gameObject.AddComponent<WebcamPhoto>();
    }

    public void StartMultiplayer(){
        mode = CurrentMode.MultiPlayer;
        
        // get Multiplayer references
        _player1Object = GameObject.Find("Players/Player1_Blue");
        _player1Movement = _player1Object.GetComponent<PlayerMovement>();
        _player2Object = GameObject.Find("Players/Player2_Blue");
        _player2Movement = _player2Object.GetComponent<PlayerMovement>();
        multiPlayer = gameObject.GetComponent<MultiPlayer>();

        multiPlayer.StartMultiplayer();
    }

    public void StopMultiplayer(){
        mode = CurrentMode.MainMenu;
        _player1Object = null;
        _player1Movement = null;
        _player2Object = null;
        _player2Movement = null;
        multiPlayer.StopMultiplayer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Record(){
        _audioRecorder.Record();
        _photoRecorder.Record();
    }

    public void HandleDeath(GameObject diedObject){
        switch(mode){
            case CurrentMode.MultiPlayer:
                PlayerDied(diedObject);
                break;
            case CurrentMode.None:
            default:
                throw new Exception("Mode is: " + mode + " which is an invalid state!");
        }
    }

    public void PlayerDied(GameObject player){
        _player1Movement.m_inputIsLocked = true;
        _player2Movement.m_inputIsLocked = true;
        lastDiedPlayer = player;
        cameraMover.FadeOut();
    }

    public void TriggerWin(GameObject player){
        mode = CurrentMode.MainMenu;
        Debug.Log(player.name + " has won the game!");
        Application.Quit();
    }

    public void FadedOut() {
        multiPlayer.PlayerDied(lastDiedPlayer);
        if(mode != CurrentMode.MainMenu){
            multiPlayer.SetPlayerPositions();
            multiPlayer.ResetPlayersActions();
            multiPlayer.SetCameraPosition();
            cameraMover.FadeIn();
        }
        lastDiedPlayer = null;
    }

    public void FadedIn() {
        _player1Movement.m_inputIsLocked = false;
        _player2Movement.m_inputIsLocked = false;
    }
}