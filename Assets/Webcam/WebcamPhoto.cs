using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class WebcamPhoto : MonoBehaviour
{

    public WebCamTexture _webcamtex;
    private static string _PHOTO_RECORD_DIR = "photos";

    // Start is called before the first frame update
    void Start()
    {
        _webcamtex = new WebCamTexture();
    }

    public void SaveWebcamPhoto(){
        _webcamtex.Play();
        StartCoroutine(CaptureTextureAsPNG());
    }

    private IEnumerator CaptureTextureAsPNG()
    {
        yield return new WaitForEndOfFrame();
        Texture2D _TextureFromCamera = new Texture2D(_webcamtex.width, _webcamtex.height);
        _TextureFromCamera.SetPixels(_webcamtex.GetPixels());
        _TextureFromCamera.Apply();
        _webcamtex.Stop();
        byte[] bytes = _TextureFromCamera.EncodeToPNG();
        string filename = "img_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + ".png";
        var filepath = Path.Combine(Application.dataPath, _PHOTO_RECORD_DIR);
        filepath = Path.Combine(filepath, filename);

        File.WriteAllBytes(filepath, bytes);
    }
}
