﻿using UnityEngine;
using System;
using Assets.Scripts.Tools;
using System.Collections.Generic;

// Makes sure this shows up in the inspector.
[Serializable]

// This class controlls the camera movement and fade to black of the multiplayer scene camera.
public class CameraMultiplayer : MonoBehaviour, ICamera
{
    [SerializeField]
    // Transforms from outer left to outer right stage.
    private Transform m_cameraPositionsTransform;
    [SerializeField]
    private GameObject m_fadeImage;
    [SerializeField]
    private int m_fadeTime;

    private int m_fadeStartTime;
    private int m_fadeMode;

    // Positions from outer left to outer right stage as they are in the scene.
    public IList<Vector2> Positions { get; set; }

    // We need to get the positions on Awake so we can externally access them on Start.
    void Awake()
    {
        Positions = new List<Vector2>();
        foreach (Transform positionsTransform in m_cameraPositionsTransform)
        {
            Positions.Add(positionsTransform.position);
        }
    }

    void Start()
    {


        m_fadeImage.transform.position = transform.position + new Vector3(0, 0, 1);
        m_fadeStartTime = 0;
        m_fadeMode = 0;
        Camera.main.fieldOfView = Camera.main.fieldOfView * 1.5f;
    }

    void Update()
    {
        Fade();
    }

    // This method implements the fade in and fade out logic.
    void Fade()
    {
        // Skip function if fade in / fade out was succesfully completed and has not been triggered again.
        if (m_fadeMode == 0)
        {
            return;
        }
        // Fade to black
        if (m_fadeMode == 1)
        {
            // Lower the opacity of the fade to black canvas object based on the time passed since the fade to black trigger.
            Color tmp = m_fadeImage.GetComponent<SpriteRenderer>().color;
            float takenTime = (Toolkit.CurrentTimeMillisecondsToday() - m_fadeStartTime) * 1.0f;
            float floatFadeTime = m_fadeTime * 1.0f;
            float percentage = takenTime / floatFadeTime;
            tmp.a = percentage;
            m_fadeImage.GetComponent<SpriteRenderer>().color = tmp;
            // Complete the fade to black when enough time has passed.
            if (Toolkit.CurrentTimeMillisecondsToday() - m_fadeStartTime >= m_fadeTime)
            {
                tmp.a = 1.0f;
                m_fadeImage.GetComponent<SpriteRenderer>().color = tmp;
                m_fadeMode = 0;
                GameMediator.Instance.FadedOut();
            }
        }
        // Fade in
        else if (m_fadeMode == 2)
        {
            // Increase the opacity of the fade to black canvas object based on the time passed since the fade in trigger.
            Color tmp = m_fadeImage.GetComponent<SpriteRenderer>().color;
            float takenTime = (Toolkit.CurrentTimeMillisecondsToday() - m_fadeStartTime) * 1.0f;
            float floatFadeTime = m_fadeTime * 1.0f;
            float percentage = 1 - takenTime / floatFadeTime;
            tmp.a = percentage;
            m_fadeImage.GetComponent<SpriteRenderer>().color = tmp;
            // Complete the fade in when enough time has passed.
            if (Toolkit.CurrentTimeMillisecondsToday() - m_fadeStartTime >= m_fadeTime)
            {
                tmp.a = 0.0f;
                m_fadeImage.GetComponent<SpriteRenderer>().color = tmp;
                m_fadeMode = 0;
                GameMediator.Instance.FadedIn();
            }
        }
        // Somehow wrong fadeMode was triggered.
        else
        {
            Debug.Log("Error: Wrong fade mode");
        }

    }

    // This method moves the center of both the camera and the fade to black canvas object to the given position while retaining the z-position.
    public void MoveCamera(int index)
    {
        Vector2 position = Positions[index];
        transform.position = new Vector3(position.x, position.y, transform.position[2]);
        m_fadeImage.transform.position = transform.position + new Vector3(0, 0, 1);
    }

    // This method triggers the fade to black animation.
    public void FadeOut()
    {
        m_fadeStartTime = Toolkit.CurrentTimeMillisecondsToday();
        m_fadeMode = 1;
    }

    // This method triggers the fade in animation.
    public void FadeIn()
    {
        m_fadeStartTime = Toolkit.CurrentTimeMillisecondsToday();
        m_fadeMode = 2;
    }
}
