using System.Collections;
using System.Collections.Generic;
using DigitalRubyShared;
using UnityEngine;

public class Gestures : MonoBehaviour {

    public GameObject Brenda;

    private TapGestureRecognizer tapGesture;
    private TapGestureRecognizer doubleTapGesture;
    private ScaleGestureRecognizer scaleGesture;
    private RotateGestureRecognizer rotateGesture;
    private LongPressGestureRecognizer longPressGesture;

    private void DebugText(string text, params object[] format)
    {
        //bottomLabel.text = string.Format(text, format);
        Debug.Log(string.Format(text, format));
    }

    // Use this for initialization
    void Start () {
        Brenda.SetActive(false);

        CreateDoubleTapGesture();
        CreateTapGesture();
        CreateScaleGesture();
        CreateRotateGesture();
        CreateLongPressGesture();


        // prevent the one special no-pass button from passing through,
        //  even though the parent scroll view allows pass through (see FingerScript.PassThroughObjects)
        FingersScript.Instance.CaptureGestureHandler = CaptureGestureHandler;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void CreateLongPressGesture()
    {
        DebugText("Hallloooowww???");
        longPressGesture = new LongPressGestureRecognizer();
        longPressGesture.MinimumDurationSeconds = 0.1f;
        longPressGesture.MaximumNumberOfTouchesToTrack = 1;
        longPressGesture.StateUpdated += LongPressGestureCallback;
        FingersScript.Instance.AddGesture(longPressGesture);
    }

    private void LongPressGestureCallback(GestureRecognizer gesture)
    {
        //DebugText(gesture.State.ToString());

        if (gesture.State == GestureRecognizerState.Began)
        {
            DebugText("Long press began: {0}, {1}", gesture.FocusX, gesture.FocusY);
            BeginDrag(gesture.FocusX, gesture.FocusY);
        }
        else if (gesture.State == GestureRecognizerState.Executing)
        {
            DebugText("Long press moved: {0}, {1}", gesture.FocusX, gesture.FocusY);
            DragTo(gesture.FocusX, gesture.FocusY);
        }
        else if (gesture.State == GestureRecognizerState.Ended)
        {
            DebugText("Long press end: {0}, {1}, delta: {2}, {3}", gesture.FocusX, gesture.FocusY, gesture.DeltaX, gesture.DeltaY);
            EndDrag(longPressGesture.VelocityX, longPressGesture.VelocityY);
        }
    }

    private void BeginDrag(float screenX, float screenY)
    {
        DebugText("Inside BeginDrag");

        Vector3 pos = new Vector3(screenX, screenY, 0.0f);
        pos = Camera.main.ScreenToWorldPoint(pos);

        DebugText(pos.x.ToString());
        DebugText(pos.y.ToString());

        //Brenda.gameObject.transform.position = 
        //Brenda.transform.position = pos;
        //Brenda.GetComponent<Rigidbody2D>().MovePosition(pos);

        //RaycastHit2D hit = Physics2D.CircleCast(pos, 10.0f, Vector2.zero);
        //if (hit.transform != null && hit.transform.gameObject.name == "Brenda")
        //{
        //    DebugText("Inside Hit");
        //    Brenda = hit.transform.gameObject;
        //    Brenda.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        //    Brenda.GetComponent<Rigidbody2D>().angularVelocity = 0.0f;
        //}
        //else
        //{
        //    DebugText("Nope nope nope");
        //    longPressGesture.Reset();
        //}
    }

    private void DragTo(float screenX, float screenY)
    {
        if (Brenda == null)
        {
            return;
        }

        Vector3 pos = new Vector3(screenX, screenY, 0.0f);
        //pos = Camera.main.ScreenToWorldPoint(pos);
        Brenda.GetComponent<Rigidbody2D>().MovePosition(pos);
    }

    private void EndDrag(float velocityXScreen, float velocityYScreen)
    {
        if (Brenda == null)
        {
            return;
        }

        Vector3 origin = Camera.main.ScreenToWorldPoint(Vector3.zero);
        Vector3 end = Camera.main.ScreenToWorldPoint(new Vector3(velocityXScreen, velocityYScreen, 0.0f));
        Vector3 velocity = (end - origin);
        //Brenda.GetComponent<Rigidbody2D>().velocity = velocity;
        //Brenda.GetComponent<Rigidbody2D>().angularVelocity = UnityEngine.Random.Range(5.0f, 45.0f);
        //Brenda = null;

        DebugText("Long tap flick velocity: {0}", velocity);
        longPressGesture.Reset();
    }

    private void CreateDoubleTapGesture()
    {
        doubleTapGesture = new TapGestureRecognizer();
        doubleTapGesture.NumberOfTapsRequired = 2;
        doubleTapGesture.StateUpdated += DoubleTapGestureCallback;
        //doubleTapGesture.RequireGestureRecognizerToFail = tapGesture;
        FingersScript.Instance.AddGesture(doubleTapGesture);
    }

    private void DoubleTapGestureCallback(GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Ended)
        {
            DebugText("Double tapped at {0}, {1}", gesture.FocusX, gesture.FocusY);
            //RemoveAsteroids(gesture.FocusX, gesture.FocusY, 16.0f);
        }
    }

    private void CreateTapGesture()
    {
        DebugText("CreateTapGesture");
        tapGesture = new TapGestureRecognizer();
        tapGesture.StateUpdated += TapGestureCallback;
        tapGesture.RequireGestureRecognizerToFail = doubleTapGesture;
        FingersScript.Instance.AddGesture(tapGesture);
    }

    private void TapGestureCallback(GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Ended)
        {
            Brenda.SetActive(true);
            DebugText("Tapped at {0}, {1}", gesture.FocusX, gesture.FocusY);
            //CreateAsteroid(gesture.FocusX, gesture.FocusY);
        }
    }

    private static bool? CaptureGestureHandler(GameObject obj)
    {
        // I've named objects PassThrough* if the gesture should pass through and NoPass* if the gesture should be gobbled up, everything else gets default behavior
        if (obj.name == "Brenda" || obj.name == "Background")
        {
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

    private void LateUpdate()
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
        scaleGesture = new ScaleGestureRecognizer();
        scaleGesture.StateUpdated += ScaleGestureCallback;
        FingersScript.Instance.AddGesture(scaleGesture);
    }

    private void ScaleGestureCallback(GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Executing)
        {
            // dont get too big or too small
            DebugText("Scaled: {0}, Focus: {1}, {2}, {3}", scaleGesture.ScaleMultiplier, scaleGesture.FocusX, scaleGesture.FocusY, Screen.width);
            Brenda.transform.localScale *= scaleGesture.ScaleMultiplier;
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
            Brenda.transform.Rotate(0.0f, 0.0f, rotateGesture.RotationRadiansDelta * Mathf.Rad2Deg);
        }
    }
}
