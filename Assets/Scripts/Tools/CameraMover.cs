using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[System.Serializable]//makes sure this shows up in the inspector


public class CameraMover : MediatableMonoBehavior
{
    [SerializeField]
    GameObject fadeImage;

    [SerializeField]
    int fadeTime;

    int fadeStartTime;
    int fadeMode;
    // Start is called before the first frame update, used for initialisation
    void Start()
    {
        fadeImage.transform.position = transform.position + new Vector3(0, 0, 1);
        fadeStartTime = 0;
        fadeMode = 0;
    }


    // Update is called once per frame
    void Update()
    {
        
        //Call fade to black calculation
        fade();
    }

    void fade()
    {
        //Skip function if no player has died since last fade to black ended
        if (fadeMode == 0)
        {
            return;
        }
        //Fade to black
        if (fadeMode == 1)
        {
            Color tmp = fadeImage.GetComponent<SpriteRenderer>().color;
            float takenTime = (Toolkit.currentTimeMillisecondsToday() - fadeStartTime) * 1.0f; //Possible better solution
            float floatFadeTime = fadeTime * 1.0f;   //Possible better solution
            float percentage = takenTime / floatFadeTime;
            tmp.a = percentage;
            fadeImage.GetComponent<SpriteRenderer>().color = tmp;
            if(Toolkit.currentTimeMillisecondsToday() - fadeStartTime >= fadeTime)
            {
                tmp.a = 1.0f;
                fadeImage.GetComponent<SpriteRenderer>().color = tmp;
                fadeMode = 0;
                gameMediator.FadedOut();
            }
        }
        //Fade in
        else if (fadeMode == 2)
        {
            Color tmp = fadeImage.GetComponent<SpriteRenderer>().color;
            float takenTime = (Toolkit.currentTimeMillisecondsToday() - fadeStartTime) * 1.0f; //Possible better solution
            float floatFadeTime = fadeTime * 1.0f;   //Possible better solution
            float percentage = 1 - takenTime / floatFadeTime;
            tmp.a = percentage;
            fadeImage.GetComponent<SpriteRenderer>().color = tmp;
            if (Toolkit.currentTimeMillisecondsToday() - fadeStartTime >= fadeTime)
            {
                tmp.a = 0.0f;
                fadeImage.GetComponent<SpriteRenderer>().color = tmp;
                fadeMode = 0;
                gameMediator.FadedIn();
            }
        }
        //Somehow wrong fadeMode
        else
        {
            Debug.Log("Error: Wrong fade mode");
        }

    }

    /* Moves the center of the camera and the center of the fadeImage to the given coordinates
     * the z-value of both objects remains the same
     * x, y: the coordinates
     */
    public void MoveCamera(float x, float y)
    {
        transform.position = new Vector3(x, y, transform.position[2]);
        fadeImage.transform.position = transform.position + new Vector3(0, 0, 1);
    }

    /* Slowly fades the game out and shows the fadeImage
     */
    public void FadeOut()
    {
        fadeStartTime = Toolkit.currentTimeMillisecondsToday();
        fadeMode = 1;
    }

    /* Slowly fades the game in
     */
    void FadeIn()
    {
        fadeStartTime = Toolkit.currentTimeMillisecondsToday();
        fadeMode = 2;
    }
}
