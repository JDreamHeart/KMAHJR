using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("GameScripts/RewardSpawn")]

public class RewardSpawn : MonoBehaviour
{
    public Transform[] m_rewardPrefabs;

    public bool m_isDown;

    float m_seconds = 3f;
    
    float m_velocity = 3f;

    List<Reward> m_rewards = new List<Reward>();

    float m_offsetX = 0;
    float m_offsetSign = 1;
    public float m_maxOffsetX = 800;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnReward());
    }

    // Update is called once per frame
    void Update()
    {
        m_offsetX += m_offsetSign * Time.deltaTime * m_velocity * Random.Range(0, 1.0f);
        if (Mathf.Abs(m_offsetX) > m_maxOffsetX) {
            m_offsetSign = -m_offsetSign;
        }
        this.transform.Translate(m_offsetX, 0, 0);
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

    IEnumerator SpawnReward() {
        while (true) {
            Reward reward = getReward(); // 获取块状
            reward.SetRewardSpawn(this);
            if (m_isDown) {
                reward.UpdateVelocity(m_velocity);
            } else {
                reward.UpdateVelocity(-m_velocity);
            }
            // 等待一段时间
            yield return new WaitForSeconds(m_seconds);
        }
    }

    // 获取块状（使用缓存）
    Reward getReward() {
        if (m_rewards.Count > 0) {
            Reward reward = m_rewards[0];
            m_rewards.RemoveAt(0);
            reward.gameObject.SetActive(true);
            reward.GetComponent<Transform>().position = transform.position; // 更新位置
            return reward;
        }
        Transform rewardPrefab = m_rewardPrefabs[Random.Range(0, m_rewardPrefabs.Length-1)];
        Transform trans = Instantiate(rewardPrefab, transform.position, Quaternion.identity, transform) as Transform; // 生成块状实例
        return trans.GetComponent<Reward>();
    }

    // 回收块状
    public void RecoverReward(Reward reward) {
        reward.gameObject.SetActive(false);
        m_rewards.Add(reward);
    }

}
