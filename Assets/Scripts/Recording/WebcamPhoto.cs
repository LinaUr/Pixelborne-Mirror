using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

// This class is used to take photos with the webcam.
public class WebcamPhoto : MonoBehaviour
{
    public WebCamTexture m_webcamtex;
    private static string m_PHOTO_RECORD_DIR = "photos";
    private string m_filedir;

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
    public void Record(){
        if(m_webcamtex != null){
            m_webcamtex.Play();
            StartCoroutine(CaptureTextureAsPNG());
        }
    }

    // This method is a Coroutine function that takes the screen shot and writes it to the disk.
    private IEnumerator CaptureTextureAsPNG()
    {
        yield return new WaitForSeconds(0.5f);

        Texture2D _TextureFromCamera = new Texture2D(m_webcamtex.width, m_webcamtex.height);
        _TextureFromCamera.SetPixels(m_webcamtex.GetPixels());
        _TextureFromCamera.Apply();
        byte[] bytes = _TextureFromCamera.EncodeToPNG();

        string filename = "img_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + ".png";
        var filepath = Path.Combine(m_filedir, filename);

        File.WriteAllBytes(filepath, bytes);
        m_webcamtex.Stop();
    }
}
