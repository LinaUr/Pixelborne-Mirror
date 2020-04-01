using UnityEngine;
using System.Collections.Generic;

// This class controlls the camera movement and fade to black of the multiplayer scene camera.
public class CameraMultiplayer : GameCamera
{
    [SerializeField]
    // Transforms from outer left to outer right stage.
    private Transform m_cameraPositionsTransform;

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

    protected override void Update()
    {
        base.Update();
    }

    private protected override void FadedOut()
    {
        Multiplayer.Instance.FadedOut();
    }
    private protected override void FadedIn()
    {
        Multiplayer.Instance.FadedIn();
    }

    // This method moves the center of both the camera and the fade to black canvas object to the given position
    // while retaining the z-position.
    public void SetPosition(int index)
    {
        Vector2 position = Positions[index];
        gameObject.transform.position = new Vector3(position.x, position.y, gameObject.transform.position.z);
        m_fadeImage.transform.position = transform.position + new Vector3(0, 0, 1);
    }
}
