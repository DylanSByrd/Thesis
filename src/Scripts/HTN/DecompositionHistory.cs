using System.Collections.Generic;


//-----------------------------------------------------------------------------------------------
namespace HTN
{
   //-----------------------------------------------------------------------------------------------
   public class DecompositionHistory
   {
      //-----------------------------------------------------------------------------------------------
      private Stack<CompoundTask> m_taskHistory = new Stack<CompoundTask>();
      private Stack<Plan> m_planHistory = new Stack<Plan>();
      private Stack<Stack<Task>> m_decompHistory = new Stack<Stack<Task>>();


      //-----------------------------------------------------------------------------------------------
      public void RecordTaskDecomposition(
         CompoundTask currentTask, Plan currentPlan, Stack<Task> decompHistory)
      {
         m_taskHistory.Push(currentTask);

         Plan planCopy = currentPlan.Clone();
         m_planHistory.Push(planCopy);

         Stack<Task> decompClone = new Stack<Task>(decompHistory);
         m_decompHistory.Push(decompClone);
      }


      //-----------------------------------------------------------------------------------------------
      public void RestoreToLastDecomposedTask(
         out CompoundTask lastDecomposedTask, out Plan lastPlan, out Stack<Task> lastTaskHistory)
      {
         lastDecomposedTask = m_taskHistory.Pop();
         lastPlan = m_planHistory.Pop();
         lastTaskHistory = m_decompHistory.Pop();
      }
   }
}
