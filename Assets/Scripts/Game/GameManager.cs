using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.IO;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private GameData data;
    private ManagerVars vars;
    /// <summary>
    /// 游戏是否开始
    /// </summary>
    public bool IsGameStarted { get; set; }

    /// <summary>
    /// 游戏是否结束
    /// </summary>
    public bool IsGameOver { get; set; }
    public bool IsPause { get; set; }
    /// <summary>
    /// 玩家是否开始移动
    /// </summary>
    public bool IsPlayerMove { get; set; }

    private void Awake()
    {
        Instance = this;
        vars = ManagerVars.GetManagerVars();
        EventCenter.AddListener(EventType.AddScore, AddGameScore);
        EventCenter.AddListener(EventType.PlayerMove, PlayerMove);
        EventCenter.AddListener(EventType.AddDiamond, AddGameDiamond);

        if(GameData.isAgainGame)
        {
            IsGameStarted = true;
        }

        InitGameData();

    }

    /// <summary>
    /// 游戏成绩
    /// </summary>
    private int gameScore;

    private int gameDiamond;

    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventType.AddScore, AddGameScore);
        EventCenter.RemoveListener(EventType.PlayerMove, PlayerMove);
        EventCenter.RemoveListener(EventType.AddDiamond, AddGameDiamond);
    }

    /// <summary>
    /// 保存成绩
    /// </summary>
    /// <param name="score"></param>
    public void SaveScore(int score)
    {
        List<int> list = bestScoreArr.ToList();
        //从大到小排序list
        list.Sort((x, y) => (-x.CompareTo(y)));
        bestScoreArr = list.ToArray();

        int index = -1;
        for(int i = 0; i < bestScoreArr.Length; i++)
        {
            if(score > bestScoreArr[i])
            {
                index = i;
            }
        }

        if (-1 == index)
        {
            return;
        }

        for(int i =bestScoreArr.Length -1; i > index; i--)
        {
            bestScoreArr[i] = bestScoreArr[i - 1];
        }
        bestScoreArr[index] = score;
        Save();

    }

    /// <summary>
    /// 获取最高分
    /// </summary>
    /// <returns></returns>
    public int GetBestScore()
    {
        return bestScoreArr.Max();
    }

    /// <summary>
    /// 获得排行榜的数组
    /// </summary>
    /// <returns></returns>
    public int[] GetScoreArr()
    {
        List<int> list = bestScoreArr.ToList();
        //从大到小排序list
        list.Sort((x, y) => (-x.CompareTo(y)));
        bestScoreArr = list.ToArray();
        return bestScoreArr;
    }
    private void AddGameScore()
    {
        if(IsGameStarted == false || IsGameOver || IsPause)
        {
            return;
            
        }
        gameScore++;
        EventCenter.Broadcast(EventType.UpdateScoreText, gameScore);

    }

    /// <summary>
    /// 玩家移动调用此方法
    /// </summary>
    private void PlayerMove()
    {
        IsPlayerMove = true;
    }
    /// <summary>
    /// 获取游戏成绩
    /// </summary>
    /// <returns></returns>
    public int GetGameScore()
    {
        return gameScore;
    }

    /// <summary>
    /// 更新砖石数量
    /// </summary>
    private void AddGameDiamond()
    {
        gameDiamond++;
        EventCenter.Broadcast(EventType.UpdateDiamondText, gameDiamond);
    }
    /// <summary>
    /// 获得吃到的砖石数
    /// </summary>
    /// <returns></returns>
    public int GetGameDiamond()
    {
        return gameDiamond;
    }

    /// <summary>
    /// 得到所有的砖石数
    /// </summary>
    /// <returns></returns>
    public int GetAllDiamond()
    {
        return diamondNum;
    }

    /// <summary>
    /// 更新总钻石数量
    /// </summary>
    /// <param name="value"></param>
    public void UpdateAllDiamond(int value)
    {
        diamondNum += value;
        Save();
    }
    /// <summary>
    /// 设置当前皮肤解锁
    /// </summary>
    /// <param name="index"></param>
    public void SetSkinUnlocaked(int index)
    {
        isSkinUnlocked[index] = true;
        Save();
    }
    /// <summary>
    /// 获取当前皮肤是否解锁
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool GetSkinUnlocked(int index)
    {
        return isSkinUnlocked[index];
    }

    /// <summary>
    /// 设置当前选中的皮肤下标
    /// </summary>
    /// <param name="index"></param>
    public void SetSelectedSkin(int index)
    {
        selectSkin = index;
        Save();
    }

    /// <summary>
    /// 获得当前选择的皮肤
    /// </summary>
    /// <returns></returns>
    public int GetCurrentSelectedSkin()
    {
        return selectSkin;
    }

    /// <summary>
    /// 设置音效是否开启
    /// </summary>
    /// <param name="value"></param>
    public void SetIsMusicOn(bool value)
    {
        isMusicOn = value;
        Save();
    }

    /// <summary>
    /// 获取音效是否开启
    /// </summary>
    /// <returns></returns>
    public bool GetIsMusicOn()
    {
        return isMusicOn;
    }
    private bool isFirstGame;
    private bool isMusicOn;
    private int[] bestScoreArr;

    private int selectSkin;
    private bool[] isSkinUnlocked;
    private int diamondNum;

    /// <summary>
    /// 初始化游戏数据
    /// </summary>
    private void InitGameData()
    {
        Read();
        if (data != null)
        {
            isFirstGame = data.GetIsFirstGame();
        }
        else
        {
            isFirstGame = true;   
        }

        //若第一次开始游戏
        if(isFirstGame)
        {
            isFirstGame = false;
            isMusicOn = true;
            bestScoreArr = new int[3];
            selectSkin = 0;
            isSkinUnlocked = new bool[vars.skinSpriteList.Count];
            isSkinUnlocked[0] = true;
            diamondNum = 10;
            data = new GameData();
            Save();
        }
        else
        {
            isMusicOn = data.GetIsMusicOn();
            bestScoreArr = data.GetBestScoreArr();
            selectSkin = data.GetSelectSkin();
            isSkinUnlocked = data.GetIsSkinUnlocked();
            diamondNum = data.GetDiamondNum();
        }
    }

    /// <summary>
    /// 储存数据
    /// </summary>
    private void Save()
    {
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            //使用using将会自动释放，若不使用，将需要使用close()释放
            using (FileStream fs = File.Create(Application.persistentDataPath + "/GameData.Data"))
            {
                data.SetBestScoreArr(bestScoreArr);
                data.SetDiamondNum(diamondNum);
                data.SetIsFirstGame(isFirstGame);
                data.SetIsMusicOn(isMusicOn);
                data.SetIsSkinUnlocked(isSkinUnlocked);
                data.SetSelectSkin(selectSkin);
                bf.Serialize(fs, data);
            }
        }
        catch(System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    private void Read()
    {
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream fs = File.Open(Application.persistentDataPath + "/GameData.Data", FileMode.Open))
            {
                data = (GameData)bf.Deserialize(fs);
            }
        }
        catch(System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    /// <summary>
    /// 重置数据
    /// </summary>
    public void ResetData()
    {
        isFirstGame = false;
        isMusicOn = true;
        bestScoreArr = new int[3];
        selectSkin = 0;
        isSkinUnlocked = new bool[vars.skinSpriteList.Count];
        isSkinUnlocked[0] = true;
        diamondNum = 10;
        Save();
    }
}
