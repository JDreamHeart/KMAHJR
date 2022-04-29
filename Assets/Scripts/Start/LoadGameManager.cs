using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using DG.Tweening;

[AddComponentMenu("GameScripts/LoadGameManager")]

public class LoadGameManager : MonoBehaviour
{
    float m_delayStart = 1.0f;

    GameObject m_sprite;

    Image m_fadeEffectImg;

    Text m_updateTips;

    Slider m_updateProgress;

    Transform m_updateConfirm;
    Text m_updatePkgSize;

    Dictionary<string, bool> m_updatePlatformMap = new Dictionary<string, bool>();

    string m_downloadUrl = "";

    void Awake() {
        GameData.Instance.Load();
        // 自动更新的平台映射表
        m_updatePlatformMap.Add("android", true);
    }

    // Start is called before the first frame update
    void Start() {
        // 精灵主角
        m_sprite = GameObject.FindGameObjectWithTag("Player");
        // 淡出效果
        m_fadeEffectImg = this.transform.Find("FadeEffect").GetComponent<Image>();
        m_fadeEffectImg.gameObject.SetActive(true);
        m_fadeEffectImg.DOFade(0, 0);
        // 更新信息
        Transform updateInfo = this.transform.Find("UpdateInfo");
        m_updateTips = updateInfo.Find("UpdateTips").GetComponent<Text>();
        m_updateProgress = updateInfo.Find("UpdateProgress").GetComponent<Slider>();
        m_updateConfirm = this.transform.Find("UpdateConfirm");
        m_updateConfirm.DOScale(0, 0);
        m_updatePkgSize = m_updateConfirm.Find("Size").GetComponent<Text>();
        // 开始游戏场景
        StartCoroutine(startScene());
    }

    IEnumerator startScene() {
        m_updateTips.text = "...";
        m_updateProgress.value = 1;
        yield return new WaitForSeconds(m_delayStart);
        m_fadeEffectImg.DOFade(0.8f, 0.3f).OnComplete(() => {
            SceneManager.LoadScene("StartScene"); // 启动开始游戏场景
        });
    }

}
