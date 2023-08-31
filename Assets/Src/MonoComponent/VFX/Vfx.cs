using System;
using System.Collections.Generic;
using GameAddressables;
using Unity.VisualScripting;
using UnityEngine;

public class Vfx : MonoBehaviour
{
	public VfxPrefab Effect;
	
	public void StartEffect(Vector3 pos)
	{
		if (gameObject == null) return;
		gameObject.SetActive(true);
		foreach (var particle in GetComponentsInChildren<ParticleSystem>(true))
		{
			particle.Stop();
			particle.time = 0;
			var main = particle.main;
			particle.gameObject.SetActive(true);
			main.stopAction = ParticleSystemStopAction.Disable;
			//particle.Play();
		}

		var rootParticle = GetComponent<ParticleSystem>();
		if (rootParticle == null) return;
		rootParticle.Stop();
		rootParticle.gameObject.SetActive(true);
		transform.position = pos;
		var rootMain = rootParticle.main;
		rootMain.stopAction = ParticleSystemStopAction.Disable;
		rootParticle.time = 0;
		rootParticle.Play();
;	}

	private void OnDisable()
	{
		Main.Services.Vfx.ReturnToPool(this);
	}
}
