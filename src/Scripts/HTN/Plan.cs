using System.Collections.Generic;


//-----------------------------------------------------------------------------------------------
namespace HTN
{
   //-----------------------------------------------------------------------------------------------
   public class Plan
   {
      //-----------------------------------------------------------------------------------------------
      public List<PrimitiveTask> TaskList
      {
         get { return m_taskList; }
      }
         
         
      //-----------------------------------------------------------------------------------------------
      private List<PrimitiveTask> m_taskList = new List<PrimitiveTask>();


      //-----------------------------------------------------------------------------------------------
      public void AddTask(PrimitiveTask task)
      {
         m_taskList.Add(task);
      }


      //-----------------------------------------------------------------------------------------------
      public void ClearPlan()
      {
         m_taskList.Clear();
      }


      //-----------------------------------------------------------------------------------------------
      public PrimitiveTask GetTaskAtIndex(int index)
      {
         if (IsIndexOutOfPlan(index))
         {
            return null;
         }

         return m_taskList[index];
      }


      //-----------------------------------------------------------------------------------------------
      public bool IsIndexOutOfPlan(int index)
      {
         const bool INDEX_IS_OUT_OF_PLAN = true;

         if (index >= m_taskList.Count)
         {
            return INDEX_IS_OUT_OF_PLAN;
         }

         return !INDEX_IS_OUT_OF_PLAN;
      }


      //-----------------------------------------------------------------------------------------------
      public Plan Clone()
      {
         Plan clone = new Plan();
         clone.m_taskList = new List<PrimitiveTask>(m_taskList);

         return clone;
      }
   }
}