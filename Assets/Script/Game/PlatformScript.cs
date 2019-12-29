using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformScript : MonoBehaviour
{
    public SpriteRenderer[] spriteRenderers;
    public GameObject obstacles;
    private bool startTime;
    private float fallTime;
    private Rigidbody2D my_Body;
    private void Awake()
    {
        my_Body = GetComponent<Rigidbody2D>();
    }
    public void Init(Sprite sprite,float fallTime, int obstacleDir)
    {
        startTime = true;
        this.fallTime = fallTime;
        my_Body.bodyType = RigidbodyType2D.Static;
        for (int i=0;i<spriteRenderers.Length;i++)
        {
            spriteRenderers[i].sprite = sprite;
        }
        if(obstacleDir == 0)//向右
        {
            if(obstacles!=null)
            {
                //position是世界坐标系，可以理解为绝对坐标系，localPosition是相对坐标系，相对于父对象的位置
                obstacles.transform.localPosition = new Vector3(-obstacles.transform.localPosition.x,
                    obstacles.transform.localPosition.y,
                    obstacles.transform.localPosition.z);
            }
          
        }
    }
    private void Update()
    {
        if (GameManager.Instance.PlayerIsMove)
        {
            if (startTime)
            {
                fallTime -= Time.deltaTime;
                if (fallTime < 0)
                {
                    //掉落
                    startTime = false;
                    if (my_Body.bodyType != RigidbodyType2D.Dynamic)
                    {
                        my_Body.bodyType = RigidbodyType2D.Dynamic;
                    }
                    StartCoroutine(DealyHide());
                }
            }
        }
        if(transform.position.y-Camera.main.transform.position.y < -6)
        {
            StartCoroutine(DealyHide());
        }
    }
    private IEnumerator DealyHide()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
}
