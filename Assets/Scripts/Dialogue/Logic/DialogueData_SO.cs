using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Dialogue Data")]
public class DialogueData_SO : ScriptableObject
{
    public List<DialoguePiece> dialoguePieces = new List<DialoguePiece>();
    public Dictionary<string, DialoguePiece> dialoguePiecesDict = new Dictionary<string, DialoguePiece>();

    // 内置函数,在Unity面板当中修改后这个函数就会被修改
#if UNITY_EDITOR
    private void OnValidate()
    {
        dialoguePiecesDict.Clear();
        foreach (var piece in dialoguePieces)
        {
            if (!dialoguePiecesDict.ContainsKey(piece.ID))
            {
                dialoguePiecesDict.Add(piece.ID, piece);
            }
        }
    }
#else
    private void Awake() 
    {
        dialoguePiecesDict.Clear();
        foreach (var piece in dialoguePieces)
        {
            if (!dialoguePiecesDict.ContainsKey(piece.ID))
            {
                dialoguePiecesDict.Add(piece.ID, piece);
            }
        }
    }
#endif

    public QuestData_SO GetQuest()
    {
        QuestData_SO currentQuest = null;

        foreach (var piece in dialoguePieces)
        {
            if(piece.quest != null)
                currentQuest = piece.quest;
        }

        return currentQuest;
    }


}
