using System.Collections.Generic;


//-----------------------------------------------------------------------------------------------
namespace Influence
{
   //-----------------------------------------------------------------------------------------------
   public enum eInfluenceOpType : byte
   {
      INVALID_INFLUENCE_OP,
      INFLUENCE_OP_ADD,
      INFLUENCE_OP_ADD_INVERSE,
      INFLUENCE_OP_MULTIPLY,
      INFLUENCE_OP_NORMALIZE,
      NUM_INFLLUENCE_OP_TYPES,
   }


   //-----------------------------------------------------------------------------------------------
   public struct MapOperation
   {
      //-----------------------------------------------------------------------------------------------
      public eInfluenceOpType OpType { get; set; }
      public float Weight { get; set; }
      public string InfluenceID { get; set; }
      public string ObjectTag { get; set; }


      //-----------------------------------------------------------------------------------------------
      public readonly static float DEFAULT_OPERATION_WEIGHT = 1f;


      //-----------------------------------------------------------------------------------------------
      public static eInfluenceOpType GetOpTypeForString(string opTypeAsString)
      {
         string caseInsensitiveOpTypeAsString = opTypeAsString.ToLower();

         switch (caseInsensitiveOpTypeAsString)
         {
         case "add":
         {
            return eInfluenceOpType.INFLUENCE_OP_ADD;
         }

         case "addinverse":
         {
            return eInfluenceOpType.INFLUENCE_OP_ADD_INVERSE;
         }

         case "multiply":
         {
            return eInfluenceOpType.INFLUENCE_OP_MULTIPLY;
         }

         case "normalize":
         {
            return eInfluenceOpType.INFLUENCE_OP_NORMALIZE;
         }
         }

         return eInfluenceOpType.INVALID_INFLUENCE_OP;
      }
   }


   //-----------------------------------------------------------------------------------------------
   public class MapFormula
   {
      //-----------------------------------------------------------------------------------------------
      public string FormulaID
      {
         get { return m_formulaID; }
      }

      public int OperationCount
      {
         get { return m_operationInstructions.Count; }
      }
      

      //-----------------------------------------------------------------------------------------------
      private List<MapOperation> m_operationInstructions = new List<MapOperation>();
      private string m_formulaID;


      //-----------------------------------------------------------------------------------------------
      public MapFormula(string formulaID)
      {
         m_formulaID = formulaID;
      }
         
         
      //-----------------------------------------------------------------------------------------------
      public void AddOperationInstruction(eInfluenceOpType operationType, float weight, string influenceID, string objectTag)
      {
         MapOperation instructionToAdd = new MapOperation();
         instructionToAdd.OpType = operationType;
         instructionToAdd.Weight = weight;
         instructionToAdd.InfluenceID = influenceID;
         instructionToAdd.ObjectTag = objectTag;

         m_operationInstructions.Add(instructionToAdd);
      }


      //-----------------------------------------------------------------------------------------------
      public WorkingMap ConstructMapFromFormula()
      {
         WorkingMap workingMap = new WorkingMap();

         foreach (MapOperation operation in m_operationInstructions)
         {
            PerformOperation(operation, workingMap);
         }

         return workingMap;
      }


      //-----------------------------------------------------------------------------------------------
      public void PerformOperation(MapOperation operation, WorkingMap workingMap)
      {
         if (operation.OpType == eInfluenceOpType.INFLUENCE_OP_NORMALIZE)
         {
            workingMap.Normalize();
            return;
         }

         string operandMapID = operation.InfluenceID;
         string operandObjectTag = operation.ObjectTag;
         BaseMap operandMap = InfluenceSystem.GetInstance().GetInfluenceMapByIDWithTag(operandMapID, operandObjectTag);

         switch (operation.OpType)
         {
         case eInfluenceOpType.INFLUENCE_OP_ADD:
         {
            workingMap.AddMap(operandMap, operation.Weight);
            break;
         }

         case eInfluenceOpType.INFLUENCE_OP_ADD_INVERSE:
         {
            workingMap.AddMapInverse(operandMap, operation.Weight);
            break;
         }

         case eInfluenceOpType.INFLUENCE_OP_MULTIPLY:
         {
            workingMap.MultiplyMap(operandMap, operation.Weight);
            break;
         }
         }
      }
   }
}
