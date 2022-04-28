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
    DoubleChromosphere = 3, // 双色球
}

[AddComponentMenu("GameScripts/StartGameManager")]

public class StartGameManager : MonoBehaviour
{
    public static StartGameManager Instance;

    public float m_escapeDuration = 2; // 退出提示时长
    float m_escapeActualDuration = 0; // 退出提示显示当前时长

    Transform m_enterBtn; // 进入游戏按钮

    Transform m_patternName; // 模式名称
    
    Transform m_patternDetail; // 模式详情
    Transform m_patternDetailDetail; // 模式详情的细节

    Image m_escapeTips; // 退出游戏提示

    PatternType m_patternType; // 当前所选择的模式类型

    Dictionary<PatternType, GameObject> m_patternDetailMap = new Dictionary<PatternType, GameObject>();
    Dictionary<PatternType, string> m_patternSceneMap = new Dictionary<PatternType, string>();

    void Awake() {
        Instance = this;
        m_enterBtn = this.transform.Find("Enter");
        m_escapeTips = this.transform.Find("EscapeTips").GetComponent<Image>();
        m_patternName = this.transform.Find("Pattern").Find("PatternName");
        m_patternDetail = this.transform.Find("PatternDetail");
        m_patternDetailDetail = m_patternDetail.Find("Detail");
        m_patternDetailMap.Add(PatternType.Normal, m_patternDetailDetail.Find("Normal").gameObject); // 模式详情
        m_patternSceneMap.Add(PatternType.Normal, "NormalScene"); // 对应模式的场景
        m_patternDetailMap.Add(PatternType.DoubleJump, m_patternDetailDetail.Find("DoubleJump").gameObject); // 模式详情
        m_patternSceneMap.Add(PatternType.DoubleJump, "DoubleJumpScene"); // 对应模式的场景
        m_patternDetailMap.Add(PatternType.Guardian, m_patternDetailDetail.Find("Guardian").gameObject);
        m_patternDetailMap.Add(PatternType.DoubleChromosphere, m_patternDetailDetail.Find("DoubleChromosphere").gameObject); // 双色球详情
        m_patternSceneMap.Add(PatternType.DoubleChromosphere, "DoubleChromosphereScene"); // 双色球
    }

    // Start is called before the first frame update
    void Start()
    {
        m_patternType = GameData.Instance.GetPatternType();
        m_patternDetail.gameObject.SetActive(false);
        hideAllPatternDetail();
        m_escapeTips.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_escapeActualDuration > 0) {
            m_escapeActualDuration -= Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (m_escapeActualDuration > 0) {
                AudioSource audioSource = this.GetComponent<AudioSource>();
                audioSource.volume = 0; // 降低背景音乐
                audioSource.Stop(); // 停止背景音乐
                Application.Quit(); // 退出游戏
            } else {
                m_escapeActualDuration = m_escapeDuration;
                m_escapeTips.gameObject.SetActive(true);
                m_escapeTips.DOFade(1, 0).OnComplete(() => {
                    m_escapeTips.DOFade(0.6f, m_escapeDuration).OnComplete(() => {
                        m_escapeTips.DOFade(0, 0.1f).OnComplete(() => {
                            m_escapeTips.gameObject.SetActive(false);
                        });
                    });
                });
            }
        }
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
