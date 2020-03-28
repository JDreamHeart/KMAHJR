using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("GameScripts/ScaleBreathLight")]

public class ScaleBreathLight : MonoBehaviour
{
    public float m_minScale = 0.8f;

    public float m_maxScale = 1;

    float duration = 0;

    Transform m_transform;

    // Start is called before the first frame update
    void Start()
    {
        m_transform = this.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        duration += Time.deltaTime;
        if (duration >= 1) {
            duration = duration - (int) duration;
        }
        float scale = m_minScale + (1 - Mathf.Abs(0.5f - duration) / 0.5f) * (m_maxScale - m_minScale);
        // Debug.Log(string.Format("duration: {0}, scale:{1}", duration, scale));
        m_transform.localScale = new Vector3(scale, scale, 1);
    }
}
