using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]

[AddComponentMenu("GameScripts/EditController")]

public class EditController : MonoBehaviour
{
    public bool m_isActive;

    void Awake() {
        this.gameObject.SetActive(m_isActive);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
