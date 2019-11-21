using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]//makes sure this shows up in the inspector
public class Coordinate
{
    public float x;
    public float y;
}

//ToDo---------Fliessende Bewegung statt Sprung---------------------Globale Koordinatenliste----------------------------------------

public class CameraFixed : MonoBehaviour
{


    //[SerializeField]
    //protected int player1;

    //[SerializeField]
    //protected int player2;

    [SerializeField]
    int startPosition;

    [SerializeField]
    Coordinate[] coordinates;

    bool hasRevivedSinceDeath;
    int positionCounter;

    PlayerHealth player1Health = PlayerHealthController.player1Health;
    PlayerHealth player2Health = PlayerHealthController.player2Health;

    // Start is called before the first frame update
    void Start()
    {
        hasRevivedSinceDeath = true;
        if (startPosition >= coordinates.Length)
        {
            startPosition = coordinates.Length - 1;
        }
        if (startPosition < 0)
        {
            startPosition = 0;
        }
        positionCounter = startPosition;
        transform.position = new Vector3(coordinates[positionCounter].x, coordinates[positionCounter].y, transform.position[2]);
    }


    // Update is called once per frame
    void Update()
    {
        //if (player1.health.isAlive && !hasRevivedSinceDeath && player2.health.isAlive)
        if (player1Health.isAlive && !hasRevivedSinceDeath && player2Health.isAlive)
        {
            hasRevivedSinceDeath = true;
        }
        if (!player1Health.isAlive && hasRevivedSinceDeath)
        {
            //xOffset = xOffset + 1;
            hasRevivedSinceDeath = false;
            positionCounter--;
            if (positionCounter >= coordinates.Length)
            {
                positionCounter = coordinates.Length - 1;
            }
            if (positionCounter < 0)
            {
                positionCounter = 0;
            }
            transform.position = new Vector3(coordinates[positionCounter].x, coordinates[positionCounter].y, transform.position[2]);
            //transform.position = transform.position + new Vector3(xOffset, yOffset, 0);
        }
        if (!player2Health.isAlive && hasRevivedSinceDeath)
        {
            //xOffset = xOffset + 1;
            hasRevivedSinceDeath = false;
            positionCounter++;
            if (positionCounter >= coordinates.Length)
            {
                positionCounter = coordinates.Length - 1;
            }
            if (positionCounter < 0)
            {
                positionCounter = 0;
            }
            transform.position = new Vector3(coordinates[positionCounter].x, coordinates[positionCounter].y, transform.position[2]);
            //transform.position = transform.position + new Vector3(xOffset, yOffset, 0);
        }
        //transform.position = new Vector3(trackingTarget.position.x + xOffset,
        //trackingTarget.position.y + yOffset, transform.position.z);
    }
}
