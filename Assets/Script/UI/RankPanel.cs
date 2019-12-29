using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RankPanel : MonoBehaviour
{
    private Button btn_Back;
    private Text score_1, score_2, score_3;
    private GameObject go_ScoreList;
    private void Awake()
    {


        EventCenter.AddListener(EventDefine.ShowRankPanel, ShowRankPanel);
        btn_Back = transform.Find("btn_Back").GetComponent<Button>();
        btn_Back.GetComponent<Image>().color = new Color(btn_Back.GetComponent<Image>().color.r, btn_Back.GetComponent<Image>().color.g, btn_Back.GetComponent<Image>().color.b,0);
        btn_Back.onClick.AddListener(OnBackButtonClick);
        score_1 = transform.Find("ScoreList/Score_1/Text").GetComponent<Text>();
        score_2 = transform.Find("ScoreList/Score_2/Text").GetComponent<Text>();
        score_3 = transform.Find("ScoreList/Score_3/Text").GetComponent<Text>();
        go_ScoreList = transform.Find("ScoreList").gameObject;
        go_ScoreList.transform.localScale = Vector3.zero;
        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.ShowRankPanel, ShowRankPanel);
    }
    private void ShowRankPanel()
    {
        gameObject.SetActive(true);
        btn_Back.GetComponent<Image>().DOColor(new Color(btn_Back.GetComponent<Image>().color.r, btn_Back.GetComponent<Image>().color.g, btn_Back.GetComponent<Image>().color.b,0.3f),0.3f);
        go_ScoreList.transform.DOScale(Vector3.one,0.3f);
        score_1.text = GameManager.Instance.GetBestScoreByIndex(0).ToString();
        score_2.text = GameManager.Instance.GetBestScoreByIndex(1).ToString();
        score_3.text = GameManager.Instance.GetBestScoreByIndex(2).ToString();
       
    }
    private void OnBackButtonClick()
    {
        btn_Back.GetComponent<Image>().DOColor(new Color(btn_Back.GetComponent<Image>().color.r, btn_Back.GetComponent<Image>().color.g, btn_Back.GetComponent<Image>().color.b, 0), 0.3f);
        go_ScoreList.transform.DOScale(Vector3.zero, 0.3f).OnComplete(()=> gameObject.SetActive(false));
        
    }
}
