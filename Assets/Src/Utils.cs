using System;
using Src.MonoComponent;
using UnityEngine;

namespace Src
{

	public class Cooldown
	{
		public TimeSpan Delay;

		private DateTime _ends = DateTime.MinValue;

		public bool IsCooldown() => DateTime.UtcNow < _ends;

		public void Trigger()
		{
			_ends = DateTime.UtcNow + Delay;
		}
	}
	
	public static class Utils
	{
		private static Ray _rayDown = new ()
		{
			direction = Vector3.down
		};
		
		public static float? GetClosestFloorY(this GameObject o)
		{
			_rayDown.origin = o.transform.position + Vector3.up;

			foreach (var hit in Physics.RaycastAll(_rayDown, 10))
			{
				if (hit.transform.gameObject.CompareTag("Ground"))
				{
					return hit.point.y;
				}
			}
			return null;
		}
		
		public static T FindComponent<T>(this GameObject o)
		{
			var c = o.GetComponent<T>();
			if (c == null) c = o.GetComponentInChildren<T>(true);
			return c;
		}
		
		public static Vector3 GetCenterTop(this Collider col)
		{
			var bounds = col.bounds;
			return new Vector3(bounds.center.x, bounds.center.y + bounds.extents.y, bounds.center.z);
		}
		public static bool CanSee(this Transform t1, Transform t2)
		{
			var rayDirection = (t2.position - t1.position).normalized * 200f;
			if (Physics.Raycast (t1.position, rayDirection,  out var hit)) {
				return hit.transform == t2;
			}
			return false;
		}
	}
}