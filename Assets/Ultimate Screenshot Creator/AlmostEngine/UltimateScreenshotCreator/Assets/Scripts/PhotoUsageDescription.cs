using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AlmostEngine.Screenshot
{
	public class PhotoUsageDescription : ScriptableObject
	{
		public string m_UsageDescription = "This application requires the access to the photo library to allow the user to take screenshots that are automatically added to the Camera Roll.";

		static PhotoUsageDescription m_Usage;

		#if UNITY_EDITOR
		public static PhotoUsageDescription GetOrCreateUsage ()
		{
			if (m_Usage == null) {
				PhotoUsageDescription[] usages = Resources.LoadAll<PhotoUsageDescription> ("");
				if (usages.Length != 0) {
					m_Usage = usages [0];
				} else {
					string path = "Assets/Resources/PhotoUsageDescription.asset";
					Debug.Log ("Creating new PhotoUsageDescription file at " + path);
					m_Usage = ScriptableObject.CreateInstance<PhotoUsageDescription> ();
					AssetDatabase.CreateAsset (m_Usage, path);				
					AssetDatabase.SaveAssets ();
					AssetDatabase.Refresh ();
				}
			}
			return m_Usage;
		}
		#endif

	}
}