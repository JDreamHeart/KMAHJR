using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("GameScripts/SkillsInfo")]

public class SkillsInfo : MonoBehaviour
{
    List<Skill> m_skillList = new List<Skill>();
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public Transform AddSkill(GameObject obj) {
        GameObject newObj = Instantiate(obj, this.transform) as GameObject;
        m_skillList.Add(newObj.AddComponent<Skill>());
        RectTransform rt = newObj.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(64, 64);
        return rt;
    }
}
