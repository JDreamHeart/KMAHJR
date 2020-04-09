using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [ExecuteInEditMode]

[AddComponentMenu("GameScripts/ActiveInEditor")]

public class ActiveInEditor : MonoBehaviour
{
    public bool m_isActive;

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.SetActive(m_isActive);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
