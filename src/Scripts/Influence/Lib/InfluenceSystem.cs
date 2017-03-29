using System;
using System.Collections.Generic;


//-----------------------------------------------------------------------------------------------
namespace Influence
{
   //-----------------------------------------------------------------------------------------------
   public class InfluenceSystem 
   {
      //-----------------------------------------------------------------------------------------------
      private static InfluenceSystem s_instance = new InfluenceSystem();
         
         
      //-----------------------------------------------------------------------------------------------
      public List<BaseMap> InfluenceMaps
      {
         get { return m_registeredInfluenceMaps; }
      }

      public List<InfluenceFunction> InfluenceFunctions
      {
         get { return m_registeredInfluenceFunctions; }
      }

      public Dictionary<string, List<IInfluenceObject>> InfluenceObjects
      {
         get { return m_registeredInfluenceObjects; }
      }

      public Dictionary<string, List<InfluenceTemplate>> InfluenceTemplates
      {
         get { return m_registeredInfluenceTemplates; }
      }

      public List<MapFormula> InfluenceMapFormulas
      {
         get { return m_registeredMapFormulas; }
      }


      //-----------------------------------------------------------------------------------------------
      private InfluenceGenerator m_influenceGenerator = new InfluenceGenerator();
         
         
      //-----------------------------------------------------------------------------------------------
      private List<BaseMap> m_registeredInfluenceMaps = new List<BaseMap>();
      private List<InfluenceFunction> m_registeredInfluenceFunctions = new List<InfluenceFunction>();
      private Dictionary<string, List<IInfluenceObject>> m_registeredInfluenceObjects = new Dictionary<string, List<IInfluenceObject>>();
      private Dictionary<string, List<InfluenceTemplate>> m_registeredInfluenceTemplates = new Dictionary<string, List<InfluenceTemplate>>();
      private List<MapFormula> m_registeredMapFormulas = new List<MapFormula>();

      
      //-----------------------------------------------------------------------------------------------
      private InfluenceSystem() {}
      private InfluenceSystem(InfluenceSystem copy) {}


      //-----------------------------------------------------------------------------------------------
      public static InfluenceSystem GetInstance()
      {
         return s_instance;
      }


      //-----------------------------------------------------------------------------------------------
      // Called after all influence data has been registered
      public void InitializeGenerator(uint gameMapDimensions)
      {
         m_influenceGenerator.Initialize(gameMapDimensions);
      }
         
         
      //-----------------------------------------------------------------------------------------------//-----------------------------------------------------------------------------------------------
      public BaseMap GetInfluenceMapByIDWithTag(string influenceID, string objectTag)
      {
          foreach (BaseMap map in InfluenceMaps)
          {
              if (map.InfluenceID.Equals(influenceID, StringComparison.CurrentCultureIgnoreCase) 
               && map.ObjectTag.Equals(objectTag, StringComparison.CurrentCultureIgnoreCase))
              {
                  return map;
              }
          }
   
          return null;
      }


      //-----------------------------------------------------------------------------------------------
      public InfluenceFunction GetInfluenceFunctionByID(string influenceID)
      {
         foreach (InfluenceFunction func in InfluenceFunctions)
         {
            if (func.InfluenceID.Equals(influenceID, StringComparison.CurrentCultureIgnoreCase))
            {
               return func;
            }
         }

         return null;
      }


      //-----------------------------------------------------------------------------------------------
      public InfluenceTemplate GetInfluenceTemplateByIDWithSize(string influenceID, uint size)
      {
         List<InfluenceTemplate> registeredTemplatesForID = null;
         bool isIDRegistered = m_registeredInfluenceTemplates.TryGetValue(influenceID.ToLower(), out registeredTemplatesForID);

         if (!isIDRegistered)
         {
            return null;
         }

         foreach (InfluenceTemplate influenceTemplate in registeredTemplatesForID)
         {
            if (influenceTemplate.TemplateSize == size)
            {
               return influenceTemplate;
            }
         }

         // no template with given size exists
         return null;
      }


      //-----------------------------------------------------------------------------------------------
      public List<InfluenceTemplate> GetInfluencesTemplatesWithID(string influenceID)
      {
         List<InfluenceTemplate> registeredTemplatesForID = null;
         bool isIDRegistered = m_registeredInfluenceTemplates.TryGetValue(influenceID.ToLower(), out registeredTemplatesForID);

         if (!isIDRegistered)
         {
            return null;
         }

         return registeredTemplatesForID;
      }


      //-----------------------------------------------------------------------------------------------
      public MapFormula GetMapFormulaByID(string formulaID)
      {
         foreach (MapFormula registeredFormula in m_registeredMapFormulas)
         {
            if (registeredFormula.FormulaID.Equals(formulaID, StringComparison.CurrentCultureIgnoreCase))
            {
               return registeredFormula;
            }
         }

         return null;
      }


      //-----------------------------------------------------------------------------------------------
      public void MakeAndRegisterBaseMap(string influenceID, string objectTag)
      {
         // Make sure map does not exist
         foreach(BaseMap map in InfluenceMaps)
         {
            if (map.InfluenceID.Equals(influenceID, StringComparison.CurrentCultureIgnoreCase)
               && map.ObjectTag.Equals(objectTag, StringComparison.CurrentCultureIgnoreCase))
            {
               return;
            }
         }

         BaseMap newMap = new BaseMap(influenceID, objectTag);
         InfluenceMaps.Add(newMap);
      }

      //-----------------------------------------------------------------------------------------------
      public InfluenceObjectWorldPoint ConvertMapPosToWorldPos(InfluenceMapPoint mapPoint)
      {
         return m_influenceGenerator.ConvertMapPosToWorldPos(mapPoint);
      }


      //-----------------------------------------------------------------------------------------------
      public void MakeAndRegisterInfluenceFunction(string influenceID, float influenceMax, uint distanceMax, uint distanceMin, uint falloffExponent)
      {
         // Make sure a function isn't already registered with the given id
         foreach (InfluenceFunction func in m_registeredInfluenceFunctions)
         {
            if (func.InfluenceID.Equals(influenceID, StringComparison.CurrentCultureIgnoreCase))
            {
               return;
            }
         }

         InfluenceFunction newFunction = new InfluenceFunction(influenceMax, distanceMax, distanceMin, falloffExponent, influenceID);
         m_registeredInfluenceFunctions.Add(newFunction);
      }


      //-----------------------------------------------------------------------------------------------
      public void RegisterInfluenceObject(IInfluenceObject objectToRegister)
      {
         List<IInfluenceObject> objectsWithTag = null;
         string objectTag = objectToRegister.ObjectTag;
         bool isTagAlreadyRegistered = InfluenceObjects.TryGetValue(objectTag.ToLower(), out objectsWithTag);

         if (isTagAlreadyRegistered)
         {
            // Avoid adding an object twice
            if (objectsWithTag.Contains(objectToRegister))
            {
               return;
            }

            objectsWithTag.Add(objectToRegister);
            return;
         }

         // Add the new tag with a new list
         objectsWithTag = new List<IInfluenceObject>();
         objectsWithTag.Add(objectToRegister);
         InfluenceObjects.Add(objectTag.ToLower(), objectsWithTag);
      }


      //-----------------------------------------------------------------------------------------------
      public void UnregisterInfluenceObject(IInfluenceObject objectToUnregister)
      {
         List<IInfluenceObject> objectsWithTag = null;
         string objectTag = objectToUnregister.ObjectTag;
         bool isTagAlreadyRegistered = InfluenceObjects.TryGetValue(objectTag.ToLower(), out objectsWithTag);

         if (!isTagAlreadyRegistered)
         {
            return;
         }

         foreach(IInfluenceObject influenceObject in objectsWithTag)
         {
            if (influenceObject == objectToUnregister)
            {
               objectsWithTag.Remove(objectToUnregister);
               return;
            }
         }
      }


      //-----------------------------------------------------------------------------------------------
      public void MakeAndRegisterInfluenceTemplate(string influenceID, uint size)
      {
         List<InfluenceTemplate> templateList = null;
         bool isInfluenceIDRegistered = InfluenceTemplates.TryGetValue(influenceID.ToLower(), out templateList);

         if (isInfluenceIDRegistered)
         {
            // Make sure a template with size not already registered
            foreach (InfluenceTemplate registeredTemplate in templateList)
            {
               if (registeredTemplate.TemplateSize == size)
               {
                  return;
               }
            }

            // Add template with the new size
            InfluenceTemplate newTemplate = new InfluenceTemplate(influenceID.ToLower(), size);
            templateList.Add(newTemplate);
            return;
         }

         // Not registered; make a new list and add to collection
         templateList = new List<InfluenceTemplate>();
         templateList.Add(new InfluenceTemplate(influenceID.ToLower(), size));
         m_registeredInfluenceTemplates.Add(influenceID.ToLower(), templateList);
      }


      //-----------------------------------------------------------------------------------------------
      public void RegisterMapFormula(MapFormula newFormula)
      {
         // Don't register duplicates
         foreach (MapFormula registeredFormula in m_registeredMapFormulas)
         {
            if (registeredFormula.FormulaID.Equals(newFormula.FormulaID, StringComparison.CurrentCultureIgnoreCase))
            {
               return;
            }
         }

         m_registeredMapFormulas.Add(newFormula);
      }


      //-----------------------------------------------------------------------------------------------
      public List<IInfluenceObject> GetAllObjectsWithTag(string objectTag)
      {
         List<IInfluenceObject> objectsWithTag;
         bool objectTagExists = m_registeredInfluenceObjects.TryGetValue(objectTag.ToLower(), out objectsWithTag);

         if (!objectTagExists)
         {
            return null;
         }

         return objectsWithTag;
      }
         
         
      //-----------------------------------------------------------------------------------------------
      public void UpdateInfluenceMaps()
      {
         m_influenceGenerator.GenerateInfluenceMaps();
      }


      //-----------------------------------------------------------------------------------------------
      public WorkingMap CreateMapWithFormula(string formulaID)
      {
         MapFormula formulaToCreate = GetMapFormulaByID(formulaID);
         if (formulaToCreate == null)
         {
            throw new ArgumentNullException("Formula: " + formulaID);
         }

         WorkingMap result = formulaToCreate.ConstructMapFromFormula();
         return result;
      }


      //-----------------------------------------------------------------------------------------------
      public InfluenceMapPoint GetPointOfHighestInfluenceForMap(string mapFormulaID)
      {
         WorkingMap mapResult = CreateMapWithFormula(mapFormulaID);
         return mapResult.GetPointOfHighestInfluence();
      }


      //-----------------------------------------------------------------------------------------------
      public void ClearAllMapData()
      {
         m_registeredInfluenceMaps.Clear();
         m_registeredInfluenceFunctions.Clear();
         m_registeredInfluenceTemplates.Clear();
         m_registeredMapFormulas.Clear();
      }


      //-----------------------------------------------------------------------------------------------
      public void ClearAllObjectData()
      {
         m_registeredInfluenceObjects.Clear();
      }
   }
}