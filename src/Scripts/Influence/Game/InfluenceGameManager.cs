/*
   This file contains the influence game manager.  It's the middle layer of communication between
   the game code and the influence library code.
*/


//-----------------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using Influence;


//-----------------------------------------------------------------------------------------------
[ExecuteInEditMode]
public class InfluenceGameManager 
   : MonoBehaviour, IInfluenceGameManager
{
   //-----------------------------------------------------------------------------------------------
   public bool IsRunning
   {
      get { return m_isRunning; }
   }


   //-----------------------------------------------------------------------------------------------
   readonly float INFLUENCE_MAP_UPDATE_RATE_SECONDS = 1f / 30f;


   //-----------------------------------------------------------------------------------------------
   private bool m_isRunning = true;

   private InfluenceSystem m_influenceSystem;// = InfluenceSystem.GetInstance();


   //-----------------------------------------------------------------------------------------------
   public void Awake()
   {
      GetInfluenceSystemReferenceIfNeeded();
   }


   //-----------------------------------------------------------------------------------------------
   public void Start()
   {
      GetInfluenceSystemReferenceIfNeeded();
      StartCoroutine(PeriodicBaseMapUpdate());
   }


   //-----------------------------------------------------------------------------------------------
   public void Update()
   {
   }
    
   
   //-----------------------------------------------------------------------------------------------
   public void OnDisable()
   {
      m_isRunning = false;
   }


   //-----------------------------------------------------------------------------------------------
   public IEnumerator PeriodicBaseMapUpdate()
   {
      while (IsRunning)
      {
         UpdateInfluenceSystem();

         yield return new WaitForSeconds(INFLUENCE_MAP_UPDATE_RATE_SECONDS);
      }

      // Compute shader version
      //while (IsRunning)
      //{
      //   // Tell generator to run
      //   yield return null; // wait until next frame

      //   // Copy data to buffers
      //   yield return new WaitForSeconds(INFLUENCE_MAP_UPDATE_RATE_SECONDS);
      //}
   }


   //-----------------------------------------------------------------------------------------------
   public void RegisterInfluenceObject(IInfluenceObject influenceObject)
   {
      GetAndInitializeInfluenceSystemReferenceIfNeeded();
      m_influenceSystem.RegisterInfluenceObject(influenceObject);
   }


   //-----------------------------------------------------------------------------------------------
   public void UnregisterInfluenceObject(IInfluenceObject influenceObject)
   {
      GetAndInitializeInfluenceSystemReferenceIfNeeded();
      m_influenceSystem.UnregisterInfluenceObject(influenceObject);
   }


   //-----------------------------------------------------------------------------------------------
   public void RegisterAllActiveInfluenceObjectsInScene()
   {
      InfluenceGameObject[] influenceObjectsInScene = FindObjectsOfType<InfluenceGameObject>();

      foreach (InfluenceGameObject influenceObject in influenceObjectsInScene)
      {
         if (influenceObject.enabled)
         {
            influenceObject.Awake();
            m_influenceSystem.RegisterInfluenceObject(influenceObject);
         }
      }
   }


   //-----------------------------------------------------------------------------------------------
   private void GetInfluenceSystemReferenceIfNeeded()
   {
      if (m_influenceSystem != null)
      {
         return;
      }

      m_influenceSystem = InfluenceSystem.GetInstance();
   }


   //-----------------------------------------------------------------------------------------------
   private void GetAndInitializeInfluenceSystemReferenceIfNeeded()
   {
      if (m_influenceSystem != null)
      {
         return;
      }

      GetInfluenceSystemReferenceIfNeeded();
      InitializeInfluenceSystem();
   }


   //-----------------------------------------------------------------------------------------------
   public void InitializeInfluenceSystem()
   {
      InitializeInfluenceDataFromXml();
      m_influenceSystem.InitializeGenerator(30U);
   }


   //-----------------------------------------------------------------------------------------------
   private void InitializeInfluenceDataFromXml()
   {
      InfluenceXmlLoader.LoadInfluenceFunctionsIntoManager(m_influenceSystem);
      InfluenceXmlLoader.LoadInfluenceTemplatesIntoManager(m_influenceSystem);
      InfluenceXmlLoader.LoadBaseInfluenceMapsIntoManager(m_influenceSystem);
      InfluenceXmlLoader.LoadMapFormulasIntoManager(m_influenceSystem);
   }


   //-----------------------------------------------------------------------------------------------
   public void UpdateInfluenceSystem()
   {
      m_influenceSystem.UpdateInfluenceMaps();
   }


   //-----------------------------------------------------------------------------------------------
   public void ReloadInfluenceMapData()
   {
      GetAndInitializeInfluenceSystemReferenceIfNeeded();
      m_influenceSystem.ClearAllMapData();
      InitializeInfluenceSystem();
      m_influenceSystem.UpdateInfluenceMaps();
   }


   //-----------------------------------------------------------------------------------------------
   public void ReloadInfluenceObjectData()
   {
      GetAndInitializeInfluenceSystemReferenceIfNeeded();
      m_influenceSystem.ClearAllObjectData();
      RegisterAllActiveInfluenceObjectsInScene();
      m_influenceSystem.UpdateInfluenceMaps();
   }
}
