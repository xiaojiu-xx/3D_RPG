using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 数据库
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Inventory Data")]
public class InventoryData_OS : ScriptableObject
{
    public List<InventoryItem> items = new List<InventoryItem>();
    public void AddItem(ItemData_SO newItemData, int amount)
    {
        bool isExist = false;
        // 判断是否可以叠加
        if (newItemData.stackable)
        {
            foreach (var item in items)
            {
                if (item.itemData == newItemData)
                {
                    item.amount += amount;
                    isExist = true;
                }
            }
        }

        for (int i = 0; i<items.Count; i++)
        {
            if(!isExist && items[i].itemData == null)
            {
                items[i].itemData = newItemData;
                items[i].amount = amount;
                break; 
            }
        }
    }
}

[System.Serializable]
public class InventoryItem
{
    public ItemData_SO itemData;
    public int amount;
}
