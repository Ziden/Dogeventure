using System;
using Assets.Code.Assets.Code.UIScreens.Base;
using Src.Data;
using UnityEngine;

namespace Src.Services
{
	public interface IUpdatedService : IService
	{
		void Update();
	}

	public class Direction
	{
		public float X;
		public float Y;

		public bool Zero() => X == 0 && Y == 0;
	}
	
	public class InputService : IUpdatedService
	{
		public static event Action<Vector3> OnDirectionChange;
		public static event Action OnButton1;
		public static event Action OnButton2;
		public static event Action OnAnyButton;
		public static event Action OnMenu;
		private Vector3 _direction = new();

		public void OnSceneChange() {}
		
		public void Update()
		{
			_direction.x = Input.GetAxisRaw("Horizontal");
			_direction.z = Input.GetAxisRaw("Vertical");
			OnDirectionChange?.Invoke(_direction);
			
			if (Input.GetButtonDown("Fire1"))
			{
				OnAnyButton?.Invoke();
				OnButton1?.Invoke();
			}

			if (Input.GetButtonDown("Fire2"))
			{
				OnAnyButton?.Invoke();
				OnButton2?.Invoke();
			}

			if (Input.GetKeyDown(KeyCode.LeftShift))
			{
				OnAnyButton?.Invoke();
				OnMenu?.Invoke();
			}
		}
	}
}