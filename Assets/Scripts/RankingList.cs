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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateRankingList(PatternType ptype) {
        Transform itemTrans = m_content.Find("Items");
        if (itemTrans == null) {
            return;
        }
        ScoreDataItem[] items = GameData.Instance.GetScoreData(ptype);
        int curIdx = 0;
        for (int i = 0; i < itemTrans.childCount; i++) {
            Transform child = itemTrans.GetChild(i);
            if (i < items.Length) {
                curIdx++;
                child.GetChild(0).GetComponent<Text>().text = curIdx.ToString();
                child.GetChild(1).GetComponent<Text>().text = items[i].score.ToString();
                child.GetChild(2).GetComponent<Text>().text = items[i].time.ToString();
                child.gameObject.SetActive(true); // 显示
            } else {
                child.gameObject.SetActive(false); // 隐藏不需要的孩子节点
            }
        }
        Vector3 pos = Camera.main.WorldToScreenPoint(itemTrans.position);
        while (curIdx < items.Length) {
            Transform child = Instantiate(m_itemPrefab, itemTrans.position, Quaternion.identity, itemTrans) as Transform; // 生成块状实例
            child.GetChild(0).GetComponent<Text>().text = (curIdx+1).ToString();
            child.GetChild(1).GetComponent<Text>().text = items[curIdx].score.ToString();
            child.GetChild(2).GetComponent<Text>().text = items[curIdx].time.ToString();
            child.position = Camera.main.ScreenToWorldPoint(new Vector3(pos.x, pos.y - curIdx * m_itemSpacing, pos.z));
            curIdx++;
        }
        // 更新内容尺寸
        RectTransform rt = m_content.GetComponent<RectTransform>();
        float sizeY = itemTrans.childCount * m_itemSpacing;
        rt.sizeDelta = new Vector2(rt.rect.width, sizeY);
    }
}
