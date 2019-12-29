using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlatformGroupType
{
    Grass,
    Winter,
}
public class PlatformSpawner : MonoBehaviour
{
    public Vector3 startSpawnPOs;
    /// <summary>
    /// 掉落时间
    /// </summary>
    public float fallTime;
    /// <summary>
    /// 最小掉落时间
    /// </summary>
    public float minFallTime;
    /// <summary>
    /// 掉落系数
    /// </summary>
    public float mulTiple;
    /// <summary>
    /// 里程碑
    /// </summary>
    public int milestoneCount = 10;
    /// <summary>
    /// 生成平台的数量
    /// </summary>
    private int spawnPlatformCount;
    private ManagerVars vars;
    /// <summary>
    /// 平台的生成位置
    /// </summary>
    private Vector3 platformSpawnPostion;
    /// <summary>
    /// 是否向左边生成，否则向右
    /// </summary>
    private bool isLeftSpawn = false;
    /// <summary>
    /// 选择的平台图
    /// </summary>
    private Sprite selectPlatformSprite;
    /// <summary>
    /// 组合平台的类型
    /// </summary>
    private PlatformGroupType groupType;
    /// <summary>
    /// 钉子组合平台是否生成在左边，反之生成在右边
    /// </summary>
    private bool spikeSpawnLeft = false;
    /// <summary>
    /// 钉子方向平台的位置
    /// </summary>
    private Vector3 spikeDirPlatformPos;
    /// <summary>
    /// 钉子方向生成平台的数量
    /// </summary>
    private int afterSpawnSpikeSpawnCount;
    private bool isSpawnSpike;
    private void Awake()
    {
        EventCenter.AddListener(EventDefine.DecidePath, DecidePath);
        vars = ManagerVars.GetManagerVars();
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.DecidePath,DecidePath);
    }
    private void Start()
    {
        RandomPlatformTheme();
        platformSpawnPostion = startSpawnPOs;
       
        for (int i = 0; i < 5; i++)
        {
            spawnPlatformCount = 5;
            DecidePath();
        }
        
        //生人物模型
        GameObject go = Instantiate(vars.characterPre);
        go.transform.position = new Vector3(0, -1.8f, 0);
        
    }
    private void Update()
    {
        if(GameManager.Instance.IsGameStarted&&!GameManager.Instance.IsGameOver&&!GameManager.Instance.IsPause)
        {
            UpdateFallTime();
        }
    }
    /// <summary>
    /// 更新平台掉落时间
    /// </summary>
    private void UpdateFallTime()
    {
        if(GameManager.Instance.GetGameScore()> milestoneCount)
        {
            milestoneCount *= 2;
            fallTime *= mulTiple;
            if(fallTime<minFallTime)
            {
                fallTime = minFallTime;
            }

        }
    }
    /// <summary>
    /// 确定路径，这里一般是其他程序调用
    /// </summary>
    private void DecidePath()
    {
        if(isSpawnSpike)
        {
            AfterSpawnSpike();
            return;
        }
        if(spawnPlatformCount>0)
        {
            spawnPlatformCount--;
            SpawnPlatform();
        }
        else
        {
            //反正生成方向
            isLeftSpawn = !isLeftSpawn;
            spawnPlatformCount = Random.Range(1, 4);
            SpawnPlatform();
        }
    }
    /// <summary>
    /// 生成平台
    /// </summary>
    private void SpawnPlatform()
    {
        int ranObstacleDir = Random.Range(0,2);
        if(spawnPlatformCount >= 1)
        {
            SpawnNormalPlatform(ranObstacleDir);//生成普通平台
        }
        else if(spawnPlatformCount==0)//生成组合平台，拐角位置
        {
            int ran = Random.Range(0, 3);//一共三种，普通，钉子障碍
            switch(ran)
            {
                //普通平台生成
                case 0:
                    SpawnCommonPlatformGroup(ranObstacleDir);
                    break;
                //组合平台生成
                case 1:
                    switch(groupType)
                    {
                        case PlatformGroupType.Grass:
                            SpawnGrassPlatformGroup(ranObstacleDir);
                            break;
                        case PlatformGroupType.Winter:
                            SpawnwinterePlatformGroup(ranObstacleDir);
                            break;
                        default:
                            break;
                    }
                    break;
                //钉子平台生成
                case 2:
                    int value = -1;
                    if(isLeftSpawn)
                    {
                        value = 0;//生成右边方向得钉子
                
                    }
                    else
                    {
                        value = 1;//生成左边的
                    }
                    SpawnSpikelPlatform(value, ranObstacleDir);
                    isSpawnSpike = true;
                    afterSpawnSpikeSpawnCount = 5;
                    if (spikeSpawnLeft)
                    {
                        spikeDirPlatformPos = new Vector3(platformSpawnPostion.x - 1.65f,platformSpawnPostion.y + vars.nestYPos,0);
                    }
                    else
                    {
                        spikeDirPlatformPos = new Vector3(platformSpawnPostion.x + 1.65f, platformSpawnPostion.y + vars.nestYPos, 0);
                    }
                    break;
            }
        }
        int ranSpawnDiamond = Random.Range(0, 10);
        if(ranSpawnDiamond == 6&& GameManager.Instance.PlayerIsMove)
        {
           GameObject go =   ObjectPool.Instance.GetDiamondPre();
            go.transform.position = new Vector3(platformSpawnPostion.x, platformSpawnPostion.y + 0.5f, 0);
            go.SetActive(true);
        }
        //计算下一个
        if(isLeftSpawn)
        {
            //platformSpawnPostion = new Vector3(platformSpawnPostion.x - vars.nestXPos, platformSpawnPostion.y + vars.nestYPos);
            platformSpawnPostion.x = platformSpawnPostion.x - vars.nestXPos;
            platformSpawnPostion.y = platformSpawnPostion.y + vars.nestYPos;
        }
        else
        {
            platformSpawnPostion.x = platformSpawnPostion.x + vars.nestXPos;
            platformSpawnPostion.y = platformSpawnPostion.y + vars.nestYPos;
        }
    }
    /// <summary>
    /// 生成普通平台（单个普通平台）
    /// </summary>
    private void SpawnNormalPlatform(int ranObstacleDir)
    {
        GameObject go = ObjectPool.Instance.GetNormalPlatform(); 
        go.transform.position = platformSpawnPostion;
        //拿到主题的script后，得到普通平台的Play脚本，放入即可
        go.GetComponent<PlatformScript>().Init(selectPlatformSprite, fallTime,ranObstacleDir);
        go.SetActive(true);
    }
    /// <summary>
    /// 生成草地主题平台
    /// </summary>
    private void SpawnGrassPlatformGroup(int ranObstacleDir)
    {
        GameObject go = ObjectPool.Instance.GetGrassPlatform();
        go.transform.position = platformSpawnPostion;
        go.GetComponent<PlatformScript>().Init(selectPlatformSprite, fallTime, ranObstacleDir);
        go.SetActive(true);
    }
    /// <summary>
    /// 生成冬季主题平台
    /// </summary>
    private void SpawnwinterePlatformGroup(int ranObstacleDir)
    {
        GameObject go = ObjectPool.Instance.GetWinterPlatform();
        go.transform.position = platformSpawnPostion;
        go.GetComponent<PlatformScript>().Init(selectPlatformSprite, fallTime,ranObstacleDir);
        go.SetActive(true);
    }
    /// <summary>
    /// 生成通用平台
    /// </summary>
    private void SpawnCommonPlatformGroup(int ranObstacleDir)
    {
        GameObject go = ObjectPool.Instance.GetCommonPlatform();
        go.transform.position = platformSpawnPostion;
        go.GetComponent<PlatformScript>().Init(selectPlatformSprite, fallTime, ranObstacleDir);
        go.SetActive(true);
    }
    /// <summary>
    /// 生成钉子组合平台
    /// </summary>
    /// <param name="dir"></param>
    private void SpawnSpikelPlatform(int dir,int ranObstacleDir)
    {
        GameObject go = null;
        if (dir == 0)
        {
            spikeSpawnLeft = false;
            go = ObjectPool.Instance.GetRightSpikePlatform();    
        }
        else if(dir == 1)
        {
            spikeSpawnLeft = true;
            go = ObjectPool.Instance.GetLeftSpikePlatform();
        }
        go.transform.position = platformSpawnPostion;
        go.GetComponent<PlatformScript>().Init(selectPlatformSprite, fallTime, dir);
        go.SetActive(true);

    }
    /// <summary>
    /// 随即平台的主题
    /// </summary>
    private void RandomPlatformTheme()
    {
        //通过随机拿到脚本的script
        int ran = Random.Range(0, vars.platformThemeSpriteList.Count);
        selectPlatformSprite = vars.platformThemeSpriteList[ran];

        if(ran == 2)
        {
            groupType = PlatformGroupType.Winter;
        }
        else
        {
            groupType = PlatformGroupType.Grass;
        }
    }
    /// <summary>
    /// 生成钉子平台之后，需要生成的平台
    /// 包括钉子方向也包括原来的方向
    /// </summary>
    public void AfterSpawnSpike()
    {
        if(afterSpawnSpikeSpawnCount>0)
        {
            afterSpawnSpikeSpawnCount--;
            for(int i=0;i<2;i++)
            {
                GameObject temp = ObjectPool.Instance.GetNormalPlatform();
                if(i==0)//原来的方向
                {
                    temp.transform.position = platformSpawnPostion;
                    //如果钉子在左边
                    if (spikeSpawnLeft)
                    {
                        platformSpawnPostion = new Vector3(platformSpawnPostion.x+vars.nestXPos,platformSpawnPostion.y+vars.nestYPos,0);
                    }
                    else
                    {
                        platformSpawnPostion = new Vector3(platformSpawnPostion.x - vars.nestXPos, platformSpawnPostion.y + vars.nestYPos, 0);
                    }
                }
                else//生成钉子方向
                {
                    temp.transform.position = spikeDirPlatformPos;
                    if (spikeSpawnLeft)
                    {
                        spikeDirPlatformPos = new Vector3(spikeDirPlatformPos.x - vars.nestXPos, spikeDirPlatformPos.y + vars.nestYPos, 0);
                    }
                    else
                    {
                        spikeDirPlatformPos = new Vector3(spikeDirPlatformPos.x + vars.nestXPos, spikeDirPlatformPos.y + vars.nestYPos, 0);
                    }
                }
                temp.GetComponent<PlatformScript>().Init(selectPlatformSprite, fallTime, 1);//用来加载平台脚本
                temp.SetActive(true);
            }
        }
        else
        {
            isSpawnSpike = false;
            DecidePath();
        }
        
    }
}
