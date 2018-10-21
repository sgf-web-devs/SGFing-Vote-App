using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NatCamU.Core;
using UnityEngine.UI;

public class SGFCamera : MonoBehaviour {

    public RawImage preview;
    public AspectRatioFitter aspectFilter;

    // Use this for initialization
    void Start()
    {
        NatCam.Camera = DeviceCamera.FrontCamera ?? DeviceCamera.Cameras[0];

        if (!NatCam.Camera)
        {
            Debug.LogError("Camera is null");
            return;
        }
        NatCam.Camera.PreviewResolution = CameraResolution._1920x1080;
        NatCam.Camera.PhotoResolution = CameraResolution._1920x1080;
        NatCam.Play();


        NatCam.OnStart += NatCam_OnStart;
	}

    void NatCam_OnStart()
    {
        preview.texture = NatCam.Preview;
        aspectFilter.aspectRatio = NatCam.Preview.width / (float)NatCam.Preview.height;
        NatCam.Camera.FocusMode = FocusMode.AutoFocus;
    }

    public void SwitchCamera()
    {
        // Switch camera
        if (NatCam.Camera.IsFrontFacing)
            NatCam.Camera = DeviceCamera.RearCamera;
        else
            NatCam.Camera = DeviceCamera.FrontCamera;

        NatCam.Play();
    }


    // Update is called once per frame
    void Update () {
		
	}
}
