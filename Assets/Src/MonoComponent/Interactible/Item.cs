using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Weapon, Shield, Page, SuperHeart, DungeonKey
}

public class Item : MonoBehaviour
{
    public ItemType Type;
}
