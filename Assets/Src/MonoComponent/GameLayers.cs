using UnityEngine;

namespace Src.MonoComponent
{
	public static class GameLayers
	{
		public static readonly int ENEMY_DAMAGE_DEALER = LayerMask.NameToLayer("EnemyDamageDealer");
		public static readonly int MONSTER = LayerMask.NameToLayer("Monster");
		public static readonly int GROUND = LayerMask.NameToLayer("Ground");
		public static readonly int STATIC_MAP = LayerMask.NameToLayer("StaticMap");
		public static readonly int COLLECTIBLES = LayerMask.NameToLayer("PlayerCollectible");

		public static bool IsStaticMap(this GameObject o) => o.layer == STATIC_MAP;
		
		public static bool IsGround(this GameObject o) => o.layer == GROUND;
	}
}