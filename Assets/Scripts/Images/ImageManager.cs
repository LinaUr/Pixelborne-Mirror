using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

// This class handles loading and application of images.
// It is a Singleton.
// NOTE: In order to be able to use coroutines (to be thread safe)
// it has to derive from MonoBehaviour.
public class ImageManager : MonoBehaviour
{
    private bool m_isLoadingPaths = true;
    private float m_alpha;
    private List<string> m_imagePaths = new List<string>();
    private List<Texture2D> m_imageStore = new List<Texture2D>();

    private static bool s_isInstanceDestroyed = false;
    private static ImageManager s_instance = null;

    private static readonly CancellationTokenSource CTS = new CancellationTokenSource();
    private static readonly int ALPHA_DISTANCE = 100;

    public bool IsFirstLoad { get; set; } = true;
    public GameObject ImageHolder { get; set; }
    public Vector2 PlayerSpawnPosition { get; set; }

    public static ImageManager Instance
    {
        get
        {
            // We have to make use of AddComponent because this class derives 
            // from MonoBehaviour.
            if (s_instance == null && !s_isInstanceDestroyed)
            {
                GameObject go = new GameObject();
                s_instance = go.AddComponent<ImageManager>();
                s_instance.name = "ImageManager";
                s_instance.LoadAllPaths();
            }
            return s_instance;
        }
    }

    // This method searches for images on the computer and stores their paths.
    private async void LoadAllPaths()
    {
        m_isLoadingPaths = true;

        await Task.Run(() =>
        {
            // Find JPGs, JPEGs and PNGs in folder Pictures and its subdirectories and put the paths of the images in a list.
            string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            m_imagePaths = Toolkit.GetFiles(directory, new List<string>() { "jpg", "jpeg", "png" }, CTS.Token);
            
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

    void OnDestroy()
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

    void OnApplicationQuit()
    {
        CTS.Cancel();
    }
}
