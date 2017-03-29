//-----------------------------------------------------------------------------------------------
namespace Influence
{
   //-----------------------------------------------------------------------------------------------
   public class WorkingMap
   {
      //-----------------------------------------------------------------------------------------------
      public float[] Data
      {
         get { return m_influenceCellData; }
      }
        
        
      //-----------------------------------------------------------------------------------------------
      private InfluenceMapPoint m_dimensions = new InfluenceMapPoint(DEFAULT_WORKING_MAP_DIMENSIONS, DEFAULT_WORKING_MAP_DIMENSIONS);
      private float[] m_influenceCellData = new float[DEFAULT_WORKING_MAP_DIMENSIONS * DEFAULT_WORKING_MAP_DIMENSIONS];
      //private Vector2 m_center = Vector2.zero;
   
   
      //-----------------------------------------------------------------------------------------------
      public const uint DEFAULT_WORKING_MAP_DIMENSIONS = 120;
   
   
      //-----------------------------------------------------------------------------------------------
      public void MakeNew()
      {
          ResetData();
      }
   
   
      //-----------------------------------------------------------------------------------------------
      public void MakeNew(uint newDimensions)
      {
          ResetData(newDimensions);
      }
   
   
      //-----------------------------------------------------------------------------------------------
      public void Normalize()
      {
          float max = 0f;
   
          foreach (float cellValue in m_influenceCellData)
          {
              if (cellValue > max)
              {
                  max = cellValue;
              }
          }
   
          if (max == 0f)
          {
              return;
          }
   
          for (int dataIndex = 0; dataIndex < m_influenceCellData.Length; ++dataIndex)
          {
              m_influenceCellData[dataIndex] /= max;
          }
      }
   
   
      //-----------------------------------------------------------------------------------------------
      public void AddMap(BaseMap mapToAdd, float weight = 1f)
      {
          float[] operandData = mapToAdd.Data;
   
          for (int dataIndex = 0; dataIndex < m_influenceCellData.Length; ++dataIndex)
          {
              float currentCellValueToAdd = operandData[dataIndex];
              float weightedValueToAdd = currentCellValueToAdd * weight;
              m_influenceCellData[dataIndex] += weightedValueToAdd;
          }
      }
   
   
      //-----------------------------------------------------------------------------------------------
      public void AddMapInverse(BaseMap mapToAddInverse, float weight = 1f)
      {
          float[] operandData = mapToAddInverse.Data;
   
          for (int dataIndex = 0; dataIndex < m_influenceCellData.Length; ++dataIndex)
          {
              float currentCellValueToAdd = 1f - operandData[dataIndex];
              float weightedValueToAdd = currentCellValueToAdd * weight;
              m_influenceCellData[dataIndex] += weightedValueToAdd;
          }
      }
   
   
      //-----------------------------------------------------------------------------------------------
      public void MultiplyMap(BaseMap mapToMultiply, float weight = 1f)
      {
          float[] operandData = mapToMultiply.Data;
   
          for (int dataIndex = 0; dataIndex < m_influenceCellData.Length; ++dataIndex)
          {
   
              float currentCellValueToMultiply = operandData[dataIndex];
              float weightedValueToMultiply = currentCellValueToMultiply * weight;
              m_influenceCellData[dataIndex] *= weightedValueToMultiply;
          }
      }
   
   
      //-----------------------------------------------------------------------------------------------//-----------------------------------------------------------------------------------------------
      public uint GetIndexOfHighestInfluence()
      {
          float max = 0f;
          uint maxIndex = 0U;
   
          for (uint currentIndex = 0U; currentIndex < m_influenceCellData.Length; ++currentIndex)
          {
              float cellValue = m_influenceCellData[currentIndex];
              if (cellValue > max)
              {
                  max = cellValue;
                  maxIndex = currentIndex;
              }
          }
   
          return maxIndex;
      }


      //-----------------------------------------------------------------------------------------------
      private InfluenceMapPoint ConvertIndexToMapPoint(uint index)
      {
         InfluenceMapPoint result;
         result.x = index % (m_dimensions.x);
         result.y = index / (m_dimensions.x);
         return result;
      }
         
      
      //-----------------------------------------------------------------------------------------------
      public InfluenceMapPoint GetPointOfHighestInfluence()
      {
         uint indexOfHighestInfluence = GetIndexOfHighestInfluence();
         InfluenceMapPoint pointOfHighestInfluence = ConvertIndexToMapPoint(indexOfHighestInfluence);
         return pointOfHighestInfluence;
      }
   
   
      //-----------------------------------------------------------------------------------------------
      private void ResetData(uint newDimensions = DEFAULT_WORKING_MAP_DIMENSIONS)
      {
          uint numNewCells = newDimensions * newDimensions;
          m_influenceCellData = new float[numNewCells];
      }
   }
}
