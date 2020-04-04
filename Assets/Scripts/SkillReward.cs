using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[AddComponentMenu("GameScripts/SkillReward")]

public class SkillReward : Reward
{
    Transform m_targetTrans;

    // public override void PlayDeadAnim(Vector3 targetPos, TweenCallback callback) {
    //     Transform rewardTrans = this.GetComponent<Transform>();
    //     Image img = this.GetComponent<Image>();
    //     Tweener tweener = rewardTrans.DOScale(0.3f, 1);
    //     img.DOFade(0.3f, 1);
    //     tweener.SetEase(Ease.OutExpo);
    //     tweener.OnComplete(() => {
    //         rewardTrans.DOScale(1, 0);
    //         img.DOFade(1, 0);
    //         this.onDead(); // 回调死亡事件
    //         callback(); // 执行动画回调
    //     });
    // }

    // public override void PlayDeadAnim(Transform targetTrans, TweenCallback callback) {
    //     m_targetTrans = targetTrans;
    //     this.PlayDeadAnim(targetTrans.position, () => {
    //         m_targetTrans = null;
    //         callback();
    //     });
    // }

    // public override void OnUpdate() {
    //     if (m_targetTrans != null) {
    //         this.transform.position = m_targetTrans.position;
    //     }
    // }
}
