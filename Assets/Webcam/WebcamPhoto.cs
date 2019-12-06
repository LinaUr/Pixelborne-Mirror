using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class WebcamPhoto : MonoBehaviour
{
    public WebCamTexture _webcamtex;
    private static string _PHOTO_RECORD_DIR = "photos";
    private string filedir;

    // Start is called before the first frame update
    void Start()
    {
        WebCamDevice[] webcamDevices = WebCamTexture.devices;
        if (webcamDevices.Length > 0){
            _webcamtex = new WebCamTexture(webcamDevices[0].name);
            filedir = Path.Combine(Application.dataPath, _PHOTO_RECORD_DIR);
            Directory.CreateDirectory(filedir);
        } else {
            Debug.Log("No webcam device found. Therefore Photos are not supported.");
        }
    }

    public void Record(){
        if(_webcamtex != null){
            _webcamtex.Play();
            StartCoroutine(CaptureTextureAsPNG());
        }
    }

    private IEnumerator CaptureTextureAsPNG()
    {
        yield return new WaitForSeconds(0.5f);

        Texture2D _TextureFromCamera = new Texture2D(_webcamtex.width, _webcamtex.height);
        _TextureFromCamera.SetPixels(_webcamtex.GetPixels());
        _TextureFromCamera.Apply();
        byte[] bytes = _TextureFromCamera.EncodeToPNG();

        string filename = "img_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + ".png";
        var filepath = Path.Combine(filedir, filename);

        File.WriteAllBytes(filepath, bytes);
        _webcamtex.Stop();
    }
}
