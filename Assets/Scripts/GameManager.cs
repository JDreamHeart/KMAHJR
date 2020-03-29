using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        m_sprite = GameObject.FindGameObjectWithTag("Player").GetComponent<Sprite>();
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
}
