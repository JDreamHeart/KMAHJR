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

    PatternType m_patternType;

    void Awake() {
        Instance = this;
        m_patternName = this.transform.Find("Pattern").Find("PatternName");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 更新模式名称
    public void UpdatePatternInfo(string name, PatternType type) {
        Text nameText = m_patternName.Find("NameText").GetComponent<Text>();
        nameText.text = string.Format("- <color=yellow>{0}模式</color> -", name);
        m_patternType = type;
    }

    public void OnStartGame() {
        switch (m_patternType) {
            case PatternType.Normal:
                SceneManager.LoadScene("NormalScene"); // 启动普通模式场景
                break;
        }
        
    }
}
