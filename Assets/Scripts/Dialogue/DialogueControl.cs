using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueControl : MonoBehaviour
{
    public DialogueData_SO currentData;
    private bool isTalk;

    private void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Player") && currentData != null)
        {
            isTalk = true;
        }    
    }

    private void OnTriggerExit(Collider other) 
    {
        if(other.CompareTag("Player"))
        {
            DialogueUI.Instance.dialoguePanel.SetActive(false);
            isTalk = false;
        }
    }

    private void Update() 
    {
        if(isTalk && Input.GetMouseButtonDown(1))
        {
            OpenDialoguePanel();
        }
    }

    private void OpenDialoguePanel()
    {
        // 打开UI面板
        // 传输对话数据
        DialogueUI.Instance.UpdateDiaLogueData(currentData);
        DialogueUI.Instance.UpdateMainDialogue(currentData.dialoguePieces[0]);
    }
}
