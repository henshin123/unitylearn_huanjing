using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public Transform rayDown,rayLeft,rayRight;
    public LayerMask platformLater,obstacleLayer;
    public AudioSource m_AudioSource;
    /// <summary>
    /// 是否向左移动，反之向右
    /// </summary>
    private bool isMoveLeft = false;
    private bool isJumping = false;//是否正在跳跃
    private Vector3 nextPlatformLeft, nextPlatformRight;
    private ManagerVars vars;
    private Rigidbody2D my_Body;//2d刚体，主要为了使用物理引擎
    private SpriteRenderer spriteRenderer;
    private bool isMove;
    private void Awake()
    {
        EventCenter.AddListener<int>(EventDefine.ChangeSkin, ChangeSkin);
        EventCenter.AddListener<bool>(EventDefine.IsMusicOn, IsMusicOn);
        vars = ManagerVars.GetManagerVars();
        m_AudioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        my_Body = GetComponent<Rigidbody2D>();//这里是获取游戏人物的刚体，因为这个就是绑定在了人物身上的脚本
    }
    private void Start()
    {
        ChangeSkin(GameManager.Instance.GetCurrentSelectedSkin());
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener<int>(EventDefine.ChangeSkin, ChangeSkin);
        EventCenter.RemoveListener<bool>(EventDefine.IsMusicOn, IsMusicOn);
    }
    /// <summary>
    /// 更换皮肤的调用
    /// </summary>
    /// <param name="skinIndex"></param>
    private void ChangeSkin(int skinIndex)
    {
        spriteRenderer.sprite = vars.characterSkinSpriteList[skinIndex];
    }
    private bool IsPointerOverGameObject(Vector2 mousePosition)
    {
        //创建一个点击事件
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = mousePosition;
        List<RaycastResult> list = new List<RaycastResult>();
        //向点击处发射一条射线，检测是否点击到了UI
        EventSystem.current.RaycastAll(eventData, list);
        return list.Count > 0;
    }
    private void Update()
    {
        //用于调试
        //Debug.DrawRay(rayDown.position,Vector2.down*1, Color.red);
        //Debug.DrawRay(rayLeft.position, Vector2.left * 0.15f,Color.red);
        //Debug.DrawRay(rayRight.position, Vector2.right * 0.15f, Color.red);
        //if(Application.platform==RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        //{
        //    int fingerId = Input.GetTouch(0).fingerId;
        //    if (EventSystem.current.IsPointerOverGameObject(fingerId))
        //    {
        //        return;
        //    }
        //}
        //else
        //{
        //    if (EventSystem.current.IsPointerOverGameObject())
        //    {
        //        return;
        //    }
        //}
        if (IsPointerOverGameObject(Input.mousePosition)) return;
        if (GameManager.Instance.IsGameStarted == false || GameManager.Instance.IsGameOver == true || GameManager.Instance.IsPause == true)
            return;
        if(Input.GetMouseButtonDown(0) && isJumping ==false && nextPlatformLeft!=Vector3.zero)
        {
            if(isMove == false)
            {
                EventCenter.Broadcast(EventDefine.PlayerMove);
                isMove = true;
            }
            m_AudioSource.PlayOneShot(vars.jumpClip);
            isJumping = true;
            Vector3 mousePos = Input.mousePosition;
            if(mousePos.x<=Screen.width/2)//点击的左边
            {
                isMoveLeft = true;
            }
            else if(mousePos.x>Screen.width/2)
            {
                isMoveLeft = false;
            }
            Jump();
            EventCenter.Broadcast(EventDefine.DecidePath);
        }
        //游戏结束
        if(my_Body.velocity.y<0 && !IsRayPlayform() && !GameManager.Instance.IsGameOver)
        {
            m_AudioSource.PlayOneShot(vars.fallClip);
            spriteRenderer.sortingLayerName = "Default";//把渲染的图层置于最下面
            GetComponent<BoxCollider2D>().enabled = false;
            GameManager.Instance.IsGameOver = true;
            //调用结束的面板
            StartCoroutine(DealyShowGameOverPanel());
        }
        if(isJumping && IsRayObetancle()&&!GameManager.Instance.IsGameOver)
        {
            m_AudioSource.PlayOneShot(vars.hitClip);
            GameObject go =  ObjectPool.Instance.GetDeathEffect();
            go.SetActive(true);
            go.transform.position = transform.position;
            GameManager.Instance.IsGameOver = true;
            spriteRenderer.enabled = false;//玩家不显示
            //调用结束的面板
            StartCoroutine(DealyShowGameOverPanel());
        }
        if(transform.position.y-Camera.main.transform.position.y <-6 && !GameManager.Instance.IsGameOver)
        {
            m_AudioSource.PlayOneShot(vars.fallClip);
            GameManager.Instance.IsGameOver = true;
            //调用结束的面板
            StartCoroutine(DealyShowGameOverPanel());
        }
    }
    IEnumerator DealyShowGameOverPanel()
    {
        yield return new WaitForSeconds(1f);
        EventCenter.Broadcast(EventDefine.ShowGameOverPanel);
    }
    /// <summary>
    /// 是否检测到障碍物
    /// </summary>
    /// <returns></returns>
    private bool IsRayObetancle()
    {
        //2D物理引擎的射线检测，返回值是是否碰触
        RaycastHit2D hitLeft = Physics2D.Raycast(rayLeft.position,Vector2.left,0.2f,obstacleLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(rayRight.position,Vector2.right,0.2f,obstacleLayer);
        if(hitLeft.collider!=null)
        {
            if (hitLeft.collider.tag == "Obstacle")
            {
                return true;
            }
        }
        if (hitRight.collider != null)
        {
            if (hitRight.collider.tag == "Obstacle")
            {
                return true;
            }
        }
        return false;
    }
    private GameObject lastHitGo = null;
    /// <summary>
    /// 是否检测到平台
    /// </summary>
    /// <returns></returns>
    private bool IsRayPlayform()
    {
        RaycastHit2D hit =  Physics2D.Raycast(rayDown.position, Vector2.down, 1f, platformLater);
        if(hit.collider!=null)
        {
            if (hit.collider.tag == "Platform")
            {
                if(lastHitGo != hit.collider.gameObject)
                {
                    if (lastHitGo == null)
                    {
                        lastHitGo = hit.collider.gameObject;
                        return true;
                    }
                    EventCenter.Broadcast(EventDefine.AddScore);
                    lastHitGo = hit.collider.gameObject;
                }
                return true;
            }
        }
        return false;
        
    }
    private void Jump()
    {
        if(isMoveLeft)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            transform.DOMoveX(nextPlatformLeft.x,0.2f);
            transform.DOMoveY(nextPlatformLeft.y+0.8f,0.15f);
        }
        else
        {
            transform.localScale = Vector3.one;
            transform.DOMoveX(nextPlatformRight.x, 0.2f);
            transform.DOMoveY(nextPlatformRight.y + 0.8f, 0.15f);
        }
    }
    /// <summary>
    /// 碰撞检测
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Platform")
        {
            isJumping = false;
            Vector3 currentPlatformPos = collision.gameObject.transform.position;
            nextPlatformLeft = new Vector3(currentPlatformPos.x - vars.nestXPos, 
                currentPlatformPos.y + vars.nestYPos, 0);
            nextPlatformRight = new Vector3(currentPlatformPos.x + vars.nestXPos,
                currentPlatformPos.y + vars.nestYPos, 0);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "Pickup")
        {
            m_AudioSource.PlayOneShot(vars.diamondClip);
            EventCenter.Broadcast(EventDefine.AddDiamond);
            //代表吃到钻石
            collision.gameObject.SetActive(false);

        }
    }
    /// <summary>
    /// 音效是否开启
    /// </summary>
    /// <param name="isMusic"></param>
    private void IsMusicOn(bool isMusic)
    {
        m_AudioSource.mute = !isMusic;
    }
}
