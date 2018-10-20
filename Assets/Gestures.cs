using System.Collections;
using System.Collections.Generic;
using DigitalRubyShared;
using UnityEngine;

public class Gestures : MonoBehaviour {

    //public GameObject Brenda;

    private ScaleGestureRecognizer scaleGesture;
    private RotateGestureRecognizer rotateGesture;
    private TapGestureRecognizer tapGesture;

    private static GameObject objectToTransform;

    private void DebugText(string text, params object[] format)
    {
        Debug.Log(string.Format(text, format));
    }

    // Use this for initialization
    void Start () {
        //Brenda.SetActive(false);

        CreateScaleGesture();
        CreateRotateGesture();
        //CreateTapGesture();

        rotateGesture.AllowSimultaneousExecution(scaleGesture);
        scaleGesture.AllowSimultaneousExecution(rotateGesture);


        // prevent the one special no-pass button from passing through,
        //  even though the parent scroll view allows pass through (see FingerScript.PassThroughObjects)
        FingersScript.Instance.CaptureGestureHandler = CaptureGestureHandler;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void CreateTapGesture()
    {
        DebugText("tapp registered");
        tapGesture = new TapGestureRecognizer();
        tapGesture.StateUpdated += TapGestureCallback;
        //tapGesture.RequireGestureRecognizerToFail = SwipeGestureRecognizer;
        FingersScript.Instance.AddGesture(tapGesture);
    }

    private void TapGestureCallback(GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Ended)
        {
            DebugText("tapppped");
            DebugText("Tapped at {0}, {1}", gesture.FocusX, gesture.FocusY);
            //CreateAsteroid(gesture.FocusX, gesture.FocusY);
        }
    }

    private static bool? CaptureGestureHandler(GameObject obj)
    {
        //return false;

        // I've named objects PassThrough* if the gesture should pass through and NoPass* if the gesture should be gobbled up, everything else gets default behavior
        if(obj.tag.Contains("gestureable"))
        //if (obj.name.Contains("star"))
        {
            objectToTransform = obj;
            // allow the pass through for any element named "PassThrough*"
            return false;
        }
        else if (obj.name.StartsWith("NoPass"))
        {
            // prevent the gesture from passing through, this is done on some of the buttons and the bottom text so that only
            // the triple tap gesture can tap on it
            return true;
        }

        // fall-back to default behavior for anything else
        return null;
    }

    private void LateUpdateJunk()
    {
        //if (Time.timeSinceLevelLoad > nextAsteroid)
        //{
        //    nextAsteroid = Time.timeSinceLevelLoad + UnityEngine.Random.Range(1.0f, 4.0f);
        //    CreateAsteroid(float.MinValue, float.MinValue);
        //}

        int touchCount = Input.touchCount;
        if (FingersScript.Instance.TreatMousePointerAsFinger && Input.mousePresent)
        {
            touchCount += (Input.GetMouseButton(0) ? 1 : 0);
            touchCount += (Input.GetMouseButton(1) ? 1 : 0);
            touchCount += (Input.GetMouseButton(2) ? 1 : 0);
        }
        string touchIds = string.Empty;
        int gestureTouchCount = 0;
        foreach (GestureRecognizer g in FingersScript.Instance.Gestures)
        {
            gestureTouchCount += g.CurrentTrackedTouches.Count;
        }
        foreach (GestureTouch t in FingersScript.Instance.CurrentTouches)
        {
            touchIds += ":" + t.Id + ":";
        }
    }

    private void CreateScaleGesture()
    {
        Debug.Log("hello?");
        scaleGesture = new ScaleGestureRecognizer();
        scaleGesture.StateUpdated += ScaleGestureCallback;
        FingersScript.Instance.AddGesture(scaleGesture);
    }

    private void ScaleGestureCallback(GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Executing)
        {
            // Need to limit/cap the size of the scaled
            // items so so they dont't get too big/small

            DebugText("Scaled: {0}, Focus: {1}, {2}, {3}", scaleGesture.ScaleMultiplier, scaleGesture.FocusX, scaleGesture.FocusY, Screen.width);
            //gesture.
            //Brenda.transform.localScale *= scaleGesture.ScaleMultiplier;
            //gameObject.transform.localScale *= scaleGesture.ScaleMultiplier;
            objectToTransform.transform.localScale *= scaleGesture.ScaleMultiplier;
        }
    }

    private void CreateRotateGesture()
    {
        rotateGesture = new RotateGestureRecognizer();
        rotateGesture.StateUpdated += RotateGestureCallback;
        FingersScript.Instance.AddGesture(rotateGesture);
    }

    private void RotateGestureCallback(GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Executing)
        {
            //Brenda.transform.Rotate(0.0f, 0.0f, rotateGesture.RotationRadiansDelta * Mathf.Rad2Deg);
            //gameObject.transform.Rotate(0.0f, 0.0f, rotateGesture.RotationRadiansDelta * Mathf.Rad2Deg);
            objectToTransform.transform.Rotate(0.0f, 0.0f, rotateGesture.RotationRadiansDelta * Mathf.Rad2Deg);
        }
    }
}
