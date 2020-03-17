using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine.UI;

// This class handles loading and application of images.
// It is a Singleton.
// NOTE: In order to be able to use coroutines (to be threadsafe)
// it has to derive from MonoBehaviour.
public class ImageManager : MonoBehaviour
{
    private static ImageManager m_instance = null;
    private List<string> m_imagePaths = new List<string>();
    private List<Texture2D> m_imageStore = new List<Texture2D>();
    private bool m_isLoadingPaths = true;
    private float m_alpha;

    private static bool s_isInstanceDestroyed = false;

    public Vector2 PlayerSpawnPosition { get; set; }

    private static readonly int ALPHA_DISTANCE = 100;

    public bool IsFirstLoad { get; set; } = true;
    public GameObject ImageHolder { get; set; }

    public static ImageManager Instance
    {
        get
        {
            // We have to make use of AddComponent because this class derives 
            // from MonoBehaviour.
            if (m_instance == null && !s_isInstanceDestroyed)
            {
                GameObject go = new GameObject();
                m_instance = go.AddComponent<ImageManager>();
                m_instance.name = "ImageManager";
                m_instance.LoadAllPaths();
            }
            return m_instance;
        }
    }

    // This method searches for images on the computer and stores their paths.
    private async void LoadAllPaths()
    {
        m_isLoadingPaths = true;

        await Task.Run(() =>
        {
            string userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            // Find JPGs, JPEGs and PNGs in folder Pictures and its subdirectories and put the paths of the images in a list.
            string picturesPath = Path.Combine(new string[] { userPath, "Pictures" });
                m_imagePaths = Toolkit.GetFiles(picturesPath, new List<string>() { "jpg", "jpeg", "png" });
            // Gitlab Issue #48
            // Load images from the entire user folder.
            //m_imagePaths = await Task.Run(() => Toolkit.GetFiles(userPath, new List<string>() { "jpg", "jpeg", "png" }));

            m_isLoadingPaths = false;
        });

        // If the Task returns when the application has been quit the reference of this is null 
        // which can throw an error if we do not check on this.
        if (m_imagePaths.Count > 0 && this != null)
        {
            StartCoroutine(StoreAllImages());
        }
    }

    // This coroutine loads all found images and stores them.
    private IEnumerator StoreAllImages()
    {
        for (int i = 0; i < m_imagePaths.Count; i++)
        {
            UnityWebRequest imageRequest = UnityWebRequestTexture.GetTexture("file://" + m_imagePaths[i]);
            // Wait until its loaded.
            yield return imageRequest.SendWebRequest();

            m_imageStore.Add(DownloadHandlerTexture.GetContent(imageRequest));
        }
    }

    public void PrepareForFirstLoad(bool doSetNewSceneImages)
    {
        IsFirstLoad = true;
        if (doSetNewSceneImages)
        {
            SetNewSceneImages();
        }
    }

    public void SetNewSceneImages()
    {
        StartCoroutine(LoadNewImages((images) => StartCoroutine(ApplyImages(images))));
    }

    // This coroutine grabs a needed amount of images from the ImageStore 
    // and passes them on.
    private IEnumerator LoadNewImages(Action<List<Texture2D>> imageCallback)
    {
        if (ImageHolder != null)
        {
            int amount = ImageHolder.transform.childCount;
            List<Texture2D> images = new List<Texture2D>();

            // Wait until search for paths finished.
            while (m_isLoadingPaths)
            {
                yield return null;
            }

            if (m_imagePaths.Count > 0)
            {
                if (m_imagePaths.Count < amount)
                {
                    // Wait until all images have been stored.
                    while (m_imageStore.Count != m_imagePaths.Count)
                    {
                        yield return null;
                    }
                }
                else
                {
                    // Wait until the needed amount of images has been stored.
                    while (m_imageStore.Count < amount)
                    {
                        yield return null;
                    }
                }

                // Grab needed amount of random images from the ImageStore.
                for (int i = 0; i < amount; i++)
                {
                    int num = UnityEngine.Random.Range(0, m_imageStore.Count - 1);

                    Texture2D image = m_imageStore[num];
                    if (image.width > image.height)
                    {
                        // Use suitable image.
                        images.Add(image);
                    }
                    else
                    {
                        // Skip not suitable image.
                        yield return i--;
                        continue;
                    }
                }
            }

            if (images.Count > 0)
            {
                imageCallback(images);
            }
        }
    }

    // This coroutine applies a given set of images to the ImageHolder.
    private IEnumerator ApplyImages(List<Texture2D> images)
    {
        if (IsFirstLoad)
        {
            // If this is the first time applying the images to the holder, 
            // then make the images fully transparent.
            m_alpha = 0.0f;
            IsFirstLoad = false;
        }
        else
        {
            // Increase alpha value.
            m_alpha += 0.1f;
        }

        for (int i = 0; i < ImageHolder.transform.childCount; i++)
        {
            // RawImage of CustomImage object.
            RawImage rawImage = ImageHolder.transform.GetChild(i).GetChild(1).GetComponent<RawImage>();
            rawImage.material.SetFloat("_Alpha", m_alpha);
            rawImage.texture = images[i];

            yield return null;
        }
    }

    public void UpdateAlphaValue()
    {
        Vector3 currentPlayerPosition = Singleplayer.Instance.Player.transform.position;
        float distance = Vector2.Distance(currentPlayerPosition, PlayerSpawnPosition);

        for (int i = 0; i < ImageHolder.transform.childCount; i++)
        {
            float alpha = distance > ALPHA_DISTANCE ? 100.0f : distance / ALPHA_DISTANCE;
            RawImage rawImage = ImageHolder.transform.GetChild(i).GetChild(1).GetComponent<RawImage>();
            rawImage.material.SetFloat("_Alpha", alpha);
        }
    }

    private void OnDestroy()
    {
        s_isInstanceDestroyed = true;

        // Reset alpha to 0 for all images.
        if (ImageHolder != null)
        {
            for (int i = 0; i < ImageHolder.transform.childCount; i++)
            {
                RawImage rawImage = ImageHolder.transform.GetChild(i).GetChild(1).GetComponent<RawImage>();
                rawImage.material.SetFloat("_Alpha", 0.0f);
            }
        }
    }
}
