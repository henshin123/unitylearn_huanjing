using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ShopPanel : MonoBehaviour
{
    private ManagerVars vars;
    private Transform parent;
    private Text txt_Name;
    private Button btn_Back;
    private Button btn_Select;
    private Button btn_Buy;
    private Text txt_Diamond;
    private int selectindex;
    private void Awake()
    {
        vars = ManagerVars.GetManagerVars();
        parent = transform.Find("ScroolRect/Parent");
        txt_Name = transform.Find("txt_Name").GetComponent<Text>();
        btn_Back = transform.Find("btn_Back").GetComponent<Button>();
        btn_Select = transform.Find("btn_Select").GetComponent<Button>();
        btn_Buy = transform.Find("btn_Buy").GetComponent<Button>();
        btn_Back.onClick.AddListener(OnBackButtonClick);
        txt_Diamond = transform.Find("Diamond/Text").GetComponent<Text>();
        btn_Buy.onClick.AddListener(OnBuyButtonClick);
        btn_Select.onClick.AddListener(OnSelectButtomClick);
        EventCenter.AddListener(EventDefine.ShowShopPanel, Show);
       
    }
    private void Start()
    {
        Init();
        gameObject.SetActive(false);//第一次调动的时候隐藏

    }
    private void Show()
    {
        gameObject.SetActive(true);
        
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.ShowShopPanel, Show);
    }
    private void OnBackButtonClick()
    {
        EventCenter.Broadcast(EventDefine.ShowMainPanel);
        gameObject.SetActive(false);
    }
    /// <summary>
    /// 购物按钮点击
    /// </summary>
    private void OnBuyButtonClick()
    {
        int price = int.Parse(btn_Buy.GetComponentInChildren<Text>().text);
        if(price>GameManager.Instance.GetAllDiamond())
        {
            EventCenter.Broadcast(EventDefine.Hint, "钻石不足");
            return;
        }
        else
        {
            GameManager.Instance.UpdateAllDiamond(-price);
            GameManager.Instance.SetSkinUnlocked(selectindex);
            parent.GetChild(selectindex).GetChild(0).GetComponent<Image>().color = Color.white;
        }
    }
    /// <summary>
    /// 选择按钮点击事件
    /// </summary>
    private void OnSelectButtomClick()
    {
        EventCenter.Broadcast(EventDefine.ChangeSkin,selectindex);
        GameManager.Instance.SetSelectSkin(selectindex);
        EventCenter.Broadcast(EventDefine.ShowMainPanel);
        gameObject.SetActive(false);
    }

    private void Init()
    {
        parent.GetComponent<RectTransform>().sizeDelta = new Vector2((vars.skinSpriteList.Count+2)*160, 302);
        for (int i = 0; i < vars.skinSpriteList.Count; i++)
        {
            GameObject go = Instantiate(vars.skinChooseItemPre, parent);
            if(GameManager.Instance.GetSkinUnlocked(i) == false)//未解锁
            {
                go.GetComponentInChildren<Image>().color = Color.gray;
            }
            else
            {
                go.GetComponentInChildren<Image>().color = Color.white;
            }
            go.GetComponentInChildren<Image>().sprite = vars.skinSpriteList[i];
            go.transform.localPosition = new Vector3((i + 1) * 160, 0, 0);
        }
        //打开就能定位到位置所在
        parent.transform.localPosition = new Vector3(GameManager.Instance.GetCurrentSelectedSkin() * -160f, 0);
    }
    private void Update()
    {
        //localPosition是相对于父对象的位置，Position世界坐标系的位置
        selectindex = (int)Mathf.Round(parent.transform.localPosition.x / -160.0f);
        if (Input.GetMouseButtonUp(0))
        {
            parent.transform.DOLocalMoveX(selectindex * -160, 0.2f);
        }
        SetItemSize(selectindex);
        RefreshUI(selectindex);
    }
    private void SetItemSize(int selectIndex)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            if(selectIndex == i)
            {
                parent.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(160,160);
            }
            else
            {
                parent.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(80, 80);
            }
        }
    }
    private void RefreshUI(int selectIndex)
    {
        txt_Name.text = vars.skinNameList[selectIndex];
        txt_Diamond.text = GameManager.Instance.GetAllDiamond().ToString();
        if(GameManager.Instance.GetSkinUnlocked(selectIndex) ==false)
        {
            btn_Buy.gameObject.SetActive(true);
            btn_Select.gameObject.SetActive(false);
            btn_Buy.GetComponentInChildren<Text>().text = vars.skinPrice[selectIndex].ToString();
        }
        else
        {
            btn_Buy.gameObject.SetActive(false);
            btn_Select.gameObject.SetActive(true);
        }
    }
}
