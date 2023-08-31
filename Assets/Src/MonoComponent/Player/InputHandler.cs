using Assets.Code.Assets.Code.UIScreens.Base;
using Src;
using Src.Data;
using Src.Services;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private Player _player;

    void Start()
    {
        Map.Current.OnPlayerInitialized += RegisterInputs;
    }

    private void OnButton1()
    {
        if (Main.Services.Map.GameFrozen || _player.BlockControls) return;
        if (!_player.WorldInteraction.TryInteract() || Main.Services.Map.Aggro.Count > 0)
        {
            if(_player.Entity.Equips.HasWeapon)  _player.Entity.AttackerEntity.TryAttack();
        }
    }

    private void OnDirectionChange(Vector3 d)
    {
        if (Main.Services.Map.GameFrozen || _player.BlockControls) return;
        _player.MoveDirection(d);
    }

    private void OnMenu()
    {
        var s =  Main.Services.Screens;
        if(!s.IsOpen<Menu>()) s.Open<Menu>();
        else s.Close<Menu>();
    }
    
    private void OnDestroy()
    {
        UnregisterInputs();
    }

    private void RegisterInputs(Player player)
    {
        _player = player;
        InputService.OnButton1 += OnButton1;
        InputService.OnDirectionChange += OnDirectionChange;
        InputService.OnMenu += OnMenu;
    }

    private void UnregisterInputs()
    {
        InputService.OnDirectionChange -= OnDirectionChange;
        InputService.OnButton1 -= OnButton1;
        InputService.OnMenu -= OnMenu;
    }
}
