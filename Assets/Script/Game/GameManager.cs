using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private ManagerVars vars;
    private void Awake()
    {
        Instance = this;
        EventCenter.AddListener(EventDefine.AddScore, AddGameScore);
        EventCenter.AddListener(EventDefine.PlayerMove, PlayerMove);
        EventCenter.AddListener(EventDefine.AddDiamond, AddDiamond);
        if(GameData.IsAgainGame)
        {
            IsGameStarted = true;
        }
        vars = ManagerVars.GetManagerVars();
        InitGameData();
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.AddScore, AddGameScore);
        EventCenter.RemoveListener(EventDefine.PlayerMove, PlayerMove);
        EventCenter.RemoveListener(EventDefine.AddDiamond, AddDiamond);
    }
    private GameData gameData;
    private bool isFirstGame;
    private bool isMusicOn;
    private int[] bestScoreArr;
    private int selectSkin;
    private bool[] skinUnlocked;
    private int diamondCount;
    /// <summary>
    /// 游戏是否开始
    /// </summary>
    public bool IsGameStarted { get; set; }
    /// <summary>
    /// 游戏是否结束
    /// </summary>
    public bool IsGameOver { set; get; }
    /// <summary>
    /// 是否暂停
    /// </summary>
    public bool IsPause { get; set; }
    /// <summary>
    /// 玩家是否移动
    /// </summary>
    public bool PlayerIsMove { get; set; }
    /// <summary>
    /// 游戏成绩
    /// </summary>
    private int gameScore;
    /// <summary>
    /// 得到的钻石
    /// </summary>
    private int gameDiamond;
    /// <summary>
    /// 增加游戏成绩
    /// </summary>
    private void AddGameScore()
    {
        if(!IsGameOver || !IsGameStarted || !IsPause)
        {
            gameScore++;
            EventCenter.Broadcast(EventDefine.UpdateScoreText, gameScore);
        }
    }
    /// <summary>
    /// 获取游戏当前的
    /// </summary>
    /// <returns></returns>
    public int GetGameScore()
    {
        return gameScore;
    }
    private void PlayerMove()
    {
        PlayerIsMove = true;
    }
    /// <summary>
    /// 更新游戏钻石数量
    /// </summary>
    private void AddDiamond()
    {
        gameDiamond++;
        EventCenter.Broadcast(EventDefine.UpdateDiamondText, gameDiamond);
    }
    /// <summary>
    /// 获取到游戏中的金币数
    /// </summary>
    /// <returns></returns>
    public int GetGameDiamond()
    {
        return gameDiamond;
    }
    /// <summary>
    /// 设置当前选择的皮肤下标
    /// </summary>
    /// <param name="index"></param>
    public void SetSelectSkin(int index)
    {
        selectSkin = index;
        Save();
    }
    /// <summary>
    /// 初始化游戏数据
    /// </summary>
    private void InitGameData()
    {
        Read();
        if (gameData != null)
        {
            isFirstGame = gameData.GetIsFirstGame();
        }
        else
        {
            isFirstGame = true;
        }
        if (isFirstGame)
        {
            isFirstGame = false;
            isMusicOn = true;
            bestScoreArr = new int[3];
            selectSkin = 0;
            skinUnlocked = new bool[vars.skinSpriteList.Count];
            skinUnlocked[0] = true;
            diamondCount = 5;
            gameData = new GameData();
            Save();
        }
        else
        {
            isMusicOn = gameData.GetIsMusicOn();
            bestScoreArr = gameData.GetBestScoreArr();
            selectSkin = gameData.GetSelectSkin();
            skinUnlocked = gameData.GetSkinUnlocked();
            diamondCount = gameData.GetDiamondCount();
        }
    }
    /// <summary>
    /// 储存数据
    /// </summary>
    private void Save()
    {
        try
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (FileStream fs = File.Create(Application.persistentDataPath+"/GameData.data"))
            {
                gameData.SetBestScoreArr(bestScoreArr);
                gameData.SetDiamondCount(diamondCount);
                gameData.SetIsFirstGame(isFirstGame);
                gameData.SetIsMusicOn(isMusicOn);
                gameData.SetSelectSkin(selectSkin);
                gameData.SetSkinUnlocked(skinUnlocked);
                binaryFormatter.Serialize(fs, gameData);
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }
    /// <summary>
    /// 读取
    /// </summary>
    private void Read()
    {
        try
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (FileStream fs = File.Open(Application.persistentDataPath + "/GameData.data",FileMode.Open))
            {
                gameData =  (GameData)binaryFormatter.Deserialize(fs);
            }
        }
        catch (System.Exception ex)
        {

            Debug.Log(ex.Message);
        }
    }
    //皮肤是否解锁
    public bool GetSkinUnlocked(int index)
    {
        return skinUnlocked[index];
    }
    public void SetSkinUnlocked(int index)
    {
        skinUnlocked[index] = true;
        Save();
    }
    public int GetAllDiamond()
    {
        return diamondCount;
    }
    public void UpdateAllDiamond(int value)
    {
        diamondCount += value;
        Save();
    }
    /// <summary>
    /// 获取当前选中的皮肤
    /// </summary>
    /// <returns></returns>
    public int GetCurrentSelectedSkin()
    {
        return selectSkin;
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
        skinUnlocked = new bool[vars.skinSpriteList.Count];
        skinUnlocked[0] = true;
        diamondCount = 5;
        Save();
    }
    /// <summary>
    /// 保存成绩
    /// </summary>
    /// <param name="score"></param>
    public void SaveScore(int score)
    {
        List<int> list = bestScoreArr.ToList();
        list.Add(score);
        //从大到小进行排序
        list.Sort((x, y) => (-x.CompareTo(y)));
        list.Remove(list.Count-1);
        bestScoreArr = list.ToArray();
    }
    public int GetBestScore()
    {
        if(bestScoreArr.Length > 0)
        {
            return bestScoreArr.Max();
        }
        return 0;
    }
    public int GetBestScoreByIndex(int index)
    {
        List<int> list = bestScoreArr?.ToList();
        //从大到小进行排序
        list?.Sort((x, y) => (-x.CompareTo(y)));
        bestScoreArr = list?.ToArray();
        if (bestScoreArr?.Length >= index)
        {
            return bestScoreArr[index];
        }
        else
        {
            return 0;
        }
    }
    public void SetIsMusicOn(bool IsMucisOn)
    {
        isMusicOn = IsMucisOn;
        Save();
    }
    public bool GetIsMusic()
    {
        return isMusicOn;
    }
}
