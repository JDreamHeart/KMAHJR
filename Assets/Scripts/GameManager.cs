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

    int m_score;
    Text m_scoreText;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        m_sprite = GameObject.FindGameObjectWithTag("Player").GetComponent<Sprite>();
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
    }

    public void OnStartGame() {
        m_isPlaying = true;
    }
    
    public void OnStopGame() {
        m_isPlaying = false;
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
