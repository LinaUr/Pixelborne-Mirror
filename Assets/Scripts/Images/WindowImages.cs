using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
//using System.Threading.Tasks;

/* This class handles the change of the background.
 * It searches for pictures on the current PC and inserts them into the scene as a texture.
 */

public class WindowImages : MonoBehaviour
{
    private static List<string> imagePaths = new List<string>();
    private GameObject[] windows;
    //private float elapsedTime;

    // Start is called before the first frame update, used for initialisation
    // async is for the second possibility to load images from the whole computer.
    /*async*/ void Start()
    {
        // get the path "C:\Users\<FolderOfCurrentUse>"
        string userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        // find JPGs, JPEGs and PNGs in folder Pictures and its subdirectories 
        // and put the paths of the images in a list:
        /* source of code: 
         * https://stackoverflow.com/questions/8443524/using-directory-getfiles-with-a-regex-in-c/8443597#8443597
         */
        var picturesPath = Path.Combine(new string[] {userPath, "Pictures"});
        imagePaths = Directory.GetFiles(picturesPath, "*.*", SearchOption.AllDirectories)
                                .Where(extension => extension.EndsWith(".jpg")
                                                 || extension.EndsWith(".jpeg")
                                                 || extension.EndsWith(".png"))
                                .ToList();

        // maybe for later (for now, it costs to much time):
        //imagePaths = await Task.Run(() => GetFiles(userPath, "*.*"));

        if (imagePaths.Count() > 0)
        {
            // find all windows in the scene:
            windows = GameObject.FindGameObjectsWithTag("WindowLandscape");
            StartCoroutine("Load_image");
        }
    }

    // here starts a Coroutine:
    /* for explanation of Coroutines:
     * "[Coroutines] don't run async they just pause execution with the yield control and 
     * continue their execution next frame (or whatever yield command you used) exactly at this position."
     * source: https://forum.unity.com/threads/coroutine-not-running-async.450672/#post-2917554
     * source of code: https://forum.unity.com/threads/read-image-from-disk.117866/#post-787801
     */
    IEnumerator Load_image()
    {
        // put in each window in the scene one downloaded image:
        foreach (GameObject window in windows)
        {
            RawImage windowImage = window.GetComponent<RawImage>();

            // find an image:
            while (!windowImage.texture) {

                // take one random image and "download" it:          
                int num = UnityEngine.Random.Range(0, imagePaths.Count());
                UnityWebRequest imageRequest = UnityWebRequestTexture.GetTexture("file://" + imagePaths[num]);

                // wait until its loaded:
                yield return imageRequest.SendWebRequest();

                // get the image as an downloaded texture:
                var image = DownloadHandlerTexture.GetContent(imageRequest);

                // show only images which are wider than high to avoid image deformation:
                if (image.width > image.height)
                {
                    /* change alpha channel of RawImages in the scene to visible 
                     * if an suitable image is found:
                     * source: https://forum.unity.com/threads/changing-a-new-ui-images-alpha-value.289755/#post-1912745
                     */
                    Color c = windowImage.color;
                    c.a = 1f;
                    windowImage.color = c;
                    window.GetComponent<RawImage>().texture = image;
                }
            }
        }
        // log the path of current image (only for debugging):
        //Debug.Log(imagePaths[num]);
    }

    /* -------------------------------------------------------------------
     * Following code is for now not needed.
     * -------------------------------------------------------------------
     */

    // Update is called once per frame;
    // changes the background image every 10 sec via a Coroutine:
    /*void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= 10)
        {
            elapsedTime -= 10;
            StartCoroutine("Load_image");
        }
        
    }*/

    // This method is for the second possibility to load images from the whole computer in Start().
    /*private List<string> GetFiles(string root, string fileEnding)
    {
        var fileList = new List<string>();

        Stack<string> pending = new Stack<string>();
        pending.Push(root);
        while (pending.Count != 0)
        {
            var path = pending.Pop();
            string[] next = null;
            try
            {
                next = Directory.GetFiles(path, fileEnding)
                                    .Where(extension => extension.EndsWith(".jpg")
                                                    || extension.EndsWith(".jpeg")
                                                    || extension.EndsWith(".png")).ToArray();
            }
            catch { }
            if (next != null && next.Length != 0)
                foreach (var file in next) fileList.Add(file);
            try
            {
                next = Directory.GetDirectories(path);
                foreach (var subdir in next) pending.Push(subdir);
            }
            catch { }
        }
        return fileList;
    }*/
}
