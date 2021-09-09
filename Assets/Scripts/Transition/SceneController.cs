using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class SceneController : Singleton<SceneController>, IEndGameObserver
{
    public GameObject playerPrefab;
    public SceneFader sceneFaderPrefab;
    private bool isfadeFinish;
    private GameObject player;
    private NavMeshAgent playerAgent;
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Start() 
    {
        GameManager.Instance.AddObserver(this);
        isfadeFinish = true;
    }

    // 向目的地传送
    public void TransitionToDestination(TransitionPoint transitionPoint)
    {
        switch(transitionPoint.transitionType)
        {
            case TransitionPoint.TransitionType.SameScene:
                StartCoroutine(Transition(SceneManager.GetActiveScene().name, transitionPoint.destinationTag));
                break;
            case TransitionPoint.TransitionType.DifferenceScene:
                StartCoroutine(Transition(transitionPoint.sceneName, transitionPoint.destinationTag));
                break;
        }
    }

    // 向目的地传送
    IEnumerator Transition(string sceneName, TransitionDestination.DestinationTag destinationTag)
    {
        // TODO:传送之前保存所有的数据
        SaveManager.Instance.SavePlayerData();
        InventoryManager.Instance.SaveData();
        
        if(SceneManager.GetActiveScene().name != sceneName)
        {
            // FIXME:可设置渐入渐出效果
            yield return SceneManager.LoadSceneAsync(sceneName);
            yield return Instantiate(playerPrefab, GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            // 切换场景之后读取数据
            SaveManager.Instance.LoadPlayerData();
            yield break;
        }
        else
        {
            player = GameManager.Instance.playerState.gameObject;
            playerAgent = player.GetComponent<NavMeshAgent>();
            playerAgent.enabled = false;
            player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            playerAgent.enabled = true;
            yield return null;
        }
    }

    // 查找指定标签的目的地
    private TransitionDestination GetDestination( TransitionDestination.DestinationTag destinationTag )
    {
        // 找到所有的传送到达点
        var entrances = FindObjectsOfType<TransitionDestination>();

        for( int i = 0; i< entrances.Length; i++)
        {
            if(entrances[i].destinationTag == destinationTag)
            {
                return entrances[i];
            }
        }
        return null;
    }

    public void TransitionLoadMain()
    {
        StartCoroutine( LoadMain() );
    }

    // 回到主菜单
    IEnumerator LoadMain()
    {
        SceneFader fader = Instantiate(sceneFaderPrefab);
        yield return StartCoroutine(fader.FadeOut(2.0f));
        yield return SceneManager.LoadSceneAsync("Main");
        yield return StartCoroutine(fader.FadeIn(2.0f));
        yield break;
    }    

    // 创建新游戏
    public void TransitionToFirstLevel()
    {
        // 清除所有数据
        PlayerPrefs.DeleteAll();        
        // 开启协程准备加载场景
        StartCoroutine(LoadLevel("SampleScene"));
    }

    // 继续游戏 读取存档
    public void TransitionToLoadGame()
    {
        StartCoroutine( LoadLevel(SaveManager.Instance.SceneName) );
    }

    IEnumerator LoadLevel(string sceneName)
    {
        SceneFader fader = Instantiate(sceneFaderPrefab);
        if(sceneName != "")
        {
            yield return StartCoroutine(fader.FadeOut(2.0f));
            // 异步加载场景
            yield return SceneManager.LoadSceneAsync(sceneName);
            yield return player = Instantiate(playerPrefab, GameManager.Instance.GetEntrance().position, GameManager.Instance.GetEntrance().rotation);

            //
            SaveManager.Instance.SavePlayerData();
            yield return StartCoroutine(fader.FadeIn(2.0f));
            yield break;
        }
    }



    public void EndNotify()
    {
        if(isfadeFinish)
        {
            isfadeFinish = false;
            StartCoroutine(LoadMain());
        }
        
    }
}
