using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("GameScripts/Reward")]

public class Reward : MonoBehaviour
{
    RewardSpawn m_spawn;

    public int m_score = 1;

    float m_velocity = -1; // 移动速度
    
    public float m_survivalTime = -1; // 生存时间（负数表示不限制生存时间）

    public int m_maxTwinkleFrequency = 4; // 最大闪烁频率（次/秒）

    public float m_remainingTimeToTwinkle = 3; // 开始闪烁的剩余时间

    float m_duration;

    CanvasGroup m_canvasGroup;

    // Start is called before the first frame update
    void Start()
    {
        m_canvasGroup = this.GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        m_duration += Time.deltaTime;
        if (m_survivalTime > 0) {
            float diff = m_survivalTime - m_duration;
            if (diff > 0 && m_remainingTimeToTwinkle > 0) {
                // 按照一定频率进行闪烁显示
                diff = m_remainingTimeToTwinkle - diff;
                if (diff > 0) {
                    int ft = Mathf.CeilToInt(m_maxTwinkleFrequency * (diff / m_remainingTimeToTwinkle));
                    if (diff * ft % 1 > 0.5f) {
                        m_canvasGroup.alpha = 1;
                    } else {
                        m_canvasGroup.alpha = 0.68f;
                    }
                }
            } else {
                onDead();
                return;
            }
        }
        // // 移动
        // this.transform.Translate(0, m_velocity * Time.deltaTime, 0);
        // 检测是否越界
        if (isOutboundary()) {
            onDead();
            return;
        }
    }

    public void onDead() {
        m_duration = 0;
        m_canvasGroup.alpha = 1;
        if (m_spawn != null) {
            m_spawn.RecoverReward(this);
        } else {
            Destroy(this.gameObject);
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

    // void OnTriggerEnter2D(Collider2D collider) {
    //     onDead();
    // }

    public int GetScore() {
        return m_score;
    }
}
