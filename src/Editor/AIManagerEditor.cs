using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;


//-----------------------------------------------------------------------------------------------
[CustomEditor(typeof(AIManager))]
public class AIManagerEditor : Editor
{
   //-----------------------------------------------------------------------------------------------
   private AIManager m_selectedManager;


   //-----------------------------------------------------------------------------------------------
   public void OnEnable()
   {
      m_selectedManager = this.target as AIManager;
   }


   //-----------------------------------------------------------------------------------------------
   public void OnDisable()
   {
   }


   //-----------------------------------------------------------------------------------------------
   public override void OnInspectorGUI()
   {
      m_selectedManager.m_agentPrefab = (GameObject)EditorGUILayout.ObjectField("Agent Prefab", m_selectedManager.m_agentPrefab, typeof(GameObject), false);

      // Active agent listing
      GUILayout.Label("Active Agents", EditorStyles.boldLabel);
      List<Agent> activeAgents = m_selectedManager.ActiveAgents;
      for (int agentIndex = 0; agentIndex < activeAgents.Count; ++agentIndex)
      {
         GUILayout.BeginHorizontal();
         GUILayout.Label(agentIndex.ToString());

         if (GUILayout.Button("-", GUILayout.Width(20f)))
         {
            m_selectedManager.RemoveAgentAtIndex(agentIndex);
         }
         else
         {
            EditorGUILayout.ObjectField(activeAgents[agentIndex], typeof(Agent), true);
         }

         GUILayout.EndHorizontal();
      }

      // Proper way to add agents
      if (GUILayout.Button("Add Agent"))
      {
         m_selectedManager.AddAgent();
      }

      // If anything changed, we redraw the scene
      if (GUI.changed)
      {
         SceneView.RepaintAll();
      }

      // Useful to just have around while developing
      // Feel free to comment out anything below this
      DrawDefaultInspector();
   }


   //-----------------------------------------------------------------------------------------------
   public void OnSceneGUI()
   {
      // Save previous state
      Color oldColor = Handles.color;

      Color outerColor = new Color(1f, 0.92f, 0.016f, 1f);
      Color innerColor = new Color(1f, 0.92f, 0.016f, 0.3f);

      // Draw goals
      Handles.color = innerColor;
      Handles.DrawSolidDisc(new Vector3(1,0,0), m_selectedManager.transform.forward, .5f);

      Handles.color = outerColor;
      Handles.DrawWireDisc(new Vector3(1, 0, 0), m_selectedManager.transform.forward, .5f);
      
      // Draw target points
      List<Agent> activeAgents = m_selectedManager.ActiveAgents;
      for (int agentIndex = 0; agentIndex < activeAgents.Count; ++agentIndex)
      {
         Agent agent = activeAgents[agentIndex];

         if (agent == null)
         {
            continue;
         }



         // Draw labels
         Handles.BeginGUI();
         
         // Draw agent labels
         Vector2 guiPoint = HandleUtility.WorldToGUIPoint(agent.transform.position);
         Rect rect = new Rect(guiPoint.x - 50f, guiPoint.y - 40f, 100, 20);
         GUI.Box(rect, "Agent " + agentIndex);

         Handles.EndGUI();
      }

      // Restore old state
      Handles.color = oldColor;
   }
}
