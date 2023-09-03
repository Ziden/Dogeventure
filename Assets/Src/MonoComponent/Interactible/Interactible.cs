using System;
using System.Collections;
using Src;
using Src.Data;
using Src.Services;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;


public class Interactible : MonoBehaviour
{
	public float InteractDistance = 2.5f;
	
	private VisibilityListener _visibility;

	public bool OnlyOnce = true;
	public event Action OnInteract;
	public event Action OnRemoveTrigger;
	private ColliderTrigger _colliderTrigger;
	
	public bool UseInteractionCollider => _colliderTrigger != null;
	
	void Start()
	{
		_visibility = GetComponent<VisibilityListener>();
		if (_visibility == null) _visibility = GetComponentInChildren<VisibilityListener>();
		if (_visibility == null) _visibility = this.AddComponent<VisibilityListener>();

        Map.Current.OnPlayerInitialized += OnPlayerInitialized;
	}

	public void Interact()
	{
		OnInteract?.Invoke();
		Map.Current.TriggerPlayerInteracted(this);
		if(OnlyOnce) RemoveTriggers();
	}

	public void RemoveTriggers()
	{
		GLog.Debug("[Interactible] Removing Interaction Triggers");
		OnRemoveTrigger?.Invoke();
		if (_colliderTrigger != null)
		{
			_colliderTrigger.RemoveTrigger();
		}
		Destroy(this);
	}
 
	void OnPlayerInitialized(Player p) 
	{
		_visibility.OnVisible += () => Main.Services.Map.SeeInteractible(this);
		_visibility.OnInvisible += () => Main.Services.Map.UnseeInteractible(this);
		
		_colliderTrigger = gameObject.FindComponent<ColliderTrigger>();
		if (_colliderTrigger != null)
		{
			_colliderTrigger.PlayerEnter += () => p.WorldInteraction.SetInteractionTarget(this);
			_colliderTrigger.PlayerLeave += () =>
			{
				if(p.WorldInteraction.InteractionTarget == this)
					p.WorldInteraction.SetInteractionTarget(null);
			};
		}
	}


}
