using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    private bool m_isLoadingPaths = true;

    public static ImageManager Instance
    {
        get
        {
            // We have to make use of AddComponent because this class derives 
            // from MonoBehaviour.
            if (m_instance == null)
            {
                GameObject go = new GameObject();
                m_instance = go.AddComponent<ImageManager>();
            }
            return m_instance;
        }
        private set { }
    }

    public GameObject ImageHolder { get; set; }
    public List<Texture2D> ImageStore { get; set; } = new List<Texture2D>();

    void Awake()
    {
        LoadAllPaths();
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
            //m_imagePaths = await Task.Run(() => Toolkit.GetFiles(userPath, new List<string>(){ "jpg", "jpeg", "png" }));

            m_isLoadingPaths = false;
        });

        if (m_imagePaths.Count() > 0)
        {
            StartCoroutine(StoreAllImages());
        }
    }

    private IEnumerator StoreAllImages()
    {
        for (int i = 0; i < m_imagePaths.Count; i++)
        {
            //int num = UnityEngine.Random.Range(0, m_imagePaths.Count());
            UnityWebRequest imageRequest = UnityWebRequestTexture.GetTexture("file://" + m_imagePaths[i]);
            // Wait until its loaded.
            yield return imageRequest.SendWebRequest();

            ImageStore.Add(DownloadHandlerTexture.GetContent(imageRequest));
        }
    }

    public void SetNewSceneImages()
    {
        StartCoroutine(LoadNewImages());
    }

    // This coroutine grabs a needed amount of images from the ImageStore 
    // and passes it on to ApplyImages().
    private IEnumerator LoadNewImages()
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
                while (ImageStore.Count != m_imagePaths.Count)
                {
                    yield return null;
                }
            }
            else
            {
                // Wait until a good amount of images has been stored.
                while (ImageStore.Count < amount)
                {
                    yield return null;
                }
            }

            // Grab needed amount but random images from the ImageStore.
            for (int i = 0; i < amount; i++)
            {
                int num = UnityEngine.Random.Range(0, ImageStore.Count());

                Texture2D image = ImageStore[num];
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

        StartCoroutine(ApplyImages(images));
    }

    // This coroutine applies a given set of images to the ImageHolder.
    private IEnumerator ApplyImages(List<Texture2D> images)
    {
        for (int i = 0; i < ImageHolder.transform.childCount; i++)
        {
            RawImage rawImage = ImageHolder.transform.GetChild(i).GetComponent<RawImage>();
            // Change alpha channel of rawImage to visible if a suitable image is found.
            rawImage.color = new Color(1f, 1f, 1f, 1f);
            rawImage.texture = images[i];

            yield return null;
        }
    }
}
