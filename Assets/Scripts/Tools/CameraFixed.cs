using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[System.Serializable]//makes sure this shows up in the inspector
public class Coordinate
{
    public float x;
    public float y;
}


public class CameraFixed : MonoBehaviour
{


    [SerializeField]
    int startPosition;

    [SerializeField]
    Coordinate[] coordinates;

    [SerializeField]
    GameObject fadeImage;

    [SerializeField]
    int fadeTime;

    [SerializeField]
    int blackTime;

    bool hasRevivedSinceDeath;
    int positionCounter;
    Vector3 newPosition;
    bool changedPosition;
    int fadeStartTime;

    PlayerHealth player1Health = PlayerHealthController.player1Health;
    PlayerHealth player2Health = PlayerHealthController.player2Health;

    // Start is called before the first frame update, used for initialisation
    void Start()
    {
        hasRevivedSinceDeath = true;
        changedPosition = true;
        fadeStartTime = -1;
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
        fadeImage.transform.position = new Vector3(coordinates[positionCounter].x, coordinates[positionCounter].y, transform.position[2]+1);
    }


    // Update is called once per frame
    void Update()
    {
        //If one player has died in the past, check wether he has been revived since then
        if (player1Health.isAlive && !hasRevivedSinceDeath && player2Health.isAlive)
        {
            hasRevivedSinceDeath = true;
        }
        //Get new values for camera position if player1 has died
        if (!player1Health.isAlive && hasRevivedSinceDeath)
        {
            hasRevivedSinceDeath = false;
            positionCounter--;
            fadeStartTime = Toolkit.currentTimeMillisecondsToday();
            changedPosition = false;
            if (positionCounter >= coordinates.Length)
            {
                positionCounter = coordinates.Length - 1;
            }
            if (positionCounter < 0)
            {
                positionCounter = 0;
            }
            newPosition = new Vector3(coordinates[positionCounter].x, coordinates[positionCounter].y, transform.position[2]);
        }
        //Get new values for camera position if player2 has died
        if (!player2Health.isAlive && hasRevivedSinceDeath)
        {
            hasRevivedSinceDeath = false;
            positionCounter++;
            fadeStartTime = Toolkit.currentTimeMillisecondsToday();
            changedPosition = false;
            if (positionCounter >= coordinates.Length)
            {
                positionCounter = coordinates.Length - 1;
            }
            if (positionCounter < 0)
            {
                positionCounter = 0;
            }
            newPosition = new Vector3(coordinates[positionCounter].x, coordinates[positionCounter].y, transform.position[2]);
        }
        //Set camera position to new position when screen is totally black
        if (fadeStartTime != -1 && (Toolkit.currentTimeMillisecondsToday() > fadeStartTime + fadeTime) && !changedPosition)
        {
            transform.position = newPosition;
            fadeImage.transform.position = newPosition + new Vector3(0,0,1);
            changedPosition = true;
        }
        //Call fade to black calculation
        fade();
    }

    void fade()
    {
        //Skip function if no player has died since last fade to black ended
        if(fadeStartTime == -1)
        {
            return;
        }
        //Fade to black
        if(Toolkit.currentTimeMillisecondsToday() - fadeStartTime < fadeTime)
        {
            Color tmp = fadeImage.GetComponent<SpriteRenderer>().color;
            float takenTime = (Toolkit.currentTimeMillisecondsToday() - fadeStartTime) * 1.0f; //Possible better solution
            float floatFadeTime = fadeTime * 1.0f;   //Possible better solution
            float percentage = takenTime / floatFadeTime;
            tmp.a = percentage;
            fadeImage.GetComponent<SpriteRenderer>().color = tmp;
        }
        //Screen completly black
        else if(Toolkit.currentTimeMillisecondsToday() - fadeStartTime < fadeTime + blackTime)
        {
            Color tmp = fadeImage.GetComponent<SpriteRenderer>().color;
            tmp.a = 1f;
            fadeImage.GetComponent<SpriteRenderer>().color = tmp;
        }
        //Fade in
        else if (Toolkit.currentTimeMillisecondsToday() - fadeStartTime < 2*fadeTime + blackTime)
        {
            Color tmp = fadeImage.GetComponent<SpriteRenderer>().color;
            float takenTime = (Toolkit.currentTimeMillisecondsToday() - fadeStartTime - fadeTime - blackTime) * 1.0f;   //Possible better solution
            float floatFadeTime = fadeTime * 1.0f;   //Possible better solution
            float percentage = 1.0f - takenTime / floatFadeTime;
            tmp.a = percentage;
            fadeImage.GetComponent<SpriteRenderer>().color = tmp;
        }
        //Clear Screen when fade to black is completed
        else
        {
            fadeStartTime = -1;
            Color tmp = fadeImage.GetComponent<SpriteRenderer>().color;
            tmp.a = 0f;
            fadeImage.GetComponent<SpriteRenderer>().color = tmp;
        }






            /*if (type)
        {
            Color tmp = fadeImage.GetComponent<SpriteRenderer>().color;
            tmp.a = 1f;
            fadeImage.GetComponent<SpriteRenderer>().color = tmp;
        }
        else
        {
            Color tmp = fadeImage.GetComponent<SpriteRenderer>().color;
            tmp.a = 0f;
            fadeImage.GetComponent<SpriteRenderer>().color = tmp;
        }*/
    }
}
