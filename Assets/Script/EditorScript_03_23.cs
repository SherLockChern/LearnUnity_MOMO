using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorScript_03_23 : MonoBehaviour
{
    public Vector3 ScrollPos;
    public int myId;

    public string myName;

    public GameObject Prefab;

    public MyEnum myEnum = MyEnum.One;
    public bool Toogle1;
    public bool Toogle2;
    
    public enum MyEnum
    {
        One = 1,
        Two,
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

#if UNITY_EDITOR
[CustomEditor(typeof(EditorScript_03_23))]
public class ScriptEditor_03_23 : Editor
{
    private bool m_EnableToole;

    public override void OnInspectorGUI()
    {
        EditorScript_03_23 st = target as EditorScript_03_23;
        st.ScrollPos = EditorGUILayout.BeginScrollView(st.ScrollPos, false, true);
        st.myName = EditorGUILayout.TextField("text", st.myName);
        st.myId = EditorGUILayout.IntField("int", st.myId);
        st.Prefab = EditorGUILayout.ObjectField("GameObject", st.Prefab, typeof(GameObject), true) as GameObject;
            
        EditorGUILayout.BeginHorizontal();
        GUILayout.Button("1");
        GUILayout.Button("2");
        st.myEnum = (EditorScript_03_23.MyEnum)EditorGUILayout.EnumPopup("MyEnum: ", st.myEnum);
        EditorGUILayout.EndHorizontal();
        m_EnableToole = EditorGUILayout.BeginToggleGroup("EnableToogle", m_EnableToole);
        st.Toogle1 = EditorGUILayout.Toggle("toogle1", st.Toogle1);
        st.Toogle2 = EditorGUILayout.Toggle("toogle2", st.Toogle2);
        EditorGUILayout.EndToggleGroup();
            
        EditorGUILayout.EndScrollView();

    }
        
}
#endif
