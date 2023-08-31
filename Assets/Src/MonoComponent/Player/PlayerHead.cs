using System.Linq;
using Src;
using Src.Data;
using UnityEngine;

public class PlayerHead : MonoBehaviour
{
	private Vector3 _lookingAt = Vector3.zero;

	private void Update()
	{
		if (Main.Services.Map.ViewingInteractibles.Count == 1)
		{
			var i = Main.Services.Map.ViewingInteractibles.First();
			if (transform.CanSee(i.transform))
			{
				OnSetInteractionTarget(i);
			}
			else
			{
				OnSetInteractionTarget(null);
			}
		}
		else
		{
			OnSetInteractionTarget(Player.Get().WorldInteraction.InteractionTarget);
		}
	}

	private void OnSetInteractionTarget(Interactible target)
	{
		if (target == null)
		{
			_lookingAt = Vector3.zero;
			return;
		}
		if (target != null)
		{
			var looking = target.GetComponent<LookingTarget>();
			if (looking != null)
			{
				_lookingAt = looking.LookingTransform.position;
				return;
			}

			var col = target.GetComponent<Collider>();
			if (col != null)
			{
				_lookingAt = col.GetCenterTop();
				return;
			}
			_lookingAt = target.transform.position;
		}
	}

	void LateUpdate()
	{
		if (_lookingAt != Vector3.zero)
		{
			var dir = (transform.position - _lookingAt).normalized;
			var angle = Vector3.Angle(Player.Get().Graphic.transform.forward, dir);
			var rot = Quaternion.LookRotation(dir, Vector3.up).eulerAngles;
			if (angle < 110) return;
			transform.rotation = Quaternion.Euler(rot.x + 25, rot.y + 90, rot.z - 45);
		}
	}
}
