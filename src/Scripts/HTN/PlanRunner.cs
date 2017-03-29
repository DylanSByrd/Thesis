using UnityEngine;
using System.Collections;


//-----------------------------------------------------------------------------------------------
namespace HTN
{
   //-----------------------------------------------------------------------------------------------
   public class PlanRunner
   {
      //-----------------------------------------------------------------------------------------------
      public int CurrentPlanIndex
      {
         get { return m_currentPlanIndex; }
      }

      public Plan PlanToExecute
      {
         get { return m_planToExecute; }
         set
         {
            m_planToExecute = value;
            m_currentPlanIndex = 0;
         }
      }


      //-----------------------------------------------------------------------------------------------
      private int m_currentPlanIndex;
      private Plan m_planToExecute;


      //-----------------------------------------------------------------------------------------------
      public bool ExecuteNextTask()
      {
         const bool PLAN_IS_FINISHED = true;

         if (m_planToExecute == null
            || m_planToExecute.IsIndexOutOfPlan(m_currentPlanIndex))
         {
            return PLAN_IS_FINISHED;
         }

         PrimitiveTask nextTask = m_planToExecute.GetTaskAtIndex(m_currentPlanIndex);
         nextTask.Execute();
         ++m_currentPlanIndex;

         if (m_planToExecute.IsIndexOutOfPlan(m_currentPlanIndex))
         {
            return PLAN_IS_FINISHED;
         }
         else
         {
            return !PLAN_IS_FINISHED;
         }
      }


      //-----------------------------------------------------------------------------------------------
      public bool IsPlanFinished()
      {
         const bool PLAN_IS_FINISHED = true;

         if (m_planToExecute == null
            || m_planToExecute.IsIndexOutOfPlan(m_currentPlanIndex))
         {
            return PLAN_IS_FINISHED;
         }

         return !PLAN_IS_FINISHED;
      }


      //-----------------------------------------------------------------------------------------------
      public void ClearPlan()
      {
         if (m_planToExecute == null)
         {
            return;
         }

         m_planToExecute.ClearPlan();
         m_currentPlanIndex = 0;
      }


      //-----------------------------------------------------------------------------------------------
      public bool HasPlan()
      {
         return m_planToExecute != null;
      }
   }
}