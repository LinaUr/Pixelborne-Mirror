using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

// This class handles loading and of embedded images.
// It searches for pictures on the current computer and applies them to canvases in the scene.

public class WindowImages : MonoBehaviour
{
    private static List<string> m_imagePaths = new List<string>();
    
    // async for Gitlab Issue #48
    /*async*/
    void Start()
    {
        string userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        // Find JPGs, JPEGs and PNGs in folder Pictures and its subdirectories and put the paths of the images in a list.
        // source of code: https://stackoverflow.com/questions/8443524/using-directory-getfiles-with-a-regex-in-c/8443597#8443597
        string picturesPath = Path.Combine(new string[] {userPath, "Pictures"});
        m_imagePaths = Directory.GetFiles(picturesPath, "*.*", SearchOption.AllDirectories)
                                .Where(extension => extension.ToLower().EndsWith(".jpg")
                                                 || extension.ToLower().EndsWith(".jpeg")
                                                 || extension.ToLower().EndsWith(".png"))
                                .ToList();

        // Gitlab Issue #48
        // Load images from the entire user folder.
        //m_imagePaths = await Task.Run(() => Toolkit.GetFiles(userPath, "*.*"));

        if (m_imagePaths.Count() > 0)
        {
            StartCoroutine(LoadImage());
        }
    }

    // This coroutine puts one downloaded image in each window in the scene.
    // source of code: https://forum.unity.com/threads/read-image-from-disk.117866/#post-787801
    IEnumerator LoadImage()
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            RawImage rawImage = gameObject.transform.GetChild(i).GetComponent<RawImage>();
            int num = UnityEngine.Random.Range(0, m_imagePaths.Count());
            UnityWebRequest imageRequest = UnityWebRequestTexture.GetTexture("file://" + m_imagePaths[num]);

            // Wait until its loaded.
            yield return imageRequest.SendWebRequest();
            Texture2D image = DownloadHandlerTexture.GetContent(imageRequest);

            if (image.width > image.height)
            {
                // Change alpha channel of rawImage to visible if a suitable image is found.
                rawImage.color = new Color(1f,1f,1f,1f);
                rawImage.texture = image;
            } else
            {
                i--;
            }
        }
    }
}
