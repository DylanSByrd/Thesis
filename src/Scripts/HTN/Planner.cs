using System;
using System.Collections.Generic;


//-----------------------------------------------------------------------------------------------
namespace HTN
{
   //-----------------------------------------------------------------------------------------------
   public enum ePlanResult : byte
   {
      PLAN_FAIL,
      PLAN_SUCCESS,
   }

   //-----------------------------------------------------------------------------------------------
   public class Planner
   {
      //-----------------------------------------------------------------------------------------------
      public Domain PlannerDomain
      {
         get { return m_domain; }
         set { m_domain = value; }
      }

      public WorldState CurrentWorldState
      {
         get { return m_currentWorldState; }
      }

      public Plan LastCreatedPlan
      {
         get { return m_finalPlan; }
      }

      //-----------------------------------------------------------------------------------------------
      private Domain m_domain;
      private WorldState m_currentWorldState = new WorldState();


      //-----------------------------------------------------------------------------------------------
      // All internal
      private WorldState m_workingWorldState;
      private Task m_rootTask;
      private Stack<Task> m_tasksToProcess = new Stack<Task>();
      private Plan m_finalPlan;
      private DecompositionHistory m_planHistory = new DecompositionHistory();


      //-----------------------------------------------------------------------------------------------
      public bool CreatePlan()
      {
         const bool PLAN_CREATION_SUCCESSFUL = true;

         if (m_domain == null)
         {
            throw new ArgumentNullException("No domain provided for planner!");
         }

         m_rootTask = m_domain.RootTask;

         if (m_rootTask == null)
         {
            throw new ArgumentNullException("Empty domain!");
         }

         m_finalPlan = new Plan();
         m_workingWorldState = m_currentWorldState.Clone();

         m_tasksToProcess.Clear();
         m_tasksToProcess.Push(m_rootTask);

         while (m_tasksToProcess.Count != 0)
         {
            Task currentTask = m_tasksToProcess.Pop();
            if (currentTask.Type == Task.eType.COMPOUND_TASK)
            {
               HandleCompoundTask(currentTask as CompoundTask);
            }
            else
            {
               HandlePrimitiveTask(currentTask as PrimitiveTask);
            }
         }

         if (m_finalPlan.TaskList.Count == 0)
         {
            return !PLAN_CREATION_SUCCESSFUL;
         }

         return PLAN_CREATION_SUCCESSFUL;
      }


      //-----------------------------------------------------------------------------------------------
      private void HandleCompoundTask(CompoundTask task)
      {
         Method satisfiedMethod = task.FindSatisfiedMethod(m_workingWorldState);

         if (satisfiedMethod != null)
         {
            m_planHistory.RecordTaskDecomposition(task, m_finalPlan, m_tasksToProcess);
            DecomposeMethodAndAddToProcessStack(satisfiedMethod, task);
         }
         else
         {
            RestoreToLastDecomposedTask();
         }
      }


      //-----------------------------------------------------------------------------------------------
      private void HandlePrimitiveTask(PrimitiveTask task)
      {
         if (task.ArePreconditionsMet(m_workingWorldState))
         {
            m_workingWorldState.ApplyEffects(task.Effects);
            m_workingWorldState.ApplyEffects(task.ExpectedEffects);

            m_finalPlan.AddTask(task.Clone() as PrimitiveTask);
         }
         else
         {
            RestoreToLastDecomposedTask();
         }
      }


      //-----------------------------------------------------------------------------------------------
      private void RestoreToLastDecomposedTask()
      {
         CompoundTask lastDecomposedTask;

         m_planHistory.RestoreToLastDecomposedTask(out lastDecomposedTask, out m_finalPlan, out m_tasksToProcess);

         m_tasksToProcess.Push(lastDecomposedTask);
      }


      //-----------------------------------------------------------------------------------------------
      private void DecomposeMethodAndAddToProcessStack(Method method, CompoundTask task)
      {
         List<Method.TaskWithModifiers> methodTasks = method.Subtasks;

         for (int taskIndex = (methodTasks.Count - 1); taskIndex >= 0; --taskIndex)
         {
            // Clone so we can store modifiers
            Task taskToPush = methodTasks[taskIndex].StoredTask.Clone();

            if (methodTasks[taskIndex].StoredMods != null)
            {
               foreach (Task.eModifier mod in methodTasks[taskIndex].StoredMods)
               {
                  taskToPush.AddModifier(mod);
               }
            }

            // Also add parent task's modifiers!
            taskToPush.CombineModifierMasks(task.ModifierMask);
            
            m_tasksToProcess.Push(taskToPush);
         }
      }


      //-----------------------------------------------------------------------------------------------
      public void ClearWorldState()
      {
         m_currentWorldState.Clear();
      }
   }
}