using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("GameScripts/StartPatternItem")]

public class PatternItem : MonoBehaviour
{
    public string m_name;
    public PatternType m_type;
    public string m_description;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string GetName() {
        return m_name;
    }
    
    public PatternType GetPatternType() {
        return m_type;
    }
}
