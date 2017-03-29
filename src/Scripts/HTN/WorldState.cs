using System.Collections.Generic;
using System;


//-----------------------------------------------------------------------------------------------
namespace HTN
{
   //-----------------------------------------------------------------------------------------------
   public class WorldProperties
      : Dictionary<string, byte>
   {
      //-----------------------------------------------------------------------------------------------
      public WorldProperties()
         : base()
      { }
         
         
      //-----------------------------------------------------------------------------------------------
      public WorldProperties(WorldProperties clone)
         : base(clone)
      { }
   }


   //-----------------------------------------------------------------------------------------------
   public class PreconditionProperty
   {
      //-----------------------------------------------------------------------------------------------
      public enum eOperator : byte
      {
         OP_EQUALS,
         OP_NOT_EQUAL,
         OP_GREATER_THAN,
         OP_LESS_THAN,
         OP_GREATER_OR_EQUAL,
         OP_LESS_OR_EQUAL,
         NUM_OPERATORS,
      }


      //-----------------------------------------------------------------------------------------------
      public string PropertyName
      {
         get { return m_propertyName; }
      }


      public byte PropertyValue
      {
         get { return m_propertyValue; }
      }


      public eOperator OperatorToEvaluate
      {
         get { return m_operatorToEvaluate; }
      }
         
         
      //-----------------------------------------------------------------------------------------------
      private string m_propertyName;
      private byte m_propertyValue;
      private eOperator m_operatorToEvaluate;


      //-----------------------------------------------------------------------------------------------
      public PreconditionProperty(string name, byte value, eOperator op)
      {
         m_propertyName = name;
         m_propertyValue = value;
         m_operatorToEvaluate = op;
      }
   }


   //-----------------------------------------------------------------------------------------------
   public class EffectProperty
   {
      //-----------------------------------------------------------------------------------------------
      public enum eOperator : byte
      {
         OP_ASSIGN,
         OP_ADD,
         OP_SUBTRACT,
         NUM_OPERATORS,
      }


      //-----------------------------------------------------------------------------------------------
      public string PropertyName
      {
         get { return m_propertyName; }
      }


      public byte PropertyValue
      {
         get { return m_propertyValue; }
      }


      public eOperator OperatorToEvaluate
      {
         get { return m_operatorToEvaluate; }
      }


      //-----------------------------------------------------------------------------------------------
      private string m_propertyName;
      private byte m_propertyValue;
      private eOperator m_operatorToEvaluate;


      //-----------------------------------------------------------------------------------------------
      public EffectProperty(string name, byte value, eOperator op)
      {
         m_propertyName = name;
         m_propertyValue = value;
         m_operatorToEvaluate = op;
      }
   }


   //-----------------------------------------------------------------------------------------------
   public class WorldState
   {
      //-----------------------------------------------------------------------------------------------
      public WorldProperties Properties
      {
         get { return m_properties; }
      }


      //-----------------------------------------------------------------------------------------------
      private WorldProperties m_properties = new WorldProperties();


      //-----------------------------------------------------------------------------------------------
      public bool RegisterProperty(string name, byte initialValue = 0)
      {
         const bool PROPERTY_REGISTRATION_SUCCESS = true;

         if (HasProperty(name))
         {
            return !PROPERTY_REGISTRATION_SUCCESS;
         }

         m_properties.Add(name.ToLower(), initialValue);
         return PROPERTY_REGISTRATION_SUCCESS;
      }


      //-----------------------------------------------------------------------------------------------
      public bool GetPropertyValue(string name, out byte out_value)
      {
         bool propertyFound = m_properties.TryGetValue(name.ToLower(), out out_value);
         return propertyFound;
      }


      //-----------------------------------------------------------------------------------------------
      public void SetPropertyValue(string name, byte valueToSet)
      {
         if (HasProperty(name))
         {
            m_properties[name.ToLower()] = valueToSet;
         }
         else
         {
            RegisterProperty(name, valueToSet);
         }
      }


      //-----------------------------------------------------------------------------------------------
      public bool HasProperty(string name)
      {
         const bool PROPERTY_IS_REGISTERED = true;

         if (m_properties.ContainsKey(name.ToLower()))
         {
            return PROPERTY_IS_REGISTERED;
         }

         return !PROPERTY_IS_REGISTERED;
      }


      //-----------------------------------------------------------------------------------------------
      public WorldState Clone()
      {
         WorldState clone = new WorldState();
         clone.m_properties = new WorldProperties(m_properties);
         return clone;
      }


      //-----------------------------------------------------------------------------------------------
      public void ApplyEffects(List<EffectProperty> effects)
      {
         foreach (EffectProperty effect in effects)
         {
            switch (effect.OperatorToEvaluate)
            {
            case EffectProperty.eOperator.OP_ASSIGN:
            {
               SetPropertyValue(effect.PropertyName, effect.PropertyValue);
               break;
            }

            case EffectProperty.eOperator.OP_ADD:
            {
               byte currentValue;
               if (!GetPropertyValue(effect.PropertyName, out currentValue))
               {
                  throw new ArgumentNullException(effect.PropertyName + " is not registered!");
               }

               currentValue = (byte)(currentValue + effect.PropertyValue);
               SetPropertyValue(effect.PropertyName, currentValue);
               break;
            }

            case EffectProperty.eOperator.OP_SUBTRACT:
            {
               byte currentValue;
               if (!GetPropertyValue(effect.PropertyName, out currentValue))
               {
                  throw new ArgumentNullException(effect.PropertyName + " is not registered!");
               }

               currentValue = (byte)(currentValue - effect.PropertyValue);
               SetPropertyValue(effect.PropertyName, currentValue);
               break;
            }
            }
         }
      }

      
      //-----------------------------------------------------------------------------------------------
      public void Clear()
      {
         m_properties.Clear();
      }
   }
}