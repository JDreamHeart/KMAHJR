using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public enum PatternType {
    Normal = 0,
    DoubleJump = 1, // 二段跳模式
    Guardian = 2, // 守护模式
}

[AddComponentMenu("GameScripts/StartGameManager")]

public class StartGameManager : MonoBehaviour
{
    public static StartGameManager Instance;

    Transform m_enterBtn; // 进入游戏按钮

    Transform m_patternName; // 模式名称
    
    Transform m_patternDetail; // 模式详情
    Transform m_patternDetailDetail; // 模式详情的细节

    PatternType m_patternType; // 当前所选择的模式类型

    Dictionary<PatternType, GameObject> m_patternDetailMap = new Dictionary<PatternType, GameObject>();
    Dictionary<PatternType, string> m_patternSceneMap = new Dictionary<PatternType, string>();

    void Awake() {
        GameData.Instance.Load();
        Instance = this;
        m_enterBtn = this.transform.Find("Enter");
        m_patternName = this.transform.Find("Pattern").Find("PatternName");
        m_patternDetail = this.transform.Find("PatternDetail");
        m_patternDetailDetail = m_patternDetail.Find("Detail");
        m_patternDetailMap.Add(PatternType.Normal, m_patternDetailDetail.Find("Normal").gameObject); // 模式详情
        m_patternSceneMap.Add(PatternType.Normal, "NormalScene"); // 对应模式的场景
        m_patternDetailMap.Add(PatternType.DoubleJump, m_patternDetailDetail.Find("DoubleJump").gameObject); // 模式详情
        m_patternSceneMap.Add(PatternType.DoubleJump, "DoubleJumpScene"); // 对应模式的场景
        m_patternDetailMap.Add(PatternType.Guardian, m_patternDetailDetail.Find("Guardian").gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_patternType = GameData.Instance.GetPatternType();
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
        GameData.Instance.SetPatternType(type); // 保存数据
        // 校验模式场景
        Text enterText = m_enterBtn.Find("Text").GetComponent<Text>();
        if (m_patternSceneMap.ContainsKey(m_patternType)) {
            m_enterBtn.GetComponent<Button>().interactable = true;
            enterText.DOFade(1, 0.1f);
        } else {
            m_enterBtn.GetComponent<Button>().interactable = false;
            enterText.DOFade(0.2f, 0.1f);
        }
    }

    public void OnStartGame() {
        // 播放音效
        AudioClip clip = Resources.Load<AudioClip>("Sounds/Button");
        AudioSource.PlayClipAtPoint(clip, Vector3.zero);
        // 跳转场景
        Invoke("RunScene", 0.3f);
    }

    public void RunScene() {
        if (m_patternSceneMap.ContainsKey(m_patternType)) {
            SceneManager.LoadScene(m_patternSceneMap[m_patternType]); // 启动对应模式场景
        }
    }

    public void ShowPatternDetail() {
        // 播放音效
        AudioClip clip = Resources.Load<AudioClip>("Sounds/Button");
        AudioSource.PlayClipAtPoint(clip, Vector3.zero);
        // 显示模式详情
        Tweener tweener = m_patternDetailDetail.DOScale(0, 0);
        tweener.OnComplete(() => {
            m_patternDetail.gameObject.SetActive(true);
            hideAllPatternDetail();
            if (m_patternDetailMap.ContainsKey(m_patternType)) {
                m_patternDetailMap[m_patternType].SetActive(true);
            }
            // 显示
            m_patternDetailDetail.DOScale(1, 0.3f);
        });
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
