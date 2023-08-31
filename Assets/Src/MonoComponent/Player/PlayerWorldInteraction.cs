using System;
using DG.Tweening;
using GameAddressables;
using Src.Data;
using UnityEngine;

namespace Src.Services
{
	public class PlayerWorldInteraction : MonoBehaviour
	{
		private GameObject _interactionIndicator;
		private Vector3 _interactionIndicatorOffset = new Vector3(0, 1f, 0);
		private Interactible _targetInteractible = null;
		
		public Interactible InteractionTarget => _targetInteractible;
		
		void Start()
		{
			Main.Services.Assets.InstantiateVfxPrefabAsync(VfxPrefab.ActionIndicator, o =>
			{
				o.SetActive(false);
				_interactionIndicator = o;
				if (_targetInteractible != null)
				{
					OnTargetInteractible(Player.Get(), _targetInteractible);
				}
			});
		}

		public bool TryInteract()
		{
			if (_targetInteractible == null) return false;
			GLog.Debug($"[WorldInteraction] Interacting with {_targetInteractible.name}");
			_targetInteractible.Interact();
			return true;
		}
		
		private void OnTargetInteractible(Player p, Interactible i)
		{
			if (_interactionIndicator == null) return;
			var collider = i.gameObject.FindComponent<Collider>();
			var target = Vector3.zero;
			if (collider != null)
				target = collider.GetCenterTop() + _interactionIndicatorOffset;
			else
				target = i.transform.position + _interactionIndicatorOffset;
			if(!_interactionIndicator.activeSelf)
				_interactionIndicator.transform.position = p.Collider.bounds.center;
			_interactionIndicator.SetActive(true);
			_interactionIndicator.transform.DOMove(target, 0.2f).SetAutoKill(true).Play();
		}

		private void OnUntargetInteractible(Player p, Interactible i)
		{
			if (_interactionIndicator == null) return;
			_interactionIndicator.transform.DOMove(p.Collider.bounds.center, 0.1f).SetAutoKill(true).OnComplete(() =>
			{
				_interactionIndicator.SetActive(false);
			}).Play();
			
		}

		private void OnRemoveTriggers()
		{
			GLog.Debug("[World Interaction] Removing triggers for " + _targetInteractible?.name);
			//Map.Current.OnSetTargetInteractible?.Invoke(null);
			Main.Services.Map.UnseeInteractible(_targetInteractible);
			Destroy(_targetInteractible);
			_targetInteractible = null;
			_interactionIndicator.SetActive(false);
		}

		public void SetInteractionTarget(Interactible i)
		{
			var player = Player.Get();
			if (i == null)
			{
				if (_targetInteractible != null)
				{
					_targetInteractible.OnRemoveTrigger -= OnRemoveTriggers;
					GLog.Debug("[World Interaction] Removing target "+_targetInteractible.name);
				}
				OnUntargetInteractible(player, _targetInteractible);
				//OnSetTargetInteractible?.Invoke( null);
			}
			_targetInteractible = i;
			if (i != null)
			{
				GLog.Debug("[World Interaction] Setting target "+_targetInteractible.name);
				OnTargetInteractible(player, _targetInteractible);
				//OnSetTargetInteractible?.Invoke(_targetInteractible);
				_targetInteractible.OnRemoveTrigger += OnRemoveTriggers;
			}
		}

		private void Update()
		{
			var player = Player.Get();
			Interactible closest = null;
			float closestDistance = 999f;
			foreach (var i in Main.Services.Map.ViewingInteractibles)
			{
				if (i.UseInteractionCollider) continue;
				var distance = Vector3.Distance(i.transform.position, player.transform.position);
				if (distance < i.InteractDistance && (closest is null || distance < closestDistance))
				{
					closest = i;
					closestDistance = distance;
				} 
			}

			if (closest != _targetInteractible && _targetInteractible == null ||
			    (_targetInteractible != null && !_targetInteractible.UseInteractionCollider))
				SetInteractionTarget(closest);
		}
	}
}