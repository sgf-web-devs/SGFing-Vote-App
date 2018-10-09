using UnityEngine;

public class AppController : MonoBehaviour {

    public static Texture2D ScreenShotTexture;

    void Awake()
    {

    }

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void SetScreenShotTexture(Texture2D texture)
    {
        ScreenShotTexture = texture;
    }
}
