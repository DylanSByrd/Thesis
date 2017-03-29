using System;
using System.Collections.Generic;

//-----------------------------------------------------------------------------------------------
namespace HTN
{
   //-----------------------------------------------------------------------------------------------
   public class CompoundTask : Task
   {
      //-----------------------------------------------------------------------------------------------
      public List<Method> Methods
      {
         get { return m_methods; }
      }


      //-----------------------------------------------------------------------------------------------
      private List<Method> m_methods = new List<Method>();

      
      //-----------------------------------------------------------------------------------------------
      public CompoundTask(string name, params eModifier[] mods)
         : base(name, eType.COMPOUND_TASK, mods)
      { }


      //-----------------------------------------------------------------------------------------------
      public CompoundTask(string name, uint modifierMask)
         : base(name, eType.COMPOUND_TASK, modifierMask)
      { }


      //-----------------------------------------------------------------------------------------------
      public Method FindSatisfiedMethod(WorldState currentWorldState)
      {
         foreach (Method method in m_methods)
         {
            if (method.IsSatisfied(currentWorldState))
            {
               return method;
            }
         }

         return null;
      }


      //-----------------------------------------------------------------------------------------------
      public void RegisterMethod(Method method)
      {
         m_methods.Add(method);
      }


      //-----------------------------------------------------------------------------------------------
      public override Task Clone()
      {
         CompoundTask clone = new CompoundTask(m_name, m_modifierMask);

         clone.m_methods = m_methods;

         return clone;
      }
   }
}