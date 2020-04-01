using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("GameScripts/Reward")]

public class Reward : MonoBehaviour
{
    RewardSpawn m_spawn;

    public int m_score = 1;

    float m_velocity = -1; // 移动速度

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 移动
        this.transform.Translate(0, m_velocity * Time.deltaTime, 0);
        // 检测是否越界
        if (isOutboundary()) {
            if (m_spawn != null) {
                m_spawn.RecoverReward(this);
            } else {
                Destroy(this.gameObject);
            }
        }
    }
    
    // 设置块生成器
    public void SetRewardSpawn(RewardSpawn spawn) {
        m_spawn = spawn;
    }
    
    // 更新速度
    public void UpdateVelocity(float velocity) {
        m_velocity = velocity;
    }
    
    // 检测是否超出边界
    bool isOutboundary() {
        if (m_spawn != null) {
            Vector2 pos = Camera.main.WorldToScreenPoint(this.transform.position);
            if (!m_spawn.IsDown()) {
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

    void OnCollisionEnter2D(Collision2D collision) {
        if (m_spawn != null) {
            m_spawn.RecoverReward(this);
        } else {
            Destroy(this.gameObject);
        }
    }

    public int GetScore() {
        return m_score;
    }
}
