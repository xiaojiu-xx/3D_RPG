using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class MainMenu : MonoBehaviour
{
    private Button newGameBtn;
    private Button continueBtn;
    private Button quitBtn;

    PlayableDirector director;

    private void Awake()
    {
        newGameBtn = transform.GetChild(1).GetComponent<Button>();
        continueBtn = transform.GetChild(2).GetComponent<Button>();
        quitBtn = transform.GetChild(3).GetComponent<Button>();

        newGameBtn.onClick.AddListener(PlayTimeline);
        continueBtn.onClick.AddListener(ContinueGame);
        quitBtn.onClick.AddListener(QuitGame);

        director = FindObjectOfType<PlayableDirector>();
        // 当TimeLine播放完成后执行NewGame
        director.stopped += NewGame;
    }

    void PlayTimeline()
    {
        director.Play();
    }

    // 创建一个新的游戏
    private void NewGame( PlayableDirector obj)
    {
        // 切换场景
        SceneController.Instance.TransitionToFirstLevel();
    }

    // 继续游戏
    private void ContinueGame()
    {
        // 切换场景，读取进度
        SceneController.Instance.TransitionToLoadGame();
    }

    // 退出游戏
    private void QuitGame()
    {
        Application.Quit();
    }
}
