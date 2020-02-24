using UnityEngine;

public class ImageHolderPasser : MonoBehaviour
{
    [SerializeField]
    private bool m_LoadAndSetSceneImages = true;

    void Awake()
    {
        ImageManager.Instance.ImageHolder = gameObject;
        ImageManager.Instance.PrepareForFirstLoad(m_LoadAndSetSceneImages);
    }

    private void OnDestroy()
    {
        ImageManager.Instance.ImageHolder = null;
    }
}
