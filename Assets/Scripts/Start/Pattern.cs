using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("GameScripts/StartPattern")]

public class Pattern : MonoBehaviour
{
    Transform m_patternName;
    Transform m_patternTips;
    
    LightningChain m_leftLightning;
    LightningChain m_rightLightning;

    public PatternCtrl m_patternCtrl;

    AudioSource m_audioSource;

    // Start is called before the first frame update
    void Start()
    {
        m_audioSource = this.GetComponent<AudioSource>();
        m_patternName = this.transform.Find("PatternName");
        m_patternTips = this.transform.Find("PatternTips");
        m_leftLightning = this.transform.Find("LeftLightning").GetComponent<LightningChain>();
        m_rightLightning = this.transform.Find("RightLightning").GetComponent<LightningChain>();
        // 暂时隐藏提示
        // m_patternTips.gameObject.SetActive(false);
        // 获取闪电位置
        Vector3 pos = Camera.main.WorldToScreenPoint(m_patternName.position);
        Vector3 startPos = Camera.main.ScreenToWorldPoint(new Vector3(0, pos.y, pos.z));
        Vector3 endPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, pos.y, pos.z));
        float width = m_patternName.GetComponent<RectTransform>().rect.width;
        Vector3 leftPos = Camera.main.ScreenToWorldPoint(new Vector3(pos.x - width/2, pos.y, pos.z));
        Vector3 rightPos = Camera.main.ScreenToWorldPoint(new Vector3(pos.x + width/2, pos.y, pos.z));
        // 更新闪电链位置
        m_leftLightning.UpdatePosList(startPos, leftPos);
        m_rightLightning.UpdatePosList(rightPos, endPos);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_patternCtrl != null && m_patternCtrl.IsDowning()) {
            m_leftLightning.StopLightning();
            m_rightLightning.StopLightning();
            if (m_audioSource.isPlaying) {
                m_audioSource.Stop();
            }
        } else {
            m_leftLightning.AwakeLightning();
            m_rightLightning.AwakeLightning();
            if (!m_audioSource.isPlaying) {
                m_audioSource.Play();
            }
        }
    }

}
