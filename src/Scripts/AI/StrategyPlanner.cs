using UnityEngine;
using HTN;


//-----------------------------------------------------------------------------------------------
public enum eStrategy : byte
{
   AMBUSH_STRATEGY,
   FLANK_STRATEGY,
}


//-----------------------------------------------------------------------------------------------
public struct AgentPlanRequest
{
   //-----------------------------------------------------------------------------------------------
   public Agent m_agent;
   public int m_nextTaskIndex;

   //-----------------------------------------------------------------------------------------------
   public AgentPlanRequest(Agent agent, int nextTaskIndex)
   {
      m_agent = agent;
      m_nextTaskIndex = nextTaskIndex;
   }
}


//-----------------------------------------------------------------------------------------------
[ExecuteInEditMode]
public class StrategyPlanner : MonoBehaviour
{
   //-----------------------------------------------------------------------------------------------
   public eStrategy ActiveStrategy
   {
      get { return m_activeStrategy; }
      set
      {
         m_activeStrategy = value;
         AnalyzeWorldState();
      }
   }

   public Plan ActivePlan
   {
      get { return m_activeStrategyPlan; }
   }

   public int SyncIndex
   {
      get { return m_syncIndex; }
   }

   public int SyncCount
   {
      get { return m_syncCount; }
   }

   public WorldState CurrentWorldState
   {
      get { return m_htnPlanner.CurrentWorldState; }
   }


   //-----------------------------------------------------------------------------------------------
   [SerializeField]
   private eStrategy m_activeStrategy;
   private Planner m_htnPlanner = new Planner();
   private Domain m_htnDomain = new Domain();
   private Plan m_activeStrategyPlan = new Plan();
   private AIManager m_aiManager;
   private bool m_isSyncing;
   private int m_numActiveAgents;
   private int m_syncCount;
   private int m_syncIndex;

      
   //-----------------------------------------------------------------------------------------------
   public void Awake()
   {
      InitializeManager();
   }


   //-----------------------------------------------------------------------------------------------
   public void Start()
   {
      AnalyzeWorldState();
      GeneratePlan();
   }


   //-----------------------------------------------------------------------------------------------
   private void InitializeManager()
   {
      RegisterOperators();
      RegisterWorldStateProperties();
      PopulateDomain();
      AssignDomainToPlanner();
      GetNumActiveAgents();
   }


   //-----------------------------------------------------------------------------------------------
   private void RegisterOperators()
   {
      m_htnDomain.TryRegisterOperator("MoveToInfluenceOperator", new MoveToInfluenceOperator());
      m_htnDomain.TryRegisterOperator("MoveToButtonOperator", new MoveToButtonOperator());
      m_htnDomain.TryRegisterOperator("ToggleButtonOperator", new ToggleButtonOperator());
   }


   //-----------------------------------------------------------------------------------------------
   private void RegisterWorldStateProperties()
   {
      WorldState ws = m_htnPlanner.CurrentWorldState;

      ws.RegisterProperty("Strategy");
      ws.RegisterProperty("Num_Buttons");
      ws.RegisterProperty("Does_Choke_Point_Exist");
   }


   //-----------------------------------------------------------------------------------------------
   private void PopulateDomain()
   {
      DomainXmlLoader.LoadDomain(m_htnDomain);
   }


   //-----------------------------------------------------------------------------------------------
   private void AssignDomainToPlanner()
   {
      m_htnPlanner.PlannerDomain = m_htnDomain;
   }


   //-----------------------------------------------------------------------------------------------
   public void GeneratePlan()
   {
      if (m_htnPlanner.PlannerDomain == null)
      {
         AssignDomainToPlanner();
      }

      bool planSuccess = m_htnPlanner.CreatePlan();

      if (!planSuccess)
      {
         Debug.Log("Plan generation failed.");
      }

      m_activeStrategyPlan = m_htnPlanner.LastCreatedPlan;
   }


   //-----------------------------------------------------------------------------------------------
   private void GetNumActiveAgents()
   {
      m_aiManager = FindObjectOfType<AIManager>();
      m_numActiveAgents = m_aiManager.ActiveAgents.Count;
   }


   //-----------------------------------------------------------------------------------------------
   public Plan HandleAgentPlanRequest(AgentPlanRequest request)
   {
      int currentIndex = request.m_nextTaskIndex;

      // plan finished
      if (IsPlanFinished(currentIndex))
      {
         return null;
      }

      // Can't get a new plan if syncing
      if (m_isSyncing && (currentIndex > m_syncIndex))
      {
         return null;
      }

      Plan agentPlan = new Plan();
      bool canGetNewTask = true;
      while (canGetNewTask)
      {
         PrimitiveTask taskToGet = m_activeStrategyPlan.TaskList[currentIndex];
         if (taskToGet.IsClaimed)
         {
            ++currentIndex;
            continue;
         }

         // Handle modifiers on task
         uint taskMods = taskToGet.ModifierMask;

         // Blocking tasks mean we can't get more tasks until we're done
         if ((taskMods & (byte)Task.eModifier.BLOCKING_MODIFIER) != 0)
         {
            canGetNewTask = false;
         }

         // Sync tasks require everyone to reach the task before continuing
         if ((taskMods &(byte)Task.eModifier.SYNC_MODIFIER) != 0)
         {
            m_isSyncing = true;
            m_syncIndex = currentIndex;
            ++m_syncCount;
            
            // extra check to see if done syncing
            if (m_syncCount == m_numActiveAgents)
            {
               StopSync();
               ++currentIndex;
               agentPlan.ClearPlan();
               continue;
            }
            else
            {
               canGetNewTask = false;
            }
         }

         // Reservable tasks can be claimed and ran only once
         if ((taskMods & (byte)Task.eModifier.RESERVABLE_MODIFIER) != 0)
         {
            taskToGet.Claim();
         }

         // Add task to plan and increment index
         PrimitiveTask taskClone = taskToGet.Clone() as PrimitiveTask;
         taskClone.Op.AssignVariables(request.m_agent);

         agentPlan.AddTask(taskClone);
         ++currentIndex;

         if (m_activeStrategyPlan.IsIndexOutOfPlan(currentIndex))
         {
            canGetNewTask = false;
         }
      }

      request.m_agent.CurrentPlanIndex = currentIndex;
      return agentPlan;
   }


   //-----------------------------------------------------------------------------------------------
   public void StopSync()
   {
      m_isSyncing = false;
      m_syncCount = 0;
      m_syncIndex = 0;

      if (m_aiManager == null)
      {
         m_aiManager = FindObjectOfType<AIManager>();
      }
      m_aiManager.ClearAgentCurrentPlan();
   }


   //-----------------------------------------------------------------------------------------------
   public void AnalyzeWorldState()
   {
      WorldState ws = m_htnPlanner.CurrentWorldState;

      ws.SetPropertyValue("Strategy", (byte)m_activeStrategy);
      ws.SetPropertyValue("Num_Buttons", (byte)GameObject.FindGameObjectsWithTag("Button").Length);
      ws.SetPropertyValue("Does_Choke_Point_Exist", (byte)(GameObject.FindGameObjectsWithTag("Chokepoint").Length != 0 ? 1 : 0));
   }


   //-----------------------------------------------------------------------------------------------
   public void Reload()
   {
      StopSync();
      m_htnDomain.Clear();
      RegisterOperators();

      DomainXmlLoader.LoadDomain(m_htnDomain);

      m_htnPlanner.ClearWorldState();
      AnalyzeWorldState();
   }


   //-----------------------------------------------------------------------------------------------
   public bool DoesNeedRefresh()
   {
      const bool NEEDS_REFRESH = true;

      if (m_htnPlanner.PlannerDomain == null)
      {
         return NEEDS_REFRESH;
      }

      return !NEEDS_REFRESH;
   }


   //-----------------------------------------------------------------------------------------------
   public bool IsPlanFinished(int currentIndex)
   {
      const bool PLAN_IS_FINISHED = true;

      if (m_activeStrategyPlan.IsIndexOutOfPlan(currentIndex))
      {
         return PLAN_IS_FINISHED;
      }

      return !PLAN_IS_FINISHED;
   }


   //-----------------------------------------------------------------------------------------------
   public void ClearPlan()
   {
      m_activeStrategyPlan.ClearPlan();
      StopSync();
   }

}
