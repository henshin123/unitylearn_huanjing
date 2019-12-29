using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class ResetPanel : MonoBehaviour
{
    private Button btn_Yes;
    private Button btn_No;
    private Image img_Bg;
    private GameObject dialog;
    private Text txt_context;
    private void Awake()
    {
        EventCenter.AddListener<string>(EventDefine.ShowResetPanel, ShowResetPanel);
        btn_No = transform.Find("Dialog/btn_No").GetComponent<Button>();
        btn_No.onClick.AddListener(OnNoButtonClick);
        btn_Yes = transform.Find("Dialog/btn_Yes").GetComponent<Button>();
        btn_Yes.onClick.AddListener(OnYesButtonClick);
        img_Bg = transform.Find("bg").GetComponent<Image>();
        dialog = transform.Find("Dialog").gameObject;
        txt_context = transform.Find("Dialog/Text").GetComponent<Text>();
        img_Bg.color = new Color(img_Bg.color.r, img_Bg.color.g, img_Bg.color.b,0);
        dialog.transform.localPosition = Vector3.zero;
        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener<string>(EventDefine.ShowResetPanel, ShowResetPanel);
    }
    private void ShowResetPanel(string context)
    {
        txt_context.text = context;
        gameObject.SetActive(true);
        img_Bg.DOColor(new Color(img_Bg.color.r, img_Bg.color.g, img_Bg.color.b, 0.3f), 0.3f);
        dialog.transform.DOScale(Vector3.one, 0.3f);
    }
    /// <summary>
    /// 是按钮点击
    /// </summary>
    private void OnYesButtonClick()
    {
        GameManager.Instance.ResetData();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    /// <summary>
    /// 否按钮点击
    /// </summary>
    private void OnNoButtonClick()
    {
        img_Bg.DOColor(new Color(img_Bg.color.r, img_Bg.color.g, img_Bg.color.b, 0), 0.3f);
        dialog.transform.DOScale(Vector3.zero, 0.3f).OnComplete(()=> gameObject.SetActive(false)); 
    }
}
