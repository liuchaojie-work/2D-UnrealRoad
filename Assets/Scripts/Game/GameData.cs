using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    /// <summary>
    /// 是否再来一次
    /// </summary>
    public static bool isAgainGame = false;

    private bool isFirstGame;
    private bool isMusicOn;
    private int[] bestScoreArr;

    private int selectSkin;
    private bool[] isSkinUnlocked;
    private int diamondNum;


    public void SetIsFirstGame(bool isFirstGame)
    {
        this.isFirstGame = isFirstGame;
    }

    public void SetIsMusicOn(bool isMusicOn)
    {
        this.isMusicOn = isMusicOn;
    }

    public void SetBestScoreArr(int[] bestScoreArr)
    {
        this.bestScoreArr = bestScoreArr;
    }

    public void SetSelectSkin(int selectSkin)
    {
        this.selectSkin = selectSkin;
    }

    public void SetIsSkinUnlocked(bool[] isSkinUnlocked)
    {
        this.isSkinUnlocked = isSkinUnlocked;
    }

    public void SetDiamondNum(int diamondNum)
    {
        this.diamondNum = diamondNum;
    }


    public bool GetIsFirstGame()
    {
        return isFirstGame;
    }

    public bool GetIsMusicOn()
    {
        return isMusicOn;
    }

    public int[] GetBestScoreArr()
    {
        return bestScoreArr;
    }

    public int GetSelectSkin()
    {
        return selectSkin;
    }

    public bool[] GetIsSkinUnlocked()
    {
        return isSkinUnlocked;
    }

    public int GetDiamondNum()
    {
        return diamondNum;
    }
}
