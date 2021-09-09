using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// 背包栏   武器栏  盔甲栏
public enum SlotType { BAG, WEAPON, ARMOR, ACTION }
public class SlotHolder : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public SlotType slotType;
    public ItemUI itemUI;
    public InventoryItem currInventoryItem;                  // 格子拥有物品的数据
    public InventoryData_OS DataBase { get; set; }           // 格子拥有物品的数据来源
    public int Index { get; set; } = -1;                    // 格子拥有的物品数据在数据库中的索引

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount % 2 == 0)
        {
            UseItem();
        }
    }

    public void UseItem()
    {
        if (itemUI.GetItemData() != null)
        {
            if (itemUI.GetItemData().itemType == ItemType.Useable && itemUI.Bag_B.items[itemUI.Index_C].amount > 0)
            {
                GameManager.Instance.playerState.ApplyHealth(itemUI.GetItemData().useItemData.HP);
                itemUI.Bag_B.items[itemUI.Index_C].amount -= 1;

                // 更新任务当中的物品数量
                QuestManage.Instance.UpdateQuestProgress(itemUI.GetItemData().ItemName, -1);
            }
        }
        UpdateItem();
    }

    public void UpdateItem()
    {
        // 判断是哪一种装载容器
        switch (slotType)
        {
            case SlotType.BAG:
                DataBase = InventoryManager.Instance.inventoryData;
                break;
            case SlotType.WEAPON:
                DataBase = InventoryManager.Instance.equipmentData;
                if (DataBase.items[Index].itemData != null)
                {
                    GameManager.Instance.playerState.ChangeWeapon(DataBase.items[Index].itemData);
                }
                else
                {
                    GameManager.Instance.playerState.UnEquipment();
                }
                break;
            case SlotType.ARMOR:
                DataBase = InventoryManager.Instance.equipmentData;
                break;
            case SlotType.ACTION:
                DataBase = InventoryManager.Instance.actionData;
                break;
        }

        // 获得此格子的数据 映射到数据库中没有数据则为空
        currInventoryItem = DataBase.items[Index];
        // 将数据交给 ItemUI
        itemUI.SetupItemUI(ref currInventoryItem.itemData, ref currInventoryItem.amount);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // if (itemUI.GetItemData())
        // {
        //     InventoryManager.Instance.itemTooltip.SetupTooltip(itemUI.GetItemData());
        //     InventoryManager.Instance.itemTooltip.gameObject.SetActive(true);
        // }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryManager.Instance.itemTooltip.gameObject.SetActive(false);
    }

    public void OnDisable()
    {
        InventoryManager.Instance.itemTooltip.gameObject.SetActive(false);
    }
}
