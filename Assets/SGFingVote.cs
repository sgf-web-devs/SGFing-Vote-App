using System.Collections;
using System.Collections.Generic;
using AlmostEngine.Screenshot;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SGFingVote : MonoBehaviour {

    private AppController AppController;

    public Button CameraButton;
    public ScreenshotTaker ScreenshotTaker;
    public RawImage ScreenshotRawImage;
    Texture2D TargetTexture;

    void Start ()
    {

    }

    void Update ()
    {
		
	}

    public void TakePicture()
    {
        Debug.Log("Take the picture");
        CaptureTexture();

        //SceneManager.LoadScene("ShareScene", LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync("ShareScene", LoadSceneMode.Additive);
    }

    public void CaptureTexture()
    {
        StartCoroutine(CaptureToTexture());
    }

    IEnumerator CaptureToTexture()
    {
        // The texture must be initialized before calling the capture method.
        if (TargetTexture == null)
        {
            TargetTexture = new Texture2D(2, 2);
        }

        // We call the capture coroutine and wait for its termination
        yield return StartCoroutine(ScreenshotTaker.CaptureScreenToTextureCoroutine(TargetTexture));

        // We apply the texture to the GUI element
        AppController.ScreenShotTexture = TargetTexture;
    }

}
