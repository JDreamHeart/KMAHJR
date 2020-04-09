using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]

[AddComponentMenu("GameScripts/StartPatternCtrl")]

public class PatternCtrl : MonoBehaviour
{
    public float m_spacing = 500; // 子项的间距

    public ScrollRect m_scrollView; // 滚动视图

    public float m_minScrollSpacing = 0.1f; // 最小滚动距离（会与Time.deltaTime相乘）

    bool m_isChangeScrollVal; // 是否改变滚动值
    float m_staticDuration; // 静止（未进行滚动）的时长

    bool m_isDowning; // 是否正在按下鼠标

    int m_targetItemIdx = -1; // 目标模式项下标

    // Start is called before the first frame update
    void Start()
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(this.transform.position);
        float lastPosX = Screen.width/2;
        for (int i = 0; i < this.transform.childCount; i++) {
            Transform child = this.transform.GetChild(i);
            child.position = Camera.main.ScreenToWorldPoint(new Vector3(lastPosX, pos.y, pos.z));
            lastPosX += m_spacing;
        }
        RectTransform rt = this.GetComponent<RectTransform>();
        float sizeX = Screen.width + Mathf.Max(0, this.transform.childCount - 1) * m_spacing;
        rt.sizeDelta = new Vector2(sizeX, rt.rect.height);
        // 更新目标项下标
        if (Application.isPlaying) {
            this.updateTargetItemIdx(0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            m_isDowning = true;
            return;
        }
        if (Input.GetMouseButtonUp(0)) {
            m_isDowning = false;
        }
        if (!m_isDowning && m_isChangeScrollVal) {
            m_staticDuration += Time.deltaTime;
            if (m_staticDuration >= 0.01f && this.transform.childCount > 1) {
                float fidx = m_scrollView.horizontalNormalizedPosition * (this.transform.childCount - 1);
                int previdx = Mathf.FloorToInt(fidx), nextidx = Mathf.CeilToInt(fidx);
                float targetPos = previdx / (this.transform.childCount - 1);
                int targetItemIdx = previdx;
                if (fidx - previdx > nextidx - fidx) {
                    targetPos = nextidx / (this.transform.childCount - 1);
                    targetItemIdx = nextidx;
                }
                this.updateTargetItemIdx(targetItemIdx);
                float minScrollSpacing = m_minScrollSpacing * Time.deltaTime;
                if (Mathf.Abs(targetPos - m_scrollView.horizontalNormalizedPosition) < minScrollSpacing) {
                    m_scrollView.horizontalNormalizedPosition = targetPos;
                    m_isChangeScrollVal = false;
                } else {
                    minScrollSpacing = targetPos < m_scrollView.horizontalNormalizedPosition ? -minScrollSpacing : minScrollSpacing;
                    m_scrollView.horizontalNormalizedPosition += minScrollSpacing;
                }
            }
        }
    }

    void updateTargetItemIdx(int targetItemIdx) {
        if (m_targetItemIdx != targetItemIdx) {
            m_targetItemIdx = targetItemIdx;
            if (this.transform.childCount > 0) {
                Transform child = this.transform.GetChild(m_targetItemIdx);
                if (child != null) {
                    StartGameManager.Instance.UpdatePatternInfo(child.GetComponent<PatternItem>().GetName(), child.GetComponent<PatternItem>().GetPatternType());
                }
            }
        }
    }

    public void OnChangeScrollVal(Vector2 pos) {
        m_isChangeScrollVal = true;
        m_staticDuration = 0;
    }

    public bool IsDowning() {
        return m_isDowning;
    }
}
