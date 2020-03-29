using UnityEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;

// This class controlls the camera movement and fade to black of the multiplayer scene camera.
public class CameraMultiplayer : MonoBehaviour, ICamera
{
    [SerializeField]
    private float m_fadeTime = 1500;
    [SerializeField]
    private GameObject m_fadeImage;
    [SerializeField]
    // Transforms from outer left to outer right stage.
    private Transform m_cameraPositionsTransform;

    private FadeMode m_fadeMode = FadeMode.NoFade;
    private Stopwatch m_fadeStopwatch = new Stopwatch();

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
        Multiplayer.Instance.Camera = this;

        Positions = new List<Vector2>();
        foreach (Transform positionsTransform in m_cameraPositionsTransform)
        {
            Positions.Add(positionsTransform.position);
        }
    }

    void Start()
    {
        // Position the fade image right in front of the camera.
        m_fadeImage.transform.position = gameObject.transform.position + new Vector3(0, 0, 1);
    }

    void Update()
    {
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
                Multiplayer.Instance.FadedOut();
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
                Multiplayer.Instance.FadedIn();
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
        m_fadeStopwatch.Start();
        m_fadeMode = FadeMode.FadeOut;
    }

    // This method triggers the fade in animation.
    public void FadeIn()
    {
        m_fadeStopwatch.Start();
        m_fadeMode = FadeMode.FadeIn;
    }

    public void SwapHudSymbol(GameObject gameObject, Sprite sprite)
    {
        GameObject hudObject = transform.Find($"{gameObject.name}HudSymbol").gameObject;
        SpriteRenderer spriteRenderer = hudObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
    }
}
