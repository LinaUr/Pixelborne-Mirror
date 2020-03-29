using System;
using System.Diagnostics;
using UnityEngine;

// This class controlls the camera of the singleplayer scene.
public class CameraSingleplayer : MonoBehaviour, ICamera
{
    [SerializeField]
    private GameObject m_fadeImage;
    [SerializeField]
    private float m_fadeTime = 1500;

    private FadeMode m_fadeMode = FadeMode.NoFade;
    private Stopwatch m_fadeStopwatch = new Stopwatch();

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
        // Position the fade image right in front of the camera.
        m_fadeImage.transform.position = gameObject.transform.position + new Vector3(0, 0, 1);
    }

    void Update()
    {
        if (Singleplayer.Instance.Player != null)
        {
            // Follow the player.
            gameObject.transform.position = new Vector3(Singleplayer.Instance.Player.transform.position.x,
                                                        Singleplayer.Instance.Player.transform.position.y,
                                                        gameObject.transform.position.z);
        }

        Fade();
    }

    // This method implements the fade in and fade out logic.
    void Fade()
    {
        // Skip function if fade in / fade out was succesfully completed and has not been triggered again.
        if (m_fadeMode == FadeMode.NoFade)
        {
            return;
        }

        long elapsedTime = m_fadeStopwatch.ElapsedMilliseconds;
        // Complete the fade when enough time has passed.
        bool isFadeComplete = elapsedTime >= m_fadeTime;

        Color color = m_fadeImage.GetComponent<SpriteRenderer>().color;

        if (m_fadeMode == FadeMode.FadeOut)
        {
            // Lower the opacity of the fade to black canvas object based on the time passed since the fade to black trigger.
            float percentage = elapsedTime / m_fadeTime;
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
            float percentage = 1 - elapsedTime / m_fadeTime;
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
        m_fadeStopwatch.Reset();
    }

    public void FadeOut()
    {
        m_fadeStopwatch.Start();
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
