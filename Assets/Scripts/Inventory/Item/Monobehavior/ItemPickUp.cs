using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public ItemData_SO itemData;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 将物品添加到背包
            InventoryManager.Instance.inventoryData.AddItem(itemData, itemData.itemAmount);

            InventoryManager.Instance.inventoryUI.RefreshUI();

            // 装备武器
            // GameManager.Instance.playerState.EquipWeapon(itemData);

            // 检查任务
            QuestManage.Instance.UpdateQuestProgress(itemData.ItemName, itemData.itemAmount);

            Destroy(gameObject);
        }
    }
}
