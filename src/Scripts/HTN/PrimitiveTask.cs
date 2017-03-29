using System;
using System.Collections.Generic;


//-----------------------------------------------------------------------------------------------
namespace HTN
{
   //-----------------------------------------------------------------------------------------------
   public class PrimitiveTask 
      : Task
   {
      //-----------------------------------------------------------------------------------------------
      public List<PreconditionProperty> Preconditions
      {
         get { return m_preconditions; }
      }

      public List<EffectProperty> Effects
      {
         get { return m_effects; }
      }

      public List<EffectProperty> ExpectedEffects
      {
         get { return m_expectedEffects; }
      }

      public Operator Op
      {
         get { return m_op; }
         set { m_op = value; }
      }

      public bool IsClaimed
      {
         get { return m_isClaimed; }
      }


      //-----------------------------------------------------------------------------------------------
      private List<PreconditionProperty> m_preconditions = new List<PreconditionProperty>();
      private List<EffectProperty> m_effects = new List<EffectProperty>();
      private List<EffectProperty> m_expectedEffects = new List<EffectProperty>();
      private Operator m_op;
      private bool m_isClaimed;
         
         
      //-----------------------------------------------------------------------------------------------
      public PrimitiveTask(string name, params eModifier[] mods)
         : base(name, eType.PRIMITIVE_TASK, mods)
      {}


      //-----------------------------------------------------------------------------------------------
      public PrimitiveTask(string name, uint modifierMask)
         : base(name, eType.PRIMITIVE_TASK, modifierMask)
      { }


      //-----------------------------------------------------------------------------------------------
      public void AddPrecondition(PreconditionProperty property)
      {
         m_preconditions.Add(property);
      }


      //-----------------------------------------------------------------------------------------------
      public void AddEffect(EffectProperty effect)
      {
         m_effects.Add(effect);
      }


      //-----------------------------------------------------------------------------------------------
      public void AddExpectedEffect(EffectProperty effect)
      {
         m_expectedEffects.Add(effect);
      }


      //-----------------------------------------------------------------------------------------------
      public bool ArePreconditionsMet(WorldState currentWorldState)
      {
         const bool PRECONDITIONS_ARE_MET = true;

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
               return !PRECONDITIONS_ARE_MET;
            }

            if (!IsPreconditionMet(precondition, worldStateValue))
            {
               return !PRECONDITIONS_ARE_MET;
            }
         }

         return PRECONDITIONS_ARE_MET;
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
            if (precondition.PropertyValue > currentWorldValue)
            {
               return PRECONDITION_IS_MET;
            }
            break;
         }

         case PreconditionProperty.eOperator.OP_LESS_THAN:
         {
            if (precondition.PropertyValue < currentWorldValue)
            {
               return PRECONDITION_IS_MET;
            }
            break;
         }

         case PreconditionProperty.eOperator.OP_GREATER_OR_EQUAL:
         {
            if (precondition.PropertyValue >= currentWorldValue)
            {
               return PRECONDITION_IS_MET;
            }
            break;
         }

         case PreconditionProperty.eOperator.OP_LESS_OR_EQUAL:
         {
            if (precondition.PropertyValue <= currentWorldValue)
            {
               return PRECONDITION_IS_MET;
            }
            break;
         }
         }

         return !PRECONDITION_IS_MET;
      }


      //-----------------------------------------------------------------------------------------------
      public void Execute()
      {
         Op.Run();
      }


      //-----------------------------------------------------------------------------------------------
      public override Task Clone()
      {
         PrimitiveTask clone = new PrimitiveTask(m_name, m_modifierMask);

         clone.m_preconditions = m_preconditions;
         clone.m_effects = m_effects;
         clone.m_expectedEffects = m_expectedEffects;
         clone.m_op = m_op.Clone();

         return clone;
      }


      //-----------------------------------------------------------------------------------------------
      public void Claim()
      {
         m_isClaimed = true;
      }
   }
}