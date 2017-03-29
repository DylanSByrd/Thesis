using System;


//-----------------------------------------------------------------------------------------------
namespace Influence
{
   //-----------------------------------------------------------------------------------------------
   public class BaseMap
   {
      //-----------------------------------------------------------------------------------------------
      public string InfluenceID
      {
         get { return m_influenceID; }
      }

      public string ObjectTag
      {
         get { return m_objectTag; }
      }

      public float[] Data
      {
         get { return m_mapData; }
      }


      //-----------------------------------------------------------------------------------------------
      public static readonly uint INFLUENCE_MAP_RESOLUTION = 120;


      //-----------------------------------------------------------------------------------------------
      private string m_influenceID;
      private string m_objectTag;
      private float[] m_mapData = new float[INFLUENCE_MAP_RESOLUTION * INFLUENCE_MAP_RESOLUTION];


      //-----------------------------------------------------------------------------------------------
      public BaseMap(string influenceID, string objectTag)
      {
         m_influenceID = influenceID;
         m_objectTag = objectTag;
      }


      //-----------------------------------------------------------------------------------------------
      public void Reset()
      {
         Array.Clear(m_mapData, 0, m_mapData.Length);
      }


      //-----------------------------------------------------------------------------------------------
      public void ApplyTemplate(InfluenceTemplate template, uint locationX, uint locationY)
      {
         float[] templateData = template.TemplateData;
         uint templateDimension = template.TemplateTextureDimension;

         uint zeroCoordX = locationX - template.TemplateSize - 1;
         uint zeroCoordY = locationY - template.TemplateSize - 1;

         for (uint rowIndex = 0; rowIndex < templateDimension; ++rowIndex)
         {
            uint mapCoordY = zeroCoordY + rowIndex;
            if (mapCoordY <= 0U || mapCoordY >= INFLUENCE_MAP_RESOLUTION)
            {
               continue;
            }

            for (uint colIndex = 0; colIndex < templateDimension; ++colIndex)
            {
               uint mapCoordX = zeroCoordX + colIndex;
               if (mapCoordX <= 0U || mapCoordX >= INFLUENCE_MAP_RESOLUTION)
               {
                  continue;
               }

               uint templateIndex = (rowIndex * templateDimension) + colIndex;
               uint mapIndex = (mapCoordY * INFLUENCE_MAP_RESOLUTION) + mapCoordX;

               m_mapData[mapIndex] += templateData[templateIndex];
            }
         }
      }
   }
}
