using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

// This class is used to take photos with the webcam.
public class WebcamPhoto : MonoBehaviour
{
    public WebCamTexture m_webcamtex;
    private static string m_PHOTO_RECORD_DIR = "photos";
    private string m_filedir;

    [DllImport("ImageEditing")]
    private static extern void processImage(ref Color32[] rawImage, int width, int height);

    // This method sets the webcam device if aviable.
    void Start()
    {
        WebCamDevice[] webcamDevices = WebCamTexture.devices;
        if (webcamDevices.Length > 0)
        {
            m_webcamtex = new WebCamTexture(webcamDevices[0].name);
            m_filedir = Path.Combine(Application.dataPath, m_PHOTO_RECORD_DIR);
            Directory.CreateDirectory(m_filedir);
        } else 
        {
            Debug.Log("No webcam device found. Therefore Photos are not supported.");
        }
    }

    // This method takes the webcam photo if a device has been found.
    public void Record()
    {
        if(m_webcamtex != null)
        {
            m_webcamtex.Play();
            StartCoroutine(CaptureTextureAsPNG());
        }
    }

    // This method is a Coroutine function that takes the taken webcam photo 
    // and call the processImage() function in the ImageEditing.dll 
    // which find the faces and save they as images in Assets/photos.
    private IEnumerator CaptureTextureAsPNG()
    {
        yield return new WaitForSeconds(0.5f);

        Texture2D textureFromCamera = new Texture2D(m_webcamtex.width, m_webcamtex.height);
        Color32[] image = m_webcamtex.GetPixels32();
        processImage(ref image, m_webcamtex.width, m_webcamtex.height);
        m_webcamtex.Stop();
    }
}
