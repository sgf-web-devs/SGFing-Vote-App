using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using AlmostEngine.Screenshot;
using System.Collections;
using VoxelBusters.NativePlugins;
using UnityEngine.Animations;

public class ShareScene : MonoBehaviour {

    private AppController AppController;
    public ScreenshotTaker ScreenshotTaker;
    Texture2D TargetTexture;

    public RawImage ScreenshotRawImage;
    public GameObject CameraImageButtonsContainer;
    public GameObject StickerContainer;
    public GameObject StickerCanvas;

    private eShareOptions[] m_excludedOptions = new eShareOptions[0];
    private Button saveButton;
    private bool imageSaved = false;

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
        StartCoroutine(CaptureToTexture());
    }

    public void SavePicture(Button button)
    {
        Debug.Log("Saving the picture");

        button.GetComponent<Image>().color = new Color32(255, 255, 255, 100);
        saveButton = button;


        StartCoroutine(CaptureToTexture("save"));
    }

    IEnumerator AnimateSaveButton()
    {
        Debug.Log("changing button color back to white");
        yield return new WaitForSeconds(1f);
        saveButton.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
    }

    IEnumerator CaptureToTexture(string action = "share")
    {
        if (TargetTexture == null)
        {
            TargetTexture = new Texture2D(2, 2);
        }

        yield return StartCoroutine(ScreenshotTaker.CaptureScreenToTextureCoroutine(TargetTexture));

        if(action == "share")
        {
            ShareSheet _shareSheet = new ShareSheet();
            _shareSheet.Text = "SGFing Vote On Nov 6th!";
            _shareSheet.AttachImage(TargetTexture);
            NPBinding.UI.SetPopoverPointAtLastTouchPosition();
            NPBinding.Sharing.ShowView(_shareSheet, FinishedSharing);
        }

        if(action == "save")
        {
            NPBinding.MediaLibrary.SaveImageToGallery(TargetTexture, SaveImageToGalleryFinished);
        }
    }

    private void FinishedSharing(eShareResult _result)
    {
        Debug.Log("Finished Sharing");
    }

    private void SaveImageToGalleryFinished(bool _saved)
    {
        if(_saved) {
            //StartCoroutine(AnimateSaveButton());
            saveButton.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }

        Debug.Log("Saved image to gallery successfully ? " + _saved);
    }

    public void StickerSelectorClick(string StickerName)
    {
        Debug.Log(StickerName);
        Instantiate(Resources.Load(StickerName), StickerCanvas.transform);
    }
}
