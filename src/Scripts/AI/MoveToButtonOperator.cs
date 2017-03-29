//-----------------------------------------------------------------------------------------------
using UnityEngine;
using System;
using HTN;
using Influence;


//-----------------------------------------------------------------------------------------------
public class MoveToButtonOperator
   : Operator
{
   //-----------------------------------------------------------------------------------------------
   public GameObject ObjectToMoveTo { get; set; }


   //-----------------------------------------------------------------------------------------------
   public MoveToButtonOperator()
      : base("MoveToButtonOperator")
   { }


   //-----------------------------------------------------------------------------------------------
   public override void Run()
   {
      AgentToOperateOn.transform.position = ObjectToMoveTo.transform.position;

      InfluenceGameManager influenceManager = GameObject.FindObjectOfType<InfluenceGameManager>();
      influenceManager.UpdateInfluenceSystem();
   }


   //-----------------------------------------------------------------------------------------------
   public override void AssignVariables(Agent owner)
   {
      AgentToOperateOn = owner;

      GameObject[] buttons = GameObject.FindGameObjectsWithTag("Button");

      float minDistToFreeButton = float.MaxValue;
      Button nearestFreeButton = null;

      foreach (GameObject buttonObject in buttons)
      {
         Button currentButton = buttonObject.GetComponent<Button>();

         if (currentButton.ClaimingAgent != null)
         {
            continue;
         }

         float distToCurrentButton = Vector2.Distance(AgentToOperateOn.transform.position, buttonObject.transform.position);

         if (nearestFreeButton == null)
         {
            nearestFreeButton = currentButton;
            minDistToFreeButton = distToCurrentButton;
            continue;
         }

         if (distToCurrentButton < minDistToFreeButton)
         {
            nearestFreeButton = currentButton;
            minDistToFreeButton = distToCurrentButton;
         }
      }

      if (nearestFreeButton == null)
      {
         throw new ArgumentException("Looking for free button that does not exist!");
      }

      nearestFreeButton.Claim(AgentToOperateOn);
      ObjectToMoveTo = nearestFreeButton.gameObject;
   }


   //-----------------------------------------------------------------------------------------------
   public override Operator Clone()
   {
      MoveToButtonOperator clone = new MoveToButtonOperator();
      clone.m_params = m_params;
      return clone;
   }
}
