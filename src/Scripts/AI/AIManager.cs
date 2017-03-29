using UnityEngine;
using System.Collections.Generic;
using HTN;


//-----------------------------------------------------------------------------------------------
public class AIManager : MonoBehaviour
{
   //-----------------------------------------------------------------------------------------------
   public List<Agent> ActiveAgents
   {
      get
      {
         return m_activeAgents;
      }
   }

   public int NextAgentToExecuteIndex
   {
      get { return m_indexOfNextAgentToExecute; }
   }


   //-----------------------------------------------------------------------------------------------
   public GameObject m_agentPrefab;


   //-----------------------------------------------------------------------------------------------
   [SerializeField]
   private List<Agent> m_activeAgents = new List<Agent>();

   private StrategyPlanner m_strategyManagerRef;
   private int m_indexOfNextAgentToExecute;
   


   //-----------------------------------------------------------------------------------------------
   public void Awake()
   {
      m_strategyManagerRef = GameObject.FindGameObjectWithTag("StrategyManager").GetComponent<StrategyPlanner>();
   }


   //-----------------------------------------------------------------------------------------------
   public void Update()
   {
      RequestPlansForAllAgents();
   }
      
      
   //-----------------------------------------------------------------------------------------------
   public void AddAgent()
   {
      Agent newAgent = Instantiate(m_agentPrefab).GetComponent<Agent>();
      newAgent.name = "Agent" + m_activeAgents.Count;
      m_activeAgents.Add(newAgent);
   }


   //-----------------------------------------------------------------------------------------------
   public void RemoveAgentAtIndex(int agentIndex)
   {
      if (m_activeAgents[agentIndex] != null)
      {
         DestroyImmediate(m_activeAgents[agentIndex].gameObject);
      }

      m_activeAgents.RemoveAt(agentIndex);

      UpdateAgentNames();
   }


   //-----------------------------------------------------------------------------------------------
   public void UpdateAgentNames()
   {
      for (int agentIndex = 0; agentIndex < m_activeAgents.Count; ++agentIndex)
      {
         m_activeAgents[agentIndex].name = "Agent" + agentIndex;
      }
   }


   //-----------------------------------------------------------------------------------------------
   public void RequestPlansForAllAgents()
   {
      foreach (Agent agent in m_activeAgents)
      {
         if (!agent.DoesAgentNeedPlan())
         {
            continue;
         }

         RequestPlanForAgent(agent);
      }
   }


   //-----------------------------------------------------------------------------------------------
   public void ClearAgentPlanHistories()
   {
      foreach (Agent agent in m_activeAgents)
      {
         agent.ClearPlanHistory();
      }
      m_indexOfNextAgentToExecute = 0;
   }


   //-----------------------------------------------------------------------------------------------
   public void ExecuteNextTaskForAllAgents()
   {
      foreach (Agent agent in m_activeAgents)
      {
         if (agent.DoesAgentNeedPlan())
         {
            RequestPlanForAgent(agent);
         }

         agent.ExecuteNextTask();
      }
      m_indexOfNextAgentToExecute = 0;
   }


   //-----------------------------------------------------------------------------------------------
   private void RequestPlanForAgent(Agent agent)
   {
      AgentPlanRequest newRequest = new AgentPlanRequest(agent, agent.CurrentPlanIndex);

      Plan newAgentPlan = m_strategyManagerRef.HandleAgentPlanRequest(newRequest);
      agent.AssignPlan(newAgentPlan);
   }


   //-----------------------------------------------------------------------------------------------
   public void ExecuteNextTaskForNextAgent()
   {
      Agent agent = m_activeAgents[m_indexOfNextAgentToExecute];

      if (agent.DoesAgentNeedPlan())
      {
         RequestPlanForAgent(agent);
      }

      agent.ExecuteNextTask();

      // Request again to keep tool updated

      if (agent.DoesAgentNeedPlan())
      {
         RequestPlanForAgent(agent);
      }

      m_indexOfNextAgentToExecute = ((m_indexOfNextAgentToExecute + 1) % m_activeAgents.Count);
   }


   //-----------------------------------------------------------------------------------------------
   public void ClearAgentCurrentPlan()
   {
      foreach (Agent agent in m_activeAgents)
      {
         agent.ActivePlanRunner.ClearPlan();
      }
   }
}
