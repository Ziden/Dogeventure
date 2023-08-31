using System.Collections;
using GameAddressables;
using Src;
using Src.Data;
using Unity.VisualScripting;
using UnityEngine;


	public static class OpenChestSequence
	{
		public static IEnumerator PlayOpenChestSequence(this PlayerAnimation animation, Chest chest)
		{
			var player = Player.Get();
			Main.Services.Audio.PlaySoundEffect(AssetSoundEffect.Handy_introduction_022_glbml_21786, 1f, 0.2f);
			var chestPosition = chest.transform.position;
			player.Entity.Equips.KeepWeapon();
			animation.Play(CharacterAnimation.openchest);
			Main.Services.Map.GameFrozen = true;
			player.Graphic.LookAt(new Vector3(chestPosition.x, player.Graphic.transform.position.y, chestPosition.z));
			var cam = Main.Services.Camera;
			cam.SetEase(0.01f);
			cam.PlayTrack(player.GetItemTrack);
			yield return new WaitForSeconds(2.4f);
			player.Graphic.LookAt(new Vector3(Camera.main.transform.position.x, player.Graphic.transform.position.y, Camera.main.transform.position.z));
			animation.Play(CharacterAnimation.raisehands);
			var weapon = chest.Item.GetComponent<Weapon>();
			if (weapon != null) // preload
				Main.Services.Assets.InstantiateWeaponPrefabAsync(weapon.WeaponPrefab, Object.Destroy);
			var displayItem = Object.Instantiate(chest.Item);
			displayItem.AddComponent<Rotating>().Z = 1;
			displayItem.transform.SetPositionAndRotation(player.Collider.GetCenterTop() + new Vector3(0, 1f, 0), Quaternion.Euler(-90, 0, 0));
			yield return new WaitForSeconds(1f);
			animation.ChangeExpression(FaceExpression.Happy);
			yield return new WaitForSeconds(1.5f);
			cam.ReturnCamera();
			Object.Destroy(displayItem.gameObject);
			chest.OnFinishOpening(player);
			cam.SetEase(2f);
			Main.Services.Map.GameFrozen = false;
			if (displayItem.Type == ItemType.Weapon && Main.Services.Data.Weapons().Count == 1)
			{
				yield return new WaitForSeconds(0.1f);
				player.Entity.AttackerEntity.TryAttack();
				yield return new WaitForSeconds(player.Entity.AttackerEntity.AttackCooldownSeconds + 0.1f);
				player.Entity.AttackerEntity.TryAttack();
			}
		}
	}
