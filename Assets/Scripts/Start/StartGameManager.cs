using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public enum PatternType {
    Normal = 0,
    Guardian = 1, // 守护模式
}

[AddComponentMenu("GameScripts/StartGameManager")]

public class StartGameManager : MonoBehaviour
{
    public static StartGameManager Instance;

    Transform m_patternName;
    
    Transform m_patternDetail;
    Transform m_patternDetailDetail;

    PatternType m_patternType;

    Dictionary<PatternType, GameObject> m_patternDetailMap = new Dictionary<PatternType, GameObject>();

    void Awake() {
        Instance = this;
        m_patternName = this.transform.Find("Pattern").Find("PatternName");
        m_patternDetail = this.transform.Find("PatternDetail");
        m_patternDetailDetail = m_patternDetail.Find("Detail");
        m_patternDetailMap.Add(PatternType.Normal, m_patternDetailDetail.Find("Normal").gameObject);
        m_patternDetailMap.Add(PatternType.Guardian, m_patternDetailDetail.Find("Guardian").gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_patternDetail.gameObject.SetActive(false);
        hideAllPatternDetail();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 隐藏所有模式详情
    void hideAllPatternDetail() {
        foreach (GameObject val in m_patternDetailMap.Values) {
            val.SetActive(false);
        }
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
        hideAllPatternDetail();
        if (m_patternDetailMap.ContainsKey(m_patternType)) {
            m_patternDetailMap[m_patternType].SetActive(true);
        }
        m_patternDetailDetail.DOScale(0, 0);
        m_patternDetailDetail.DOScale(1, 0.3f);
    }
    
    public void HidePatternDetail() {
        // 播放音效
        AudioClip clip = Resources.Load<AudioClip>("Sounds/Button");
        AudioSource.PlayClipAtPoint(clip, Vector3.zero);
        // 隐藏模式详情
        Tweener tweener = m_patternDetailDetail.DOScale(0, 0.2f);
        tweener.OnComplete(() => {
            m_patternDetail.gameObject.SetActive(false);
        });
    }
}
