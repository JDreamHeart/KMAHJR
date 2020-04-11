using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("GameScripts/RankingList")]

public class RankingList : MonoBehaviour
{
    public Transform m_content;

    public Transform m_itemPrefab;

    public float m_itemSpacing = 60;

    List<Transform> m_rankingItems = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Transform getRankingItem() {
        if (m_rankingItems.Count > 0) {
            Transform trans = m_rankingItems[0];
            m_rankingItems.RemoveAt(0);
            trans.gameObject.SetActive(true); // 显示节点
            return trans;
        }
        return Instantiate(m_itemPrefab, m_content.position, Quaternion.identity, m_content) as Transform; // 生成块状实例
    }

    public void UpdateRankingList(PatternType ptype) {
        if (m_content == null) {
            return;
        }
        ScoreDataItem[] items = GameData.Instance.GetScoreData(ptype);
        int curIdx = 0;
        for (int i = 0; i < m_content.childCount; i++) {
            Transform child = m_content.GetChild(i);
            if (i < items.Length) {
                curIdx++;
                child.GetChild(0).GetComponent<Text>().text = curIdx.ToString();
                child.GetChild(1).GetComponent<Text>().text = items[i].score.ToString();
                child.GetChild(2).GetComponent<Text>().text = items[i].time.ToString();
            } else {
                child.gameObject.SetActive(false); // 隐藏不需要的子节点
                m_rankingItems.Add(child);
            }
        }
        while (curIdx < items.Length) {
            Transform child = getRankingItem();
            child.GetChild(0).GetComponent<Text>().text = (curIdx+1).ToString();
            child.GetChild(1).GetComponent<Text>().text = items[curIdx].score.ToString();
            child.GetChild(2).GetComponent<Text>().text = items[curIdx].time.ToString();
            child.localPosition = new Vector3(0, - curIdx * m_itemSpacing, 0);
            curIdx++;
        }
        // 更新内容尺寸
        RectTransform rt = m_content.GetComponent<RectTransform>();
        float sizeY = m_content.childCount * m_itemSpacing;
        rt.sizeDelta = new Vector2(rt.rect.width, sizeY);
    }
}
