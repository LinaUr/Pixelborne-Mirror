﻿using System;
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
    private float m_alpha;

    public bool IsFirstLoad { get; set; } = true;
    public GameObject ImageHolder { get; set; }
    public List<Texture2D> ImageStore { get; set; } = new List<Texture2D>();
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

    // This coroutine loads all found images and stores them.
    private IEnumerator StoreAllImages()
    {
        for (int i = 0; i < m_imagePaths.Count; i++)
        {
            UnityWebRequest imageRequest = UnityWebRequestTexture.GetTexture("file://" + m_imagePaths[i]);
            // Wait until its loaded.
            yield return imageRequest.SendWebRequest();

            ImageStore.Add(DownloadHandlerTexture.GetContent(imageRequest));
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

        imageCallback(images);
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
            RawImage rawImage = ImageHolder.transform.GetChild(i).GetComponent<RawImage>();
            rawImage.material.SetFloat("_Alpha", m_alpha);
            rawImage.texture = images[i];

            yield return null;
        }
    }
}