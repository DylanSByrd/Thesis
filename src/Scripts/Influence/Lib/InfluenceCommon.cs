﻿/*
   This file contains a collection of helper structs and classes for the influence system.
*/


//-----------------------------------------------------------------------------------------------
namespace Influence
{
   //-----------------------------------------------------------------------------------------------
   public struct InfluenceMapPoint
   {
      //-----------------------------------------------------------------------------------------------
      public uint x;
      public uint y;


      //-----------------------------------------------------------------------------------------------
      public InfluenceMapPoint(uint x, uint y)
      {
         this.x = x;
         this.y = y;
      }
   }


   //-----------------------------------------------------------------------------------------------
   public struct InfluenceObjectWorldPoint
   {
      //-----------------------------------------------------------------------------------------------
      public float x;
      public float y;


      //-----------------------------------------------------------------------------------------------
      public InfluenceObjectWorldPoint(float x, float y)
      {
         this.x = x;
         this.y = y;
      }
   }
}