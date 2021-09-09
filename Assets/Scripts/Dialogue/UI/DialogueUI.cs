using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class DialogueUI : Singleton<DialogueUI>
{
    [Header("Basic Element")]
    public Image icon;
    public Text mainText;
    public Button nextBtn;
    public GameObject dialoguePanel;

    [Header("Options")]
    public RectTransform optionPanel;
    public OptionUI optionPrefab;

    [HideInInspector]
    public DialogueData_SO currentData;

    private int currentIndex = 0;

    protected override void Awake()
    {
        base.Awake();
        nextBtn.onClick.AddListener(ContinueDialogue);
    }

    private void ContinueDialogue()
    {
        if(currentIndex < currentData.dialoguePieces.Count)
            UpdateMainDialogue(currentData.dialoguePieces[currentIndex]);
        else
            dialoguePanel.SetActive(false);
    }

    public void UpdateDiaLogueData(DialogueData_SO data)
    {
        currentData = data;
        currentIndex = 0;
    }

    public void UpdateMainDialogue(DialoguePiece piece)
    {
        dialoguePanel.SetActive(true);
        

        if (piece.image != null)
        {
            icon.enabled = true;
            icon.sprite = piece.image;
        }
        else
            icon.enabled = false;

        mainText.text = "";
        mainText.DOText(piece.text, 2f);

        if (piece.options.Count == 0 && currentData.dialoguePieces.Count > 0)
        {
            nextBtn.interactable = true;
            nextBtn.transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            nextBtn.transform.GetChild(0).gameObject.SetActive(false);
            nextBtn.interactable = false;
        }
        
        // 创建OptionUI
        CreateOptionUI(piece);
        currentIndex++;
    }

    private void CreateOptionUI(DialoguePiece piece)
    {
        if(optionPanel.childCount > 0)
        {
            for (int i = 0; i < optionPanel.childCount; i++)
            {
                Destroy(optionPanel.GetChild(i).gameObject); 
            }
        }

        for (int i = 0; i < piece.options.Count; i++)
        {
            var option = Instantiate(optionPrefab, optionPanel);
            option.UpdateOption(piece, piece.options[i]);
        }
    }
}
