using System;
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

    // This Coroutine takes the taken webcam photo 
    // and calls the processImage() function in the ImageEditing.dll 
    // which finds the faces and saves them as images in Assets/faces 
    // if the current PC is a Windows machine (the .dll works only for Windows).
    // Otherwise it saves the webcam photo unchanged in Assets/photos.
    private IEnumerator CaptureTextureAsPNG()
    {
        yield return new WaitForSeconds(0.5f);

        Texture2D textureFromCamera = new Texture2D(m_webcamtex.width, m_webcamtex.height);
        Color32[] image = m_webcamtex.GetPixels32();

        if ((Application.platform == RuntimePlatform.WindowsPlayer) || (Application.platform == RuntimePlatform.WindowsEditor))
        {
            processImage(ref image, m_webcamtex.width, m_webcamtex.height);
        } else
        {
            textureFromCamera.SetPixels32(image);
            textureFromCamera.Apply();                                              
            byte[] bytes = textureFromCamera.EncodeToPNG();
            DateTime now = DateTime.Now;

            string filename = $"{now.Day.ToString("d2")}-{now.Month.ToString("d2")}-{now.Year}_{now.Hour.ToString("d2")}-{now.Minute.ToString("d2")}-{now.Second.ToString("d2")}.png";
            var filepath = Path.Combine(m_filedir, filename);

            File.WriteAllBytes(filepath, bytes);
        }

        m_webcamtex.Stop();
    }
}
