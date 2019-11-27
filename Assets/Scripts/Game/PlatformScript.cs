using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformScript : MonoBehaviour
{
    public SpriteRenderer[] spriteRenderers;
    public GameObject obstacle;
    private bool startTimer;
    private float fallTime;
    private Rigidbody2D body;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }
    public void PlatformScriptInit(Sprite sprite, float fallTime, int obstacleDir)
    {
        body.bodyType = RigidbodyType2D.Static;
        this.fallTime = fallTime;
        startTimer = true;
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].sprite = sprite;
        }

        if(0 == obstacleDir)
        {
            //朝右边
            if(obstacle != null)
            {
                obstacle.transform.localPosition = new Vector3(-obstacle.transform.localPosition.x,
                    obstacle.transform.localPosition.y, obstacle.transform.localPosition.z);
            }
        }
    }
    private void Update()
    {
        if(GameManager.Instance.IsGameStarted == false || GameManager.Instance.IsPlayerMove == false)
        {
            return;
        }
        if(startTimer)
        {
            fallTime -= Time.deltaTime;
            if(fallTime < 0)
            {
                //倒计时结束，掉落
                startTimer = false;
                if(body.bodyType != RigidbodyType2D.Dynamic)
                {
                    body.bodyType = RigidbodyType2D.Dynamic;
                    StartCoroutine(DealyHide());
                }
            }
        }

        if(transform.position.y - Camera.main.transform.position.y < -6.0f)
        {
            StartCoroutine(DealyHide());
        }
    }

    private IEnumerator DealyHide()
    {
        yield return new WaitForSeconds(1.0f);
        gameObject.SetActive(false);

    }
}
