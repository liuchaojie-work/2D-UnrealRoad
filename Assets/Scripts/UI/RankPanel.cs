using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class RankPanel : MonoBehaviour
{
    private Button btn_Close;
    private Text[] txt_RankList;
    private GameObject go_Img_RankList;


    private void Awake()
    {
        EventCenter.AddListener(EventType.ShowRankPanel, Show);
        txt_RankList = new Text[3];
        btn_Close = transform.Find("Btn_Close").GetComponent<Button>();
        btn_Close.onClick.AddListener(OnBtn_CloseClick);
        txt_RankList[0] = transform.Find("Img_RankList/GOE_Gold/Txt_Gold").GetComponent<Text>();
        txt_RankList[1] = transform.Find("Img_RankList/GOE_Silver/Txt_Silver").GetComponent<Text>();
        txt_RankList[2] = transform.Find("Img_RankList/GOE_Copper/Txt_Copper").GetComponent<Text>();
        go_Img_RankList = transform.Find("Img_RankList").gameObject;

        btn_Close.GetComponent<Image>().color =
            new Color(btn_Close.GetComponent<Image>().color.r,
                      btn_Close.GetComponent<Image>().color.g,
                      btn_Close.GetComponent<Image>().color.b, 0);
        go_Img_RankList.transform.localScale = Vector3.zero;
        gameObject.SetActive(false);
    }


    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventType.ShowRankPanel, Show);
    }
    private void Show()
    {
        gameObject.SetActive(true);
        btn_Close.GetComponent<Image>().DOColor(
            new Color(btn_Close.GetComponent<Image>().color.r,
                      btn_Close.GetComponent<Image>().color.g,
                      btn_Close.GetComponent<Image>().color.b, 0.3f), 0.3f);
        go_Img_RankList.transform.DOScale(Vector3.one, 0.3f);

        int[] arr = GameManager.Instance.GetScoreArr();
        for(int i =0; i < arr.Length; i++)
        {
            txt_RankList[i].text = arr[i].ToString();
        }
    }
    private void OnBtn_CloseClick()
    {
        EventCenter.Broadcast(EventType.PlayClickAudio);
        btn_Close.GetComponent<Image>().DOColor(
            new Color(btn_Close.GetComponent<Image>().color.r,
                      btn_Close.GetComponent<Image>().color.g,
                      btn_Close.GetComponent<Image>().color.b, 0), 0.3f);
        go_Img_RankList.transform.DOScale(Vector3.zero, 0.3f).OnComplete(()=>
        {
            gameObject.SetActive(false);
        });
    }
}
