using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using AlmostEngine.Screenshot;
using System.Collections;

public class ShareScene : MonoBehaviour {

    private AppController AppController;
    public ScreenshotTaker ScreenshotTaker;
    Texture2D TargetTexture;

    public RawImage ScreenshotRawImage;
    public GameObject CameraImageButtonsContainer;
    public GameObject StickerContainer;

    void Start ()
    {
        if(ScreenshotRawImage != null)
        {
            ScreenshotRawImage.texture = AppController.ScreenShotTexture;
        }
    }

    public void Close()
    {
        // "go back" to main screen
        SceneManager.UnloadSceneAsync("ShareScene");
    }

    public void HideStickers()
    {
        StickerContainer.SetActive(false);
        CameraImageButtonsContainer.SetActive(true);
    }

    public void ShowStickers()
    {
        StickerContainer.SetActive(true);
        CameraImageButtonsContainer.SetActive(false);
    }

    public void TakePicture()
    {
        Debug.Log("Take the picture");
        CaptureTexture();
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

        // to something with target texture
    }
}
