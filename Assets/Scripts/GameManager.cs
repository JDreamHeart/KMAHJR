﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public enum Direction {
    Unknown = 0,
    Left = 1,
    Right = 2,
    Up = 3,
    Down = 4,
}

[AddComponentMenu("GameScripts/GameManager")]

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // 静态实例
    
    public PatternType m_patternType; // 当前所选择的模式类型

    bool m_isPlaying; // 是否正在游戏中

    Sprite m_sprite; // 主角精灵

    // 分数
    int m_score;
    Text m_scoreText;

    // 箭头
    Transform m_startCtrl;

    // 游戏结果
    public float m_showResultDelay = 1; // 显示结果的延时
    float m_curShowResultDelay = 0; // 当前显示结果的延时
    Transform m_result;
    
    // 板块生成器列表
    List<BoardSpawn> m_boardSpawnList = new List<BoardSpawn>();

    // 奖励生成器列表
    List<RewardSpawn> m_rewardSpawnList = new List<RewardSpawn>();

    // 技能
    SkillsInfo m_skillsInfo;

    // 音频源
    AudioSource m_audioSource;

    void Awake() {
        Instance = this;
        m_audioSource = this.GetComponent<AudioSource>();
        m_sprite = GameObject.FindGameObjectWithTag("Player").GetComponent<Sprite>();
        m_startCtrl = this.transform.Find("StartCtrl");
        // 隐藏游戏结果
        m_result = this.transform.Find("Result");
        m_result.gameObject.SetActive(false);
        // 获取游戏信息
        Transform infos = this.transform.Find("Infos");
        if (infos != null) {
            m_scoreText = infos.Find("Score").GetComponent<Text>();
            m_skillsInfo = infos.Find("Skills").GetComponent<SkillsInfo>();
        }
        // 重置分数
        ResetScore();
    }

    // Start is called before the first frame update
    void Start()
    {
        // 隐藏排行榜
        m_result.Find("RankingList").gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_isPlaying) {
            return;
        }
        if (Input.GetMouseButtonDown(0)) {
            m_sprite.Jump();
        }
        if (!m_sprite.gameObject.activeSelf) {
            m_curShowResultDelay += Time.deltaTime;
            if (m_curShowResultDelay >= m_showResultDelay) {
                OnStopGame();
            }
        }
    }

    public bool IsPlaying() {
        return m_isPlaying;
    }

    public void OnStartGame() {
        m_isPlaying = true;
        if (m_startCtrl != null) {
            m_startCtrl.gameObject.SetActive(false);
        }
        m_sprite.OnStartGame();
        m_curShowResultDelay = 0;
        // 重置音量
        m_audioSource.volume = 0.2f;
    }
    
    public void OnStopGame() {
        m_isPlaying = false;
        // 保存分数
        GameData.Instance.AddScoreData(m_patternType, m_score);
        // 显示结果
        Transform resultContent = m_result.Find("ResultContent");
        Transform curScoreVal = resultContent.Find("CurrentScore").Find("ScoreValue");
        curScoreVal.GetComponent<Text>().text = m_score.ToString();
        Transform maxScoreVal = resultContent.Find("RankingList").Find("MaxScoreValue");
        ScoreDataItem[] items = GameData.Instance.GetScoreData(m_patternType);
        maxScoreVal.GetComponent<Text>().text = items[0].score.ToString();
        // 播放动画
        Tweener tweener = resultContent.DOScale(0, 0);
        tweener.OnComplete(() => {
            m_result.gameObject.SetActive(true);
            // 显示
            resultContent.DOScale(1, 0.3f);
        });
        // 重置板块生成器
        foreach (BoardSpawn spawn in m_boardSpawnList) {
            spawn.Reset();
        }
    }

    public void OnRestartGame() {
        m_isPlaying = false;
        ResetScore();
        if (m_startCtrl != null) {
            m_startCtrl.gameObject.SetActive(true);
        }
        m_sprite.OnRestartGame();
        m_result.gameObject.SetActive(false);
        m_skillsInfo.Reset();
        foreach (RewardSpawn spawn in m_rewardSpawnList) {
            spawn.Reset(); // 重置
            SkillSpawn ss = spawn.GetComponent<SkillSpawn>();
            if (ss != null) {
                ss.UpdateCurSkillData();
            }
        }
        // 重置音量
        m_audioSource.volume = 0.8f;
    }

    public Transform GetScoreTrans() {
        if (m_scoreText != null) {
            return m_scoreText.GetComponent<Transform>();
        }
        return null;
    }

    public void AddScore(int score) {
        m_score += score;
        if (m_scoreText != null) {
            m_scoreText.text = m_score.ToString();
        }
    }
    
    public int GetScore() {
        return m_score;
    }
    
    public void ResetScore() {
        m_score = 0;
        if (m_scoreText != null) {
            m_scoreText.text = m_score.ToString();
        }
    }

    // 重开一局
    public void OnTryAgain() {
        // 播放音效
        AudioClip clip = Resources.Load<AudioClip>("Sounds/Button");
        AudioSource.PlayClipAtPoint(clip, Vector3.zero);
        // 跳转场景
        Invoke("OnRestartGame", 0.3f);
    }

    // 返回主页面
    public void OnBackToHome() {
        // 播放音效
        AudioClip clip = Resources.Load<AudioClip>("Sounds/Button");
        AudioSource.PlayClipAtPoint(clip, Vector3.zero);
        // 跳转场景
        Invoke("BackToStartScene", 0.3f);
    }

    public void BackToStartScene() {
        SceneManager.LoadScene("StartScene");
    }
    
    public void AddBoardSpawn(BoardSpawn spawn) {
        if (!m_boardSpawnList.Contains(spawn)) {
            m_boardSpawnList.Add(spawn);
        }
    }
    
    public void RemoveBoardSpawn(BoardSpawn spawn) {
        m_boardSpawnList.Remove(spawn);
    }

    public void AddRewardSpawn(RewardSpawn spawn) {
        if (!m_rewardSpawnList.Contains(spawn)) {
            m_rewardSpawnList.Add(spawn);
        }
    }
    
    public void RemoveRewardSpawn(RewardSpawn spawn) {
        m_rewardSpawnList.Remove(spawn);
    }

    // 检测奖励的范围
    public bool CheckRewardRect(Reward targetReward) {
        foreach (RewardSpawn spawn in m_rewardSpawnList) {
            foreach (Reward reward in spawn.GetActiveRewards()) {
                if (reward.IsOverlaps(targetReward)) {
                    return false;
                }
            }
        }
        return true;
    }

    public Transform GetSkillTrans() {
        return m_skillsInfo.GetComponent<Transform>();
    }
    
    public void AddSkill(SkillReward reward) {
        m_skillsInfo.AddSkill(reward);
    }

    public void ShowRankingList() {
        // 播放音效
        AudioClip clip = Resources.Load<AudioClip>("Sounds/Button");
        AudioSource.PlayClipAtPoint(clip, Vector3.zero);
        // 播放动画
        Transform rlTrans = m_result.Find("RankingList");
        Tweener tweener = rlTrans.DOScale(0.2f, 0);
        tweener.OnComplete(() => {
            // 显示排行榜
            RankingList rankingList = rlTrans.GetComponent<RankingList>();
            rankingList.gameObject.SetActive(true);
            rankingList.UpdateRankingList(m_patternType);
            // 显示
            rlTrans.DOScale(1, 0.3f);
        });
    }
    
    public void HideRankingList() {
        // 播放音效
        AudioClip clip = Resources.Load<AudioClip>("Sounds/Button");
        AudioSource.PlayClipAtPoint(clip, Vector3.zero);
        // 隐藏排行榜
        Transform rlTrans = m_result.Find("RankingList");
        Tweener tweener = rlTrans.DOScale(0, 0.3f);
        tweener.OnComplete(() => {
            rlTrans.gameObject.SetActive(false);
        });
    }
}
