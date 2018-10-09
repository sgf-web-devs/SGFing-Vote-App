using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NatCamU.Core;
using UnityEngine.UI;

public class SGFCamera : MonoBehaviour {

    public RawImage preview;
    public AspectRatioFitter aspectFilter;

    // Use this for initialization
    void Awake () {
        //DontDestroyOnLoad(transform.gameObject);
        //NatCam.Play(DeviceCamera.FrontCamera);
        if (DeviceCamera.FrontCamera != null) {
            NatCam.Play(DeviceCamera.FrontCamera);
        } else {
            NatCam.Play(DeviceCamera.Cameras[0]);
        }

        NatCam.Play();

        NatCam.Camera.FocusMode = FocusMode.AutoFocus;
        NatCam.OnStart += NatCam_OnStart;
	}

    void NatCam_OnStart()
    {
        preview.texture = NatCam.Preview;
        aspectFilter.aspectRatio = NatCam.Preview.width / (float)NatCam.Preview.height;
    }


    // Update is called once per frame
	void Update () {
		
	}
}
