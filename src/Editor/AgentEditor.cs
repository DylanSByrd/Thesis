using UnityEngine;
using System.Collections;
using UnityEditor;


//-----------------------------------------------------------------------------------------------
[CustomEditor(typeof(Agent))]
public class AgentEditor : Editor
{
    //-----------------------------------------------------------------------------------------------
    private Agent m_selectedAgent;


   //-----------------------------------------------------------------------------------------------
    public void OnEnable()
    {
        m_selectedAgent = this.target as Agent;
    }


    //-----------------------------------------------------------------------------------------------
    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Agent ID: " + m_selectedAgent.AgentID);

        GUILayout.Label("Agent Plan", EditorStyles.boldLabel);
    }


    //-----------------------------------------------------------------------------------------------
    public void OnDisable()
    {
    }


    //-----------------------------------------------------------------------------------------------
    public void OnSceneGUI()
    {
    }


    //-----------------------------------------------------------------------------------------------
    private void UpdateInfluenceOverlay()
    {
    }
}
