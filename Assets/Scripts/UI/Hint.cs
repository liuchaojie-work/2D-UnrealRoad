using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class Hint : MonoBehaviour
{
    private Image img_BG;
    private Text txt_Hint;
    private void Awake()
    {
        img_BG = GetComponent<Image>();
        txt_Hint = GetComponentInChildren<Text>();
        img_BG.color = new Color(img_BG.color.r, img_BG.color.g, img_BG.color.b, 0);
        txt_Hint.color = new Color(txt_Hint.color.r, txt_Hint.color.g, txt_Hint.color.b, 0);
        EventCenter.AddListener<string>(EventType.Hint, Show);
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener<string>(EventType.Hint, Show);
    }
    private void Show(string text)
    {
        StopCoroutine("Dealy");
        transform.localPosition = new Vector3(0, -70, 0);
        transform.DOLocalMoveY(0, 0.3f).OnComplete(()=>
        {
            StartCoroutine("Dealy");
        });
        img_BG.DOColor(new Color(img_BG.color.r, img_BG.color.g, img_BG.color.b, 0.2f), 0.1f);
        txt_Hint.DOColor(new Color(txt_Hint.color.r, txt_Hint.color.g, txt_Hint.color.b, 1), 0.1f);
    }

    private IEnumerator Dealy()
    {
        yield return new WaitForSeconds(1.0f);
        transform.DOLocalMoveY(70, 0.3f);
        img_BG.DOColor(new Color(img_BG.color.r, img_BG.color.g, img_BG.color.b, 0), 0.1f);
        txt_Hint.DOColor(new Color(txt_Hint.color.r, txt_Hint.color.g, txt_Hint.color.b, 0), 0.1f);
    }
}
