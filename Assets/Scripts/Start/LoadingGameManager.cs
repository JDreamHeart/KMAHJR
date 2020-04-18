using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

[AddComponentMenu("GameScripts/LoadingGameManager")]

public class LoadingGameManager : MonoBehaviour
{
    public float m_delay = 1;
    float m_duration = 0;

    GameObject m_sprite;

    Image m_fadeEffectImg;

    void Awake() {
        GameData.Instance.Load();
    }

    // Start is called before the first frame update
    void Start() {
        // 精灵主角
        m_sprite = GameObject.FindGameObjectWithTag("Player");
        // 淡出效果
        m_fadeEffectImg = this.transform.Find("FadeEffect").GetComponent<Image>();
        m_fadeEffectImg.gameObject.SetActive(true);
        m_fadeEffectImg.DOFade(0, 0);
    }

    // Update is called once per frame
    void Update() {
        m_duration += Time.deltaTime;
        if (m_duration > m_delay) {
            m_sprite.SetActive(false);
            m_fadeEffectImg.DOFade(1, 0.8f).OnComplete(() => {
                SceneManager.LoadScene("StartScene"); // 启动开始游戏场景
            });
        }
    }
}
