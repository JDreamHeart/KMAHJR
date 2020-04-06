using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("GameScripts/GameStartCtrl")]

public class GameStartCtrl : MonoBehaviour
{
    Sprite m_sprite; // 主角精灵

    // Start is called before the first frame update
    void Start()
    {
        m_sprite = GameObject.FindGameObjectWithTag("Player").GetComponent<Sprite>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            float diff = Input.mousePosition.x - Screen.width/2;
            if (diff > 0) {
                m_sprite.TurnRight();
            } else {
                m_sprite.TurnLeft();
            }
            GameManager.Instance.OnStartGame();
        }
    }
}
