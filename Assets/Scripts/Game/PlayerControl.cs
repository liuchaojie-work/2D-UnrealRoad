using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
public class PlayerControl : MonoBehaviour
{
    public Transform rayDown;
    public Transform rayLeft;
    public Transform rayRight;
    public LayerMask platformLayer;
    public LayerMask obstacleLayer;
    /// <summary>
    /// 是否向左移动
    /// </summary>
    private bool isMoveLeft = false;

    private bool isJumping = false;

    private Vector3 nextPlatfromLeft;
    private Vector3 nextPlatfromRight;

    private ManagerVars vars;
    private Rigidbody2D body;
    private SpriteRenderer spriteRenderer;
    private bool isMove = false;

    private AudioSource audioSource;


    private void Awake()
    {
        EventCenter.AddListener<bool>(EventType.IsMusicOn, IsMusicOn);
        EventCenter.AddListener<int>(EventType.ChangeSkin, ChangeSkin);
        vars = ManagerVars.GetManagerVars();
        spriteRenderer = GetComponent<SpriteRenderer>();
        body = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        ChangeSkin(GameManager.Instance.GetCurrentSelectedSkin());
    }
    /// <summary>
    /// 更换皮肤的调用
    /// </summary>
    /// <param name="index"></param>
    private void ChangeSkin(int index)
    {
        spriteRenderer.sprite = vars.characterSkinSpriteList[index];
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener<bool>(EventType.IsMusicOn, IsMusicOn);
        EventCenter.RemoveListener<int>(EventType.ChangeSkin, ChangeSkin);
    }

    private bool IsPointerOverGameObject(Vector2 mousePosition)
    {
        //创建一个点击事件
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = mousePosition;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        //向点击位置发射一条射线，检测是否是UI
        EventSystem.current.RaycastAll(eventData, raycastResults);
        return raycastResults.Count > 0;
    }
    // Update is called once per frame
    void Update()
    {
        /*
        Debug.DrawRay(rayDown.position, Vector2.down * 1,Color.red);
        Debug.DrawRay(rayLeft.position, Vector2.left * 0.15f, Color.green);
        Debug.DrawRay(rayRight.position, Vector2.right * 0.15f, Color.green);
        */
        //if(Application.platform == RuntimePlatform.Android ||
        //    Application.platform == RuntimePlatform.IPhonePlayer)
        //{
        //    int fingerId = Input.GetTouch(0).fingerId;
        //    if(EventSystem.current.IsPointerOverGameObject(fingerId))
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
        
        if(IsPointerOverGameObject(Input.mousePosition))
        {
            return;
        }
        if (GameManager.Instance.IsGameStarted == false || GameManager.Instance.IsGameOver|| GameManager.Instance.IsPause)
        {
            return;
        }
        if(Input.GetMouseButtonDown(0) && isJumping == false && nextPlatfromLeft != Vector3.zero)
        {
            if(isMove == false)
            {
                EventCenter.Broadcast(EventType.PlayerMove);
                isMove = true;
            }
            audioSource.PlayOneShot(vars.jumpCLip);
            EventCenter.Broadcast(EventType.DecidePath);
            isJumping = true;
            Vector3 mousePos = Input.mousePosition;
            //点击左边屏幕
            if(mousePos.x < Screen.width / 2)
            {
                isMoveLeft = true;
            }
            else if(mousePos.x > Screen.width / 2)
            {
                isMoveLeft = false;
            }
            Jump();
        }
        //游戏结束
        if(body.velocity.y < 0 && IsRayPlatform() == false && GameManager.Instance.IsGameOver == false)
        {
            audioSource.PlayOneShot(vars.fallClip);
            spriteRenderer.sortingLayerName = "Default";
            GetComponent<BoxCollider2D>().enabled = false;
            GameManager.Instance.IsGameOver = true;
            StartCoroutine(DealyShowGameOverPanel());
        }

        if (isJumping && IsRayObstacle() && GameManager.Instance.IsGameOver == false)
        {
            audioSource.PlayOneShot(vars.hitClip);
            GameObject go = GameObjectPool.Instance.GetDeathEffect();
            go.SetActive(true);
            go.transform.position = transform.position;
            GameManager.Instance.IsGameOver = true;
            spriteRenderer.enabled = false;
            StartCoroutine(DealyShowGameOverPanel());
            
        }
        if(transform.position.y - Camera.main.transform.position.y < -6.0f && GameManager.Instance.IsGameOver == false)
        {
            audioSource.PlayOneShot(vars.fallClip);
            GameManager.Instance.IsGameOver = true;
            StartCoroutine(DealyShowGameOverPanel());
            
        }
    }

    
    IEnumerator DealyShowGameOverPanel()
    {
        yield return new WaitForSeconds(1.0f);
        //调用结束面板
        EventCenter.Broadcast(EventType.ShowGameOverPanel);
    }

    private GameObject lastHitGo = null;
    /// <summary>
    /// 是否检测到平台
    /// </summary>
    /// <returns></returns>
    private bool IsRayPlatform()
    {
        RaycastHit2D hit = Physics2D.Raycast(rayDown.position, Vector2.down, 1.0f, platformLayer);
        if (hit.collider != null)
        {
            if (hit.collider.tag == "Platform")
            {
                if(lastHitGo != hit.collider.gameObject)
                {
                    if(lastHitGo == null)
                    {
                        lastHitGo = hit.collider.gameObject;
                        return true;
                    }
                    EventCenter.Broadcast(EventType.AddScore);
                    lastHitGo = hit.collider.gameObject;
                }
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// 是否检测到障碍物
    /// </summary>
    /// <returns></returns>
    private bool IsRayObstacle()
    {
        RaycastHit2D leftHit = Physics2D.Raycast(rayLeft.position, Vector2.left, 0.15f, obstacleLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(rayRight.position, Vector2.right, 0.15f, obstacleLayer);

        if (leftHit.collider != null)
        {
            if(leftHit.collider.tag == "Obstacle")
            {
                return true;
            }
            
        }
        

        if (rightHit.collider != null)
        {
            if (rightHit.collider.tag == "Obstacle")
            {
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
            transform.DOMoveX(nextPlatfromLeft.x, 0.2f);
            transform.DOMoveY(nextPlatfromLeft.y + 0.8f, 0.15f);
        }
        else
        {
            transform.localScale = Vector3.one;
            transform.DOMoveX(nextPlatfromRight.x, 0.2f);
            transform.DOMoveY(nextPlatfromRight.y + 0.8f, 0.15f);
        }
    }

    /// <summary>
    /// 音效是否开启
    /// </summary>
    /// <param name="value"></param>
    private void IsMusicOn(bool value)
    {
        audioSource.mute = !value;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Platform")
        {
            isJumping = false;
            Vector3 currentPlatfromPos = collision.gameObject.transform.position;
            nextPlatfromLeft = new Vector3(currentPlatfromPos.x - vars.nextXPos,
                                           currentPlatfromPos.y + vars.nextYPos, 0);
            nextPlatfromRight = new Vector3(currentPlatfromPos.x + vars.nextXPos,
                                           currentPlatfromPos.y + vars.nextYPos, 0);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "PackUp")
        {
            audioSource.PlayOneShot(vars.diamondClip);
            EventCenter.Broadcast(EventType.AddDiamond);
            //吃到砖石
            collision.gameObject.SetActive(false);
        }
    }
}
