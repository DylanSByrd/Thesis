using UnityEngine;
using System.Collections.Generic;
using HTN;


//-----------------------------------------------------------------------------------------------
public struct AgentRestoreState
{
   public Agent m_agent;
   public Vector2 m_positionToRestoreTo;
}


//-----------------------------------------------------------------------------------------------
public class StrategyEditorRestorePoint : ScriptableObject
{
   //-----------------------------------------------------------------------------------------------
   public List<AgentRestoreState> m_agentStatesToRestoreTo = new List<AgentRestoreState>();
   public Button[] m_buttonsToReset;


   //-----------------------------------------------------------------------------------------------
   public void Restore()
   {
      foreach (AgentRestoreState state in m_agentStatesToRestoreTo)
      {
         state.m_agent.transform.position = state.m_positionToRestoreTo;
      }

      foreach (Button button in m_buttonsToReset)
      {
         button.ClearClaim();
         foreach (Tile tile in button.tilesToToggle)
         {
            tile.Type = TileType.TILE_TYPE_FLOOR;
         }
      }
   }
}
