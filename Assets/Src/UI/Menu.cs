using System;
using System.Linq;
using Assets.Code.Assets.Code.UIScreens.Base;
using GameAddressables;
using Src.Data;
using Src.Services;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public enum MenuType
{
    Main, Items
}

public enum MainOptions
{
    Book, N1, N2, N3, N4, N5, N6, N7, N8, N9
}

public class Menu : UIBase
{
    private VisualElement _cursor;
    private Label _name;
    private Player _player;
    private MenuType _menuIn;
    private int _selected;
    private bool _moved;
    
    public override void OnOpen()
    {
        _player = Player.Get();
        _cursor = Root.Q("Cursor");
        ushort coins = 0;
        ushort shrooms = 0;
        PlayerData.Collectibles().TryGetValue(CollectiblePrefab.Coin, out coins);
        PlayerData.Collectibles().TryGetValue(CollectiblePrefab.Shroom, out shrooms);
        Root.Q<Label>("Coins").text = coins + "x";
        Root.Q<Label>("Shrooms").text = shrooms + "x";
        _name = Root.Q<Label>("SelectedText");
        var book = Root.Q("Book");
        if (PlayerData.Pages().Count == 0)  book.style.display = DisplayStyle.None;
        Main.Services.Map.GameFrozen = true;
        _menuIn = MenuType.Main;
        InputService.OnDirectionChange += OnDirectionChange;
        InputService.OnButton1 += OnB1;
        InputService.OnButton2 += OnB2;
        AdjustCursor();
    }

    private void OnB1()
    {
        if (_menuIn == MenuType.Main)
        {
            var selected = (MainOptions)_selected;
            if (selected == MainOptions.Book && PlayerData.Pages().Count > 0)
            {
                ScreenService.Close(this);
                Main.Services.Scenes.AddScene(
                    AssetScene.Book, new SceneSettings(
                        s => Main.Services.Map.GameFrozen = true, 
                        () => Main.Services.Screens.Open<Menu>()
                ));
            }
        }
    }

    private void OnB2()
    {
        ScreenService.Close(this);
    }

    public override void OnClose()
    {
        InputService.OnButton1 -= OnB1;
        InputService.OnButton2 -= OnB2;
        InputService.OnDirectionChange -= OnDirectionChange;
        Main.Services.Map.GameFrozen = false;
    }

    private void AdjustCursor()
    {
        if (_menuIn == MenuType.Main)
        {
            _cursor.style.left = 22 + (_selected * 80f);
            _name.text = ((MainOptions)_selected).ToString();
        }
    }

    private void OnDirectionChange(Vector3 v)
    {
        if (v.x == 0 && v.y == 0 && _moved) _moved = false;
        
        if (_moved) return;
        
        if (v.x > 0 && _selected < 8)
        {
            _selected++;
            AdjustCursor();
            _moved = true;
        }
        if (v.x < 0 && _selected > 0)
        {
            _selected--;
            AdjustCursor();
            _moved = true;
        }
    }

    public override AssetUIScreen ScreenAsset => AssetUIScreen.Menu;
}
