using UnityEngine;
using System;
using UnityEditor;


//-----------------------------------------------------------------------------------------------
[CustomEditor(typeof(Map))]
public class MapEditor : Editor
{
   //-----------------------------------------------------------------------------------------------
   private Map m_selectedMap;

   
   //-----------------------------------------------------------------------------------------------
   public void OnEnable()
   {
      m_selectedMap = this.target as Map;
   }
        
   //-----------------------------------------------------------------------------------------------
   public override void OnInspectorGUI()
   {
      m_selectedMap.FloorPrefab = EditorGUILayout.ObjectField("Floor Prefab", m_selectedMap.FloorPrefab, typeof(GameObject), true) as GameObject;
      m_selectedMap.WallPrefab = EditorGUILayout.ObjectField("Wall Prefab", m_selectedMap.WallPrefab, typeof(GameObject), true) as GameObject;
      m_selectedMap.CoverPrefab = EditorGUILayout.ObjectField("Cover Prefab", m_selectedMap.CoverPrefab, typeof(GameObject), true) as GameObject;

      int editorRows = EditorGUILayout.DelayedIntField("Number of rows:", m_selectedMap.RowCount);
      if (editorRows < 1)
      {
         editorRows = 1;
      }

      int editorColumns = EditorGUILayout.DelayedIntField("Number of columns:", m_selectedMap.ColumnCount);
      if (editorColumns < 1)
      {
         editorColumns = 1;
      }

      m_selectedMap.TryResizeMap(editorRows, editorColumns);
   }
}
