using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : Singleton<SaveManager>
{
    private string sceneName = "level";
    public string SceneName{ get{return PlayerPrefs.GetString(sceneName); } }  // 获得存储在磁盘的场景名称
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Update() 
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SceneController.Instance.TransitionLoadMain();
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            SavePlayerData();
        }

        if(Input.GetKeyDown(KeyCode.L))
        {
            LoadPlayerData();
        }
    }

    public void Save(Object data, string key)
    {
        var jsonData = JsonUtility.ToJson(data, true);
        PlayerPrefs.SetString(key, jsonData);
        PlayerPrefs.SetString(sceneName, SceneManager.GetActiveScene().name);
        // 
        PlayerPrefs.Save();
    }

    public void Load(Object data, string key)
    {
        if(PlayerPrefs.HasKey(key))
        {
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(key), data);
        }
    }

    public void SavePlayerData()
    {
        Save(GameManager.Instance.playerState.characterData, GameManager.Instance.playerState.characterData.name);
    }

    public void LoadPlayerData()
    {
        Load(GameManager.Instance.playerState.characterData, GameManager.Instance.playerState.characterData.name);
    }

}
