using UnityEngine;
using System;
using System.Xml;
using System.ComponentModel;
using HTN;


//-----------------------------------------------------------------------------------------------
public static class DomainXmlLoader
{
   //-----------------------------------------------------------------------------------------------
   public static void LoadDomain(Domain domain)
   {
      XmlDocument dataAsXmlDoc = LoadXmlDocumentFromFile("Data/HTN_Domains/Strategy.Domain");
      if (dataAsXmlDoc == null)
      {
         return;
      }

      ParseCompoundTasks(dataAsXmlDoc, domain);
      ParsePrimitiveTasks(dataAsXmlDoc, domain);
      ParseCompoundTaskMethods(dataAsXmlDoc, domain);
   }


   //-----------------------------------------------------------------------------------------------
   private static void ParseCompoundTasks(XmlDocument xmlData, Domain domain)
   {
      XmlNodeList compoundTaskList = xmlData.GetElementsByTagName("CompoundTask");
      foreach (XmlNode compoundTaskNode in compoundTaskList)
      {
         string taskName = "";
         if (!TryLoadRequiredAttributeFromXmlNode(compoundTaskNode, "name", ref taskName))
         {
            throw new ArgumentNullException("CompoundTask missing name!");
         }

         CompoundTask task = new CompoundTask(taskName);
         domain.TryRegisterTask(taskName, task);
      }
   }


   //-----------------------------------------------------------------------------------------------
   private static void ParsePrimitiveTasks(XmlDocument xmlData, Domain domain)
   {
      XmlNodeList primitiveTaskList = xmlData.GetElementsByTagName("PrimitiveTask");
      foreach (XmlNode primitiveTaskNode in primitiveTaskList)
      {
         string taskName = "";
         if (!TryLoadRequiredAttributeFromXmlNode(primitiveTaskNode, "name", ref taskName))
         {
            throw new ArgumentNullException("PrimitiveTask missing name!");
         }

         PrimitiveTask task = new PrimitiveTask(taskName.ToLower());

         XmlNodeList taskChildren = primitiveTaskNode.ChildNodes;
         foreach (XmlNode taskChild in taskChildren)
         {
            string dataType = taskChild.Name;
            switch (dataType)
            {
            case "Precondition":
            {
               ParsePreconditionForPrimitiveTask(taskChild, task);
               break;
            }

            case "Effect":
            {
               ParseEffectForPrimitiveTask(taskChild, task);
               break;
            }

            case "Operator":
            {
               ParseOperatorForPrimitiveTask(taskChild, task, domain);
               break;
            }
            }
         }

         domain.TryRegisterTask(taskName, task);
      }
   }


   //-----------------------------------------------------------------------------------------------
   private static void ParsePreconditionForPrimitiveTask(XmlNode preconditionNode, PrimitiveTask task)
   {
      PreconditionProperty precondition = ParsePreconditionFromNode(preconditionNode);
      task.AddPrecondition(precondition);
   }


   //-----------------------------------------------------------------------------------------------
   private static void ParseEffectForPrimitiveTask(XmlNode effectNode, PrimitiveTask task)
   {
      EffectProperty effect = ParseEffectFromNode(effectNode);
      task.AddEffect(effect);
   }


   //-----------------------------------------------------------------------------------------------
   private static void ParseOperatorForPrimitiveTask(XmlNode operatorNode, PrimitiveTask task, Domain domain)
   {
      if (task.Op != null)
      {
         throw new ArgumentException("Task " + task.Name + " already has an operator!");
      }

      string operatorName = "";
      if (!TryLoadRequiredAttributeFromXmlNode(operatorNode, "name", ref operatorName))
      {
         throw new ArgumentNullException("Operator missing name!");
      }

      Operator op = domain.GetOperatorByName(operatorName).Clone();

      XmlNodeList paramNodes = operatorNode.ChildNodes;
      foreach (XmlNode paramNode in paramNodes)
      {
         string paramName = "";
         if (!TryLoadRequiredAttributeFromXmlNode(paramNode, "name", ref paramName))
         {
            throw new ArgumentNullException("OperatorParam missing name!");
         }

         string paramValue = "";
         if (!TryLoadRequiredAttributeFromXmlNode(paramNode, "value", ref paramValue))
         {
            throw new ArgumentNullException("OperatorParam " + paramName + " missing value!");
         }

         OperatorParam param = new OperatorParam(paramName, paramValue);
         op.AddParam(param);
      }

      task.Op = op;
   }
      
      
   //-----------------------------------------------------------------------------------------------
   private static void ParseCompoundTaskMethods(XmlDocument xmlData, Domain domain)
   {
      XmlNodeList compoundTaskList = xmlData.GetElementsByTagName("CompoundTask");
      foreach (XmlNode compoundTaskNode in compoundTaskList)
      {
         string taskName = "";
         if (!TryLoadRequiredAttributeFromXmlNode(compoundTaskNode, "name", ref taskName))
         {
            throw new ArgumentNullException("CompoundTask missing name!");
         }

         CompoundTask task = domain.GetTaskByName(taskName) as CompoundTask;
         ParseMethodsFromNode(compoundTaskNode, task, domain);         
      }
   }


   //-----------------------------------------------------------------------------------------------
   private static void ParseMethodsFromNode(XmlNode compoundTaskNode, CompoundTask task, Domain domain)
   {
      XmlNodeList methodList = compoundTaskNode.ChildNodes;
      foreach (XmlNode methodNode in methodList)
      {
         Method methodToAdd = new Method();

         XmlNodeList methodDataMembers = methodNode.ChildNodes;

         foreach (XmlNode methodDataMember in methodDataMembers)
         {
            string dataType = methodDataMember.Name;

            switch (dataType)
            {
            case "Precondition":
            {
               ParsePreconditionForMethod(methodDataMember, methodToAdd);
               break;
            }

            case "Subtasks":
            {
               ParseSubtasksForMethod(methodDataMember, methodToAdd, domain);
               break;
            }
            }
         }

         task.RegisterMethod(methodToAdd);
      }
   }


   //-----------------------------------------------------------------------------------------------
   private static void ParsePreconditionForMethod(XmlNode preconditionNode, Method method)
   {
      PreconditionProperty precondition = ParsePreconditionFromNode(preconditionNode);
      method.AddPrecondition(precondition);
   }


   //-----------------------------------------------------------------------------------------------
   private static EffectProperty ParseEffectFromNode(XmlNode effectNode)
   {
      string effectName = "";
      if (!TryLoadRequiredAttributeFromXmlNode(effectNode, "name", ref effectName))
      {
         throw new ArgumentNullException("Effect missing name!");
      }

      string opAsString = "";
      if (!TryLoadRequiredAttributeFromXmlNode(effectNode, "operator", ref opAsString))
      {
         throw new ArgumentNullException("Effect " + effectName + " missing operator!");
      }
      EffectProperty.eOperator op = GetEffectOpFromString(opAsString);

      string valueAsString = "";
      if (!TryLoadRequiredAttributeFromXmlNode(effectNode, "value", ref valueAsString)
         && valueAsString.Equals(""))
      {
         throw new ArgumentNullException("Effect " + effectName + " missing value!");
      }
      byte value = WorldPropertyValueParseHelper(valueAsString);

      EffectProperty effect = new EffectProperty(effectName, value, op);
      return effect;
   }


   //-----------------------------------------------------------------------------------------------
   private static PreconditionProperty ParsePreconditionFromNode(XmlNode preconditionNode)
   {
      string preconditionName = "";
      if (!TryLoadRequiredAttributeFromXmlNode(preconditionNode, "name", ref preconditionName))
      {
         throw new ArgumentNullException("Precondition missing name!");
      }

      string opAsString = "";
      if (!TryLoadRequiredAttributeFromXmlNode(preconditionNode, "operator", ref opAsString))
      {
         throw new ArgumentNullException("Precondition " + preconditionName + " missing operator!");
      }
      PreconditionProperty.eOperator op = GetPreconditionOpFromString(opAsString);

      string valueAsString = "";
      if (!TryLoadRequiredAttributeFromXmlNode(preconditionNode, "value", ref valueAsString)
         && valueAsString.Equals(""))
      {
         throw new ArgumentNullException("Precondition " + preconditionName + " missing value!");
      }
      byte value = WorldPropertyValueParseHelper(valueAsString);

      PreconditionProperty precondition = new PreconditionProperty(preconditionName, value, op);
      return precondition;
   }
      
      
   //-----------------------------------------------------------------------------------------------
   private static void ParseSubtasksForMethod(XmlNode subtasksNode, Method method, Domain domain)
   {
      XmlNodeList subtaskList = subtasksNode.ChildNodes;
      foreach (XmlNode subtaskNode in subtaskList)
      {
         string taskName = "";
         if (!TryLoadRequiredAttributeFromXmlNode(subtaskNode, "name", ref taskName))
         {
            throw new ArgumentNullException("Subtask missing name!");
         }

         Task registeredTask = domain.GetTaskByName(taskName);
         Task.eModifier[] mods = ParseModifiersForTask(subtaskNode);
         method.AddSubTask(registeredTask, mods);         
      }
   }


   //-----------------------------------------------------------------------------------------------
   private static Task.eModifier[] ParseModifiersForTask(XmlNode taskNode)
   {
      XmlNodeList modList = taskNode.ChildNodes;
      if (modList.Count == 0)
      {
         return null;
      }

      Task.eModifier[] modArray = new Task.eModifier[modList.Count];
      for (int modIndex = 0; modIndex < modList.Count; ++modIndex)
      {
         XmlNode modNode = modList[modIndex];
         string modName = "";
         if (!TryLoadRequiredAttributeFromXmlNode(modNode, "name", ref modName))
         {
            continue;
         }

         switch (modName)
         {
         case "sync":
         {
            modArray[modIndex] = Task.eModifier.SYNC_MODIFIER;
            break;
         }
         
         case "blocking":
         {
            modArray[modIndex] = Task.eModifier.BLOCKING_MODIFIER;
            break;
         }

         case "reserve":
         {
            modArray[modIndex] = Task.eModifier.RESERVABLE_MODIFIER;
            break;
         }
         }
      }

      return modArray;
   }


   //-----------------------------------------------------------------------------------------------
   private static XmlDocument LoadXmlDocumentFromFile(string fileName)
   {
      TextAsset fileAsset = Resources.Load(fileName) as TextAsset;
      if (fileAsset == null)
      {
         return null;
      }

      XmlDocument dataAsXmlDoc = new XmlDocument();
      dataAsXmlDoc.LoadXml(fileAsset.text);
      return dataAsXmlDoc;
   }


   //-----------------------------------------------------------------------------------------------
   private static ValueType TryLoadXmlAttribute<ValueType>(XmlNode node, string attributeName, ValueType defaultValue)
   {
      XmlAttribute nodeAttribute = node.Attributes[attributeName];
      ValueType loadedValue = defaultValue;
      if (nodeAttribute != null)
      {
         string nodeValueAsString = nodeAttribute.Value;
         var converter = TypeDescriptor.GetConverter(typeof(ValueType));
         if (converter != null && converter.IsValid(nodeValueAsString))
         {
            return (ValueType)converter.ConvertFromString(nodeValueAsString);
         }
      }

      return loadedValue;
   }


   //-----------------------------------------------------------------------------------------------
   public static bool TryLoadRequiredAttributeFromXmlNode<ValueType>(XmlNode node, string attributeName, ref ValueType loadedValue)
   {
      const bool ATTRIBUTE_IS_PRESENT = true;

      XmlAttribute nodeAttribute = node.Attributes[attributeName];
      if (nodeAttribute == null)
      {
         Debug.Log("Node " + node.Name + " is missing required attribute " + attributeName);
         return !ATTRIBUTE_IS_PRESENT;
      }

      string nodeValueAsString = nodeAttribute.Value;
      var converter = TypeDescriptor.GetConverter(typeof(ValueType));
      if (converter == null || !converter.IsValid(nodeValueAsString))
      {
         Debug.Log("Node " + node.Name + " has invalid value " + nodeValueAsString + " for attribute " + attributeName);
         return !ATTRIBUTE_IS_PRESENT;
      }

      try
      {
         loadedValue = (ValueType)converter.ConvertFromString(nodeValueAsString);
      }
      catch
      {
         Debug.Log("Node " + node.Name + " has invalid value " + nodeValueAsString + " for attribute " + attributeName);
         return !ATTRIBUTE_IS_PRESENT;
      }

      return ATTRIBUTE_IS_PRESENT;
   }


   //-----------------------------------------------------------------------------------------------
   private static byte WorldPropertyValueParseHelper(string propertyValue)
   {
      switch (propertyValue.ToLower())
      {
      case "ambush":
      {
         return (byte)eStrategy.AMBUSH_STRATEGY;
      }

      case "flank":
      {
         return (byte)eStrategy.FLANK_STRATEGY;
      }

      case "true":
      {
         return 1;
      }

      case "false":
      {
         return 0;
      }

      default:
      {
         var converter = TypeDescriptor.GetConverter(typeof(byte));
         return (byte)converter.ConvertFromString(propertyValue);
      }
      }
   }


   //-----------------------------------------------------------------------------------------------
   private static PreconditionProperty.eOperator GetPreconditionOpFromString(string opAsString)
   {
      switch (opAsString)
      {
      case "=":
      {
         return PreconditionProperty.eOperator.OP_EQUALS;
      }

      case "!=":
      {
         return PreconditionProperty.eOperator.OP_NOT_EQUAL;
      }

      case ">":
      {
         return PreconditionProperty.eOperator.OP_GREATER_THAN;
      }

      case ">=":
      {
         return PreconditionProperty.eOperator.OP_GREATER_OR_EQUAL;
      }

      case "<":
      {
         return PreconditionProperty.eOperator.OP_LESS_THAN;
      }

      case "<=":
      {
         return PreconditionProperty.eOperator.OP_LESS_OR_EQUAL;
      }
      }

      throw new ArgumentException("Invalid operator type for precondition!");
   }


   //-----------------------------------------------------------------------------------------------
   private static EffectProperty.eOperator GetEffectOpFromString(string opAsString)
   {
      switch (opAsString)
      {
      case "=":
      {
         return EffectProperty.eOperator.OP_ASSIGN;
      }

      case "+":
      {
         return EffectProperty.eOperator.OP_ADD;
      }

      case "-":
      {
         return EffectProperty.eOperator.OP_SUBTRACT;
      }
      }

      throw new ArgumentException("Invalid operator type for effect!");
   }
}
