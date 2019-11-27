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
        vars = ManagerVars.GetManagerVars();
        EventCenter.AddListener(EventType.ShowMainPanel,Show);
        EventCenter.AddListener<int>(EventType.ChangeSkin, ChangeSkin);
        MainPanelInit();
    }
    // Start is called before the first frame update
    void Start()
    {
        if (GameData.isAgainGame)
        {
            EventCenter.Broadcast(EventType.ShowGamePanel);
            gameObject.SetActive(false);
        }
        PlayOrStopSound();
        ChangeSkin(GameManager.Instance.GetCurrentSelectedSkin());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void MainPanelInit()
    {
        btn_Start = transform.Find("Btn_Start").GetComponent<Button>();
        btn_Start.onClick.AddListener(OnBtn_StartClick);
        btn_Shop = transform.Find("GOE_Buttons/Btn_Shop").GetComponent<Button>();
        btn_Shop.onClick.AddListener(OnBtn_ShopClick);
        btn_Rank = transform.Find("GOE_Buttons/Btn_Rank").GetComponent<Button>();
        btn_Rank.onClick.AddListener(OnBtn_RankClick);
        btn_Sound = transform.Find("GOE_Buttons/Btn_Sound").GetComponent<Button>();
        btn_Sound.onClick.AddListener(OnBtn_SoundClick);
        btn_Reset = transform.Find("GOE_Buttons/Btn_Reset").GetComponent<Button>();
        btn_Reset.onClick.AddListener(OnBtn_ResetClick);
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventType.ShowMainPanel, Show);
        EventCenter.RemoveListener<int>(EventType.ChangeSkin, ChangeSkin);
    }
    private void Show()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 皮肤更换，更换UI皮肤图片
    /// </summary>
    /// <param name="index"></param>
    private void ChangeSkin(int index)
    {
        btn_Shop.transform.GetChild(0).GetComponent<Image>().sprite = vars.skinSpriteList[index];
    }
    /// <summary>
    /// 开始按钮点击后调用此方法
    /// </summary>
    private void OnBtn_StartClick()
    {
        EventCenter.Broadcast(EventType.PlayClickAudio);
        GameManager.Instance.IsGameStarted = true;
        EventCenter.Broadcast(EventType.ShowGamePanel);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 商店按钮点击后调用此方法
    /// </summary>
    private void OnBtn_ShopClick()
    {
        EventCenter.Broadcast(EventType.PlayClickAudio);
        EventCenter.Broadcast(EventType.ShowShopPanel);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 排行榜按钮点击后调用此方法
    /// </summary>
    private void OnBtn_RankClick()
    {
        EventCenter.Broadcast(EventType.PlayClickAudio);
        EventCenter.Broadcast(EventType.ShowRankPanel);
    }

    /// <summary>
    /// 音效按钮点击后调用此方法
    /// </summary>
    private void OnBtn_SoundClick()
    {
        EventCenter.Broadcast(EventType.PlayClickAudio);

        GameManager.Instance.SetIsMusicOn(!GameManager.Instance.GetIsMusicOn());
        PlayOrStopSound();
    }

    private void PlayOrStopSound()
    {
        if (GameManager.Instance.GetIsMusicOn())
        {
            btn_Sound.transform.GetChild(0).GetComponent<Image>().sprite = vars.musicOn;
            
        }
        else
        {
            btn_Sound.transform.GetChild(0).GetComponent<Image>().sprite = vars.musicOff;
        }
        EventCenter.Broadcast(EventType.IsMusicOn, GameManager.Instance.GetIsMusicOn());
    }
    /// <summary>
    /// 重置游戏按钮
    /// </summary>
    private void OnBtn_ResetClick()
    {
        EventCenter.Broadcast(EventType.PlayClickAudio);
        EventCenter.Broadcast(EventType.ShowResetPanel);
    }
}
