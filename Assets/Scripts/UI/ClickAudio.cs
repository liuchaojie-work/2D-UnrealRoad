using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickAudio : MonoBehaviour
{
    private AudioSource audioSource;
    private ManagerVars vars;
    private void Awake()
    {
        vars = ManagerVars.GetManagerVars();
        audioSource = GetComponent<AudioSource>();
        EventCenter.AddListener(EventType.PlayClickAudio, PlayAudio);
        EventCenter.AddListener<bool>(EventType.IsMusicOn, IsMusicOn);
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventType.PlayClickAudio, PlayAudio);
        EventCenter.RemoveListener<bool>(EventType.IsMusicOn, IsMusicOn);
    }
    private void PlayAudio()
    {
        audioSource.PlayOneShot(vars.buttonClip);
    }

    /// <summary>
    /// 音效是否开启
    /// </summary>
    /// <param name="value"></param>
    private void IsMusicOn(bool value)
    {
        audioSource.mute = !value;
    }
}
