using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

// This class handles the change of the embedded background images.
// It searches for pictures on the current PC and inserts them into the scene as a texture.

public class WindowImages : MonoBehaviour
{
    [SerializeField]
    private GameObject[] m_windows;
    private static List<string> m_imagePaths = new List<string>();
    //private float m_elapsedTime;

    // async is for the second possibility to load images from the whole computer.
    // If you want to use the async possibility please import System.Threading.Tasks.
    /*async*/ void Start()
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

        // Maybe for later (for now, it costs too much time). It has to do with the async possibility.
        //m_imagePaths = await Task.Run(() => GetFiles(userPath, "*.*"));

        if (m_imagePaths.Count() > 0)
        {
            StartCoroutine(LoadImage());
        }
    }

    // Couroutine which puts in each window in the scene one downloaded image.
    // source of code: https://forum.unity.com/threads/read-image-from-disk.117866/#post-787801
    IEnumerator LoadImage()
    {
        for (int window = 0; window < m_windows.Length; window++)
        {
            RawImage windowImage = m_windows[window].GetComponent<RawImage>();
            int num = UnityEngine.Random.Range(0, m_imagePaths.Count());
            UnityWebRequest imageRequest = UnityWebRequestTexture.GetTexture("file://" + m_imagePaths[num]);

            // Wait until its loaded.
            yield return imageRequest.SendWebRequest();
            Texture2D image = DownloadHandlerTexture.GetContent(imageRequest);

            if (image.width > image.height)
            {
                // Change alpha channel of RawImages in the scene to visible if an suitable image is found.
                // source of code: https://forum.unity.com/threads/changing-a-new-ui-images-alpha-value.289755/#post-1912745
                Color c = windowImage.color;
                c.a = 1f;
                windowImage.color = c;
                m_windows[window].GetComponent<RawImage>().texture = image;
            } else
            {
                window--;
            }
        }
    }

    // This method is for the future if you want to include the temporal aspect.
    // Changes the background image every 10 sec.
    /*void Update()
    {
        m_elapsedTime += Time.deltaTime;

        if (m_elapsedTime >= 10)
        {
            m_elapsedTime -= 10;
            StartCoroutine("LoadImage");
        }
        
    }*/

    // This method is for the second possibility to load images from the whole computer.
    /*private List<string> GetFiles(string root, string fileEnding)
    {
        List<string> fileList = new List<string>();

        Stack<string> pending = new Stack<string>();
        pending.Push(root);
        while (pending.Count != 0)
        {
            string path = pending.Pop();
            string[] next = null;
            try
            {
                next = Directory.GetFiles(path, fileEnding)
                                    .Where(extension => extension.ToLower().EndsWith(".jpg")
                                                    || extension.ToLower().EndsWith(".jpeg")
                                                    || extension.ToLower().EndsWith(".png")).ToArray();
            }
            catch { }
            if ((next != null) && (next.Length != 0))
                foreach (string file in next) fileList.Add(file);
            try
            {
                next = Directory.GetDirectories(path);
                foreach (string subdir in next) pending.Push(subdir);
            }
            catch { }
        }
        return fileList;
    }*/
}
