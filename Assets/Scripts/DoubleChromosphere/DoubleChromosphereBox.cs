using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleChromosphereBox : MonoBehaviour
{
    int RedCount = 33;  // 红球数量
    int BlueCount = 16;  // 蓝球数量

    int RedResultCount = 6;  // 红球结果数量
    int BlueResultCount = 1;  // 蓝球结果数量

    List<DoubleChromosphereBall> m_redBallList = new List<DoubleChromosphereBall>();
    DoubleChromosphereBall m_blueBall;

    void Awake() {
        Transform container = this.transform.Find("RedBox/Container");
        for (int i = 0; i < RedResultCount; i++) {
            m_redBallList.Add(container.Find(string.Format("Ball{0}", i+1)).GetComponent<DoubleChromosphereBall>());
            m_redBallList[i].num = 0;  // 初始为0
        }

        m_blueBall = this.transform.Find("BlueBox/Ball").GetComponent<DoubleChromosphereBall>();
    }

    List<int> randomRange(int maxCount, int count) {
        List<int> m_numList = new List<int>();
        for (int i =0; i < maxCount; i++) {
            m_numList.Add(i+1);
        }

        List<int> ret = new List<int>();
        for (int i = 0; i < count; i++) {
            int idx = UnityEngine.Random.Range(0, m_numList.Count);
            ret.Add(m_numList[idx]);
            m_numList.RemoveAt(idx);
        }
        return ret;
    }

    public void RollBallNum() {
        List<int> redResult = this.randomRange(RedCount, RedResultCount);
        redResult.Sort();
        for (int i = 0; i < redResult.Count; i++){
            m_redBallList[i].num = redResult[i];
        }
        for (int i = redResult.Count; i < m_redBallList.Count; i++) {
            m_redBallList[i].num = 0;
        }
        
        List<int> blueResult = this.randomRange(BlueCount, BlueResultCount);
        if (blueResult.Count > 0) {
            m_blueBall.num = blueResult[0];
        } else {
            m_blueBall.num = 0;
        }
    }
}
