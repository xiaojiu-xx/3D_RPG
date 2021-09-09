using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionButton : MonoBehaviour
{
    public KeyCode actionKey;

    private SlotHolder currentSlotHolder;

    private void Awake() 
    {
        currentSlotHolder = GetComponent<SlotHolder>();    
    }

    public void Update() 
    {
        if(Input.GetKeyDown(actionKey) && currentSlotHolder.itemUI.GetItemData())
            currentSlotHolder.UseItem();
    }
}
