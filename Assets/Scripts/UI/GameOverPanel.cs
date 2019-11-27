using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameOverPanel : MonoBehaviour
{
    private Text txt_Score;
    private Text txt_BestScore;
    private Text txt_AddDiamondNum;

    private Button btn_Restart;
    private Button btn_Rank;
    private Button btn_MainMenu;

    private Image img_New;

    private void Awake()
    {
        EventCenter.AddListener(EventType.ShowGameOverPanel, Show);
        GameOverPanelInit();
    }

    private void GameOverPanelInit()
    {
        txt_Score = transform.Find("Txt_Score").GetComponent<Text>();
        txt_BestScore = transform.Find("Txt_BestScore").GetComponent<Text>();
        txt_AddDiamondNum = transform.Find("GOE_Diamond/Txt_AddDiamondNum").GetComponent<Text>();
        btn_Restart = transform.Find("Btn_Restart").GetComponent<Button>();
        btn_Restart.onClick.AddListener(OnBtn_RestartClick);
        btn_Rank = transform.Find("Btn_Rank").GetComponent<Button>();
        btn_Rank.onClick.AddListener(OnBtn_RankClick);
        btn_MainMenu = transform.Find("Btn_MainMenu").GetComponent<Button>();
        btn_MainMenu.onClick.AddListener(OnBtn_MainMenuClick);
        img_New = transform.Find("Img_New").GetComponent<Image>();
        gameObject.SetActive(false);

    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventType.ShowGameOverPanel, Show);
    }
    private void Show()
    {
        if(GameManager.Instance.GetGameScore() > GameManager.Instance.GetBestScore())
        {
            img_New.gameObject.SetActive(true);
            txt_BestScore.text = "最高分： " + GameManager.Instance.GetGameScore();
        }
        else
        {
            img_New.gameObject.SetActive(false);
            txt_BestScore.text = "最高分： " + GameManager.Instance.GetBestScore();
        }
        GameManager.Instance.SaveScore(GameManager.Instance.GetGameScore());
        gameObject.SetActive(true);
        txt_Score.text = GameManager.Instance.GetGameScore().ToString();
        txt_AddDiamondNum.text = "+" + GameManager.Instance.GetGameDiamond().ToString();
        //更新总的钻石数量
        GameManager.Instance.UpdateAllDiamond(GameManager.Instance.GetGameDiamond());
    }

    /// <summary>
    /// 再试一次按钮点击
    /// </summary>
    private void OnBtn_RestartClick()
    {
        EventCenter.Broadcast(EventType.PlayClickAudio);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameData.isAgainGame = true;
    }

    /// <summary>
    /// 排行榜按钮
    /// </summary>
    private void OnBtn_RankClick()
    {
        EventCenter.Broadcast(EventType.PlayClickAudio);
        EventCenter.Broadcast(EventType.ShowRankPanel);
    }

    /// <summary>
    /// 主菜单按钮
    /// </summary>
    private void OnBtn_MainMenuClick()
    {
        EventCenter.Broadcast(EventType.PlayClickAudio);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameData.isAgainGame = false;
    }
}
