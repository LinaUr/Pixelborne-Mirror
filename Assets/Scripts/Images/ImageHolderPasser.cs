using UnityEngine;

// This class assigns the GameObject that functions as an ImageHolder to the ImageManager.
// It needs to be assigned to a GameObject as a Script component.
public class ImageHolderPasser : MonoBehaviour
{
    [SerializeField]
    private bool m_LoadAndSetSceneImages = true;

    void Awake()
    {
        ImageManager.Instance.ImageHolder = gameObject;
        ImageManager.Instance.PrepareForFirstLoad(m_LoadAndSetSceneImages);
    }

    private void Update()
    {
        if (Game.Mode == Mode.Singleplayer)
        {
            ImageManager.Instance.UpdateAlphaValue();
        }
    }

    private void OnDestroy()
    {
        ImageManager.Instance.ImageHolder = null;
    }
}
