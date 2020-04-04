using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[AddComponentMenu("GameScripts/RewardSpawn")]

public class RewardSpawn : MonoBehaviour
{
    public Transform[] m_rewardPrefabs;

    public bool m_isDown;

    float m_seconds = 3f;
    
    float m_velocity = 3f;

    List<Reward> m_rewards = new List<Reward>();

    float m_duration;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 生成奖励物
        m_duration += Time.deltaTime;
        if (GameManager.Instance.IsPlaying() && m_duration >= m_seconds) {
            Reward reward = getReward(); // 获取块状
            reward.SetRewardSpawn(this);
            if (m_isDown) {
                reward.UpdateVelocity(m_velocity);
            } else {
                reward.UpdateVelocity(-m_velocity);
            }
            m_duration = 0;
        }
    }
    
    public bool IsDown() {
        return m_isDown;
    }

    public void UpdateSeconds(float seconds) {
        m_seconds = seconds;
    }

    public void UpdateVelocity(float velocity) {
        m_velocity = velocity;
    }

    // 获取块状（使用缓存）
    Reward getReward() {
        Rect rect = this.GetComponent<RectTransform>().rect;
        Vector3 pos = Camera.main.WorldToScreenPoint(this.transform.position);
        Vector3 targetPos = Camera.main.ScreenToWorldPoint(pos + new Vector3(rect.width * Random.Range(-0.5f, 0.5f), rect.height * Random.Range(-0.5f, 0.5f), 0));
        if (m_rewards.Count > 0) {
            Reward reward = m_rewards[0];
            m_rewards.RemoveAt(0);
            reward.gameObject.SetActive(true);
            reward.GetComponent<Transform>().position = targetPos; // 更新位置
            return reward;
        }
        Transform rewardPrefab = m_rewardPrefabs[Random.Range(0, m_rewardPrefabs.Length-1)];
        Transform trans = Instantiate(rewardPrefab, targetPos, Quaternion.identity, this.transform) as Transform; // 生成块状实例
        Tweener tweener =  trans.DOShakeRotation(2, 10); // 震动动画
        tweener.SetLoops(-1);
        return trans.GetComponent<Reward>();
    }

    // 回收块状
    public void RecoverReward(Reward reward) {
        reward.gameObject.SetActive(false);
        m_rewards.Add(reward);
    }

}
