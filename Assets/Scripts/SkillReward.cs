using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[AddComponentMenu("GameScripts/SkillReward")]

public class SkillReward : Reward
{
    public override void PlayDeadAnim(Transform targetTrans, TweenCallback callback) {
        if (targetTrans != null) {
            Vector3 targetPos = Camera.main.WorldToScreenPoint(targetTrans.position) + new Vector3(-32, 0, 0);
            this.PlayDeadAnim(Camera.main.ScreenToWorldPoint(targetPos), callback);
        }
    }
}
