using UnityEngine;
using HTN;


//-----------------------------------------------------------------------------------------------
public class Agent : MonoBehaviour
{
   //-----------------------------------------------------------------------------------------------
   public int AgentID
   {
      get { return m_agentID; }
   }

   public int CurrentPlanIndex
   {
      get { return m_currentPlanIndex; }
      set { m_currentPlanIndex = value; }
   }

   public PlanRunner ActivePlanRunner
   {
      get { return m_planRunner; }
   }


   //-----------------------------------------------------------------------------------------------
   [SerializeField]
   private int m_agentID;


   //-----------------------------------------------------------------------------------------------
   private PlanRunner m_planRunner = new PlanRunner();
   private int m_currentPlanIndex;

   
   //-----------------------------------------------------------------------------------------------
   private static int s_agentID;


   //-----------------------------------------------------------------------------------------------
   Agent()
   {
      m_agentID = s_agentID++;
   }


   //-----------------------------------------------------------------------------------------------
   public bool DoesAgentNeedPlan()
   {
      if (!m_planRunner.HasPlan())
      {
         return true;
      }

      if (m_planRunner.IsPlanFinished())
      {
         return true;
      }

      return false;
   }


   //-----------------------------------------------------------------------------------------------
   public void AssignPlan(Plan newPlan)
   {
      m_planRunner.PlanToExecute = newPlan;
   }
      
      
   //-----------------------------------------------------------------------------------------------
   public void ExecuteNextTask()
   {
      if (m_planRunner.IsPlanFinished())
      {
         return;
      }

      m_planRunner.ExecuteNextTask();
   }


   //-----------------------------------------------------------------------------------------------
   public void ClearPlanHistory()
   {
      m_planRunner.ClearPlan();
      m_currentPlanIndex = 0;
   }

}
