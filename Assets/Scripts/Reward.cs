using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[AddComponentMenu("GameScripts/Reward")]

public class Reward : MonoBehaviour
{
    protected RewardSpawn m_spawn;

    public int m_score = 1;

    float m_velocity = -1; // 移动速度

    public bool m_isStatic = true; // 是否为静态物体
    
    public float m_survivalTime = -1; // 生存时间（负数表示不限制生存时间）

    public int m_maxTwinkleFrequency = 2; // 最大闪烁频率（次/秒）

    public float m_remainingTimeToTwinkle = 3; // 开始闪烁的剩余时间

    float m_duration;

    protected CanvasGroup m_canvasGroup;

    // Start is called before the first frame update
    void Start()
    {
        m_canvasGroup = this.GetComponent<CanvasGroup>();
        syncScoreText();
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
                OnDead();
                return;
            }
        }
        // 移动
        if (!m_isStatic) {
            this.transform.Translate(0, m_velocity * Time.deltaTime, 0);
        }
        // 检测是否越界
        if (isOutboundary()) {
            OnDead();
            return;
        }
        this.OnUpdate();
    }

    public void OnDead() {
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
    //     OnDead();
    // }

    public int GetScore() {
        return m_score;
    }

    public virtual void PlayDeadAnim(Vector3 targetPos, TweenCallback callback) {
        Transform rewardTrans = this.GetComponent<Transform>();
        Image img = this.GetComponent<Image>();
        Tweener tweener = rewardTrans.DOMove(targetPos, 1);
        tweener.SetEase(Ease.OutExpo);
        Tweener scaleTweener = rewardTrans.DOScale(0.2f, 0.8f);
        img.DOFade(0.2f, 0.8f);
        scaleTweener.OnComplete(() => {
            callback(); // 执行动画回调
        });
        tweener.OnComplete(() => {
            // 恢复缩放和透明度
            rewardTrans.DOScale(1, 0);
            img.DOFade(1, 0);
            // 回调死亡事件
            this.OnDead();
        });
    }
    
    public virtual void PlayDeadAnim(Transform targetTrans, TweenCallback callback) {
        if (targetTrans != null) {
            this.PlayDeadAnim(targetTrans.position, callback);
        }
    }

    // 更新时的回调
    protected virtual void OnUpdate() {

    }

    // 检测是否部分重叠
    public bool IsOverlaps(Reward reward) {
        Rect rect1 = this.GetComponent<RectTransform>().rect;
        Vector3 pos1 = Camera.main.WorldToScreenPoint(this.transform.position);
        rect1.x = pos1.x;
        rect1.y = pos1.y;
        Rect rect2 = reward.GetComponent<RectTransform>().rect;
        Vector3 pos2 = Camera.main.WorldToScreenPoint(reward.transform.position);
        rect2.x = pos2.x;
        rect2.y = pos2.y;
        return rect1.Overlaps(rect2);
    }

    // 同步分数文本
    protected virtual void syncScoreText() {
        Transform score = this.transform.Find("Score");
        if (score != null) {
            score.GetComponent<Text>().text = m_score.ToString();
        }
    }
}
