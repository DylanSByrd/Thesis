using UnityEngine;
using System.Xml;
using System.ComponentModel;
using Influence;


//-----------------------------------------------------------------------------------------------
public static class InfluenceXmlLoader
{
   //-----------------------------------------------------------------------------------------------
   public static void LoadInfluenceFunctionsIntoManager(InfluenceSystem manager)
   {
      XmlDocument dataAsXmlDoc = LoadXmlDocumentFromFile("Data/InfluenceFunctions");
      if (dataAsXmlDoc == null)
      {
         return;
      }

      XmlNodeList functionList = dataAsXmlDoc.GetElementsByTagName("InfluenceFunction");
      foreach (XmlNode functionInfo in functionList)
      {
         string influenceID = null;
         if (!TryLoadRequiredAttributeFromXmlNode(functionInfo, "InfluenceID", ref influenceID))
         {
            continue;
         }

         float influenceMax = TryLoadXmlAttribute(functionInfo, "InfluenceMax", InfluenceFunction.DEFAULT_MAX_INFLUENCE);
         uint distanceMax = TryLoadXmlAttribute(functionInfo, "DistanceMax", InfluenceFunction.DEFAULT_MAX_DISTANCE);
         uint distanceMin = TryLoadXmlAttribute(functionInfo, "DistanceMin", InfluenceFunction.DEFAULT_MIN_DISTANCE);
         uint falloffExponent = TryLoadXmlAttribute(functionInfo, "FalloffExponent", InfluenceFunction.DEFAULT_FALLOFF_EXPONENT);

         manager.MakeAndRegisterInfluenceFunction(influenceID, influenceMax, distanceMax, distanceMin, falloffExponent);
      }
   }


   //-----------------------------------------------------------------------------------------------
   public static void LoadBaseInfluenceMapsIntoManager(InfluenceSystem manager)
   {
      XmlDocument dataAsXmlDoc = LoadXmlDocumentFromFile("Data/BaseInfluenceMaps");
      if (dataAsXmlDoc == null)
      {
         return;
      }

      XmlNodeList baseMapList = dataAsXmlDoc.GetElementsByTagName("BaseMap");
      foreach (XmlNode baseMapInfo in baseMapList)
      {
         string influenceID = null;
         string objectTag = null;

         if (!TryLoadRequiredAttributeFromXmlNode(baseMapInfo, "InfluenceID", ref influenceID)
            || !TryLoadRequiredAttributeFromXmlNode(baseMapInfo, "ObjectTag", ref objectTag))
         {
            continue;
         }

         manager.MakeAndRegisterBaseMap(influenceID, objectTag);
      }
   }


   //-----------------------------------------------------------------------------------------------
   public static void LoadInfluenceTemplatesIntoManager(InfluenceSystem manager)
   {
      XmlDocument dataAsXmlDoc = LoadXmlDocumentFromFile("Data/InfluenceTemplates");
      if (dataAsXmlDoc == null)
      {
         return;
      }

      XmlNodeList templateList = dataAsXmlDoc.GetElementsByTagName("InfluenceTemplate");
      foreach (XmlNode templateInfo in templateList)
      {
         string influenceID = null;
         uint templateSize = 0U;

         if (!TryLoadRequiredAttributeFromXmlNode(templateInfo, "InfluenceID", ref influenceID)
            || !TryLoadRequiredAttributeFromXmlNode(templateInfo, "TemplateSize", ref templateSize))
         {
            continue;
         }

         manager.MakeAndRegisterInfluenceTemplate(influenceID, templateSize);
      }
   }


   //-----------------------------------------------------------------------------------------------
   public static void LoadMapFormulasIntoManager(InfluenceSystem manager)
   {
      XmlDocument dataAsXmlDoc = LoadXmlDocumentFromFile("Data/InfluenceMapFormulas");
      if (dataAsXmlDoc == null)
      {
         return;
      }

      XmlNodeList formulaList = dataAsXmlDoc.GetElementsByTagName("MapFormula");
      foreach (XmlNode formulaInfo in formulaList)
      {
         string formulaID = "";
         if (!TryLoadRequiredAttributeFromXmlNode(formulaInfo, "FormulaID", ref formulaID))
         {
            continue;
         }

         MapFormula newFormula = new MapFormula(formulaID);
         XmlNodeList operationList = formulaInfo.ChildNodes;
         PopulateMapFormulaFromXmlNodelist(newFormula, operationList);

         if (newFormula.OperationCount > 0)
         {
            manager.RegisterMapFormula(newFormula);
         }
      }
   }


   //-----------------------------------------------------------------------------------------------
   private static void PopulateMapFormulaFromXmlNodelist(MapFormula formula, XmlNodeList operationList)
   {
      foreach (XmlNode operationInfo in operationList)
      {
         string opTypeAsString = "";
         if (!TryLoadRequiredAttributeFromXmlNode(operationInfo, "Type", ref opTypeAsString))
         {
            continue;
         }

         // Depending on the type, we may be able to early out
         eInfluenceOpType opType = MapOperation.GetOpTypeForString(opTypeAsString);

         if (opType == eInfluenceOpType.INVALID_INFLUENCE_OP)
         {
            Debug.Log("Operation has invalid type attribute.");
            continue;
         }

         // Don't need to grab a map for normalizing
         if (opType == eInfluenceOpType.INFLUENCE_OP_NORMALIZE)
         {
            formula.AddOperationInstruction(opType, 1f, null, null);
            continue;
         }

         string influenceID = "";
         string objectTag = "";
         if (!TryLoadRequiredAttributeFromXmlNode(operationInfo, "InfluenceID", ref influenceID)
            || !TryLoadRequiredAttributeFromXmlNode(operationInfo, "ObjectTag", ref objectTag))
         {
            continue;
         }

         float opWeight = TryLoadXmlAttribute(operationInfo, "Weight", MapOperation.DEFAULT_OPERATION_WEIGHT);

         string caseInsensitiveInfluenceID = influenceID.ToLower();
         string caseInsensitiveObjectTag = objectTag.ToLower();
         formula.AddOperationInstruction(opType, opWeight, caseInsensitiveInfluenceID, caseInsensitiveObjectTag);
      }
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
}
