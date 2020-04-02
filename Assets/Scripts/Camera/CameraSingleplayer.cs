using UnityEngine;

// This class controlls the camera of the singleplayer scene.
public class CameraSingleplayer : GameCamera
{    
    void Start()
    {
        Singleplayer.Instance.Camera = this;
        // Position the fade image right in front of the camera.
        m_fadeImage.transform.position = gameObject.transform.position + new Vector3(0, 0, 1);
    }

    protected override void Update()
    {
        base.Update();

        if (Singleplayer.Instance.Player != null)
        {
            // Follow the player.
            gameObject.transform.position = new Vector3(Singleplayer.Instance.Player.transform.position.x,
                                                        Singleplayer.Instance.Player.transform.position.y,
                                                        gameObject.transform.position.z);
        }
    }

    protected override void FadedOut()
    {
        Singleplayer.Instance.FadedOut();
    }
    protected override void FadedIn()
    {
        // Do nothing. Singleplayer does not need a fade in.
        Singleplayer.Instance.FadedIn();
    }
}
