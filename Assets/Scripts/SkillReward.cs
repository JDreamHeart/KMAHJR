using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[AddComponentMenu("GameScripts/SkillReward")]

public class SkillReward : Reward
{
    SkillType m_skillType;

    public void SetSkillType(SkillType stype) {
        m_skillType = stype;
    }
    
    public SkillType GetSkillType() {
        return m_skillType;
    }
    
    public override void PlayDeadAnim(Vector3 targetPos, TweenCallback callback) {
        // 更新位置
        targetPos = Camera.main.ScreenToWorldPoint(Camera.main.WorldToScreenPoint(targetPos) + new Vector3(-32, 0, 0));
        // 执行动画
        Transform rewardTrans = this.GetComponent<Transform>();
        Image img = this.GetComponent<Image>();
        Tweener tweener = rewardTrans.DOMove(targetPos, 0.8f);
        tweener.SetEase(Ease.OutExpo);
        // 恢复缩放和透明度
        rewardTrans.DOScale(64/this.GetComponent<RectTransform>().rect.width, 0);
        tweener.OnComplete(() => {
            // 恢复缩放和透明度
            rewardTrans.DOScale(1, 0);
            this.OnDead(); // 回调死亡事件
            callback(); // 执行动画回调
        });
    }

    public void OnAddSkill() {
        if (m_spawn != null) {
            m_spawn.GetComponent<SkillSpawn>().UpdateCurSkillData();
        }
    }
    
    // 同步分数文本
    protected override void syncScoreText() {
        
    }
}
