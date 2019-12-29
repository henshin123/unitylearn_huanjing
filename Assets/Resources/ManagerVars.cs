using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//该特性用于创建Asset文件
[CreateAssetMenu(menuName ="CreatManagerContainer")]
public class ManagerVars : ScriptableObject
{
    public static ManagerVars GetManagerVars()
    {
        return Resources.Load<ManagerVars>("ManagerVarsContainer");
    }
    public List<Sprite> bgThemeSpriteList = new List<Sprite>();
    public List<Sprite> platformThemeSpriteList = new List<Sprite>();
    public List<Sprite> skinSpriteList = new List<Sprite>();
    public List<Sprite> characterSkinSpriteList = new List<Sprite>();
    public GameObject characterPre;
    public GameObject normalPlatformPre;
    public GameObject skinChooseItemPre;
    public List<int> skinPrice = new List<int>();
    public List<string> skinNameList = new List<string>();
    public List<GameObject> commonPlatformGroup = new List<GameObject>();
    public List<GameObject> grassPlatformGroup = new List<GameObject>();
    public List<GameObject> winterPlatformGroup = new List<GameObject>();
    public GameObject spikePlatformLeft;
    public GameObject spikePlatformRight;
    public GameObject deathEffect;
    public GameObject diamondPre;
    public float nestXPos = 0.554f, nestYPos = 0.645f;

    public AudioClip jumpClip, fallClip, diamondClip, buttonClip,hitClip;
    public Sprite musicOn, musicOff;
}
