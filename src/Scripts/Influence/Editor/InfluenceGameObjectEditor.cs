using UnityEngine;
using UnityEditor;


//-----------------------------------------------------------------------------------------------
[CustomEditor(typeof(InfluenceGameObject))]
public class InfluenceGameObjectEditor : Editor
{
   //-----------------------------------------------------------------------------------------------
   public override void OnInspectorGUI()
   {
      DrawDefaultInspector();
   }
}
