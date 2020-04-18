using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[AddComponentMenu("GameScripts/RewardSpawn")]

public class RewardSpawn : MonoBehaviour
{
    public Transform[] m_rewardPrefabs; // 奖励预制体

    public bool m_isDown; // 是否向下的奖励

    public int m_checkRewardLimit = 3; // 单次检测奖励的次数限制

    public int m_minRewardCount = 2; // 最小奖励数量
    public int m_maxRewardCount = 5; // 最大奖励数量

    public float m_minGenerateSeconds = 3; // 最小生成奖励的时间间隔
    public float m_maxGenerateSeconds = 3; // 最大生成奖励的时间间隔
    
    protected float m_velocity = 3f;

    protected List<Reward> m_activeRewards = new List<Reward>(); // 激活状态中的奖励对象
    protected List<Reward> m_rcRewards = new List<Reward>(); // 回收后的奖励对象

    protected float m_generateDuration;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.AddRewardSpawn(this);
        OnStart();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_activeRewards.Count < m_maxRewardCount) {
            // 生成奖励物
            m_generateDuration += Time.deltaTime;
            if (GameManager.Instance.IsPlaying() && (m_generateDuration >= Random.Range(m_minGenerateSeconds, m_maxGenerateSeconds) || m_activeRewards.Count < m_minRewardCount)) {
                Reward reward = getReward(); // 获取块状
                if (reward != null) {
                    if (checkRewardRect(reward)) {
                        reward.SetRewardSpawn(this);
                        m_activeRewards.Add(reward);
                        if (m_isDown) {
                            reward.UpdateVelocity(m_velocity);
                        } else {
                            reward.UpdateVelocity(-m_velocity);
                        }
                        m_generateDuration = 0;
                        // 生成奖励的回调
                        this.OnGenerateReward(reward);
                    } else {
                        RecoverReward(reward);
                    }
                }
            }
        }
    }
    
    public bool IsDown() {
        return m_isDown;
    }

    public void UpdateVelocity(float velocity) {
        m_velocity = velocity;
    }

    // 获取奖励的随机位置
    protected virtual Vector3 getRewardPos() {
        Rect rect = this.GetComponent<RectTransform>().rect;
        Vector3 pos = Camera.main.WorldToScreenPoint(this.transform.position);
        return Camera.main.ScreenToWorldPoint(pos + new Vector3(rect.width * Random.Range(-0.5f, 0.5f), rect.height * Random.Range(-0.5f, 0.5f), 0));
    }

    // 获取块状（使用缓存）
    protected virtual Reward getReward() {
        Vector3 targetPos = getRewardPos();
        if (m_rcRewards.Count > 0) {
            Reward reward = m_rcRewards[0];
            m_rcRewards.RemoveAt(0);
            reward.gameObject.SetActive(true);
            reward.GetComponent<Transform>().position = targetPos; // 更新位置
            return reward;
        }
        Transform rewardPrefab = m_rewardPrefabs[Random.Range(0, m_rewardPrefabs.Length)];
        Transform trans = Instantiate(rewardPrefab, targetPos, Quaternion.identity, this.transform) as Transform; // 生成块状实例
        Tweener tweener =  trans.DOShakeRotation(2, 10); // 震动动画
        tweener.SetLoops(-1);
        return trans.GetComponent<Reward>();
    }

    // 回收块状
    public void RecoverReward(Reward reward) {
        reward.gameObject.SetActive(false);
        m_rcRewards.Add(reward);
        m_activeRewards.Remove(reward);
        OnRecoverReward(reward);
    }

    public List<Reward> GetActiveRewards() {
        return m_activeRewards;
    }

    // 检测奖励范围
    bool checkRewardRect(Reward reward) {
        for (int i = 0; i < m_checkRewardLimit; i++) {
            if (GameManager.Instance.CheckRewardRect(reward)) {
                return true;
            } else {
                Vector3 targetPos = getRewardPos();
                reward.GetComponent<Transform>().position = targetPos; // 更新位置
            }
        }
        return false;
    }

    public void Reset() {
        m_generateDuration = 0;
        StartCoroutine(RecoverAllActiveReward());
    }

    IEnumerator RecoverAllActiveReward() {
        while (m_activeRewards.Count > 0) {
            m_activeRewards[0].OnDead();
            yield return null;
        }
    }

    // 生成奖励的回调
    protected virtual void OnGenerateReward(Reward reward) {

    }

    // 回收奖励对象的回调
    protected virtual void OnRecoverReward(Reward reward) {

    }

    // 开始后的回调
    protected virtual void OnStart(){

    }

}
