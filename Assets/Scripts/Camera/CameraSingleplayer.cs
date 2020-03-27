using System;
using System.Diagnostics;
using UnityEngine;

// This class controlls the camera of the singleplayer scene.
public class CameraSingleplayer : MonoBehaviour, ICamera
{
    [SerializeField]
    private GameObject m_fadeImage;
    [SerializeField]
    private float m_fadeTime;

    private bool m_didFadePause;
    private FadeMode m_fadeMode;
    private int m_fadeStartTime;
    private Stopwatch m_stopwatchPauseFade;

    public GameObject FollowedObject { get; set; }

    private enum FadeMode
    {
        FadeIn,
        FadeOut,
        NoFade
    }

    void Start()
    {
        Singleplayer.Instance.Camera = this;
    }

    void Update()
    {
        // Follow the player.
        gameObject.transform.position = new Vector3(FollowedObject.transform.position.x, FollowedObject.transform.position.y, gameObject.transform.position.z);

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
        bool isFadeComplete = elapsedTime - m_fadeStartTime >= m_fadeTime;

        if (m_fadeMode == FadeMode.FadeOut)
        {
            // Lower the opacity of the fade to black canvas object based on the time passed since the fade to black trigger.
            float percentage = takenTime / m_fadeTime;
            color.a = isFadeComplete ? 1.0f : percentage;
            m_fadeImage.GetComponent<SpriteRenderer>().color = color;

            if (isFadeComplete)
            {
                FadeCompleted();
                Singleplayer.Instance.FadedOut();
            }
        }
        else if (m_fadeMode == FadeMode.FadeIn)
        {
            // Increase the opacity of the fade to black canvas object based on the time passed since fade was triggered. 
            float percentage = 1 - takenTime / m_fadeTime;
            color.a = isFadeComplete ? 0.0f : percentage;
            m_fadeImage.GetComponent<SpriteRenderer>().color = color;

            if (isFadeComplete)
            {
                FadeCompleted();
                Singleplayer.Instance.FadedIn();
            }
        }
        else
        {
            throw new Exception("Error: Wrong fade mode was triggered.");
        }
    }

    // This method resets important variables after a fade finished.
    private void FadeCompleted()
    {
        m_fadeMode = FadeMode.NoFade;
        m_didFadePause = false;
        m_stopwatchPauseFade.Reset();
    }

    public void FadeOut()
    {
        m_fadeStartTime = Toolkit.CurrentTimeMillisecondsToday();
        m_fadeMode = FadeMode.FadeOut;
    }

    public void FadeIn()
    {
        // TODO: If empty at end of project: Remove!
    }

    public void SwapHudSymbol (GameObject gameObject, Sprite sprite)
    {
        GameObject hudObject = transform.Find($"{gameObject.name}HudSymbol").gameObject;
        SpriteRenderer spriteRenderer = hudObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
    }
}
