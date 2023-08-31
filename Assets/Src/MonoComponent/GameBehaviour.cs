using UnityEngine;

namespace Src.MonoComponent
{
	public class GameBehaviour : MonoBehaviour
	{
		protected IGameServices Services => Main.Services;
	}
}