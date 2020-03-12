using System;
using System.Collections;
using System.IO;
using UnityEngine;

// This class is used to take photos with the webcam.
public class WebcamPhoto : MonoBehaviour
{
    private string m_filedir;
    
    private readonly static string PHOTO_RECORD_DIR = "photos";

    public WebCamTexture m_webcamtex;

    // This method sets the webcam device if available.
    void Start()
    {
        WebCamDevice[] webcamDevices = WebCamTexture.devices;
        if (webcamDevices.Length > 0)
        {
            m_webcamtex = new WebCamTexture(webcamDevices[0].name);
            m_filedir = Path.Combine(Application.dataPath, PHOTO_RECORD_DIR);
            Directory.CreateDirectory(m_filedir);
        }
        else 
        {
            Debug.Log("No webcam device found. Therefore Photos are not supported.");
        }
    }

    // This method takes the webcam photo if a device has been found.
    public void Record()
    {
        if (m_webcamtex != null)
        {
            m_webcamtex.Play();
            StartCoroutine(CaptureTextureAsPNG());
        }
    }

    // This coroutine takes the taken webcam photo and writes it to the disk.
    private IEnumerator CaptureTextureAsPNG()
    {
        yield return new WaitForSeconds(0.5f);

        Texture2D textureFromCamera = new Texture2D(m_webcamtex.width, m_webcamtex.height);
        Color32[] image = m_webcamtex.GetPixels32();
        
        textureFromCamera.SetPixels32(image);
        textureFromCamera.Apply();
        byte[] bytes = textureFromCamera.EncodeToPNG();

        DateTime now = DateTime.Now;
        string filename = $"{now.Year}-{now.Month.ToString("d2")}-{now.Day.ToString("d2")}_{now.Hour.ToString("d2")}-{now.Minute.ToString("d2")}-{now.Second.ToString("d2")}.png";
        var filepath = Path.Combine(m_filedir, filename);

        File.WriteAllBytes(filepath, bytes);

        m_webcamtex.Stop();
    }
}
