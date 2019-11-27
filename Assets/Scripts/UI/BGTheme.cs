using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGTheme : MonoBehaviour
{
    private SpriteRenderer m_spriteRenderer;
    private ManagerVars vars;
    private void Awake()
    {
        vars = ManagerVars.GetManagerVars();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        int randomValue = Random.Range(0, vars.bgThemeSpriteList.Count);
        m_spriteRenderer.sprite = vars.bgThemeSpriteList[randomValue];
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
