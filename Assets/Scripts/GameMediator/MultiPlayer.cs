using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiPlayer : MediatableMonoBehavior
{
    private Vector2[] cameraPositions = {
        new Vector2(-30.54f, 1.95f),
        new Vector2(0.0f, -1.0f),
        new Vector2(30.29f, 1.06f)
    };

    private Vector2[] player1SpawnPositions = {
        new Vector2(-34.5f, 3.5f),
        new Vector2(-8f, -1.5f),
        new Vector2(25f, 2.5f)
    };

    private Vector2[] player2SpawnPositions = {
        new Vector2(-22f, 3.5f),
        new Vector2(8f, -1.5f),
        new Vector2(38f, 2.5f)
    };

    private int currentCameraPositionIndex;
    private GameObject player1;
    private GameObject player2;
    private CameraMover cameraMover;
    // Start is called before the first frame update
    void Start()
    {
        ResetMultiplayer();
    }


    public void PlayerDied(GameObject player){
        if (player == player1){
            currentCameraPositionIndex--;
        } else if(player == player2){
            currentCameraPositionIndex++;
        } else {
            Debug.Log("ERROR no player was given!");
        }
        testForWin(player);
    }

    private void testForWin(GameObject player){
        if(currentCameraPositionIndex < 0 ||
            currentCameraPositionIndex >= cameraPositions.Length) {
            GameObject winningPlayer = player == player1 ? player2 : player1;
            gameMediator.triggerWin(winningPlayer);
            // reset the currentCameraPositionIndex
            StopMultiplayer();
        }
    }

    public void ResetMultiplayer(){
        currentCameraPositionIndex = cameraPositions.Length / 2;
    }

    public void SetPlayerPositions(){
        player1.GetComponent<PlayerMovement>().setPosition(player1SpawnPositions[currentCameraPositionIndex]);
        player2.GetComponent<PlayerMovement>().setPosition(player2SpawnPositions[currentCameraPositionIndex]);
        player1.GetComponent<PlayerMovement>().resetPlayer();
        player2.GetComponent<PlayerMovement>().resetPlayer();
    }

    public void SetCameraPosition(){
        Vector2 newPosition = cameraPositions[currentCameraPositionIndex];
        cameraMover.MoveCamera(newPosition.x, newPosition.y);
    }

    public void StartMultiplayer(){
        ResetMultiplayer();
        player1 = GameObject.Find("Player1_Blue");
        player2 = GameObject.Find("Player2_Blue");
        cameraMover = GameObject.Find("Main Camera").GetComponent<CameraMover>();
        SetPlayerPositions();
        SetCameraPosition();
    }

    public void StopMultiplayer(){
        ResetMultiplayer();
        player1 = null;
        player2 = null;
        cameraMover = null;
    }
}
