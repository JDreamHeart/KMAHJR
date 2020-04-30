using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
        Debug.Log(Application.persistentDataPath);
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
        // 校验版本
        StartCoroutine(verifyApkVersion());
    }

    // Update is called once per frame
    void Update() {
        
    }

    IEnumerator startScene() {
        m_updateTips.text = "版本更新检测完毕，即将打开游戏...";
        m_updateProgress.value = 1;
        yield return new WaitForSeconds(m_delayStart);
        m_fadeEffectImg.DOFade(0.8f, 0.3f).OnComplete(() => {
            SceneManager.LoadScene("StartScene"); // 启动开始游戏场景
        });
    }

    string getPlatform() {
        switch(Application.platform) {
            case RuntimePlatform.Android:
                return "android";
            case RuntimePlatform.IPhonePlayer:
                return "ios";
            case RuntimePlatform.WindowsPlayer:
                return "win";
        }
        return "";
    }

    // 校验并更新安卓包版本
    IEnumerator verifyApkVersion() {
        // 请求更新信息
        WWW www = new WWW("https://jdreamheart.com/game?k=detail&gid=3&gameinfo=version&platform="+getPlatform());
        yield return www;
        if (www.error != null) {
            Debug.Log(www.error);
        } else {
            string sizeStr = "(未知大小)";
            string url = getDownloadUrl(www.text, ref sizeStr);
            if (url != "" && m_updatePlatformMap.ContainsKey(getPlatform())) {
                new System.Uri(url); // 转换中文
                m_downloadUrl = url;
                m_updateConfirm.DOScale(1, 0.3f);
                m_updatePkgSize.text = string.Format("({0})", sizeStr);
                yield break;
            }
        }
        StartCoroutine(startScene()); // 开始游戏场景
    }

    // 下载并安装更新包
    IEnumerator downloadPkg() {
        // 检测当前平台是否自动更新
        if (m_downloadUrl != "") {
            m_updateTips.text = "开始下载更新包...";
            m_updateProgress.value = 0;
            WWW pkgWww = new WWW(m_downloadUrl);
            while (!pkgWww.isDone) {
                m_updateTips.text = string.Format("正在下载更新包{0} [{1:F2}%]...", m_updatePkgSize.text, pkgWww.progress * 100);
                m_updateProgress.value = pkgWww.progress;
                yield return null;
            }
            if (pkgWww.error == null) {
                m_updateTips.text = "更新包下载完成";
                m_updateProgress.value = 1;
                #if UNITY_ANDROID
                    if (tryToInstallApk(pkgWww, "KMAHJR")) {
                        yield break;
                    }
                #endif
            } else {
                m_updateTips.text = "更新包下载失败";
                Debug.Log(pkgWww.error);
            }
        }
        StartCoroutine(startScene()); // 开始游戏场景
    }

    // 获取下载路径
    string getDownloadUrl(string gameInfo, ref string sizeStr) {
        try {
            var dict = MiniJSON.Json.Deserialize(gameInfo) as Dictionary<string, object>;
            if (dict.ContainsKey("version") && dict.ContainsKey("url") && GameData.Instance.VerifyVersion(dict["version"].ToString())) {
                if (dict.ContainsKey("size")) {
                    float size = float.Parse(dict["size"].ToString());
                    sizeStr = string.Format("{0:F2}M", size / 1024 / 1024);
                }
                return dict["url"].ToString();
            }
        } catch (System.Exception e) {
            Debug.Log(string.Format("Error to verify version -> {0}", e));
        }
        return "";
    }

    bool tryToInstallApk(WWW www, string apkName) {
        try {
            //将apk写入沙盒目录
            string path = Application.persistentDataPath + "/" + apkName + ".apk";
            Debug.Log(string.Format("Install apk path: {0}", path));
            File.WriteAllBytes(path, www.bytes);
            // 调用android的安装
            using(AndroidJavaClass cl = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
                using(AndroidJavaObject ob = cl.GetStatic<AndroidJavaObject>("currentActivity")) {
                    return ob.Call<bool>("InstallApk", path);
                }
            }
        } catch (System.Exception e) {
            Debug.Log(string.Format("Error to install apk -> {0}", e));
        }
        return false;
    }
    
    bool tryToOpenBrowser(string apkUrl) {
        try {
            // 调用android的安装
            using(AndroidJavaClass cl = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
                using(AndroidJavaObject ob = cl.GetStatic<AndroidJavaObject>("currentActivity")) {
                    return ob.Call<bool>("OpenBrowser", apkUrl);
                }
            }
        } catch (System.Exception e) {
            Debug.Log(string.Format("Error to open browser -> {0}", e));
        }
        return false;
    }

    public void OnCancelUpdate() {
        m_updateConfirm.DOScale(0, 0);
        StartCoroutine(startScene()); // 开始游戏场景
    }
    
    public void OnOkUpdate() {
        // m_updateConfirm.DOScale(0, 0);
        // StartCoroutine(downloadPkg()); // 下载并安装更新包

        // 打开浏览器下载或直接开始游戏
        if (m_downloadUrl != "") {
            #if UNITY_ANDROID
                tryToOpenBrowser(m_downloadUrl);
                return;
            #endif
        }
        StartCoroutine(startScene()); // 开始游戏场景
    }
}
