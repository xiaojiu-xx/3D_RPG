using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "QuestData_SO", menuName = "Quest/QuestData_SO", order = 0)]
public class QuestData_SO : ScriptableObject
{
    [System.Serializable]
    public class QuestRequire
    {
        public string name;
        public int requireAmount;
        public int currentAmout;
    }


    public string questName;            // 任务名称
    [TextArea]
    public string description;          // 任务描述

    [HideInInspector]
    public bool isStart;                // 任务是否开始
    [HideInInspector]
    public bool isComplete;             // 任务是否完成
    [HideInInspector]
    public bool isFinished;             // 任务是否结束

    public List<QuestRequire> questRequires = new List<QuestRequire>();
    public List<InventoryItem> rewards = new List<InventoryItem>();

    public void CheckQuestProgress()
    {
        // 去任务Require中检查要求的数量和当前的数量关系
        var finishRequires = questRequires.Where(r => r.requireAmount <= r.currentAmout);
        // 当Require全部满足关系后任务完成
        isComplete = finishRequires.Count() == questRequires.Count;

        if (isComplete)
        {
            Debug.Log("任务完成");
        }
    }

    // 当前任务需要 收集/消灭目标名字列表
    public List<string> GetRequireTargetNameList()
    {
        List<string> targetNameList = new List<string>();
        foreach (var require in questRequires)
        {
            targetNameList.Add(require.name);
        }

        return targetNameList;
    }

    // 给予奖励
    public void GiveRewards()
    {
        foreach (var reward in rewards)
        {
            if (reward.amount < 0)
            {
                // 从背包里面减去
                int requireCount = Mathf.Abs(reward.amount);

                if (InventoryManager.Instance.QuestItemInBag(reward.itemData) != null)
                {
                    if(InventoryManager.Instance.QuestItemInBag(reward.itemData).amount <= requireCount)
                    {
                        requireCount -= InventoryManager.Instance.QuestItemInBag(reward.itemData).amount;
                        InventoryManager.Instance.QuestItemInBag(reward.itemData).amount = 0;

                        if(InventoryManager.Instance.QuestItemInAction(reward.itemData) != null)
                            InventoryManager.Instance.QuestItemInAction(reward.itemData).amount -= requireCount;
                    }
                    else
                    {
                        InventoryManager.Instance.QuestItemInBag(reward.itemData).amount -= requireCount;
                    }
                }
                else
                {
                    InventoryManager.Instance.QuestItemInAction(reward.itemData).amount -= requireCount;
                }
            }
            else
            {
                // 
                InventoryManager.Instance.inventoryData.AddItem(reward.itemData, reward.amount);
            }

            InventoryManager.Instance.inventoryUI.RefreshUI();
            InventoryManager.Instance.actionUI.RefreshUI();
        }
    }
}