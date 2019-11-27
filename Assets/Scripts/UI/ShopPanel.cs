using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ShopPanel : MonoBehaviour
{
    private ManagerVars vars;
    private Transform parent;
    private Text txt_Name;
    private Text txt_Diamond;
    private Button btn_Back;
    private Button btn_Select;
    private Button btn_Pay;
    private int index;
    private void Awake()
    {
        EventCenter.AddListener(EventType.ShowShopPanel, Show);
        parent = transform.Find("Img_ScrollRect/GOE_Parent");
        txt_Name = transform.Find("Txt_Name").GetComponent<Text>();
        txt_Diamond = transform.Find("GOE_Diamond/Txt_DiamondNum").GetComponent<Text>();
        btn_Back = transform.Find("Btn_Back").GetComponent<Button>();
        btn_Back.onClick.AddListener(OnBtn_BackClick);
        btn_Select = transform.Find("Btn_Select").GetComponent<Button>();
        btn_Select.onClick.AddListener(OnBtn_SelectClick);
        btn_Pay = transform.Find("Btn_Pay").GetComponent<Button>();
        btn_Pay.onClick.AddListener(OnBtn_PayClick);
        vars = ManagerVars.GetManagerVars();
        
    }

    private void Start()
    {
        ShopPanelInit();
        gameObject.SetActive(false);
    }
    private void ShopPanelInit()
    {

        parent.GetComponent<RectTransform>().sizeDelta = new Vector2((vars.skinSpriteList.Count + 2) * 160, 256);
        for(int i = 0; i < vars.skinSpriteList.Count; i++)
        {
            GameObject go = Instantiate(vars.skinChooseItemPrefab, parent);
            //未解锁
            if (GameManager.Instance.GetSkinUnlocked(i) == false)
            {
                go.transform.Find("Img_Skin").GetComponent<Image>().color = Color.gray;
            }
            else
            {
                go.transform.Find("Img_Skin").GetComponent<Image>().color = Color.white;
            }
            go.transform.Find("Img_Skin").GetComponent<Image>().sprite = vars.skinSpriteList[i];
            go.transform.localPosition = new Vector3((i + 1) * 160, 0, 0);
        }
        //打开页面直接定位到选中的皮肤
        parent.transform.localPosition = new Vector3(GameManager.Instance.GetCurrentSelectedSkin() * -160.0f,0);
    }

    private void Update()
    {
        index = (int)Mathf.Round(parent.transform.localPosition.x / -160.0f);
        if(Input.GetMouseButtonUp(0))
        {
            parent.transform.DOLocalMoveX(index * -160.0f, 0.2f);
            //parent.transform.localPosition = new Vector3(currentIndex * -160.0f, 0);
        }
        SetItemSize(index);
        RefreshUI(index);
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventType.ShowShopPanel, Show);
    }
    private void Show()
    {
        gameObject.SetActive(true);
    }



    private void SetItemSize(int index)
    {
        for(int i = 0; i < parent.childCount; i++)
        {
            if(i == index)
            {
                parent.GetChild(i).transform.Find("Img_Skin").GetComponent<RectTransform>().localScale = new Vector2(1,1);
            }
            else
            {
                parent.GetChild(i).transform.Find("Img_Skin").GetComponent<RectTransform>().localScale = new Vector2(0.5f, 0.5f);
                // parent.GetChild(i).transform.Find("Img_Skin").GetComponent<RectTransform>().sizeDelta = new Vector2(80, 80);
            }
        }
    }

    private void RefreshUI(int index)
    {
        txt_Name.text = " "+ vars.skinNameList[index] + " ";
        txt_Diamond.text = GameManager.Instance.GetAllDiamond().ToString();
        //未解锁
        if(GameManager.Instance.GetSkinUnlocked(index) == false)
        {
            btn_Select.gameObject.SetActive(false);
            btn_Pay.gameObject.SetActive(true);
            btn_Pay.transform.Find("Txt_Price").GetComponent<Text>().text = vars.skinPrice[index].ToString();
        }
        else
        {
            btn_Pay.gameObject.SetActive(false);
            btn_Select.gameObject.SetActive(true);
            
        }
    }

    private void OnBtn_BackClick()
    {
        EventCenter.Broadcast(EventType.PlayClickAudio);
        EventCenter.Broadcast(EventType.ShowMainPanel);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 选择皮肤按钮点击
    /// </summary>
    private void OnBtn_SelectClick()
    {
        EventCenter.Broadcast(EventType.PlayClickAudio);
        EventCenter.Broadcast(EventType.ChangeSkin, index);
        GameManager.Instance.SetSelectedSkin(index);
        EventCenter.Broadcast(EventType.ShowMainPanel);
        gameObject.SetActive(false);
    }

    private void OnBtn_PayClick()
    {
        EventCenter.Broadcast(EventType.PlayClickAudio);
        int price = int.Parse(btn_Pay.transform.Find("Txt_Price").GetComponent<Text>().text);
        if(price > GameManager.Instance.GetAllDiamond())
        {
            EventCenter.Broadcast(EventType.Hint, "砖石不足");
           // Debug.Log("砖石不足，无法购买");
            return;
        }
        else
        {
            GameManager.Instance.UpdateAllDiamond(-price);
            GameManager.Instance.SetSkinUnlocaked(index);
            parent.GetChild(index).transform.Find("Img_Skin").GetComponent<Image>().color = Color.white;
        }
    }
}
