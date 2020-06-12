using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class MainCameraScreen : MonoBehaviour
{
    private MainCameraScreen instance;
    public Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = gameObject.GetComponent<Camera>();
    }
    private void Awake() {
        instance = this;
        cam = gameObject.GetComponent<Camera>();

    }
    public void GetScreenTexture(int b)
    {
        /*ScreenCapture.CaptureScreenshot(b + ".png");
        var pct = ScreenCapture.CaptureScreenshotAsTexture(ScreenCapture.StereoScreenCaptureMode.BothEyes);
        byte[] _bytes = pct.EncodeToPNG();
        System.IO.File.WriteAllBytes(b + "v.png", _bytes);*/

        /*RenderTexture rt = cam.targetTexture;
        Texture2D rs = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
        Rect rct = new Rect(0, 0, rt.width, rt.height);
        rs.ReadPixels(rct, 0, 0);
        rs.Apply();
        byte[] _bytes = rs.EncodeToPNG();
        System.IO.File.WriteAllBytes(b + "v.png", _bytes);*/

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
