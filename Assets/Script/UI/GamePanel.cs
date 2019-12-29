using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : MonoBehaviour
{
    private Button btn_Pause;
    private Button btn_Play;
    private Text txt_Score;
    private Text txt_DiamondCount;
    private void Awake()
    {
        Init();
    }
    private void Init()
    {
        //事件添加广播
        EventCenter.AddListener(EventDefine.ShowGamePanel, Show);
        EventCenter.AddListener<int>(EventDefine.UpdateScoreText, UpdateScoreText);
        EventCenter.AddListener<int>(EventDefine.UpdateDiamondText, UpdateDiamondText);
        btn_Pause = transform.Find("btn_Pause").GetComponent<Button>();
        btn_Pause.onClick.AddListener(OnPauseButtonClick);
        btn_Play = transform.Find("btn_Play").GetComponent<Button>();
        btn_Play.onClick.AddListener(OnPlayButtonClick);
        btn_Play.gameObject.SetActive(false);
        txt_Score = transform.Find("txt_Score").GetComponent<Text>();
        txt_DiamondCount = transform.Find("Diamond/txt_diamondCount").GetComponent<Text>();
        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.ShowGamePanel, Show);
        EventCenter.RemoveListener<int>(EventDefine.UpdateScoreText, UpdateScoreText);
        EventCenter.RemoveListener<int>(EventDefine.UpdateDiamondText, UpdateDiamondText);
    }
    private void Show()
    {
        gameObject.SetActive(true);
    }
    /// <summary>
    /// 更新成绩显示
    /// </summary>
    private void UpdateScoreText(int score)
    {
        txt_Score.text = score.ToString();
    }
    private void UpdateDiamondText(int diamond)
    {
        txt_DiamondCount.text = diamond.ToString();
    }
    /// <summary>
    /// 暂停按钮点击
    /// </summary>
    private void OnPauseButtonClick()
    {
        btn_Pause.gameObject.SetActive(false);
        btn_Play.gameObject.SetActive(true);
        //游戏暂停
        Time.timeScale = 0;
        GameManager.Instance.IsPause = true;
    }
    private void OnPlayButtonClick()
    {
        btn_Pause.gameObject.SetActive(true);
        btn_Play.gameObject.SetActive(false);
        Time.timeScale = 1;
        GameManager.Instance.IsPause = false;
    }

}
