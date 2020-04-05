using System.Collections;
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

    bool m_isPlaying; // 是否正在游戏中

    Sprite m_sprite; // 主角精灵

    // 分数
    int m_score;
    Text m_scoreText;

    // 箭头
    Transform m_arrows;

    // 游戏结果
    public float m_showResultDelay = 1; // 显示结果的延时
    float m_curShowResultDelay = 0; // 当前显示结果的延时
    Transform m_result;

    // 奖励生成器列表
    List<RewardSpawn> m_rewardSpawnList = new List<RewardSpawn>();

    // 技能
    SkillsInfo m_skillsInfo;

    void Awake() {
        Instance = this;
        m_sprite = GameObject.FindGameObjectWithTag("Player").GetComponent<Sprite>();
        m_arrows = this.transform.Find("Arrows");
        // 给箭头添加震动动画
        Tweener leftTweener = m_arrows.Find("LeftArrow").DOShakeRotation(10, 2);
        leftTweener.SetLoops(-1);
        Tweener rightTweener = m_arrows.Find("RightArrow").DOShakeRotation(10, 2);
        rightTweener.SetLoops(-1);
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
        if (m_arrows != null) {
            m_arrows.gameObject.SetActive(false);
        }
        m_sprite.OnStartGame();
        m_curShowResultDelay = 0;
    }
    
    public void OnStopGame() {
        m_isPlaying = false;
        Transform resultContent = m_result.Find("ResultContent");
        Transform curScoreVal = resultContent.Find("CurrentScore").Find("ScoreValue");
        curScoreVal.GetComponent<Text>().text = m_score.ToString();
        Transform maxScoreVal = resultContent.Find("RankingList").Find("MaxScoreValue");
        maxScoreVal.GetComponent<Text>().text = m_score.ToString();
        m_result.gameObject.SetActive(true);
    }

    public void OnRestartGame() {
        m_isPlaying = false;
        ResetScore();
        if (m_arrows != null) {
            m_arrows.gameObject.SetActive(true);
        }
        m_sprite.OnRestartGame();
        m_result.gameObject.SetActive(false);
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

    // 返回主页面
    public void OnBackToHome() {
        SceneManager.LoadScene("StartScene");
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
}
