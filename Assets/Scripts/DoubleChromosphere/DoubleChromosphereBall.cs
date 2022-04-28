using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoubleChromosphereBall : MonoBehaviour
{
    Text m_numText;

    int m_num;
    public int num {
        get {
            return m_num;
        }
        set {
            m_num = value;
            if (m_numText != null) {
                m_numText.text = string.Format("{0}", m_num);
            }
        }
    }

    void Awake() {
        m_numText = this.transform.Find("Text").GetComponent<Text>();
        m_numText.text = string.Format("{0}", m_num);
    }
}
