//-----------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System;


//-----------------------------------------------------------------------------------------------
namespace Influence
{
   //-----------------------------------------------------------------------------------------------
   public class InfluenceGenerator
   {
      //-----------------------------------------------------------------------------------------------
      public InfluenceMapPoint GameMapDimensions
      {
         get { return m_gameMapDimensions; }
         set { m_gameMapDimensions = value; }
      }
         
         
      //-----------------------------------------------------------------------------------------------
      private InfluenceMapPoint m_gameMapDimensions;
         
         
      //-----------------------------------------------------------------------------------------------
      public void Initialize(uint gameMapDimensions)
      {
         m_gameMapDimensions.x = gameMapDimensions;
         m_gameMapDimensions.y = gameMapDimensions;
         InitializeInfluenceTemplates();
      }


      // #TODO - does this make sense here?
      //-----------------------------------------------------------------------------------------------
      private void InitializeInfluenceTemplates()
      {
         InfluenceSystem influenceSystemRef = InfluenceSystem.GetInstance();
         Dictionary<string, List<InfluenceTemplate>> templatesToInitialize = influenceSystemRef.InfluenceTemplates;

         foreach (KeyValuePair<string, List<InfluenceTemplate>> namedTemplateList in templatesToInitialize)
         {      
            List<InfluenceTemplate> templateList = namedTemplateList.Value;
            foreach(InfluenceTemplate templateToInitialize in templateList)
            {
               templateToInitialize.InitializeData();
            }
         }
      }


      //-----------------------------------------------------------------------------------------------
      public void GenerateInfluenceMaps()
      {
         InfluenceSystem influenceSystemRef = InfluenceSystem.GetInstance();
         List<BaseMap> baseMapsToGenerate = influenceSystemRef.InfluenceMaps;

         foreach (BaseMap baseMap in baseMapsToGenerate)
         {
            baseMap.Reset();

            List<IInfluenceObject> objectsForMap = influenceSystemRef.GetAllObjectsWithTag(baseMap.ObjectTag);
            if (objectsForMap == null)
            {
               continue;
            }

            List<InfluenceTemplate> templatesForMap = influenceSystemRef.GetInfluencesTemplatesWithID(baseMap.InfluenceID);
            if (templatesForMap == null)
            {
               throw new ArgumentNullException("templatesForMap");
            }

            foreach (IInfluenceObject influenceObject in objectsForMap)
            {
               uint influenceSize = influenceObject.GetTemplateSizeForInfluenceType(baseMap.InfluenceID);
               InfluenceTemplate templateToApply = GetInfluenceTemplateForSize(templatesForMap, influenceSize);
               if (templateToApply == null)
               {
                  throw new ArgumentNullException("templateToApply");
               }

               InfluenceObjectWorldPoint objWorldPos = influenceObject.WorldPosition;
               InfluenceMapPoint mapPos = ConvertWorldPosToMapPos(objWorldPos);
               baseMap.ApplyTemplate(templateToApply, mapPos.x, mapPos.y);
            }
         }
      }


      //-----------------------------------------------------------------------------------------------
      public InfluenceMapPoint ConvertWorldPosToMapPos(InfluenceObjectWorldPoint objectWorldPos)
      {
         InfluenceMapPoint mapPos;
         mapPos.x = (uint)Math.Round((objectWorldPos.x / GameMapDimensions.x) * (BaseMap.INFLUENCE_MAP_RESOLUTION));
         mapPos.y = (uint)Math.Round((objectWorldPos.y / GameMapDimensions.y) * (BaseMap.INFLUENCE_MAP_RESOLUTION));
         return mapPos;
      }


      //-----------------------------------------------------------------------------------------------
      public InfluenceObjectWorldPoint ConvertMapPosToWorldPos(InfluenceMapPoint mapPoint)
      {
         InfluenceObjectWorldPoint worldPos;
         worldPos.x = ((float)(mapPoint.x) * GameMapDimensions.x) / BaseMap.INFLUENCE_MAP_RESOLUTION;
         worldPos.y = ((float)(mapPoint.y) * GameMapDimensions.y) / BaseMap.INFLUENCE_MAP_RESOLUTION;
         return worldPos;
      }


      //-----------------------------------------------------------------------------------------------
      private InfluenceTemplate GetInfluenceTemplateForSize(List<InfluenceTemplate> templateList, uint influenceSize)
      {
         foreach(InfluenceTemplate template in templateList)
         {
            if (template.TemplateSize == influenceSize)
            {
               return template;
            }
         }

         return null;
      }
   }
}