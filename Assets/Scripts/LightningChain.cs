using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[ExecuteInEditMode]

[AddComponentMenu("GameScripts/LightningChain")]

public class LightningChain : MonoBehaviour
{
    public float m_displacement = 10; // 位移量

    public float m_minSpacing = 1; // 最小分形间距

    public Transform m_startPos; // 开始位置
    public Transform m_endPos; // 结束位置

    LineRenderer m_lineRender; // 线条渲染器

    List<Vector3> m_linePosList = new List<Vector3>(); // 线条位置列表

    // Start is called before the first frame update
    void Start()
    {
        m_lineRender = this.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0) {
            return;
        }
        m_linePosList.Clear();
        Vector3 startPos = Vector3.zero, endPos = Vector3.zero;
        if (m_startPos != null) {
            startPos = m_startPos.position;
        }
        if (m_endPos != null) {
            endPos = m_endPos.position;
        }
        collectLinPos(startPos, endPos, m_displacement);
        m_linePosList.Add(endPos);

        m_lineRender.positionCount = m_linePosList.Count;
        for (int i = 0; i < m_linePosList.Count; i++) {
            m_lineRender.SetPosition(i, m_linePosList[i]);
        }
    }

    void collectLinPos(Vector3 startPos, Vector3 endPos, float displacement) {
        if (displacement < m_minSpacing) {
            m_linePosList.Add(startPos);
        } else {
            float midX = (startPos.x + endPos.x) / 2;
            float midY = (startPos.y + endPos.y) / 2;
            float midZ = (startPos.z + endPos.z) / 2;

            midX += (float)(UnityEngine.Random.value - 0.5) * displacement;
            midY += (float)(UnityEngine.Random.value - 0.5) * displacement;
            midZ += (float)(UnityEngine.Random.value - 0.5) * displacement;

            Vector3 midPos = new Vector3(midX,midY,midZ);

            collectLinPos(startPos, midPos, displacement / 2);
            collectLinPos(midPos, endPos, displacement / 2);
        }
    }
}
