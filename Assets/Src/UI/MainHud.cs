using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class MainHud : MonoBehaviour
{
    private VisualElement HealthBar; 
    private VisualElement [] Icons = new VisualElement[20];
    
    void Start()
    {
        Map.OnMapInitialized += OnMapInitialized;
    }

    private void OnMapInitialized()
    {
        var player = Player.Get();
        HealthBar = GetComponent<UIDocument>().rootVisualElement.Q("HealthBar");
        Icons = HealthBar.Children().ToArray();
        SetBarSize(player.Entity.Stats.MaxLife);
        player.Entity.Stats.OnHealthChanged += OnHealthChanged;
        player.Entity.Stats.OnMaxHealthChanged += OnMaxHealthChanged;
    }
    
    private void OnMaxHealthChanged(int newValue)
    {
        SetBarSize(newValue);
    }
    
    private void OnHealthChanged(int newValue)
    {
        for (var x = 0; x < Player.Get().Entity.Stats.MaxLife; x++)
        {
            if (x >= newValue)  Icons[x].style.visibility = Visibility.Hidden;
            else Icons[x].style.visibility = Visibility.Visible;
        }
    }

    private void SetBarSize(int size)
    {
        for (var x = 0; x < 20; x++)
        {
            if (x < size)  Icons[x].style.display = DisplayStyle.Flex;
            else Icons[x].style.display = DisplayStyle.None;
        }
    }
}
