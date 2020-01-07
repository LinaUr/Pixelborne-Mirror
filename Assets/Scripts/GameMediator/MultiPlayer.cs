using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiPlayer : MediatableMonoBehavior
{
    // SerializeField to make variable appear in the inspector to pass objects.
    [SerializeField]
    // Transforms from outer left to outer right stage.
    private Transform cameraPositionsTransform;

    // Positions from outer left to outer right stage.
    private IList<Vector2> cameraPositions;
    private const int PLAYER_DISTANCE_TO_CENTER_X = 7;
    private const int PLAYER_DISTANCE_TO_CENTER_Y = 0;

    private int currentCameraPositionIndex;
    private GameObject player1;
    private GameObject player2;
    private CameraMover cameraMover;

    // Start is called before the first frame update
    void Start()
    {
        cameraPositions = new List<Vector2>();
        foreach (Transform positionsTransform in cameraPositionsTransform)
        {
            cameraPositions.Add(positionsTransform.position);
        }

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
        TestForWin(player);
    }

    private void TestForWin(GameObject player){
        if(currentCameraPositionIndex < 0 ||
            currentCameraPositionIndex >= cameraPositions.Count) {
            GameObject winningPlayer = player == player1 ? player2 : player1;
            gameMediator.TriggerWin(winningPlayer);
            // reset the currentCameraPositionIndex
            StopMultiplayer();
        }
    }

    public void ResetMultiplayer(){
        currentCameraPositionIndex = cameraPositions.Count / 2;
    }

    public void SetPlayerPositions(){
        var cameraPosition = cameraPositions[currentCameraPositionIndex];
        var spawnDistance = new Vector2(PLAYER_DISTANCE_TO_CENTER_X, PLAYER_DISTANCE_TO_CENTER_Y);

        player1.GetComponent<PlayerMovement>().SetPosition(cameraPosition - spawnDistance);
        player2.GetComponent<PlayerMovement>().SetPosition(cameraPosition + spawnDistance);
    }

    public void ResetPlayersActions(){
        player1.GetComponent<PlayerMovement>().ResetPlayerActions();
        player2.GetComponent<PlayerMovement>().ResetPlayerActions();
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
