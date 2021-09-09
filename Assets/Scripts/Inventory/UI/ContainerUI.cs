using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerUI : MonoBehaviour
{
    public SlotHolder[] slotHolders;
    public void RefreshUI()
    {
        for (int i = 0; i < slotHolders.Length; i++)
        {
            // SlotHolder容器数据对应数据库中的位置
            slotHolders[i].Index = i;
            slotHolders[i].UpdateItem();
        }
    }
}
