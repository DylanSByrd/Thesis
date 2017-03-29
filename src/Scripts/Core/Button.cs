using UnityEngine;
using System;


//-----------------------------------------------------------------------------------------------
public class Button : MonoBehaviour
{
   //-----------------------------------------------------------------------------------------------
   public Agent ClaimingAgent
   {
      get { return m_claimingAgent; }
   }
   

   //-----------------------------------------------------------------------------------------------
   public Tile[] tilesToToggle;


   //-----------------------------------------------------------------------------------------------
   private Agent m_claimingAgent;


   //-----------------------------------------------------------------------------------------------
   public void Toggle()
   {
      foreach (Tile tile in tilesToToggle)
      {
         tile.Type = TileType.TILE_TYPE_WALL;
      }
   }


   //-----------------------------------------------------------------------------------------------
   public void Claim(Agent agentToClaim)
   {
      if (m_claimingAgent != null)
      {
         throw new ArgumentException("Button already claimed!");
      }

      m_claimingAgent = agentToClaim;
   }


   //-----------------------------------------------------------------------------------------------
   public void ClearClaim()
   {
      m_claimingAgent = null;
   }
}
