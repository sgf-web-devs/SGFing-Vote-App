using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AlmostEngine.Screenshot
{
	/// <summary>
	/// The ScreenshotManager manages the capture process using a ScreenshotConfig.
	/// It also handles the hotkeys, burst mode, and the preview specific features like guides.
	/// </summary>
	/// 
	[ExecuteInEditMode]
	[RequireComponent (typeof(AudioSource))]
	public class ScreenshotManager : MonoBehaviour
	{
		public ScreenshotConfig m_Config = new ScreenshotConfig ();
		protected ScreenshotTaker m_ScreenshotCature;

		#region CAPTURE PROCESS

		public bool m_IsBurstActive = false;
		public bool m_IsCapturing = false;

		#endregion


		#region DELEGATES

		public static UnityAction onCaptureBeginDelegate = () => {
		};
		public static UnityAction onCaptureEndDelegate = () => {
		};

		public delegate void ExportDelegate (ScreenshotResolution res);

		/// <summary>
		/// Delegate called when the capture is a success.
		/// </summary>
		public static ExportDelegate onResolutionExportSuccessDelegate = (ScreenshotResolution res) => {
		};
		/// <summary>
		/// Delegate called when the capture is a failure.
		/// </summary>
		public static ExportDelegate onResolutionExportFailureDelegate = (ScreenshotResolution res) => {
		};

		#endregion


		#region BEHAVIOR METHODS

		public void Awake ()
		{	
			Reset ();
			ClearCache ();

			if (Application.isPlaying) {
				DontDestroyOnLoad (this.gameObject);
			}

			// Load settings in ingame preview mode
			if (Application.isPlaying && m_Config.m_PreviewInGameViewWhilePlaying == true) {
				InitIngamePreview ();
			}

		}

		void OnDestroy ()
		{
			#if UNITY_EDITOR
			SceneView.onSceneGUIDelegate -= HandleEventsDelegate;
			#endif
		}

		public void Reset ()
		{
			StopAllCoroutines ();

			m_IsCapturing = false;
			m_IsBurstActive = false;   
			
			InitScreenshotCapture ();
		}

		public void ClearCache ()
		{
			m_Config.ClearCache ();
			if (m_ScreenshotCature != null) {
				m_ScreenshotCature.ClearCache ();
			}
		}

		void Update ()
		{	
			#if UNITY_EDITOR
			if (!Application.isPlaying && EditorApplication.isCompiling) {
				Reset ();
				ClearCache ();
				return;
			} 
			#endif

			#if UNITY_EDITOR
			if (!Application.isPlaying) {
				RegisterUpdate ();
			}
			#endif

			if (Application.isPlaying) {
				HandleHotkeys ();
			} 	
		}

		protected void InitScreenshotCapture ()
		{
			if (m_ScreenshotCature != null)
				return;
			
			m_ScreenshotCature = GameObject.FindObjectOfType<ScreenshotTaker> ();
			if (m_ScreenshotCature == null) {
				m_ScreenshotCature = gameObject.GetComponent<ScreenshotTaker> ();
			}
			if (m_ScreenshotCature == null) {
				m_ScreenshotCature = gameObject.AddComponent<ScreenshotTaker> ();
			}
		}

		protected void HandleHotkeys ()
		{
			if (m_Config.m_AlignHotkey.IsPressed ()) {
				m_Config.AlignToView ();
			}

			if (m_Config.m_PauseHotkey.IsPressed ()) {
				m_Config.TogglePause ();
			}
			
			if (m_Config.m_UpdatePreviewHotkey.IsPressed ()) {
				UpdatePreview ();
			}

			if (m_Config.m_CaptureHotkey.IsPressed ()) {
				if (m_IsBurstActive) {
					StopBurst ();			
				} else {
					Capture ();
				}
			}
		}
		
		#if UNITY_EDITOR
		protected void RegisterUpdate ()
		{
			SceneView.onSceneGUIDelegate -= HandleEventsDelegate;
			SceneView.onSceneGUIDelegate += HandleEventsDelegate;					
		}

		protected void HandleEventsDelegate (SceneView sceneview)
		{
			HandleEditorHotkeys ();
		}

		public void HandleEditorHotkeys ()
		{
			Event e = Event.current;
			if (m_Config.m_UpdatePreviewHotkey.IsPressed (e)) {
				UpdatePreview ();
				e.Use ();
			}
			if (m_Config.m_CaptureHotkey.IsPressed (e)) {
				if (m_IsBurstActive) {
					StopBurst ();			
				} else {
					Capture ();
				}
				e.Use ();
			}
			if (m_Config.m_PauseHotkey.IsPressed (e)) {
				m_Config.TogglePause ();
				e.Use ();
			}
					
			if (m_Config.m_AlignHotkey.IsPressed (e)) {
				m_Config.AlignToView ();
				e.Use ();
			}
		}
				
		#endif

		#endregion

		#region CAPTURE

		/// <summary>
		/// Captures the active resolutions.
		/// </summary>
		public void Capture ()
		{
			StartCoroutine (CaptureAllCoroutine ());

		}

		/// <summary>
		/// Updates all active resolutions.
		/// </summary>
		public void UpdateAll ()
		{
			StartCoroutine (UpdateAllCoroutine ());
		}

		/// <summary>
		/// Updates the resolutions.
		/// </summary>
		public void Update (List<ScreenshotResolution> resolutions)
		{
			StartCoroutine (CaptureCoroutine (resolutions, false, false));
		}

		public IEnumerator UpdateAllCoroutine ()
		{
			// Get resolutions to capture
			List<ScreenshotResolution> resolutions = m_Config.GetActiveResolutions ();
			m_Config.UpdateGameviewResolution ();

			// Capture the resolutions
			yield return StartCoroutine (CaptureCoroutine (resolutions, false, false));
		}

		public IEnumerator CaptureAllCoroutine ()
		{
			// Get resolutions to capture
			List<ScreenshotResolution> resolutions = m_Config.GetActiveResolutions ();
			m_Config.UpdateGameviewResolution ();

			// Capture the resolutions
			yield return StartCoroutine (CaptureCoroutine (resolutions));
		}

		public IEnumerator CaptureCoroutine (List<ScreenshotResolution> resolutions, bool export = true, bool playSoundMask = true)
		{

			// Burst
			if (m_Config.m_ShotMode == ScreenshotConfig.ShotMode.BURST && !Application.isPlaying) {
				Debug.LogError ("In burst mode the application needs to be playing.");
				yield break;	
			}

			// Prevent multiple capture process		
			if (m_IsCapturing == true) {
				Debug.LogError ("A capture process is already running.");
				yield break;	
			}

			// We set capturing to true to prevent conflicts
			m_IsCapturing = true;
			
			// Hide guides if in-game preview
			if (Application.isPlaying && m_Config.m_PreviewInGameViewWhilePlaying && m_Config.m_ShowGuidesInPreview) {
				HideGuides ();
			}

			// Notify capture start
			onCaptureBeginDelegate ();

			// Capture
			if (m_Config.m_ShotMode == ScreenshotConfig.ShotMode.ONE_SHOT) {
				yield return StartCoroutine (UpdateCoroutine (resolutions, m_Config.GetActiveCameras (), m_Config.m_Overlays, export, playSoundMask));

			} else if (m_Config.m_ShotMode == ScreenshotConfig.ShotMode.BURST) {
				m_IsBurstActive = true;

				// Capture sequence
				for (int i = 0; i < m_Config.m_MaxBurstShotsNumber && m_IsBurstActive; ++i) {

					yield return StartCoroutine (UpdateCoroutine (resolutions, m_Config.GetActiveCameras (), m_Config.m_Overlays, export, playSoundMask));

					yield return new WaitForSeconds (m_Config.m_ShotTimeStep);
				}

				m_IsBurstActive = false;
			} 

			// Notify capture end
			onCaptureEndDelegate ();

			//Restore guides if in-game preview
			if (Application.isPlaying && m_Config.m_PreviewInGameViewWhilePlaying && m_Config.m_ShowGuidesInPreview) {
				ShowGuides ();
			} else {
				HideGuides ();
			}
			
			#if UNITY_EDITOR	
			// Refresh the gameview to trigger a paint event
			SceneView.RepaintAll ();
			#endif

			// Liberate the token
			m_IsCapturing = false;

		}

		public void StopBurst ()
		{
			// Set m_IsBurstActive to false so its coroutine loop will end
			m_IsBurstActive = false;
		}

		protected IEnumerator UpdateCoroutine (List<ScreenshotResolution> resolutions, List<ScreenshotCamera> cameras, List<ScreenshotOverlay> overlays, bool export = true, bool playSoundMask = true)
		{
			InitScreenshotCapture ();

			// Sound
			if (m_Config.m_PlaySoundOnCapture && playSoundMask) {
				PlaySound ();
			}

			// Texture update
			if (m_Config.m_ExportToDifferentLayers && cameras.Count > 1) {
				for (int i = 0; i < cameras.Count; ++i) {

					// Capture
					yield return StartCoroutine (DoUpdateCoroutine (resolutions, new List<ScreenshotCamera>{ cameras [i] }, overlays, export, playSoundMask));

					if (export) {
						foreach (ScreenshotResolution resolution in resolutions) {

							// Update the filenames with the camera name as layer name
							m_Config.UpdateFileName (resolution, cameras [i].m_Camera.name);


							// Export
							if (TextureExporter.ExportToFile (resolution.m_Texture, resolution.m_FileName, m_Config.m_FileFormat, (int)m_Config.m_JPGQuality)) {
								Debug.Log ("Screenshot created : " + resolution.m_FileName);
								onResolutionExportSuccessDelegate (resolution);
							} else {
								Debug.LogError ("Failed to create : " + resolution.m_FileName);
								onResolutionExportFailureDelegate (resolution);
							}
						}
					}
				}
			} else {

				// Capture
				yield return StartCoroutine (DoUpdateCoroutine (resolutions, cameras, overlays, export, playSoundMask));


				if (export) {
					foreach (ScreenshotResolution resolution in resolutions) {

						// Update the filenames
						m_Config.UpdateFileName (resolution);


						// Export
						if (TextureExporter.ExportToFile (resolution.m_Texture, resolution.m_FileName, m_Config.m_FileFormat, (int)m_Config.m_JPGQuality)) {
							Debug.Log ("Screenshot created : " + resolution.m_FileName);
							onResolutionExportSuccessDelegate (resolution);
						} else {
							Debug.LogError ("Failed to create : " + resolution.m_FileName);
							onResolutionExportFailureDelegate (resolution);
						}
					}
				}

			}
		}

		protected IEnumerator DoUpdateCoroutine (List<ScreenshotResolution> resolutions, List<ScreenshotCamera> cameras, List<ScreenshotOverlay> overlays, bool export, bool playSoundMask)
		{


			yield return StartCoroutine (m_ScreenshotCature.CaptureAllCoroutine (resolutions, 
				cameras,
				overlays,
				(m_Config.m_CaptureMode == ScreenshotTaker.CaptureMode.GAMEVIEW_RESIZING && m_Config.m_ResolutionCaptureMode == ScreenshotConfig.ResolutionMode.GAME_VIEW) ? ScreenshotTaker.CaptureMode.FIXED_GAMEVIEW : m_Config.m_CaptureMode,
				(int)m_Config.m_MultisamplingAntiAliasing,
				m_Config.m_CaptureActiveUICanvas,
				m_Config.m_ColorFormat,			                                                                          
				m_Config.m_RecomputeAlphaLayer, 
				m_Config.m_StopTimeOnCapture));


		}

		#endregion


		#region SOUND

		void PlaySound ()
		{	
			if (GetComponent<AudioSource> () == null)
				return;

			if (m_Config.m_ShotSound == null)
				return;

			GetComponent<AudioSource> ().PlayOneShot (m_Config.m_ShotSound);
		}

		#endregion


		#region PREVIEW METHODS


		List<ScreenshotResolution> m_PreviewList = new List<ScreenshotResolution> ();

		public virtual void UpdatePreview ()
		{
			StartCoroutine (UpdatePreviewCoroutine ());
		}

		/// <summary>
		/// Updates the preview coroutine, using the preview relative settings like guides.
		/// </summary>
		public IEnumerator UpdatePreviewCoroutine ()
		{
			if (m_IsCapturing)
				yield break;

			m_IsCapturing = true;

			// Delegate call
			onCaptureBeginDelegate ();

			// Update resolutions
			m_PreviewList.Clear ();
			m_PreviewList.Add (m_Config.GetFirstActiveResolution ());
			m_Config.UpdateGameviewResolution ();
//			m_Config.UpdateResolutionFilenames (m_PreviewList);

			// Update overlays & guides
			m_PreviewOverlayList.Clear ();
			m_PreviewOverlayList.AddRange (m_Config.m_Overlays);
			if (m_Config.m_ShowGuidesInPreview) {
				m_GuidesOverlay = new ScreenshotOverlay (m_Config.m_GuideCanvas);
				m_PreviewOverlayList.Add (m_GuidesOverlay);
				ShowGuides ();
			}


			// Capture preview
			InitScreenshotCapture ();
			yield return StartCoroutine (m_ScreenshotCature.CaptureAllCoroutine (m_PreviewList, 
				m_Config.GetActiveCameras (),
				m_PreviewOverlayList,
				(m_Config.m_CaptureMode == ScreenshotTaker.CaptureMode.GAMEVIEW_RESIZING && m_Config.m_ResolutionCaptureMode == ScreenshotConfig.ResolutionMode.GAME_VIEW) ? ScreenshotTaker.CaptureMode.FIXED_GAMEVIEW : m_Config.m_CaptureMode,
				(int)m_Config.m_MultisamplingAntiAliasing,
				m_Config.m_CaptureActiveUICanvas,
				m_Config.m_ColorFormat,			                                                                          
				m_Config.m_RecomputeAlphaLayer, 
				m_Config.m_StopTimeOnCapture));

			// Restore guides
			if (Application.isPlaying && m_Config.m_PreviewInGameViewWhilePlaying && m_Config.m_ShowGuidesInPreview) {
				ShowGuides ();
			} else {
				HideGuides ();
			}

			// Delegate call
			onCaptureEndDelegate ();

			m_IsCapturing = false;

		}

		List<ScreenshotOverlay> m_PreviewOverlayList = new List<ScreenshotOverlay> ();
		ScreenshotOverlay m_GuidesOverlay;

		protected void InitIngamePreview ()
		{
			m_PreviewOverlayList.Clear ();			
			m_PreviewOverlayList.AddRange (m_Config.m_Overlays);
			if (m_Config.m_ShowGuidesInPreview) {
				m_GuidesOverlay = new ScreenshotOverlay (m_Config.m_GuideCanvas);
				m_PreviewOverlayList.Add (m_GuidesOverlay);
			}

			InitScreenshotCapture ();
			m_ScreenshotCature.ApplySettings (m_Config.GetActiveCameras (), m_PreviewOverlayList, m_Config.m_CaptureMode, m_Config.m_CaptureActiveUICanvas);
		}

		protected void ShowGuides ()
		{
			if (m_Config.m_ShowGuidesInPreview && m_Config.m_GuideCanvas != null) {
				m_Config.m_GuideCanvas.gameObject.SetActive (true);
				m_Config.m_GuideCanvas.enabled = true;
				Image[] images = m_Config.m_GuideCanvas.GetComponentsInChildren<Image> ();
				foreach (Image image in images) {
					image.color = m_Config.m_GuidesColor;
				}
			}
		}

		protected void HideGuides ()
		{
			if (m_Config.m_PreviewInGameViewWhilePlaying == true && Application.isPlaying && m_Config.m_ShowGuidesInPreview && !m_IsCapturing)
				return;

			if (m_Config.m_GuideCanvas != null) {
				m_Config.m_GuideCanvas.gameObject.SetActive (false);
			}
		}

		#endregion


	}

}
