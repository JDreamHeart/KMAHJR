using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DoubleChromosphereMgr : MonoBehaviour
{
    int ResultCount = 5; // 生成结果数量

    List<DoubleChromosphereBox> m_boxList = new List<DoubleChromosphereBox>();

    Button m_rollBtn;

    void Awake() {
        Transform resultList = this.transform.Find("Result/List");
        for (int i = 0; i < ResultCount; i++) {
            m_boxList.Add(resultList.Find(string.Format("Result{0}/Box", i+1)).GetComponent<DoubleChromosphereBox>());
        }

        m_rollBtn = this.transform.Find("RollButton").GetComponent<Button>();
        m_rollBtn.onClick.AddListener(this.onRollBoxes);
    }

    void onRollBoxes() {
        // 播放音效
        AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/Button"), Vector3.zero);

        // System.DateTime t = System.DateTime.UtcNow;
        // System.DateTime zeroT = new System.DateTime(t.Year, t.Month, t.Day);
        // UnityEngine.Random.InitState((int) (zeroT - new System.DateTime(1994, 1, 1)).TotalSeconds);

        foreach (DoubleChromosphereBox box in m_boxList) {
            box.RollBallNum();
        }
        m_rollBtn.interactable = false;
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
    
}
