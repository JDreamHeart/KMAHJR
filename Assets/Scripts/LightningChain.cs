using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[ExecuteInEditMode]

[AddComponentMenu("GameScripts/LightningChain")]

public class LightningChain : MonoBehaviour
{
    public float m_displacement = 1; // 位移量

    public float m_minSpacing = 0.1f; // 最小分形间距

    public Transform m_startTrans; // 开始位置
    public Transform m_endTrans; // 结束位置
    
    public Vector3 m_startPos; // 开始位置
    public Vector3 m_endPos; // 结束位置
    
    public bool m_isUseAnchor; // 是否使用位置锚点
    public Vector2 m_startAnchor = new Vector2(-0.5f, -0.5f); // 开始位置锚点
    public Vector2 m_endAnchor = new Vector2(0.5f, 0.5f); // 结束位置锚点

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
        Vector3[] posList = getStartAndEndPos();
        collectLinPos(posList[0], posList[1], m_displacement);
        m_linePosList.Add(posList[1]);

        m_lineRender.positionCount = m_linePosList.Count;
        
        for (int i = 0; i < m_linePosList.Count; i++) {
            m_lineRender.SetPosition(i, m_linePosList[i]);
        }
    }

    public void UpdatePosList(Vector3 startPos, Vector3 endPos) {
        m_startPos = startPos;
        m_endPos = endPos;
    }

    Vector3[] getStartAndEndPos() {
        // 获取起点和终点
        Vector3 startPos = m_startPos, endPos = m_endPos;
        if (m_isUseAnchor && this.GetComponent<RectTransform>() != null) {
            Rect rect = this.GetComponent<RectTransform>().rect;
            Vector3 pos = Camera.main.WorldToScreenPoint(this.transform.position);
            startPos = Camera.main.ScreenToWorldPoint(new Vector3(pos.x + m_startAnchor.x * rect.width, pos.y + m_startAnchor.y * rect.height, pos.z));
            endPos = Camera.main.ScreenToWorldPoint(new Vector3(pos.x + m_endAnchor.x * rect.width, pos.y + m_endAnchor.y * rect.height, pos.z));
        } else {
            if (m_startTrans != null) {
                startPos = m_startTrans.position;
            }
            if (m_endTrans != null) {
                endPos = m_endTrans.position;
            }
        }
        return new Vector3[]{startPos, endPos};
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
