using UnityEngine;

namespace Lean.Touch
{
	// This component allows you to rotate the current GameObject using events
	public class LeanManualRotate2D : MonoBehaviour
	{
		public enum AngleType
		{
			Degrees,
			Radians
		}

		public enum MultiplierType
		{
			None,
			DeltaTime
		}

		[Tooltip("Angle type")]
		public AngleType Angle;

		[Tooltip("Angle multiplier")]
		public MultiplierType Multiplier;

		[Tooltip("The rotation space")]
		public Space Space = Space.World;

		[Tooltip("The first axis of rotation")]
		public Vector3 AxisA = Vector3.down;

		[Tooltip("The second axis of rotation")]
		public Vector3 AxisB = Vector3.right;

		[Tooltip("Fixed multiplier of each rotation")]
		public float AngleMultiplier = 1.0f;

		[Tooltip("If you call the ResetRotation method, this allows you to set the Euler rotation this transform will be set to.")]
		public Vector3 DefaultRotation;

		public virtual void ResetRotation()
		{
			if (Space == Space.Self)
			{
				transform.localRotation = Quaternion.Euler(DefaultRotation);
			}
			else
			{
				transform.rotation = Quaternion.Euler(DefaultRotation);
			}
		}

		public virtual void Rotate(Vector2 delta)
		{
			if (Angle == AngleType.Radians)
			{
				delta *= Mathf.Rad2Deg;
			}

			if (Multiplier == MultiplierType.DeltaTime)
			{
				delta *= Time.deltaTime;
			}

			transform.Rotate(AxisA, delta.x * AngleMultiplier, Space);
			transform.Rotate(AxisB, delta.y * AngleMultiplier, Space);
		}
	}
}