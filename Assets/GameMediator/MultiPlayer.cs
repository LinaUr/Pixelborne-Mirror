using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiPlayer : MediatableMonoBehavior
{

    private Vector2[] cameraPositions = {
            new Vector2(-30.54f, 1.95f),
            new Vector2(0.3683965f, 0.373381f),
            new Vector2(30.29f, 1.06f)
    };
    private Vector2[] player1SpawnPositions = {
            new Vector2(0f, 0f),
            new Vector2(0f, 0f),
            new Vector2(0f, 0f)
    };

    private Vector2[] player2SpawnPositions = {
            new Vector2(0f, 0f),
            new Vector2(0f, 0f),
            new Vector2(0f, 0f)
    };
    private int currentCameraPositionIndex;
    private GameObject player1;
    private GameObject player2;
    // Start is called before the first frame update
    void Start()
    {
        currentCameraPositionIndex = cameraPositions.Length / 2;
        player1 = GameObject.Find("Player1_Blue");
        player2 = GameObject.Find("Player2_Blue");
    }

    public Vector2 GetCameraSpawnPoint(GameObject player){
        if (player == player1){
            currentCameraPositionIndex--;
        } else if(player == player2){
            currentCameraPositionIndex++;
        } else {
            Debug.Log("ERROR no player was given!");
        }
        testForWin(player);
        return cameraPositions[currentCameraPositionIndex];
    }

    public Vector2 GetPlayerSpawnPoint(GameObject player){
        if(player == player1){
            return player1SpawnPositions[currentCameraPositionIndex];
        } else if(player == player2){
            return player2SpawnPositions[currentCameraPositionIndex];
        } else {
            Debug.Log("ERROR: no player was given!");
            return new Vector2();
        }
    }

    private void testForWin(GameObject player){
        if(currentCameraPositionIndex < 0 &&
            currentCameraPositionIndex <= cameraPositions.Length) {
            GameObject winningPlayer = player == player1 ? player2 : player1;
            gameMediator.triggerWin(winningPlayer);
            // reset the currentCameraPositionIndex
            currentCameraPositionIndex = cameraPositions.Length / 2;
        }
    }
}
