using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

/* This class handles the change of the background.
 * It searches for pictures on the current PC and inserts them into the scene as a texture and
 * changes them at a certain time interval.
 */

public class WindowImages : MonoBehaviour
{
    private static List<string> imagePaths = new List<string>();
    private float elapsedTime;
    private GameObject[] windows = GameObject.FindGameObjectsWithTag("Window");

    // Start is called before the first frame update, used for initialisation
    void Start()
    {
        // get the path "C:\Users\<FolderOfCurrentUse>"
        string username = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        // find JPGs, JPEGs and PNGs in folder Pictures and its subdirectories 
        // and put the paths of the images in a list:
        /* source of code: 
         * https://stackoverflow.com/questions/8443524/using-directory-getfiles-with-a-regex-in-c/8443597#8443597
         */
        imagePaths = Directory.GetFiles(username + @"\Pictures", "*.*", SearchOption.AllDirectories)
                                .Where(extension => extension.EndsWith(".jpg")
                                                 || extension.EndsWith(".jpeg")
                                                 || extension.EndsWith(".png"))
                                .ToList();
        foreach (GameObject window in windows)
        {
            Debug.Log(window);

        }
        StartCoroutine("Load_image");

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
            // put the downloaded image file into the current gameObject (Background):
            //GameObject background = GameObject.Find("Background");
            //background.GetComponent<RawImage>().texture = image;
            //gameObject.GetComponent<RawImage>().texture = image;
            foreach (GameObject window in GameObject.FindGameObjectsWithTag("Window"))
            {
                window.GetComponent<RawImage>().texture = image;
                Debug.Log(imagePaths[num]);
                Debug.Log(window);

            }
        }

        // log the path of current image (only for debugging):
        Debug.Log(imagePaths[num]);
    }

    // Update is called once per frame;
    // changes the background image every 10 sec via a Coroutine:
    void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= 2)
        {
            elapsedTime -= 2;
            StartCoroutine("Load_image");
        }

    }
}
