using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
//using System.Threading.Tasks;

/* This class handles the change of the background.
 * It searches for pictures on the current PC and inserts them into the scene as a texture.
 */

public class WindowImages : MonoBehaviour
{
    public GameObject[] m_windows;
    private static List<string> m_imagePaths = new List<string>();
    //private float m_elapsedTime;

    // async is for the second possibility to load images from the whole computer.
    /*async*/ void Start()
    {
        // Get the path "C:\Users\<FolderOfCurrentUse>".
        string userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        // Find JPGs, JPEGs and PNGs in folder Pictures and its subdirectories and put the paths of the images in a list.
        /* source of code: 
         * https://stackoverflow.com/questions/8443524/using-directory-getfiles-with-a-regex-in-c/8443597#8443597
         */
        string picturesPath = Path.Combine(new string[] {userPath, "Pictures"});
        m_imagePaths = Directory.GetFiles(picturesPath, "*.*", SearchOption.AllDirectories)
                                .Where(extension => extension.EndsWith(".jpg")
                                                 || extension.EndsWith(".jpeg")
                                                 || extension.EndsWith(".png"))
                                .ToList();

        // Maybe for later (for now, it costs too much time).
        //imagePaths = await Task.Run(() => GetFiles(userPath, "*.*"));

        if (m_imagePaths.Count() > 0)
        {
            StartCoroutine(LoadImage());
        }
    }

    // Here starts a Coroutine.
    /* For explanation of Coroutines:
     * "[Coroutines] don't run async they just pause execution with the yield control and 
     * continue their execution next frame (or whatever yield command you used) exactly at this position."
     * source: https://forum.unity.com/threads/coroutine-not-running-async.450672/#post-2917554
     * source of code: https://forum.unity.com/threads/read-image-from-disk.117866/#post-787801
     */
    IEnumerator LoadImage()
    {
        // Put in each window in the scene one downloaded image.
        foreach (GameObject window in m_windows)
        {
            RawImage windowImage = window.GetComponent<RawImage>();

            // Find an image.
            while (!windowImage.texture) {

                // Take one random image and "download" it.          
                int num = UnityEngine.Random.Range(0, m_imagePaths.Count());
                UnityWebRequest imageRequest = UnityWebRequestTexture.GetTexture("file://" + m_imagePaths[num]);

                // Wait until its loaded.
                yield return imageRequest.SendWebRequest();

                // Get the image as an downloaded texture.
                Texture2D image = DownloadHandlerTexture.GetContent(imageRequest);

                // Show only images which are wider than high to avoid image deformation.
                if (image.width > image.height)
                {
                    /* Change alpha channel of RawImages in the scene to visible if an suitable image is found.
                     * source: https://forum.unity.com/threads/changing-a-new-ui-images-alpha-value.289755/#post-1912745
                     */
                    Color c = windowImage.color;
                    c.a = 1f;
                    windowImage.color = c;
                    window.GetComponent<RawImage>().texture = image;
                }
            }
        }
    }

    /* -------------------------------------------------------------------
     * Following code is for now not needed but it would be helpful 
     * for the future if you want to include the temporal aspect.
     * -------------------------------------------------------------------
     */

    // Changes the background image every 10 sec via a Coroutine.
    /*void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= 10)
        {
            elapsedTime -= 10;
            StartCoroutine("Load_image");
        }
        
    }*/

    // This following method is for the second possibility to load images from the whole computer in Start().
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
                                    .Where(extension => extension.EndsWith(".jpg")
                                                    || extension.EndsWith(".jpeg")
                                                    || extension.EndsWith(".png")).ToArray();
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
