//-----------------------------------------------------------------------------------------------
using UnityEngine;
using HTN;
using System.Collections;
using System;

//-----------------------------------------------------------------------------------------------
public class ToggleButtonOperator 
   : Operator
{
   //-----------------------------------------------------------------------------------------------
   public Button ButtonToToggle { get; set; }


   //-----------------------------------------------------------------------------------------------
   public ToggleButtonOperator()
      : base("ToggleButtonOperator")
   { }


   //-----------------------------------------------------------------------------------------------
   public override void Run()
   {
      ButtonToToggle.Toggle();
   }


   //-----------------------------------------------------------------------------------------------
   public override Operator Clone()
   {
      ToggleButtonOperator clone = new ToggleButtonOperator();
      clone.m_params = m_params;
      return clone;
   }
   
   
   //-----------------------------------------------------------------------------------------------
   public override void AssignVariables(Agent owner)
   {
      AgentToOperateOn = owner;

      GameObject[] buttons = GameObject.FindGameObjectsWithTag("Button");

      foreach (GameObject buttonObj in buttons)
      {
         Button buttonComponent = buttonObj.GetComponent<Button>();

         if (buttonComponent.ClaimingAgent == AgentToOperateOn)
         {
            ButtonToToggle = buttonComponent;
            return;
         }
      }

      throw new ArgumentException("No buttons claimed for toggle button operator!");
   }
}
