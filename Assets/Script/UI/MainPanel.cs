using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : MonoBehaviour
{
    private Button btn_Start;
    private Button btn_Shop;
    private Button btn_Rank;
    private Button btn_Sound;
    private Button btn_Reset;
    private ManagerVars vars;
    private void Awake()
    {
        EventCenter.AddListener(EventDefine.ShowMainPanel,Show);
        EventCenter.AddListener<int>(EventDefine.ChangeSkin, ChangeSkin);
        vars = ManagerVars.GetManagerVars();
        Init();
        
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.ShowMainPanel, Show);
        EventCenter.RemoveListener<int>(EventDefine.ChangeSkin, ChangeSkin);
    }
    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Start()
    {
        if (GameData.IsAgainGame)
        {
            gameObject.SetActive(false);
            EventCenter.Broadcast(EventDefine.ShowGamePanel);
        }
        Sound();
        ChangeSkin(GameManager.Instance.GetCurrentSelectedSkin());
    }
    private void ChangeSkin(int skinIndex)
    {
        btn_Shop.transform.GetChild(0).GetComponent<Image>().sprite = vars.skinSpriteList[skinIndex];
    }
    private void Init()
    {
        btn_Start = transform.Find("btn_Start").GetComponent<Button>();//获取btn_Start的button组件
        btn_Start.onClick.AddListener(OnStartButtonClick);//为开始按钮添加监听
        btn_Rank = transform.Find("Btns/btn_Rank").GetComponent<Button>();
        btn_Rank.onClick.AddListener(OnRankButtonClock);
        btn_Sound = transform.Find("Btns/btn_Sound").GetComponent<Button>();
        btn_Sound.onClick.AddListener(OnSoundButtonClick);
        btn_Shop = transform.Find("Btns/btn_Shop").GetComponent<Button>();
        btn_Shop.onClick.AddListener(OnShopButtonClick);
        btn_Reset = transform.Find("Btns/btn_Reset").GetComponent<Button>();
        btn_Reset.onClick.AddListener(OnResetButtonClick);
    }
    //重置按钮点击
    private void OnResetButtonClick()
    {
        EventCenter.Broadcast(EventDefine.ShowResetPanel, "是否重置游戏?");
    }
    /// <summary>
    /// 开始按钮点击后调用此方法
    /// </summary>
    private void OnStartButtonClick()
    {
        GameManager.Instance.IsGameStarted = true;
        EventCenter.Broadcast(EventDefine.ShowGamePanel);
        gameObject.SetActive(false);
    }
    /// <summary>
    /// 商场按钮点击
    /// </summary>
    private void OnShopButtonClick()
    {
        EventCenter.Broadcast(EventDefine.ShowShopPanel);
        gameObject.SetActive(false);
    }
    /// <summary>
    /// 排行榜点击事件
    /// </summary>
    private void OnRankButtonClock()
    {
        EventCenter.Broadcast(EventDefine.ShowRankPanel);
    }
    /// <summary>
    /// 声音按钮点击
    /// </summary>
    private void OnSoundButtonClick()
    {
        GameManager.Instance.SetIsMusicOn(!GameManager.Instance.GetIsMusic());
        Sound();
    }
    private void Sound()
    {
        if (GameManager.Instance.GetIsMusic())
        {
            btn_Sound.transform.GetChild(0).GetComponent<Image>().sprite = vars.musicOn;
        }
        else
        {
            btn_Sound.transform.GetChild(0).GetComponent<Image>().sprite = vars.musicOff;
        }
        EventCenter.Broadcast(EventDefine.IsMusicOn, GameManager.Instance.GetIsMusic());
    }
}
