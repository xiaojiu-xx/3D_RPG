using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    public Image icon;
    public Text amount;

    [HideInInspector]
    public ItemData_SO currentItemData_A;
    // ItemUI所属的数据库
    public InventoryData_OS Bag_B { get; set; }
    // 在数据库List中的位置
    public int Index_C { get; set; } = -1;

    public void SetupItemUI_D(ItemData_SO item, int itemAmount)
    {
        if (itemAmount == 0)
        {
            Bag_B.items[Index_C].itemData = null;
            icon.gameObject.SetActive(false);
            return;
        }

        if (itemAmount < 0)
        {
            item = null;
        }

        if (item != null)
        {
            currentItemData_A = item;
            icon.sprite = item.itemIcon;            // 设置图标
            amount.text = itemAmount.ToString();    // 设置数量
            icon.gameObject.SetActive(true);        // 显示
        }
        else
            icon.gameObject.SetActive(false);
    }

    // 将数据以UI展示 
    public void SetupItemUI(ref ItemData_SO item, ref int itemAmount)
    {
        // if(itemAmount == 0)
        // {
        //     item = null;
        //     icon.gameObject.SetActive(false);
        //     return;
        // }

        if (item != null)
        {
            icon.sprite = item.itemIcon;                // 设置图标
            amount.text = itemAmount.ToString();        // 设置数量
            icon.gameObject.SetActive(true);
        }
        else
            icon.gameObject.SetActive(false);

    }

    public ItemData_SO GetItemData()
    {// TODO:要进行修改
        return Bag_B.items[Index_C].itemData;
    }
}
