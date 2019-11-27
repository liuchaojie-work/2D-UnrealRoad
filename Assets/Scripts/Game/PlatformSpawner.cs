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
    /// <summary>
    /// 平台初始生成位置
    /// </summary>
    public Vector3 startSpawnPos;

    /// <summary>
    /// 里程碑数
    /// </summary>
    public int milestoneCount = 10;
    public float fallTime;
    public float minFallTime;
    public float multiple;


    /// <summary>
    /// 下一个平台生成的位置
    /// </summary>
    private Vector3 platformSpawnPos;

    /// <summary>
    /// 是否朝左生成
    /// </summary>
    private bool isLeftSpawn = false;

    /// <summary>
    /// 选中的平台图
    /// </summary>
    private Sprite selectedPlatformSprite;

    /// <summary>
    /// 组合平台的类型
    /// </summary>
    private PlatformGroupType platformGroupType;
    /// <summary>
    /// 判断钉子是否生成在左边
    /// </summary>
    private bool spikeSpawnLeft = false;
    /// <summary>
    /// 钉子平台钉子的位置
    /// </summary>
    private Vector3 spikeDirPlatformPos;
    /// <summary>
    /// 生成钉子平台后需要在钉子方向生成的平台数量
    /// </summary>
    private int afterSpawnSpikeSpawnCount;

    private bool isSpawnSpike;

    /// <summary>
    /// 生成平台数量
    /// </summary>
    private int spawnPlatfromCount;

    private ManagerVars vars;

    private void Awake()
    {
        EventCenter.AddListener(EventType.DecidePath, DecidePath);
        vars = ManagerVars.GetManagerVars();
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventType.DecidePath, DecidePath);
    }
    // Start is called before the first frame update
    void Start()
    {
        RandomPlatformTheme();
        platformSpawnPos = startSpawnPos;  
        for(int i = 0; i < 5; i++)
        {
            spawnPlatfromCount = 5;
            DecidePath();
        }

        //生成人物
        GameObject go = Instantiate(vars.characterPrefabs);
        go.transform.position = new Vector3(0.0f, -1.95f, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance.IsGameStarted && GameManager.Instance.IsGameOver == false)
        {
            UpdateFallTime();
        }
        
    }
    /// <summary>
    /// 更新平台掉落时间
    /// </summary>
    private void UpdateFallTime()
    {
        if(GameManager.Instance.GetGameScore() > milestoneCount)
        {
            milestoneCount *= 2;
            fallTime *= multiple;
            if(fallTime < minFallTime)
            {
                fallTime = minFallTime;
            }
        }
    }
    /// <summary>
    /// 随机平台主题
    /// </summary>
    private void RandomPlatformTheme()
    {
        int random = Random.Range(0, vars.platformThemeSpriteList.Count);
        selectedPlatformSprite = vars.platformThemeSpriteList[random];

        if( 2 == random)
        {
            platformGroupType = PlatformGroupType.Winter;
        }
        else
        {
            platformGroupType = PlatformGroupType.Grass;
        }
    }

    

    /// <summary>
    /// 确定路径
    /// </summary>
    private void DecidePath()
    {
        if(isSpawnSpike)
        {
            AfterSpawnSpike();
            return;
        }
        if(spawnPlatfromCount > 0)
        {
            spawnPlatfromCount--;
            SpawnPlatfrom();
            
        }
        else
        {
            isLeftSpawn = !isLeftSpawn;
            spawnPlatfromCount = Random.Range(1, 4);
            SpawnPlatfrom();
        }
    }

    /// <summary>
    /// 生成平台
    /// </summary>
    private void SpawnPlatfrom()
    {
        int ranObstacleDir = Random.Range(0, 2);
        if(spawnPlatfromCount >= 1)
        {
            //生成单个平台
            SpawnNormalPlatfrom(ranObstacleDir);
        }
        else if(spawnPlatfromCount == 0)
        {
            //生成组合平台
            int random = Random.Range(0, 3);
            //生成通用组合平台
            if(0 == random)
            {
                SpawnCommonPlatformGroup(ranObstacleDir);

            }
            //生成主题组合平台
            else if(1 == random)
            {
                switch (platformGroupType)
                {
                    case PlatformGroupType.Grass:
                        SpawnGrassPlatformGroup(ranObstacleDir);
                        break;
                    case PlatformGroupType.Winter:
                        SpawnWinterPlatformGroup(ranObstacleDir);
                        break;
                    default:
                        break;
                }

            }
            //生成钉子组合平台
            else
            {
                int value = -1;
                if(isLeftSpawn)
                {
                    //生成右边方向的钉子
                    value = 0;
                }
                else
                {
                    //生成左边方向的钉子
                    value = 1;
                }
                SpawnSpikePlatform(value);

                isSpawnSpike = true;
                afterSpawnSpikeSpawnCount = 3;
                if (spikeSpawnLeft == true)
                {
                    //钉子生成在左边
                    spikeDirPlatformPos = new Vector3(platformSpawnPos.x - 1.65f,
                        platformSpawnPos.y + vars.nextYPos, 0);
                }
                else
                {
                    spikeDirPlatformPos = new Vector3(platformSpawnPos.x + 1.65f,
                        platformSpawnPos.y + vars.nextYPos, 0);
                }
            }
        }


        int ranSpawnDiamond = Random.Range(0, 8);
        if(ranSpawnDiamond >= 6 && GameManager.Instance.IsPlayerMove)
        {
            GameObject go = GameObjectPool.Instance.GetDiamond();
            go.transform.position = new Vector3(platformSpawnPos.x,
                platformSpawnPos.y + 0.5f, 0);
            go.SetActive(true);
        }
        if (isLeftSpawn)
        {
            //向左生成
            platformSpawnPos = new Vector3(platformSpawnPos.x - vars.nextXPos, platformSpawnPos.y + vars.nextYPos, 0);
        }
        else
        {
            //向右生成
            platformSpawnPos = new Vector3(platformSpawnPos.x + vars.nextXPos, platformSpawnPos.y + vars.nextYPos, 0);
        }
    }

    /// <summary>
    /// 生成普通平台（单个）
    /// </summary>
    private void SpawnNormalPlatfrom(int ranObstacleDir)
    {
        GameObject go = GameObjectPool.Instance.GetNormalPlatform();
        go.transform.position = platformSpawnPos;
        go.GetComponent<PlatformScript>().PlatformScriptInit(selectedPlatformSprite,fallTime, ranObstacleDir);
        go.SetActive(true);
    }

    /// <summary>
    /// 生成通用组合平台
    /// </summary>
    private void SpawnCommonPlatformGroup(int ranObstacleDir)
    {
        GameObject go = GameObjectPool.Instance.GetCommonPlatform();
        go.transform.position = platformSpawnPos;
        go.GetComponent<PlatformScript>().PlatformScriptInit(selectedPlatformSprite, fallTime, ranObstacleDir);
        go.SetActive(true);
    }

    /// <summary>
    /// 生成草地组合平台
    /// </summary>
    private void SpawnGrassPlatformGroup(int ranObstacleDir)
    {

        GameObject go = GameObjectPool.Instance.GetGrassPlatform();
        go.transform.position = platformSpawnPos;
        go.GetComponent<PlatformScript>().PlatformScriptInit(selectedPlatformSprite, fallTime, ranObstacleDir);
        go.SetActive(true);
    }

    /// <summary>
    /// 生成冬季组合平台
    /// </summary>
    private void SpawnWinterPlatformGroup(int ranObstacleDir)
    {

        GameObject go = GameObjectPool.Instance.GetWinterPlatform();
        go.transform.position = platformSpawnPos;
        go.GetComponent<PlatformScript>().PlatformScriptInit(selectedPlatformSprite, fallTime, ranObstacleDir);
        go.SetActive(true);
    }

    /// <summary>
    /// 生成钉子组合平台
    /// </summary>
    /// <param name="dir"></param>
    private void SpawnSpikePlatform(int dir)
    {
        GameObject temp = null;
        if(0 == dir)
        {
            spikeSpawnLeft = false;
            temp = GameObjectPool.Instance.GetRightSpikePlatform();
      
        }
        else
        {
            spikeSpawnLeft = true;
            temp = GameObjectPool.Instance.GetLeftSpikePlatform();
        }
        temp.transform.position = platformSpawnPos;
        temp.GetComponent<PlatformScript>().PlatformScriptInit(selectedPlatformSprite, fallTime, dir);
        temp.SetActive(true);
    }

    /// <summary>
    /// 生成钉子平台之后需要生成的平台，包括钉子方向也包括原来方向
    /// </summary>
    private void AfterSpawnSpike()
    {
        if(afterSpawnSpikeSpawnCount > 0)
        {
            afterSpawnSpikeSpawnCount--;
            for(int i = 0; i < 2; i++)
            {
                GameObject temp = GameObjectPool.Instance.GetNormalPlatform();
                if(0 == i)
                {
                    //原来方向
                    temp.transform.position = platformSpawnPos;
                    //如果钉子在左边,原先路径就是右边
                    if (spikeSpawnLeft)
                    {
                        platformSpawnPos = new Vector3(platformSpawnPos.x + vars.nextXPos,
                                platformSpawnPos.y + vars.nextYPos, 0);
                    }
                    else
                    {  
                        platformSpawnPos = new Vector3(platformSpawnPos.x - vars.nextXPos,
                                platformSpawnPos.y + vars.nextYPos, 0);
                    }
                }
                else
                {
                    temp.transform.position = spikeDirPlatformPos;
                    //钉子方向
                    if (spikeSpawnLeft)
                    {
                        spikeDirPlatformPos = new Vector3(spikeDirPlatformPos.x - vars.nextXPos,
                                spikeDirPlatformPos.y + vars.nextYPos, 0);
                    }
                    else
                    {
                        spikeDirPlatformPos = new Vector3(spikeDirPlatformPos.x + vars.nextXPos,
                                spikeDirPlatformPos.y + vars.nextYPos, 0);
                    }
                }
                temp.GetComponent<PlatformScript>().PlatformScriptInit(selectedPlatformSprite, fallTime, 1);
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
