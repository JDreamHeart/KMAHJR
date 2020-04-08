using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum PatternType {
    Normal = 0,
}

[AddComponentMenu("GameScripts/StartGameManager")]

public class StartGameManager : MonoBehaviour
{
    public static StartGameManager Instance;

    Transform m_patternName;
    
    Transform m_patternDetail;

    PatternType m_patternType;

    void Awake() {
        Instance = this;
        m_patternName = this.transform.Find("Pattern").Find("PatternName");
        m_patternDetail = this.transform.Find("PatternDetail");
    }

    // Start is called before the first frame update
    void Start()
    {
        m_patternDetail.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 更新模式名称
    public void UpdatePatternInfo(string name, PatternType type) {
        if (m_patternType != type) {
            // 播放音效
            AudioClip clip = Resources.Load<AudioClip>("Sounds/Switch");
            AudioSource.PlayClipAtPoint(clip, Vector3.zero);
        }
        // 更新模式信息
        Text nameText = m_patternName.Find("NameText").GetComponent<Text>();
        nameText.text = string.Format("- <color=yellow>{0}模式</color> -", name);
        m_patternType = type;
    }

    public void OnStartGame() {
        // 播放音效
        AudioClip clip = Resources.Load<AudioClip>("Sounds/Button");
        AudioSource.PlayClipAtPoint(clip, Vector3.zero);
        // 跳转场景
        Invoke("RunScene", 0.3f);
    }

    public void RunScene() {
        switch (m_patternType) {
            case PatternType.Normal:
                SceneManager.LoadScene("NormalScene"); // 启动普通模式场景
                break;
        }
    }

    public void ShowPatternDetail() {
        // 播放音效
        AudioClip clip = Resources.Load<AudioClip>("Sounds/Button");
        AudioSource.PlayClipAtPoint(clip, Vector3.zero);
        // 显示模式详情
        m_patternDetail.gameObject.SetActive(true);
    }
    
    public void HidePatternDetail() {
        // 播放音效
        AudioClip clip = Resources.Load<AudioClip>("Sounds/Button");
        AudioSource.PlayClipAtPoint(clip, Vector3.zero);
        // 隐藏模式详情
        m_patternDetail.gameObject.SetActive(false);
    }
}
