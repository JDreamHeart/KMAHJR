using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("GameScripts/Board")]

public class Board : MonoBehaviour
{
    BoardSpawn m_spawn;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isOutboundary()) {
            if (m_spawn != null) {
                m_spawn.RecoverBoard(this);
            } else {
                Destroy(this.gameObject);
            }
        }
    }

    // 设置块生成器
    public void SetBoardSpawn(BoardSpawn spawn) {
        m_spawn = spawn;
    }
    
    // 检测是否超出边界
    bool isOutboundary() {
        if (m_spawn != null) {
            Vector2 pos = Camera.main.WorldToScreenPoint(this.transform.position);
            if (!m_spawn.IsRight()) {
                if (pos.y + this.GetComponent<RectTransform>().sizeDelta.y/2 <= 0) {
                    return true;
                }
            } else {
                if (pos.y - this.GetComponent<RectTransform>().sizeDelta.y/2 >= Screen.height) {
                    return true;
                }
            }
        }
        return false;
    }

    public bool IsRight() {
        if (m_spawn != null) {
            return m_spawn.IsRight();
        }
        return false;
    }
    
    void OnCollisionEnter2D(Collision2D collision) {
        // Debug.Log("Board OnCollisionEnter2D===================");
    }
}
