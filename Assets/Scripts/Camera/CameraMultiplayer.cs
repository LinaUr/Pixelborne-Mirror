using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using System;

// This class controlls the camera movement and fade to black of the multiplayer scene camera.
public class CameraMultiplayer : MonoBehaviour, ICamera
{
    [SerializeField]
    // Transforms from outer left to outer right stage.
    private Transform m_cameraPositionsTransform;
    [SerializeField]
    private GameObject m_fadeImage;
    [SerializeField]
    private float m_fadeTime;

    private int m_fadeStartTime;
    private FadeMode m_fadeMode;
    private Stopwatch m_stopwatchPauseFade;
    private bool m_didFadePause;

    private enum FadeMode
    {
        FadeIn,
        FadeOut,
        NoFade
    }

    // Positions from outer left to outer right stage as they are in the scene.
    public IList<Vector2> Positions { get; set; }

    // We need to get the positions on Awake so we can externally access them on Start.
    void Awake()
    {
        GameMediator.Instance.ActiveCamera = this;
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
        m_stopwatchPauseFade = new Stopwatch();
        m_didFadePause = false;
        Camera.main.fieldOfView = Camera.main.fieldOfView * 1.5f;
    }

    void Update()
    {

        if (Time.timeScale > 0)
        {
            // Time is not frozen.
            if (m_stopwatchPauseFade.IsRunning)
            {
                m_stopwatchPauseFade.Stop();
                m_didFadePause = true;
            }

            // Only fade if the time is not frozen.
            Fade();
        }
        else
        {
            // Time is frozen.
            if (!m_stopwatchPauseFade.IsRunning)
            {
                // Start measuring the time of the freeze.
                m_stopwatchPauseFade.Start();
            }
        }
    }

    // This method implements the fade in and fade out logic.
    void Fade()
    {
        // Skip function if fade in / fade out was succesfully completed and has not been triggered again.
        if (m_fadeMode == FadeMode.NoFade)
        {
            return;
        }

        int currentTime = Toolkit.CurrentTimeMillisecondsToday();
        long elapsedTime = m_didFadePause ? currentTime - m_stopwatchPauseFade.ElapsedMilliseconds : currentTime;
        float takenTime = elapsedTime - m_fadeStartTime;
        Color color = m_fadeImage.GetComponent<SpriteRenderer>().color;

        // Complete the fade when enough time has passed.
        bool doCompleteFade = elapsedTime - m_fadeStartTime >= m_fadeTime;

        if (m_fadeMode == FadeMode.FadeOut)
        {
            // Lower the opacity of the fade to black canvas object based on the time passed since the fade to black trigger.
            float percentage = takenTime / m_fadeTime;
            color.a = doCompleteFade ? 1.0f : percentage;
            m_fadeImage.GetComponent<SpriteRenderer>().color = color;

            if (doCompleteFade)
            {
                FadeCompleted();
                GameMediator.Instance.FadedOut();
            }
        }
        else if (m_fadeMode == FadeMode.FadeIn)
        {
            // Increase the opacity of the fade to black canvas object based on the time passed since the fade in trigger. 
            float percentage = 1 - takenTime / m_fadeTime;
            color.a = doCompleteFade ? 0.0f : percentage;
            m_fadeImage.GetComponent<SpriteRenderer>().color = color;

            if (doCompleteFade)
            {
                FadeCompleted();
                GameMediator.Instance.FadedIn();
            }
        }
        else
        {
            throw new Exception("Error: Wrong fade mode was triggered.");
        }
    }

    // This methods resets important variables after a fade finished.
    private void FadeCompleted()
    {
        m_fadeMode = FadeMode.NoFade;
        m_didFadePause = false;
        m_stopwatchPauseFade.Reset();
    }

    // This method moves the center of both the camera and the fade to black canvas object to the given position
    // while retaining the z-position.
    public void SetPosition(int index)
    {
        Vector2 position = Positions[index];
        gameObject.transform.position = new Vector3(position.x, position.y, gameObject.transform.position.z);
        m_fadeImage.transform.position = transform.position + new Vector3(0, 0, 1);
    }

    // This method triggers the fade to black animation.
    public void FadeOut()
    {
        m_fadeStartTime = Toolkit.CurrentTimeMillisecondsToday();
        m_fadeMode = FadeMode.FadeOut;
    }

    // This method triggers the fade in animation.
    public void FadeIn()
    {
        m_fadeStartTime = Toolkit.CurrentTimeMillisecondsToday();
        m_fadeMode = FadeMode.FadeIn;
    }

    public void SwapHudSymbol(GameObject gameObject, Sprite sprite)
    {
        GameObject hudObject = transform.Find($"{gameObject.name}HudSymbol").gameObject;
        SpriteRenderer spriteRenderer = hudObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
    }
}
