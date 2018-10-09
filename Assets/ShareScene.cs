using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShareScene : MonoBehaviour {

    public RawImage ScreenshotRawImage;
    private AppController AppController;

    // Use this for initialization
    void Start () {
        if(ScreenshotRawImage != null) {
            ScreenshotRawImage.texture = AppController.ScreenShotTexture;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Close()
    {
        //SceneManager.LoadScene("SampleScene");
        //SceneManager.SetActiveScene("SampleScene");
        //SceneManager.UnloadScene("ShareScene");
        SceneManager.UnloadSceneAsync("ShareScene");
    }
}
