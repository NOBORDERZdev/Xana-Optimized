using UnityEngine;
using System.Collections.Generic;

namespace Lean.Touch
{
	/// <summary>This component draws trails behind fingers.
	/// NOTE: This requires you to enable the LeanTouch.RecordFingers setting.</summary>
	[ExecuteInEditMode]
	[HelpURL(LeanTouch.HelpUrlPrefix + "LeanDragTrail")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Drag Trail")]
	public class LeanDragTrail : MonoBehaviour
	{
		// This class stores additional data for each LeanFinger
		[System.Serializable]
		public class FingerData : LeanFingerData
		{
			public LineRenderer Line;
			public float        Age;
			public float        Width;
		}

		/// <summary>The method used to find fingers to use with this component. See LeanFingerFilter documentation for more information.</summary>
		public LeanFingerFilter Use = new LeanFingerFilter(true);

		/// <summary>The method used to convert between screen coordinates, and world coordinates.</summary>
		public LeanScreenDepth ScreenDepth = new LeanScreenDepth(LeanScreenDepth.ConversionType.FixedDistance, Physics.DefaultRaycastLayers, 10.0f);

		/// <summary>The line prefab that will be used to render the trails.</summary>
		public LineRenderer Prefab;

		/// <summary>The maximum amount of active trails.
		/// -1 = Unlimited.</summary>
		public int MaxTrails = -1;

		/// <summary>How many seconds it takes for each trail to disappear after a finger is released.</summary>
		public float FadeTime = 1.0f;

		/// <summary>The color of the trail start.</summary>
		public Color StartColor = Color.white;

		/// <summary>The color of the trail end.</summary>
		public Color EndColor = Color.white;

		// This stores all the links between fingers and LineRenderer instances
		[SerializeField]
		[HideInInspector]
		protected List<FingerData> fingerDatas = new List<FingerData>();

		/// <summary>If you've set Use to ManuallyAddedFingers, then you can call this method to manually add a finger.</summary>
		public void AddFinger(LeanFinger finger)
		{
			Use.AddFinger(finger);
		}

		/// <summary>If you've set Use to ManuallyAddedFingers, then you can call this method to manually remove a finger.</summary>
		public void RemoveFinger(LeanFinger finger)
		{
			Use.RemoveFinger(finger);
		}

		/// <summary>If you've set Use to ManuallyAddedFingers, then you can call this method to manually remove all fingers.</summary>
		public void RemoveAllFingers()
		{
			Use.RemoveAllFingers();
		}

#if UNITY_EDITOR
		protected virtual void Reset()
		{
			Use.UpdateRequiredSelectable(gameObject);
		}
#endif

		protected virtual void Awake()
		{
			Use.UpdateRequiredSelectable(gameObject);
		}

		protected virtual void OnEnable()
		{
			LeanTouch.OnFingerUp += HandleFingerUp;
		}

		protected virtual void OnDisable()
		{
			LeanTouch.OnFingerUp -= HandleFingerUp;
		}

		protected virtual void Update()
		{
			// Get the fingers we want to use
			var fingers = Use.GetFingers(true);

			for (var i = 0; i < fingers.Count; i++)
			{
				var finger = fingers[i];

				if (LeanFingerData.Exists(fingerDatas, finger) == false)
				{
					// Too many active links?
					if (MaxTrails >= 0 && LeanFingerData.Count(fingerDatas) >= MaxTrails)
					{
						continue;
					}

					if (Prefab != null)
					{
						// Spawn and activate
						var clone = Instantiate(Prefab);

						clone.gameObject.SetActive(true);

						// Register with FingerData
						var fingerData = LeanFingerData.FindOrCreate(ref fingerDatas, finger);

						fingerData.Line  = clone;
						fingerData.Age   = 0.0f;
						fingerData.Width = Prefab.widthMultiplier;
					}
				}
			}

			// Update all FingerData
			for (var i = fingerDatas.Count - 1; i >= 0; i--)
			{
				var fingerData = fingerDatas[i];

				if (fingerData.Line != null)
				{
					UpdateLine(fingerData, fingerData.Finger, fingerData.Line);

					if (fingerData.Age >= FadeTime)
					{
						Destroy(fingerData.Line.gameObject);

						fingerDatas.RemoveAt(i);
					}
				}
				else
				{
					fingerDatas.RemoveAt(i);
				}
			}
		}

		protected virtual void UpdateLine(FingerData fingerData, LeanFinger finger, LineRenderer line)
		{
			var color0 = StartColor;
			var color1 =   EndColor;

			if (finger != null)
			{
				// Reserve one point for each snapshot
				line.positionCount = finger.Snapshots.Count;

				// Loop through all snapshots
				for (var i = 0; i < finger.Snapshots.Count; i++)
				{
					var snapshot = finger.Snapshots[i];

					// Get the world postion of this snapshot
					var worldPoint = ScreenDepth.Convert(snapshot.ScreenPosition, gameObject);

					// Write position
					line.SetPosition(i, worldPoint);
				}
			}
			else
			{
				fingerData.Age += Time.deltaTime;

				var alpha = Mathf.InverseLerp(FadeTime, 0.0f, fingerData.Age);

				color0.a *= alpha;
				color1.a *= alpha;
			}

			line.startColor = color0;
			line.endColor   = color1;
		}

		protected virtual void HandleFingerUp(LeanFinger finger)
		{
			var link = LeanFingerData.Find(fingerDatas, finger);

			if (link != null)
			{
				link.Finger = null; // The line will gradually fade out in Update
			}
		}
	}
}