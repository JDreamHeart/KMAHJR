using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Direction {
    Unknown = 0,
    Left = 1,
    Right = 2,
    Up = 3,
    Down = 4,
}

[AddComponentMenu("GameScripts/GameManager")]

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // 静态实例

    bool m_isPlaying; // 是否正在游戏中

    Sprite m_sprite; // 主角精灵

    // 分数
    int m_score;
    Text m_scoreText;

    // 箭头
    Transform m_arrows;

    // 游戏结果
    public float m_showResultDelay = 1; // 显示结果的延时
    float m_curShowResultDelay = 0; // 当前显示结果的延时
    Transform m_result;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        m_sprite = GameObject.FindGameObjectWithTag("Player").GetComponent<Sprite>();
        m_arrows = this.transform.Find("Arrows");
        m_result = this.transform.Find("Result");
        m_result.gameObject.SetActive(false);
        // 获取游戏信息
        Transform infos = this.transform.Find("Infos");
        if (infos != null) {
            m_scoreText = infos.Find("Score").GetComponent<Text>();
        }
        // 重置分数
        ResetScore();
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_isPlaying) {
            return;
        }
        if (Input.GetMouseButtonDown(0)) {
            m_sprite.Jump();
        }
        if (!m_sprite.gameObject.activeSelf) {
            m_curShowResultDelay += Time.deltaTime;
            if (m_curShowResultDelay >= m_showResultDelay) {
                OnStopGame();
            }
        }
    }

    public bool IsPlaying() {
        return m_isPlaying;
    }

    public void OnStartGame() {
        m_isPlaying = true;
        if (m_arrows != null) {
            m_arrows.gameObject.SetActive(false);
        }
        m_sprite.OnStartGame();
        m_curShowResultDelay = 0;
    }
    
    public void OnStopGame() {
        m_isPlaying = false;
        Transform resultContent = m_result.Find("ResultContent");
        Transform curScoreVal = resultContent.Find("CurrentScore").Find("ScoreValue");
        curScoreVal.GetComponent<Text>().text = m_score.ToString();
        Transform maxScoreVal = resultContent.Find("RankingList").Find("MaxScoreValue");
        maxScoreVal.GetComponent<Text>().text = m_score.ToString();
        m_result.gameObject.SetActive(true);
    }

    public void OnRestartGame() {
        m_isPlaying = false;
        ResetScore();
        if (m_arrows != null) {
            m_arrows.gameObject.SetActive(true);
        }
        m_sprite.OnRestartGame();
        m_result.gameObject.SetActive(false);
    }

    public void AddScore(int score) {
        m_score += score;
        if (m_scoreText != null) {
            m_scoreText.text = m_score.ToString();
        }
    }
    
    public int GetScore() {
        return m_score;
    }
    
    public void ResetScore() {
        m_score = 0;
        if (m_scoreText != null) {
            m_scoreText.text = m_score.ToString();
        }
    }
}
