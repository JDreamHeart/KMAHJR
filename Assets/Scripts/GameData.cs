﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public struct ScoreDataItem {
    public DateTime time;
    public int score;
    public override string ToString() {
        return time.Ticks.ToString() + "|" + score.ToString();
    }
    public bool Parse(string str) {
        string[] strList = str.Split('|');
        if (strList.Length >= 2) {
            time = new DateTime(long.Parse(strList[0]));
            score = int.Parse(strList[1]);
            return true;
        }
        return false;
    }
}

public class GameData : MonoBehaviour
{
    public static GameData Instance;

    public readonly string version = "0.12";

    Dictionary<PatternType, List<ScoreDataItem>> m_scoreMap = new Dictionary<PatternType, List<ScoreDataItem>>();
    
    int m_scoreItemLimit = 50;

    static GameData() {
        GameObject go = new GameObject("GameData");
        DontDestroyOnLoad(go);
        Instance = go.AddComponent<GameData>();
        // 初始化
        Instance.init();
    }

    public void Load() {
        
    }

    void init() {

    }

    // 校验版本
    public bool VerifyVersion(string targetVer) {
        try {
            string[] srcList = version.Split('.');
            string[] tgtList = targetVer.Split('.');
            if (tgtList.Length < srcList.Length) {
                return false;
            }
            for (int i = 0; i < srcList.Length; i++) {
                int idx = tgtList.Length - (srcList.Length - i);
                int src = int.Parse(srcList[i]);
                int tgt = int.Parse(tgtList[idx]);
                if (tgt > src) {
                    return true;
                } else if (tgt < src) {
                    return false;
                }
            }
        } catch (Exception e) {
            Debug.LogWarning(string.Format("Error to verify version -> {0}", e));
        }
        return false;
    }

    public void SetPatternType(PatternType ptype) {
        PlayerPrefs.SetString("PatternType", ptype.ToString());
    }

    public PatternType GetPatternType() {
        if (PlayerPrefs.HasKey("PatternType")) {
            String ptype = PlayerPrefs.GetString("PatternType");
            return (PatternType)Enum.Parse(typeof(PatternType), ptype);
        }
        return PatternType.Normal;
    }

    public ScoreDataItem NewScoreDataItem(int score) {
        ScoreDataItem item = new ScoreDataItem();
        item.time = System.DateTime.Now;
        item.score = score;
        return item;
    }

    public void AddScoreData(PatternType patternType, int score) {
        Debug.Log(string.Format("GameData AddScoreData: {0}, {1}.", patternType, score));
        DateTime time = System.DateTime.Now;
        if (!m_scoreMap.ContainsKey(patternType)) {
            m_scoreMap.Add(patternType, convertScoreData(patternType));
        }
        if (!insertScoreDataItem(m_scoreMap[patternType], NewScoreDataItem(score))) {
            return;
        }
        List<ScoreDataItem> items = m_scoreMap[patternType];
        while (items.Count > 0 && items.Count > m_scoreItemLimit) {
            items.RemoveAt(items.Count - 1);
        }
        saveScoreData(patternType);
    }

    public ScoreDataItem[] GetScoreData(PatternType patternType) {
        if (!m_scoreMap.ContainsKey(patternType)) {
            m_scoreMap.Add(patternType, convertScoreData(patternType));
        }
        return m_scoreMap[patternType].ToArray();
    }

    bool insertScoreDataItem(List<ScoreDataItem> items, ScoreDataItem item) {
        for (int i = 0; i < items.Count; i++) {
            if (items[i].score <= item.score) {
                items.Insert(i, item);
                return true;
            }
        }
        if (items.Count < m_scoreItemLimit) {
            items.Add(item);
            return true;
        }
        return false;
    }

    void saveScoreData(PatternType patternType) {
        if (!m_scoreMap.ContainsKey(patternType)) {
            return;
        }
        List<ScoreDataItem> items = m_scoreMap[patternType];
        string key = string.Format("Score|{0}|", patternType);
        PlayerPrefs.SetInt(key+"Count", items.Count);
        for (int i = 0; i < items.Count; i++) {
            PlayerPrefs.SetString(key+i.ToString(), items[i].ToString());
        }
        PlayerPrefs.Save();
    }

    List<ScoreDataItem> convertScoreData(PatternType patternType) {
        List<ScoreDataItem> result = new List<ScoreDataItem>();
        string key = string.Format("Score|{0}|", patternType);
        if (PlayerPrefs.HasKey(key+"Count")) {
            for (int i = 0; i < PlayerPrefs.GetInt(key+"Count"); i++) {
                if (PlayerPrefs.HasKey(key+i.ToString())) {
                    ScoreDataItem item = new ScoreDataItem();
                    if (item.Parse(PlayerPrefs.GetString(key+i.ToString()))) {
                        result.Add(item);
                    }
                }
            }
        }
        return result;
    }

}
