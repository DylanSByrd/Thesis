using UnityEngine;
using UnityEditor;


//-----------------------------------------------------------------------------------------------
[CustomEditor(typeof(Tile))]
[CanEditMultipleObjects]
public class TileEditor : Editor
{
   //-----------------------------------------------------------------------------------------------
   private Tile m_selectedTile;


   //-----------------------------------------------------------------------------------------------
   public void OnEnable()
   {
      m_selectedTile = this.target as Tile;
   }


   //-----------------------------------------------------------------------------------------------
   public override void OnInspectorGUI()
   {
      TileType newType = (TileType)EditorGUILayout.EnumPopup("Type", m_selectedTile.Type);
      
      if (GUI.changed)
      {
         m_selectedTile.Type = newType;
      }
   }
}
