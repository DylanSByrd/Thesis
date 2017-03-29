//-----------------------------------------------------------------------------------------------
using UnityEngine;
using UnityEditor;
using Influence;
using System;
using System.Collections.Generic;


//-----------------------------------------------------------------------------------------------
[Serializable]
public class InfluenceEditorWindow : EditorWindow 
{
   //-----------------------------------------------------------------------------------------------
   internal class MapSelection
   {
      //-----------------------------------------------------------------------------------------------
      public bool m_isHidden;
      public int m_selectedMapIndex;
      public Color m_renderColor;
      public WorkingMap m_activeMap;

      public MapSelection()
      {
         m_renderColor = new Color(1f, 0f, 0f);
      }
   }


   //-----------------------------------------------------------------------------------------------
   private InfluenceGameManager m_influenceGameManager;
   private InfluenceSystem m_influenceSystemRef;


   //-----------------------------------------------------------------------------------------------
   private Material m_debugOverlayMaterial;
   private GameObject m_debugOverlayQuad;

   private List<MapSelection> m_mapSelections = new List<MapSelection>();
   private bool m_showHighestInfluencePoint;


   //-----------------------------------------------------------------------------------------------
   [MenuItem ("Influence/Influence Debugger")]
   public static void ShowWindow()
   {
      GetWindow(typeof(InfluenceEditorWindow), false, "Influence Debugger");
   }

   
   //-----------------------------------------------------------------------------------------------
   public void Awake()
   {
      GetInfluenceSystemReferencesIfNeeded();

      m_influenceGameManager.Awake();
      m_influenceGameManager.ReloadInfluenceMapData();
      m_influenceGameManager.RegisterAllActiveInfluenceObjectsInScene();
      m_influenceGameManager.UpdateInfluenceSystem();
   }


   //-----------------------------------------------------------------------------------------------
   public void OnDestroy()
   {
      SceneView.onSceneGUIDelegate -= this.OnSceneGUI;

      DestroyMapIfNeeded();
   }


   //-----------------------------------------------------------------------------------------------
   public void OnEnable()
   {
      SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
      SceneView.onSceneGUIDelegate += this.OnSceneGUI;
      hideFlags = HideFlags.HideAndDontSave;

      GetInfluenceSystemReferencesIfNeeded();
      m_influenceGameManager.ReloadInfluenceMapData();
      m_influenceGameManager.RegisterAllActiveInfluenceObjectsInScene();
      m_influenceGameManager.UpdateInfluenceSystem();
      
      //ConstructMapAtIndex();
   }


   //-----------------------------------------------------------------------------------------------
   public void GetInfluenceSystemReferencesIfNeeded()
   {
      if (m_influenceGameManager == null)
      {
         m_influenceGameManager = FindObjectOfType<InfluenceGameManager>();
      }

      if (m_influenceSystemRef == null)
      {
         m_influenceSystemRef = InfluenceSystem.GetInstance();
      }
   }


   //-----------------------------------------------------------------------------------------------
   public void Update()
   {
      GetInfluenceSystemReferencesIfNeeded();
      m_influenceGameManager.UpdateInfluenceSystem();


      if (m_debugOverlayQuad == null)
      {
         m_debugOverlayQuad = GameObject.Find("Influence Debug Overlay");
      }

      if ((m_mapSelections.Count == 0)
         && (m_debugOverlayQuad != null))
      {
         DestroyImmediate(m_debugOverlayQuad);
      }
      else if (m_mapSelections.Count != 0)
      {
         UpdateInfluenceOverlay();
      }
   }


//-----------------------------------------------------------------------------------------------
public void OnGUI()
   {
      float showGUIWhenManagerPresent = 0f;
      if (m_influenceGameManager != null)
      {
         showGUIWhenManagerPresent = 1f;
      }

      if (EditorGUILayout.BeginFadeGroup(showGUIWhenManagerPresent))
      {
         DrawGUI();
      }
      else
      {
         EditorGUILayout.LabelField("No InfluenceGameManager in scene!");
      }

      EditorGUILayout.EndFadeGroup();
   }


   //-----------------------------------------------------------------------------------------------
   private void DrawGUI()
   {
      // Draw influence data reload buttons
      EditorGUILayout.BeginHorizontal();
      {
         if (GUILayout.Button("Reload Influence Data Files"))
         {
            m_influenceGameManager.ReloadInfluenceMapData();
         }
         
         if (GUILayout.Button("Reload Influence Objects"))
         {
            m_influenceGameManager.ReloadInfluenceObjectData();
         }
      }
      EditorGUILayout.EndHorizontal();

      // Influence maps to render
      m_showHighestInfluencePoint = EditorGUILayout.ToggleLeft("Show Highest Influence Points", m_showHighestInfluencePoint);

      DrawInfluenceMapList();

      // If anything changed, we redraw the scene
      if (GUI.changed)
      {
         SceneView.RepaintAll();
      }
   }


   //-----------------------------------------------------------------------------------------------
   private void CreateInfluenceOverlay()
   {
      m_debugOverlayMaterial = new Material(Shader.Find("Unlit/DebugInfluenceMap"));

      m_debugOverlayQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
      Renderer debugRenderer = m_debugOverlayQuad.GetComponent<Renderer>();
      debugRenderer.material = m_debugOverlayMaterial;
      debugRenderer.sortingLayerName = "Debug";
      m_debugOverlayQuad.name = "Influence Debug Overlay";
      m_debugOverlayQuad.transform.position = new Vector3(15f, 15f);
      m_debugOverlayQuad.transform.localScale = new Vector3(30f, 30f);
   }
      
      
   //-----------------------------------------------------------------------------------------------
   private void UpdateInfluenceOverlay()
   {
      if (m_debugOverlayQuad == null)
      {
         CreateInfluenceOverlay();
      }


      for (int i = 0; i < 8; ++i)
      {
         if ((i >= m_mapSelections.Count)
            || m_mapSelections[i].m_isHidden)
         {
            m_debugOverlayMaterial.SetColor("_InfluenceColor" + i, Color.black);
            continue;
         }

         m_mapSelections[i].m_activeMap = ConstructMapAtIndex(m_mapSelections[i].m_selectedMapIndex);
         if (m_mapSelections[i].m_activeMap == null)
         {
            m_debugOverlayMaterial.SetColor("_InfluenceColor" + i, Color.black);
            continue;
         }

         Texture2D influenceTexture = InfluenceUnityUtils.GetInfluenceMapAsTexture(m_mapSelections[i].m_activeMap);
         m_debugOverlayMaterial.SetTexture("_InfluenceTex" + i, influenceTexture);
         m_debugOverlayMaterial.SetColor("_InfluenceColor" + i, m_mapSelections[i].m_renderColor);
      }
   }


   //-----------------------------------------------------------------------------------------------
   private void DrawMap()
   {
      if (m_debugOverlayQuad == null)
      {
         CreateInfluenceOverlay();
      }
   }


   //-----------------------------------------------------------------------------------------------
   private WorkingMap ConstructMapAtIndex(int mapIndex)
   {
      if (mapIndex == 0)
      {
         return null;
      }

      mapIndex -= 1;
      WorkingMap newMap = null;
      if (mapIndex >= m_influenceSystemRef.InfluenceMaps.Count)
      {
         mapIndex -= m_influenceSystemRef.InfluenceMaps.Count;
         newMap = ConstructActiveMapFromMapFormula(mapIndex);
      }
      else
      {
         newMap = ConstructActiveMapFromBaseMap(mapIndex);
      }

      return newMap;
   }


   //-----------------------------------------------------------------------------------------------
   private WorkingMap ConstructActiveMapFromBaseMap(int indexIntoMapList)
   {
      WorkingMap activeMap = new WorkingMap();
      activeMap.AddMap(m_influenceSystemRef.InfluenceMaps[indexIntoMapList], 1f);
      activeMap.Normalize();
      return activeMap;
   }


   //-----------------------------------------------------------------------------------------------
   private WorkingMap ConstructActiveMapFromMapFormula(int indexIntoFormulaList)
   {
      MapFormula formulaToUse = m_influenceSystemRef.InfluenceMapFormulas[indexIntoFormulaList];
      WorkingMap activeMap = formulaToUse.ConstructMapFromFormula();
      activeMap.Normalize();
      return activeMap;
   }


   //-----------------------------------------------------------------------------------------------
   private void DestroyMapIfNeeded()
   {
      if (m_debugOverlayQuad == null)
      {
         return;
      }

      DestroyImmediate(m_debugOverlayQuad);
   }


   //-----------------------------------------------------------------------------------------------
   public void OnProjectChange()
   {
      GetInfluenceSystemReferencesIfNeeded();
      m_influenceGameManager.ReloadInfluenceMapData();
      m_influenceGameManager.ReloadInfluenceObjectData();
      //SelectedMapIndex = SelectedMapIndex;
   }


   //-----------------------------------------------------------------------------------------------
   public void OnSelectionChange()
   {
      GetInfluenceSystemReferencesIfNeeded();
      m_influenceGameManager.UpdateInfluenceSystem();
      //SelectedMapIndex = SelectedMapIndex;
   }


   //-----------------------------------------------------------------------------------------------
   private string[] ConstructMapNameList()
   {
      List<BaseMap> baseInfluenceMaps = m_influenceSystemRef.InfluenceMaps;
      List<MapFormula> influenceMapFormulas = m_influenceSystemRef.InfluenceMapFormulas;

      List<string> mapNames = new List<string>();
      mapNames.Add("None");
      foreach(BaseMap baseMap in baseInfluenceMaps)
      {
         mapNames.Add("Base_" + baseMap.InfluenceID + "_" + baseMap.ObjectTag);
      }

      foreach(MapFormula formula in influenceMapFormulas)
      {
         mapNames.Add("Formula_" + formula.FormulaID);
      }

      return mapNames.ToArray();
   }


   //-----------------------------------------------------------------------------------------------
   public void OnSceneGUI(SceneView sceneView)
   {
      if (!m_showHighestInfluencePoint)
      {
         return;
      }

      // Save previous state
      Color oldColor = Handles.color;

      foreach (MapSelection selection in m_mapSelections)
      {
         if ((selection.m_isHidden)
            || (selection.m_activeMap == null))
         {
            continue;
         }

         Color mapColor = selection.m_renderColor;
         Color outerColor = new Color(mapColor.r, mapColor.g, mapColor.b, 1f);
         Color innerColor = new Color(mapColor.r, mapColor.g, mapColor.b, .3f);         

         InfluenceMapPoint highestPoint = selection.m_activeMap.GetPointOfHighestInfluence();
         InfluenceObjectWorldPoint worldPos = m_influenceSystemRef.ConvertMapPosToWorldPos(highestPoint);
         Vector3 unityWorldPos = new Vector3(worldPos.x, worldPos.y, 0f);

         // Draw point
         Handles.color = innerColor;
         Handles.DrawSolidDisc(unityWorldPos, new Vector3(0f, 0f, 1f), .3f);

         Handles.color = outerColor;
         Handles.DrawWireDisc(unityWorldPos, new Vector3(0f, 0f, 1f), .3f);
      }

      // Restore old state
      Handles.color = oldColor;
   }


   //-----------------------------------------------------------------------------------------------
   private void DrawInfluenceMapList()
   {
      int desiredMapCount = EditorGUILayout.IntSlider("Number of Debug Maps:", m_mapSelections.Count, 0, 8);

      while (desiredMapCount < m_mapSelections.Count)
      {
         m_mapSelections.RemoveAt(m_mapSelections.Count - 1);
      }
      while (desiredMapCount > m_mapSelections.Count)
      {
         m_mapSelections.Add(new MapSelection());
      }

      string[] mapNameArray = ConstructMapNameList();
      foreach (MapSelection selection in m_mapSelections)
      {
         EditorGUILayout.BeginHorizontal();
         {
            selection.m_isHidden = EditorGUILayout.Toggle("Hide", selection.m_isHidden);
            selection.m_selectedMapIndex = EditorGUILayout.Popup("Influence Map", selection.m_selectedMapIndex, mapNameArray);
            selection.m_renderColor = EditorGUILayout.ColorField("Influence Color", selection.m_renderColor);
         }
         EditorGUILayout.EndHorizontal();
      }
   }


   //-----------------------------------------------------------------------------------------------
   private void AddNewMapToList()
   {
      m_mapSelections.Add(new MapSelection());
   }

}
