using Unity.VisualScripting;
using UnityEngine;

public class Liftable : MonoBehaviour
{
    void Start()
    {
        this.GetOrAddComponent<Interactible>().OnInteract += OnInteract;
    }

    public void OnInteract()
    {
        Debug.Log("LIFT");
        Player.Get().Animation.Play(CharacterAnimation.lift);
    }
}