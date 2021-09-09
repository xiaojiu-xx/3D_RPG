using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 物品类型 消耗品  武器  盔甲
public enum ItemType
{
    Useable,
    Weapon,
    Armor
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item Data")]
public class ItemData_SO : ScriptableObject
{// 一个物品大概具备的信息
    public ItemType itemType;                    // 物品类型
    public string ItemName;                      // 物品名称   
    public Sprite itemIcon;                      // 物品图片
    public int itemAmount;   
               
    [TextArea]
    public string description;                   // 物品描述
    public bool stackable;                       // 是否可叠加

    [Header("Useable Item")]
    public UseableItemData_SO useItemData;

    [Header("Weapon")]
    public GameObject weaponPrefab;
    public AttackData_SO weaponData;

    public AnimatorOverrideController weaponAnimator;
}
