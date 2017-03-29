using System;
using System.Collections.Generic;


//-----------------------------------------------------------------------------------------------
namespace HTN
{
   //-----------------------------------------------------------------------------------------------
   public class OperatorParam
   {
      //-----------------------------------------------------------------------------------------------
      public string Name
      {
         get { return m_name; }
      }

      public string Value
      {
         get { return m_value; }
      }
         
         
      //-----------------------------------------------------------------------------------------------
      private string m_name;
      private string m_value;


      //-----------------------------------------------------------------------------------------------
      public OperatorParam(string name, string value)
      {
         m_name = name;
         m_value = value;
      }
   }
      
      
   //-----------------------------------------------------------------------------------------------
   public abstract class Operator
   {
      //-----------------------------------------------------------------------------------------------
      public string Name
      {
         get { return m_name; }
      }

      public List<OperatorParam> Params
      {
         get { return m_params; }
      }

      public Agent AgentToOperateOn { get; set; }



      //-----------------------------------------------------------------------------------------------
      protected string m_name;
      protected List<OperatorParam> m_params = new List<OperatorParam>();
         
         
      //-----------------------------------------------------------------------------------------------
      public Operator(string name)
      {
         m_name = name;
      }
         
         
      //-----------------------------------------------------------------------------------------------
      public abstract void Run();


      //-----------------------------------------------------------------------------------------------
      public abstract Operator Clone();


      //-----------------------------------------------------------------------------------------------
      public abstract void AssignVariables(Agent owner);
         
         
      //-----------------------------------------------------------------------------------------------
      public void AddParam(OperatorParam param)
      {
         m_params.Add(param);
      }


      //-----------------------------------------------------------------------------------------------
      public OperatorParam FindParamWithName(string name)
      {
         foreach (OperatorParam param in m_params)
         {
            if (name.Equals(param.Name, StringComparison.CurrentCultureIgnoreCase))
            {
               return param;
            }
         }

         return null;
      }
   }
}