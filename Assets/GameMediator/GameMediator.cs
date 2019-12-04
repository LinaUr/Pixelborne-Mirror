using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameMediator : MonoBehaviour
{
    private GameObject _player1Object;
    private GameObject _player2Object;
    private PlayerMovement _player1Movement;
    private PlayerMovement _player2Movement;
    private RecordAudio _audioRecorder;
    private WebcamPhoto _photoRecorder;
    public enum CurrentMode { None, MainMenu, SinglePlayer, MultiPlayer };

    public CurrentMode mode {
        get { return mode; }
        set { mode = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        mode = CurrentMode.MainMenu;
        _player1Object = GameObject.Find("Players/Player1_Blue");
        _player1Movement = _player1Object.GetComponent<PlayerMovement>();
        _player2Object = GameObject.Find("Players/Player2_Blue");
        _player2Movement = _player2Object.GetComponent<PlayerMovement>();

        _audioRecorder = gameObject.AddComponent<RecordAudio>();
        _photoRecorder = gameObject.AddComponent<WebcamPhoto>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Record(){
        _audioRecorder.Record();
        _photoRecorder.Record();
    }

    public void handleDeath(GameObject diedObject){
        switch(mode){
            case CurrentMode.SinglePlayer:
                _player1Movement.IsInputLocked = true;
                _player2Movement.IsInputLocked = true;
                break;
            case CurrentMode.None:
            default:
                throw new Exception("Mode is: " + mode + " which is an invalid state!");
        }
    }

    public void FadedOut() {

    }

    public void FadedIn() {

    }
}
