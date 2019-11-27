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
    private Image img_BG;
    private GameObject img_Dialog;
    private void Awake()
    {
        EventCenter.AddListener(EventType.ShowResetPanel, Show);
        btn_Yes = transform.Find("Img_Dialog/Btn_Yes").GetComponent<Button>();
        btn_Yes.onClick.AddListener(OnBtn_YesClick);
        btn_No = transform.Find("Img_Dialog/Btn_No").GetComponent<Button>();
        btn_No.onClick.AddListener(OnBtn_NoClick);
        img_BG = transform.Find("Img_BG").GetComponent<Image>();
        img_BG.color = new Color(img_BG.color.r, img_BG.color.g, img_BG.color.b, 0);
        img_Dialog = transform.Find("Img_Dialog").gameObject;
        img_Dialog.transform.localScale = Vector3.zero;
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventType.ShowResetPanel, Show);
    }

    private void Show()
    {
        gameObject.SetActive(true);
        img_BG.DOColor(new Color(img_BG.color.r, img_BG.color.g, img_BG.color.b, 0.3f), 0.3f);
        img_Dialog.transform.DOScale(Vector3.one, 0.3f);
    }

    /// <summary>
    /// 是 按钮点击
    /// </summary>
    private void OnBtn_YesClick()
    {
        EventCenter.Broadcast(EventType.PlayClickAudio);
        GameManager.Instance.ResetData();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// 否 按钮点击
    /// </summary>
    private void OnBtn_NoClick()
    {
        EventCenter.Broadcast(EventType.PlayClickAudio);
        img_BG.DOColor(new Color(img_BG.color.r, img_BG.color.g, img_BG.color.b, 0), 0.3f);
        img_Dialog.transform.DOScale(Vector3.zero, 0.3f).OnComplete(()=> 
        {
            gameObject.SetActive(false);
        });
    }
}
