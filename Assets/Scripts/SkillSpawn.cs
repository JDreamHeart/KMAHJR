using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[AddComponentMenu("GameScripts/SkillSpawn")]

public class SkillSpawn : RewardSpawn
{
    public SkillData? m_curSkillData;

    // Start is called before the first frame update
    void Awake() {
        // m_minRewardCount = 0; // 最小奖励数量
        // m_maxRewardCount = 1; // 最大奖励数量
        // m_minGenerateSeconds = 5; // 最小生成奖励的时间间隔
        // m_maxGenerateSeconds = 10; // 最大生成奖励的时间间隔
    }

    // 开始后的回调
    protected override void OnStart(){
        // 更新当前技能数据
        UpdateCurSkillData();
    }
    
    protected override Reward getReward() {
        if (m_curSkillData == null) {
            return null;
        }
        Debug.Log(string.Format("getReward:{0}", m_rcRewards.Count));
        Vector3 targetPos = getRewardPos();
        if (m_rcRewards.Count > 0) {
            Reward reward = m_rcRewards[0];
            m_rcRewards.RemoveAt(0);
            reward.gameObject.SetActive(true);
            reward.GetComponent<Transform>().position = targetPos; // 更新位置
            return reward;
        }
        Transform rewardPrefab = m_curSkillData.Value.sprefab.GetComponent<Transform>();
        Transform trans = Instantiate(rewardPrefab, targetPos, Quaternion.identity, this.transform) as Transform; // 生成块状实例
        Tweener tweener =  trans.DOShakeRotation(2, 10); // 震动动画
        tweener.SetLoops(-1);
        return trans.GetComponent<Reward>();
    }

    protected override void OnGenerateReward(Reward reward) {
        SkillReward sr = reward.GetComponent<SkillReward>();
        sr.skillData = m_curSkillData.Value;
    }

    public void UpdateCurSkillData() {
        SkillsInfo skillsInfo = GameManager.Instance.GetSkillTrans().GetComponent<SkillsInfo>();
        m_curSkillData = skillsInfo.GetSkillData();
    }

}
