using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasButtonClip : MonoBehaviour
{
    private Button[] buttons;
    private void Awake()
    {
        buttons = GetComponentsInChildren<Button>();
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].onClick.AddListener(()=>EventCenter.Broadcast(EventDefine.PlayClickAudio));
        }
    }
}
