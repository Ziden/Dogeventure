using System;
using System.Collections.Generic;
using System.Linq;
using GameAddressables;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Src.Services
{
	public class AudioService : IService
	{
		private List<AudioSource> _sources = new();
		private SortedList<DateTime, AudioSource> _finishes = new();
		
		private GameObject _sfxObject;
		
		public void OnSceneChange()
		{
			_sources.Clear();
			_finishes.Clear();
		}
		
		private GameObject SfxObject
		{
			get
			{
				if (_sfxObject == null)
				{
					_sfxObject = new GameObject("SFX");
				}
				return _sfxObject;
			}
		}

		private void CleanOld()
		{
			if (_finishes.Count == 0) return;
			var oldest = _finishes.First();
			if (DateTime.UtcNow > oldest.Key)
			{
				_finishes.Remove(oldest.Key);
				_sources.Add(oldest.Value);
			}
		}

		private AudioSource GetAudioSource()
		{
			CleanOld();
			if (_sources.Count == 0)
			{
				_sources.Add(SfxObject.AddComponent<AudioSource>());
			}
			var source = _sources.First();
			_sources.Remove(source);
			return source;
		}

		public void PlayMusic(AssetSoundEffect fx, float pitch = 1f, float volume = 1f)
		{
			var src = GetAudioSource();
			Main.Services.Assets.GetAudioClipAsync(fx, audio =>
			{
				_finishes.Add(DateTime.UtcNow + TimeSpan.FromSeconds(audio.length), src);
				src.volume = volume;
				src.pitch = pitch;
				src.loop = true;
				src.clip = audio;
				src.Play();
			});
		}
		
		public void PlaySoundEffect(AssetSoundEffect fx, float pitch = 1f, float volume = 1f)
		{
			var src = GetAudioSource();
			Main.Services.Assets.GetAudioClipAsync(fx, audio =>
			{
				_finishes.Add(DateTime.UtcNow + TimeSpan.FromSeconds(audio.length), src);
				src.clip = audio;
				src.volume = volume;
				src.pitch = pitch;
				src.loop = false;
				src.Play();
			});
		}
		
		public void PlaySyncedSoundEffect(AssetSoundEffect fx, Action onPlay, float pitch = 1f, float volume = 1f)
		{
			var src = GetAudioSource();
			Main.Services.Assets.GetAudioClipAsync(fx, audio =>
			{
				_finishes.Add(DateTime.UtcNow + TimeSpan.FromSeconds(audio.length), src);
				src.clip = audio;
				src.volume = volume;
				src.pitch = pitch;
				src.loop = false;
				src.Play();
				onPlay();
			});
		}
	}
}