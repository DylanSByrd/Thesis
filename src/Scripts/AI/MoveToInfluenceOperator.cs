//-----------------------------------------------------------------------------------------------
using UnityEngine;
using HTN;
using Influence;
using System;


//-----------------------------------------------------------------------------------------------
public class MoveToInfluenceOperator
   : Operator
{
   //-----------------------------------------------------------------------------------------------
   public MoveToInfluenceOperator()
      : base("MoveToInfluenceOperator")
   { }


   //-----------------------------------------------------------------------------------------------
   public override void Run()
   {
      OperatorParam influenceMapParamName = FindParamWithName("Influence");

      // Can't exactly move if we don't have a goal
      if (influenceMapParamName == null)
      {
         return;
      }

      string[] mapIdentifiers = influenceMapParamName.Value.Split('_');

      // Determine map type
      InfluenceSystem influenceSys = InfluenceSystem.GetInstance();
      InfluenceMapPoint mapPoint;
      if (mapIdentifiers[0].Equals("Base", StringComparison.CurrentCultureIgnoreCase))
      {
         WorkingMap queryMap = new WorkingMap();
         queryMap.AddMap(influenceSys.GetInfluenceMapByIDWithTag(mapIdentifiers[1], mapIdentifiers[2]));
         queryMap.Normalize();
         mapPoint = queryMap.GetPointOfHighestInfluence();
      }
      else if (mapIdentifiers[0].Equals("Formula", StringComparison.CurrentCultureIgnoreCase))
      {
         MapFormula formulaToUse = influenceSys.GetMapFormulaByID(mapIdentifiers[1]);
         WorkingMap queryMap = formulaToUse.ConstructMapFromFormula();
         mapPoint = queryMap.GetPointOfHighestInfluence();
      }
      else
      {
         throw new ArgumentException("Invalid name for influence map in move to operator!");
      }

      InfluenceObjectWorldPoint worldPoint = influenceSys.ConvertMapPosToWorldPos(mapPoint);
      Vector2 newLocation = new Vector2(worldPoint.x, worldPoint.y);
      AgentToOperateOn.transform.position = newLocation;

      InfluenceGameManager influenceManager = GameObject.FindObjectOfType<InfluenceGameManager>();
      influenceManager.UpdateInfluenceSystem();
   }


   //-----------------------------------------------------------------------------------------------
   public override Operator Clone()
   {
      MoveToInfluenceOperator clone = new MoveToInfluenceOperator();
      clone.m_params.AddRange(m_params);
      return clone;
   }


   //-----------------------------------------------------------------------------------------------
   public override void AssignVariables(Agent owner)
   {
      AgentToOperateOn = owner;
   }
}
