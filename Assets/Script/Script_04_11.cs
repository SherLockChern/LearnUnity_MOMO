using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_04_11 : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField] private List<Sprite> m_values = new List<Sprite>();
    [SerializeField] private List<string> m_keys = new List<string>();
    public Dictionary<string, Sprite> spriteDic = new Dictionary<string, Sprite>();

    public void OnBeforeSerialize()
    {
        
        m_keys.Clear();
        m_values.Clear();
        foreach (var pair in spriteDic)
        {
            m_keys.Add(pair.Key);
            m_values.Add(pair.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        spriteDic.Clear();
        if (m_keys.Count != m_values.Count)
        {
            Debug.LogError("长度不匹配");
        }
        else
        {
            for (int i = 0; i < m_keys.Count; i++)
            {
                spriteDic[m_keys[i]] = m_values[i];
            }
        }
    }
}
