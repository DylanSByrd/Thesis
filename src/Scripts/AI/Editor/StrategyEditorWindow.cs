//-----------------------------------------------------------------------------------------------
using UnityEngine;
using UnityEditor;
using HTN;
using Influence;
using System;
using System.Collections.Generic;


//-----------------------------------------------------------------------------------------------
[Serializable]
public class StrategyEditorWindow : EditorWindow
{
   //-----------------------------------------------------------------------------------------------
   public eStrategy ActiveStrategy
   {
      get { return m_strategyPlanner.ActiveStrategy; }
   }


   //-----------------------------------------------------------------------------------------------
   private StrategyPlanner m_strategyPlanner;
   private AIManager m_aiManager;
   private Vector2 m_planScrollPos = Vector2.zero;
   private Vector2 m_agentScrollPos = Vector2.zero;
   private bool m_isShowingWorldState;
   private Button[] m_buttons;
   private InfluenceSystem m_influenceSystemRef;
   private InfluenceGameManager m_influenceGameManager;

   [SerializeField]
   private StrategyEditorRestorePoint m_restorePoint;


   //-----------------------------------------------------------------------------------------------
   [MenuItem ("Strategy/Strategy Debugger")]
   public static void ShowWindow()
   {
      GetWindow(typeof(StrategyEditorWindow), false, "Strategy Debugger");
   }


   //-----------------------------------------------------------------------------------------------
   public void OnEnable()
   {
      SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
      SceneView.onSceneGUIDelegate += this.OnSceneGUI;

      GetAllReferencesAsNeeded();
   }


   //-----------------------------------------------------------------------------------------------
   public void OnDestroy()
   {
      SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
   }


   //-----------------------------------------------------------------------------------------------
   public void OnSceneGUI(SceneView sceneView)
   {
      DrawAgentLabels();
   }
   
   
   //-----------------------------------------------------------------------------------------------
   public void Awake()
   {
      GetAllReferencesAsNeeded();
   }


   //-----------------------------------------------------------------------------------------------
   public void OnProjectChange()
   {
      GetAllReferencesAsNeeded();
   }


   //-----------------------------------------------------------------------------------------------
   public void OnSelectionChange()
   {
      GetAllReferencesAsNeeded();
   }


   //-----------------------------------------------------------------------------------------------
   public void Update()
   {
      GetAllReferencesAsNeeded();
      m_strategyPlanner.AnalyzeWorldState();

      if (DoesNeedRefresh())
      {
         m_strategyPlanner.Reload();
      }
   }


   //-----------------------------------------------------------------------------------------------
   public void OnFocus()
   {
      GetAllReferencesAsNeeded();
   }


   //-----------------------------------------------------------------------------------------------
   private void GetAllReferencesAsNeeded()
   {
      if (m_strategyPlanner == null)
      {
         m_strategyPlanner = FindObjectOfType<StrategyPlanner>();
         m_strategyPlanner.Awake();
         m_strategyPlanner.Start();
      }

      if (m_aiManager == null)
      {
         m_aiManager = FindObjectOfType<AIManager>();
         m_aiManager.Awake();
      }

      m_buttons = FindObjectsOfType<Button>();

      if (m_influenceGameManager == null)
      {
         m_influenceGameManager = FindObjectOfType<InfluenceGameManager>();
      }

      if (m_influenceSystemRef == null)
      {
         m_influenceSystemRef = InfluenceSystem.GetInstance();
      }

      m_influenceGameManager.ReloadInfluenceMapData();
      m_influenceGameManager.RegisterAllActiveInfluenceObjectsInScene();
      m_influenceGameManager.UpdateInfluenceSystem();
   }


   //-----------------------------------------------------------------------------------------------
   public void OnGUI()
   {
      if (m_strategyPlanner == null)
      {
         EditorGUILayout.LabelField("No strategy planner in scene!");
      }

      EditorGUILayout.BeginHorizontal();
      {
         if (GUILayout.Button("Execute Next Agent")
            && m_strategyPlanner.ActivePlan.TaskList.Count != 0)
         {
            ExecuteNextTaskForNextAgent();
         }

         if (GUILayout.Button("Execute All Agents")
            && m_strategyPlanner.ActivePlan.TaskList.Count != 0)
         {
            ExecuteNextTaskForAllAgents();
         }

         if (GUILayout.Button("Replan"))
         {
            ClearCurrentAndMakeNewPlan();
         }

         if (GUILayout.Button("Reload Domain and Replan"))
         {
            ReloadDomainAndMakeNewPlan();
         }
      }
      EditorGUILayout.EndHorizontal();

      EditorGUILayout.BeginHorizontal();
      {
         //if (m_restorePoint == null)
         //{
         //   EditorGUILayout.LabelField("No Restore Point Present!");
         //}
         //else
         //{
         //   EditorGUILayout.LabelField("Restore Point Present");
         //}

         //if (GUILayout.Button("Create Restore Point"))
         //{
         //   CreateRestorePoint();
         //}

         //if (GUILayout.Button("Revert To Restore Point"))
         //{
         //   RevertToRestorePoint();
         //}
      }
      EditorGUILayout.EndHorizontal();

      EditorGUILayout.BeginHorizontal();
      {
         if (m_restorePoint != null)
         {
            EditorGUILayout.LabelField("Restore Point Saved");
         }
         else
         {
            EditorGUILayout.LabelField("No Restore Point Present");
         }
         
         if (GUILayout.Button("Create Restore Point"))
         {
            CreateRestorePoint();
         }

         if (m_restorePoint != null)
         {
            if (GUILayout.Button("Revert To Restore Point"))
            {
               RevertToRestorePoint();
            }
         }
      }
      EditorGUILayout.EndHorizontal();

      eStrategy stratOption = (eStrategy)EditorGUILayout.EnumPopup("Active Strategy", m_strategyPlanner.ActiveStrategy);
      if (stratOption != m_strategyPlanner.ActiveStrategy)
      {
         m_strategyPlanner.ActiveStrategy = stratOption;
         ClearCurrentAndMakeNewPlan();
      }

      DrawWorldState();
      DrawPlan();
      DrawAgentList();
   }


   //-----------------------------------------------------------------------------------------------
   private void DrawPlan()
   {
      EditorGUILayout.LabelField("Current Plan", EditorStyles.boldLabel);
      m_planScrollPos = EditorGUILayout.BeginScrollView(m_planScrollPos, EditorStyles.textArea);

      Plan activePlan = m_strategyPlanner.ActivePlan;
      if (activePlan == null
         || activePlan.TaskList.Count == 0)
      {
         GUIStyle boldLabelStyle = new GUIStyle(EditorStyles.boldLabel);
         boldLabelStyle.padding = new RectOffset(0, 0, 0, 0);
         EditorGUILayout.LabelField("---Empty---", boldLabelStyle);
      }
      else
      {
         if (m_strategyPlanner.SyncIndex == 0)
         {
            EditorGUILayout.LabelField("All Agents Synced");
         }
         else
         {
            EditorGUILayout.LabelField("Current Sync Index: " + m_strategyPlanner.SyncIndex);
            EditorGUILayout.LabelField("Number of Synced Agents: " + m_strategyPlanner.SyncCount);
         }

         for (int taskIndex = 0; taskIndex < activePlan.TaskList.Count; ++taskIndex)
         {
            PrimitiveTask task = activePlan.TaskList[taskIndex];
            DrawTask(task, taskIndex + 1);
         }
      }
      EditorGUILayout.EndScrollView();

   }


   //-----------------------------------------------------------------------------------------------
   private void DrawTask(PrimitiveTask task, int taskIndex)
   {
      EditorGUILayout.BeginHorizontal();
      {
         if (task.IsClaimed)
         {
            EditorGUILayout.LabelField("(Claimed) " + taskIndex + ". " + task.Name);
         }
         else
         {
            EditorGUILayout.LabelField(taskIndex + ". " + task.Name);
         }

         // Task info
         EditorGUILayout.BeginVertical();
         {
            // Mod info
            uint modMask = task.ModifierMask;
            if ((modMask & (byte)Task.eModifier.BLOCKING_MODIFIER) != 0)
            {
               EditorGUILayout.LabelField("Mod: Blocking");
            }

            if ((modMask & (byte)Task.eModifier.SYNC_MODIFIER) != 0)
            {
               EditorGUILayout.LabelField("Mod: Sync");
            }

            if ((modMask & (byte)Task.eModifier.RESERVABLE_MODIFIER) != 0)
            {
               EditorGUILayout.LabelField("Mod: Reservable");
            }

            // Param info
            List<OperatorParam> opParams = task.Op.Params;
            foreach (OperatorParam opParam in opParams)
            {
               EditorGUILayout.LabelField("Op Param: " + opParam.Name + " == " + opParam.Value);
            }


            EditorGUILayout.LabelField("");
         }
         EditorGUILayout.EndVertical();
      }
      EditorGUILayout.EndHorizontal();
   }


   //-----------------------------------------------------------------------------------------------
   private void DrawAgentList()
   {
      EditorGUILayout.LabelField("Current Agent Plans", EditorStyles.boldLabel);

      List<Agent> agents = m_aiManager.ActiveAgents;
      EditorGUILayout.LabelField("Next Agent to Execute: " + agents[m_aiManager.NextAgentToExecuteIndex].name);

      m_agentScrollPos = EditorGUILayout.BeginScrollView(m_agentScrollPos, EditorStyles.textArea);
      if (agents == null
         || agents.Count == 0)
      {
         EditorGUILayout.LabelField("---None---", EditorStyles.boldLabel);
      }
      else
      {
         for (int agentIndex = 0; agentIndex < agents.Count; ++agentIndex)
         {
            Agent agent = agents[agentIndex];
            DrawAgent(agent, agentIndex + 1);
         }
      }
      EditorGUILayout.EndScrollView();
   }


   //-----------------------------------------------------------------------------------------------
   private void DrawAgent(Agent agent, int agentIndex)
   {
      EditorGUILayout.BeginHorizontal();
      {
         EditorGUILayout.LabelField(agent.gameObject.name);
         EditorGUILayout.LabelField("Next Task Index: " + agent.CurrentPlanIndex);
      }
      EditorGUILayout.EndHorizontal();


      EditorGUILayout.BeginHorizontal();
      {
         EditorGUILayout.LabelField("");

         // Task info
         EditorGUILayout.BeginVertical();
         {
            PlanRunner agentPlanRunner = agent.ActivePlanRunner;
            Plan agentPlan = agentPlanRunner.PlanToExecute;

            if (agentPlan != null
               && agentPlan.TaskList.Count != 0)
            {
               int planRunnerIndex = agentPlanRunner.CurrentPlanIndex;

               for (int taskIndex = 0; taskIndex < agentPlan.TaskList.Count; ++taskIndex)
               {
                  PrimitiveTask task = agentPlan.TaskList[taskIndex];
                  if (taskIndex == planRunnerIndex)
                  {
                     EditorGUILayout.LabelField("(Next) " + (taskIndex + 1) + ". " + task.Name);
                  }
                  else
                  {
                     EditorGUILayout.LabelField((taskIndex + 1) + ". " + task.Name);
                  }
               }
            }
            else
            {
               EditorGUILayout.LabelField("(Needs plan)");
            }
            EditorGUILayout.LabelField("");
         }
         EditorGUILayout.EndVertical();

         EditorGUILayout.EndHorizontal();
      }
   }


   //-----------------------------------------------------------------------------------------------
   private void DrawWorldState()
   {
      m_isShowingWorldState = EditorGUILayout.Foldout(m_isShowingWorldState, "Current World State");

      if (m_isShowingWorldState)
      {
         WorldState worldState = m_strategyPlanner.CurrentWorldState;
         WorldProperties properties = worldState.Properties;

         foreach (KeyValuePair<string, byte> property in properties)
         {
            GUIStyle style = new GUIStyle(EditorStyles.label);
            style.padding = new RectOffset(25, 0, 0, 0);
            EditorGUILayout.LabelField(property.Key + ": " + property.Value, style);
         }
      }
   }


   //-----------------------------------------------------------------------------------------------
   private void DrawAgentLabels()
   {
      // Draw labels
      Handles.BeginGUI();
      {
         // Draw agent labels
         List<Agent> agents = m_aiManager.ActiveAgents;
         if (agents == null
         || agents.Count == 0)
         {
            Handles.EndGUI();
            return;
         }

         for (int agentIndex = 0; agentIndex < agents.Count; ++agentIndex)
         {
            Agent agent = agents[agentIndex];
            Vector2 guiPoint = HandleUtility.WorldToGUIPoint(agent.transform.position);
            Rect rect = new Rect(guiPoint.x - 50f, guiPoint.y - 40f, 100, 20);
            GUI.Box(rect, "Agent " + agentIndex);
         }

      }
      Handles.EndGUI();
   }


   //-----------------------------------------------------------------------------------------------
   public bool DoesNeedRefresh()
   {
      return m_strategyPlanner.DoesNeedRefresh();
   }


   //-----------------------------------------------------------------------------------------------
   private void CreateRestorePoint()
   {
      m_restorePoint = CreateInstance<StrategyEditorRestorePoint>();

      List<Agent> agents = m_aiManager.ActiveAgents;
      List<AgentRestoreState> agentStates = new List<AgentRestoreState>();
      foreach (Agent agent in agents)
      {
         AgentRestoreState state;
         state.m_agent = agent;
         state.m_positionToRestoreTo = agent.transform.position;
         agentStates.Add(state);
      }
      m_restorePoint.m_agentStatesToRestoreTo = agentStates;
      m_restorePoint.m_buttonsToReset = m_buttons;
   }


   //-----------------------------------------------------------------------------------------------
   private void RevertToRestorePoint()
   {
      m_restorePoint.Restore();

      m_strategyPlanner.ClearPlan();
      m_aiManager.ClearAgentPlanHistories();
      m_influenceGameManager.UpdateInfluenceSystem();
   }


   //-----------------------------------------------------------------------------------------------
   private void ClearButtonStates()
   {
      if (m_buttons == null)
      {
         return;
      }

      foreach (Button button in m_buttons)
      {
         button.ClearClaim();
      }
   }


   //-----------------------------------------------------------------------------------------------
   private void ClearCurrentAndMakeNewPlan()
   {
      GetAllReferencesAsNeeded();
      MakeNewPlan();
   }


   //-----------------------------------------------------------------------------------------------
   private void ReloadDomainAndMakeNewPlan()
   {
      GetAllReferencesAsNeeded();
      m_strategyPlanner.Reload();
      MakeNewPlan();
   }


   //-----------------------------------------------------------------------------------------------
   private void MakeNewPlan()
   {
      ClearButtonStates();
      m_strategyPlanner.StopSync();
      m_influenceGameManager.UpdateInfluenceSystem();
      m_strategyPlanner.GeneratePlan();
      m_aiManager.ClearAgentPlanHistories();
      m_aiManager.RequestPlansForAllAgents();
   }


   //-----------------------------------------------------------------------------------------------
   private void ExecuteNextTaskForAllAgents()
   {
      m_influenceGameManager.UpdateInfluenceSystem();
      m_aiManager.ExecuteNextTaskForAllAgents();
   }


   //-----------------------------------------------------------------------------------------------
   private void ExecuteNextTaskForNextAgent()
   {
      m_influenceGameManager.UpdateInfluenceSystem();
      m_aiManager.ExecuteNextTaskForNextAgent();
   }

}
