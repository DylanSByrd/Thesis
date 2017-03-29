using System.Collections.Generic;


//-----------------------------------------------------------------------------------------------
namespace HTN
{
   //-----------------------------------------------------------------------------------------------
   public class Domain
   {
      //-----------------------------------------------------------------------------------------------
      public Dictionary<string, Task> Tasks
      {
         get { return m_tasks; }
      }

      public Dictionary<string, Operator> Operators
      {
         get { return m_operators; }
      }

      public Task RootTask
      {
         get { return m_rootTask; }
      }


      //-----------------------------------------------------------------------------------------------
      private Dictionary<string, Task> m_tasks = new Dictionary<string, Task>();
      private Dictionary<string, Operator> m_operators = new Dictionary<string, Operator>();
      private Task m_rootTask;


      //-----------------------------------------------------------------------------------------------
      public bool TryRegisterTask(string name, Task task)
      {
         const bool REGISTRATION_SUCCEEDED = true;

         if (IsTaskRegistered(name))
         {
            return !REGISTRATION_SUCCEEDED;
         }

         // First task added is the root
         if (m_tasks.Count == 0)
         {
            m_rootTask = task;
         }

         m_tasks.Add(name.ToLower(), task);
         return REGISTRATION_SUCCEEDED;
      }


      //-----------------------------------------------------------------------------------------------
      public bool IsTaskRegistered(string name)
      {
         const bool TASK_IS_REGISTERED = true;

         if (m_tasks.ContainsKey(name.ToLower()))
         {
            return TASK_IS_REGISTERED;
         }

         return !TASK_IS_REGISTERED;
      }


      //-----------------------------------------------------------------------------------------------
      public Task GetTaskByName(string name)
      {
         string nameLower = name.ToLower();
         if (IsTaskRegistered(nameLower))
         {
            Task task = m_tasks[nameLower];
            return task;
         }

         return null;
      }


      //-----------------------------------------------------------------------------------------------
      public bool TryRegisterOperator(string name, Operator op)
      {
         const bool REGISTRATION_SUCCEEDED = true;

         if (IsOperatorRegistered(name))
         {
            return !REGISTRATION_SUCCEEDED;
         }

         m_operators.Add(name.ToLower(), op);
         return REGISTRATION_SUCCEEDED;
      }


      //-----------------------------------------------------------------------------------------------
      public bool IsOperatorRegistered(string name)
      {
         const bool TASK_IS_REGISTERED = true;

         if (m_operators.ContainsKey(name.ToLower()))
         {
            return TASK_IS_REGISTERED;
         }

         return !TASK_IS_REGISTERED;
      }


      //-----------------------------------------------------------------------------------------------
      public Operator GetOperatorByName(string name)
      {
         string nameLower = name.ToLower();
         if (IsOperatorRegistered(nameLower))
         {
            Operator op = m_operators[nameLower];
            return op;
         }

         return null;
      }


      //-----------------------------------------------------------------------------------------------
      public void Clear()
      {
         Operators.Clear();
         Tasks.Clear();
         m_rootTask = null;
      }
   }
}