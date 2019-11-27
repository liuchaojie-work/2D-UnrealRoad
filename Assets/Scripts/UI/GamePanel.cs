using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GamePanel : MonoBehaviour
{
    private Button btn_Play;
    private Button btn_Pause;
    private Text txt_Score;
    private Text txt_DiamondNum;

    private void Awake()
    {
        EventCenter.AddListener(EventType.ShowGamePanel, Show);
        EventCenter.AddListener<int>(EventType.UpdateScoreText, UpdateScoreText);
        EventCenter.AddListener<int>(EventType.UpdateDiamondText, UpdateDiamondText);
        GamePanelInit();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GamePanelInit()
    {   
        btn_Play = transform.Find("Btn_Play").GetComponent<Button>();
        btn_Play.onClick.AddListener(OnBtn_PlayClick);
        btn_Pause = transform.Find("Btn_Pause").GetComponent<Button>();
        btn_Pause.onClick.AddListener(OnBtn_PauseClick);
        txt_Score = transform.Find("Txt_Score").GetComponent<Text>();
        txt_DiamondNum = transform.Find("Img_Diamond/Txt_DiamondNum").GetComponent<Text>();
        gameObject.SetActive(false);
        btn_Play.gameObject.SetActive(false);
    }


    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventType.ShowGamePanel, Show);
        EventCenter.RemoveListener<int>(EventType.UpdateScoreText, UpdateScoreText);
        EventCenter.RemoveListener<int>(EventType.UpdateDiamondText, UpdateDiamondText);
    }
    private void Show()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 更新成绩显示
    /// </summary>
    /// <param name="score"></param>
    private void UpdateScoreText(int score)
    {
        txt_Score.text = score.ToString();
    }

    /// <summary>
    /// 更新砖石显示
    /// </summary>
    /// <param name="diamond"></param>
    private void UpdateDiamondText(int diamond)
    {
        txt_DiamondNum.text = diamond.ToString();
    }
    /// <summary>
    /// 取消暂停按钮点击
    /// </summary>
    private void OnBtn_PlayClick()
    {
        EventCenter.Broadcast(EventType.PlayClickAudio);
        btn_Play.gameObject.SetActive(false);
        btn_Pause.gameObject.SetActive(true);
        Time.timeScale = 1;
        GameManager.Instance.IsPause = false;
    }

    /// <summary>
    /// 暂停按钮点击
    /// </summary>
    private void OnBtn_PauseClick()
    {
        EventCenter.Broadcast(EventType.PlayClickAudio);
        btn_Pause.gameObject.SetActive(false);
        btn_Play.gameObject.SetActive(true);
        //游戏暂停
        Time.timeScale = 0;
        GameManager.Instance.IsPause = true;
        
    }
}
