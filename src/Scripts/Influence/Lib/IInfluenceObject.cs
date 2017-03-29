/*
   This file serves as an interface for potential influence objects to be tracked by the
   influence system.
*/


//-----------------------------------------------------------------------------------------------
using System.Collections.Generic;


//-----------------------------------------------------------------------------------------------
namespace Influence
{
   //-----------------------------------------------------------------------------------------------
   public interface IInfluenceObject
   {
      //-----------------------------------------------------------------------------------------------
      InfluenceObjectWorldPoint WorldPosition { get; }
      float Rotation { get; }
      string ObjectTag { get; }
      string ThreatTag { get; }
      Dictionary<string, uint> InfluenceIDToTemplateSizeDictionary { get; }


      //-----------------------------------------------------------------------------------------------
      uint GetTemplateSizeForInfluenceType(string influenceID);
   }
}
