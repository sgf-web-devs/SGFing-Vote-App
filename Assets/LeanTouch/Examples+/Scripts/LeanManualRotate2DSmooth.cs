using UnityEngine;

namespace Lean.Touch
{
	// This modifies LeanManualRotate2D to be smooth
	public class LeanManualRotate2DSmooth : LeanManualRotate2D
	{
		[Tooltip("How quickly the rotation goes to the target value")]
		public float Dampening = 10.0f;

		[System.NonSerialized]
		private Quaternion remainingDelta = Quaternion.identity;

		public override void ResetRotation()
		{
			// Rotate and increment by delta
			var oldRotation = transform.localRotation;

			base.ResetRotation();

			remainingDelta *= Quaternion.Inverse(oldRotation) * transform.localRotation;

			// Revert
			transform.localRotation = oldRotation;
		}

		public override void Rotate(Vector2 delta)
		{
			// Rotate and increment by delta
			var oldRotation = transform.localRotation;

			base.Rotate(delta);

			remainingDelta *= Quaternion.Inverse(oldRotation) * transform.localRotation;

			// Revert
			transform.localRotation = oldRotation;
		}

		protected virtual void Update()
		{
			var factor   = LeanTouch.GetDampenFactor(Dampening, Time.deltaTime);
			var newDelta = Quaternion.Slerp(remainingDelta, Quaternion.identity, factor);

			transform.localRotation = transform.localRotation * Quaternion.Inverse(newDelta) * remainingDelta;

			remainingDelta = newDelta;
		}
	}
}