using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType {
    DoubleJump = 0, // 二段跳
    Accelerate = 1, // 加速
    HeavenGift = 2, // 天降礼物
    GroundGift = 3, // 地升礼物
    DoubleScore = 4, // 双倍分数
}

public enum SkillStyle {
    Times = 0, // 次数限制
    Duration = 1, // 时长限制
}

public struct SkillData {
    public SkillType stype;
    public SkillStyle sstyle;
    public SkillReward sprefab;
    public int stimes;
    public float sduration;
}

[AddComponentMenu("GameScripts/SkillsInfo")]

public class SkillsInfo : MonoBehaviour
{
    public SkillReward m_doubleJumpPrefab;

    protected Dictionary<SkillType, int> TimesConfig = new Dictionary<SkillType, int>();
    protected Dictionary<SkillType, float> DurationConfig = new Dictionary<SkillType, float>();

    Dictionary<SkillType, SkillData> m_skillMap = new Dictionary<SkillType, SkillData>();

    List<Skill> m_skillList = new List<Skill>();

    protected virtual void initConfig() {
        // 初始化次数限制配置
        TimesConfig.Add(SkillType.DoubleJump, 1);
    }

    protected void updateSDConfig(SkillData sd) {
        if (TimesConfig.ContainsKey(sd.stype)) {
            sd.sstyle = SkillStyle.Times;
            sd.stimes = TimesConfig[sd.stype];
        } else if (DurationConfig.ContainsKey(sd.stype)) {
            sd.sstyle = SkillStyle.Duration;
            sd.sduration = DurationConfig[sd.stype];
        }
    }

    void Awake() {
        initConfig();
        resetSkillMap();
    }
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void resetSkillMap() {
        m_skillMap.Clear();
        if (m_doubleJumpPrefab != null) {
            SkillData sd = new SkillData();
            sd.stype = SkillType.DoubleJump;
            sd.sprefab = m_doubleJumpPrefab;
            updateSDConfig(sd);
            m_skillMap.Add(SkillType.DoubleJump, sd);
        }
    }

    void updateSkillsPos() {
        Vector3 pos = Camera.main.WorldToScreenPoint(this.transform.position);
        for (int i = 0; i < m_skillList.Count; i++) {
            Transform trans = m_skillList[i].GetComponent<Transform>();
            trans.position = Camera.main.ScreenToWorldPoint(new Vector3(pos.x - 32 - i * 86, pos.y, pos.z));
        }
    }
    
    public void AddSkill(SkillReward reward) {
        SkillData sd = reward.skillData.Value;
        RectTransform rt = Instantiate(sd.sprefab.GetComponent<RectTransform>(), this.transform) as RectTransform;
        rt.sizeDelta = new Vector2(64, 64);
        rt.GetComponent<SkillReward>().enabled = false;
        ParticleSystem spray = rt.Find("Spark").GetComponent<ParticleSystem>();
        spray.Stop();
        m_skillList.Insert(0, rt.gameObject.AddComponent<Skill>());
        updateSkillsPos();
        // 检测更新技能表
        if (sd.sstyle == SkillStyle.Times) {
            sd.stimes--;
            if (sd.stimes <= 0) {
                m_skillMap.Remove(sd.stype);
            }
        }
    }

    public SkillData? GetSkillData() {
        if (m_skillMap.Values.Count > 0) {
            int i = 0, idx = Random.Range(0, m_skillMap.Values.Count - 1);
            foreach (SkillData sd in m_skillMap.Values) {
                if (i == idx) {
                    return sd;
                }
            }
        }
        return null;
    }

    public void Reset() {
        // 删除技能
        foreach (Skill skill in m_skillList) {
            Destroy(skill.gameObject);
        }
        m_skillList.Clear();
        // 重置技能表
        resetSkillMap();
    }
}
