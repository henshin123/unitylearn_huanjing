using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverPanel : MonoBehaviour
{
    public Text txt_score, txt_BestScore, txt_AddDiamondCount;
    public Button btn_restart, btn_Rank, btn_Home;
    public Image img_New;
    private void Awake()
    {
        btn_restart.onClick.AddListener(OnRestarButtonclick);
        btn_Rank.onClick.AddListener(OnRankButtonClick);
        btn_Home.onClick.AddListener(OnHomeButtonClick);
        img_New.gameObject.SetActive(false);
        EventCenter.AddListener(EventDefine.ShowGameOverPanel,Show);
        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.ShowGameOverPanel, Show);
    }
    private void Show()
    {
        if (GameManager.Instance.GetGameScore() > GameManager.Instance.GetBestScore()) img_New.gameObject.SetActive(true);
        GameManager.Instance.SaveScore(GameManager.Instance.GetGameScore());
        txt_BestScore.text = "最高分  " + GameManager.Instance.GetBestScore();
        txt_score.text = GameManager.Instance.GetGameScore().ToString();
        txt_AddDiamondCount.text ="+"+ GameManager.Instance.GetGameDiamond().ToString();
        //更新总的钻石数量
        GameManager.Instance.UpdateAllDiamond(GameManager.Instance.GetGameDiamond());
        gameObject.SetActive(true);
    }
    /// <summary>
    /// 再来一局点击事件
    /// </summary>
    private void OnRestarButtonclick()
    {
        //重新加载一下场景
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameData.IsAgainGame = true;
    }
    /// <summary>
    /// 排行榜
    /// </summary>
    private void OnRankButtonClick()
    {
        EventCenter.Broadcast(EventDefine.ShowRankPanel);
    }
    /// <summary>
    /// 返回主界面
    /// </summary>
    private void OnHomeButtonClick()
    {
        StartCoroutine(Dealy());//为了让点击音效出现延迟0.5秒
    }
    private IEnumerator Dealy()
    {
        yield return new WaitForSeconds(0.5f);
        //重新加载一下场景
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameData.IsAgainGame = false;
    }
}
