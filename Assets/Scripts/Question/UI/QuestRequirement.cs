using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class QuestRequirement : MonoBehaviour
{
    private Text requireName;
    private Text progressNumber;

    private void Awake() 
    {
        requireName = GetComponent<Text>();
        progressNumber = transform.GetChild(0).GetComponent<Text>();    
    }

    public void SetupRequirement(string name, int requireAmount, int currentAmout)
    {
        requireName.text = name;
        progressNumber.text = currentAmout.ToString() + " / " + requireAmount.ToString();
    }

    public void SetupRequirement(string name, bool isfinish)
    {
        if(isfinish)
        {
            requireName.text = name;
            progressNumber.text = "完成";
            requireName.color = Color.gray;
        }
    }
}
