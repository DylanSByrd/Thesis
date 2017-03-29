using System;
using System.Collections.Generic;


//-----------------------------------------------------------------------------------------------
namespace HTN
{
   //-----------------------------------------------------------------------------------------------
   public class Method
   {
      //-----------------------------------------------------------------------------------------------
      public class TaskWithModifiers
      {
         //-----------------------------------------------------------------------------------------------
         public Task StoredTask { get; set; }
         public Task.eModifier[] StoredMods { get; set; }

         //-----------------------------------------------------------------------------------------------
         public TaskWithModifiers(Task task, Task.eModifier[] mods)
         {
            StoredTask = task;
            StoredMods = mods;
         }
      }


      //-----------------------------------------------------------------------------------------------
      public List<PreconditionProperty> Preconditions
      {
         get { return m_preconditions; }
      }

      public List<TaskWithModifiers> Subtasks
      {
         get { return m_subtasks; }
      }


      //-----------------------------------------------------------------------------------------------
      private List<PreconditionProperty> m_preconditions = new List<PreconditionProperty>();
      private List<TaskWithModifiers> m_subtasks = new List<TaskWithModifiers>();
         
         
      //-----------------------------------------------------------------------------------------------
      public bool IsSatisfied(WorldState currentWorldState)
      {
         const bool METHOD_IS_SATISFIED = true;

         foreach (PreconditionProperty precondition in m_preconditions)
         {
            byte worldStateValue;
            bool hasProperty = currentWorldState.GetPropertyValue(precondition.PropertyName.ToLower(), out worldStateValue);

            // Assume a missing property is a failure, but throw the exception for debugging
            try
            {
               if (!hasProperty)
               {
                  throw new ArgumentNullException("Property not registered: " + precondition.PropertyName);
               }
            }
            catch
            {
               return !METHOD_IS_SATISFIED;
            }

            if (!IsPreconditionMet(precondition, worldStateValue))
            {
               return !METHOD_IS_SATISFIED;
            }
         }

         return METHOD_IS_SATISFIED;
      }


      //-----------------------------------------------------------------------------------------------
      private bool IsPreconditionMet(PreconditionProperty precondition, byte currentWorldValue)
      {
         const bool PRECONDITION_IS_MET = true;

         switch (precondition.OperatorToEvaluate)
         {
         case PreconditionProperty.eOperator.OP_EQUALS:
         {
            if (precondition.PropertyValue == currentWorldValue)
            {
               return PRECONDITION_IS_MET;
            }
            break;
         }

         case PreconditionProperty.eOperator.OP_NOT_EQUAL:
         {
            if (precondition.PropertyValue != currentWorldValue)
            {
               return PRECONDITION_IS_MET;
            }
            break;
         }

         case PreconditionProperty.eOperator.OP_GREATER_THAN:
         {
            if (precondition.PropertyValue < currentWorldValue)
            {
               return PRECONDITION_IS_MET;
            }
            break;
         }

         case PreconditionProperty.eOperator.OP_LESS_THAN:
         {
            if (precondition.PropertyValue > currentWorldValue)
            {
               return PRECONDITION_IS_MET;
            }
            break;
         }

         case PreconditionProperty.eOperator.OP_GREATER_OR_EQUAL:
         {
            if (precondition.PropertyValue <= currentWorldValue)
            {
               return PRECONDITION_IS_MET;
            }
            break;
         }

         case PreconditionProperty.eOperator.OP_LESS_OR_EQUAL:
         {
            if (precondition.PropertyValue >= currentWorldValue)
            {
               return PRECONDITION_IS_MET;
            }
            break;
         }
         }

         return !PRECONDITION_IS_MET;
      }
      
      
      //-----------------------------------------------------------------------------------------------
      public void AddSubTask(Task newTask, Task.eModifier[] mods)
      {
         foreach (TaskWithModifiers registeredTask in m_subtasks)
         {
            if (registeredTask.StoredTask.Name.Equals(newTask.Name, StringComparison.CurrentCultureIgnoreCase))
            {
               return;
            }
         }

         TaskWithModifiers newTaskWithMods = new TaskWithModifiers(newTask, mods);
         m_subtasks.Add(newTaskWithMods);
      }


      //-----------------------------------------------------------------------------------------------
      public void AddPrecondition(PreconditionProperty precondition)
      {
         m_preconditions.Add(precondition);
      }
   }
}