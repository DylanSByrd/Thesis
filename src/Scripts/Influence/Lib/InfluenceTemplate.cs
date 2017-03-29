using System;


//-----------------------------------------------------------------------------------------------
namespace Influence
{
   //-----------------------------------------------------------------------------------------------
   public class InfluenceTemplate
   {
      //-----------------------------------------------------------------------------------------------
      public string InfluenceID
      {
         get { return m_influenceID; }
      }

      public uint TemplateSize
      {
         get { return m_templateSize; }
      }

      public uint TemplateTextureDimension
      {
         get
         {
            return ((m_templateSize * 2U) + 1U);
         }
      }

      public float[] TemplateData { get; set; }


      //-----------------------------------------------------------------------------------------------
      private string m_influenceID;
      private uint m_templateSize; // distance from center of the texture
         
         
      //-----------------------------------------------------------------------------------------------
      public InfluenceTemplate(string influenceID, uint size)
      {
         m_influenceID = influenceID;
         m_templateSize = size;
      }


      //-----------------------------------------------------------------------------------------------
      public void InitializeData()
      {
         InfluenceFunction functionToInitializeTemplate = GetInfluenceFunctionFromInfluenceSystem();

         // Initialize texture
         // Since the template size is related to the distance from the center of the texture
         // Ex. A template of size 10 has a texture that is 21 by 21
         TemplateData = new float[(TemplateTextureDimension * TemplateTextureDimension) + 1];

         int centerX = (int)m_templateSize + 1;
         int centerY = centerX;

         for (int rowIndex = 0; rowIndex < TemplateTextureDimension; ++rowIndex)
         {
            for (int colIndex = 0; colIndex < TemplateTextureDimension; ++colIndex)
            {
               float influenceValue = functionToInitializeTemplate.CalculateInfluenceAtPoint(colIndex, rowIndex, centerX, centerY, (int)m_templateSize);

               int index = (int)(rowIndex * TemplateTextureDimension) + colIndex;
               TemplateData[index] = influenceValue;
            }
         }
      }


      //-----------------------------------------------------------------------------------------------
      private InfluenceFunction GetInfluenceFunctionFromInfluenceSystem()
      {
         InfluenceSystem influenceSystemRef = InfluenceSystem.GetInstance();
         InfluenceFunction functionToUseForTemplate = influenceSystemRef.GetInfluenceFunctionByID(m_influenceID);
         if (functionToUseForTemplate == null)
         {
            throw new ArgumentNullException("influenceIDForTemplate");
         }

         return functionToUseForTemplate;
      }
   }
}