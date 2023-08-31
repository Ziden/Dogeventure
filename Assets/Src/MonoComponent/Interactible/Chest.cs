using System.Collections;
using Src.Data;
using Src.Services;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public Item Item;
    private UniqueObject _u;
    private Animator _anim;
    public GameObject Lid;
    public GameObject Filled;
    public GameObject Empty;
    public GameObject Glow;
    private static readonly int Open = Animator.StringToHash("Open");

    void Start()
    {
        _u = GetComponent<UniqueObject>();
        _anim = GetComponent<Animator>();
        var interactible = GetComponent<Interactible>();
        if (_u.HasAlreadyInteractedWith())
        {
            interactible.RemoveTriggers();
            SetOpened();
        }
        else 
            interactible.OnInteract += () => StartCoroutine(OnInteract());
    }

    private IEnumerator OnInteract()
    {
        Glow.gameObject.SetActive(true);
        _anim.SetTrigger(Open);
        yield return new WaitForSeconds(3);
        SetOpened();
    }

    private void SetOpened()
    {
        Destroy(Lid);
        Destroy(Filled);
        Destroy(Glow);
        Empty.SetActive(true);
    }
    
    public void OnFinishOpening(Player p)
    {
        p.Entity.Equips.AddItem(Item);
        Map.Current.TriggerChestOpened(this);
    }
}
