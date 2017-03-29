using System;

/*
   Base formula for all influence calculations:

   Influence == I
   InfluenceMax == Imax
   Distance == D
   DistanceMax == Dmax
   DistanceMin == Dmin
   FalloffExponent == X

   DistanceFactor = (Clamp[0,Dmax](D - Dmin)) / Dmax
   I = Clam[0,1]((D + 1) - Dmin) * Clamp[0,Imax](Imax - (Imax * DistanceFactor)^X)
*/


//-----------------------------------------------------------------------------------------------
namespace Influence
{
   //-----------------------------------------------------------------------------------------------
   public class InfluenceFunction
   {
      //-----------------------------------------------------------------------------------------------
      public float InfluenceMax
      {
         get { return m_influenceMax; }
      }

      public uint DistanceMax
      {
         get { return m_distanceMax; }
      }

      public uint DistanceMin
      {
         get { return m_distanceMin; }
      }

      public uint FalloffExponent
      {
         get { return m_falloffExponent; }
      }

      public string InfluenceID
      {
         get { return m_influenceID; }
      }


      //-----------------------------------------------------------------------------------------------
      private float m_influenceMax;
      private uint m_distanceMax;
      private uint m_distanceMin;
      private uint m_falloffExponent;
      private string m_influenceID;


      //-----------------------------------------------------------------------------------------------
      readonly public static float DEFAULT_MAX_INFLUENCE = 1f;
      readonly public static uint DEFAULT_MAX_DISTANCE = uint.MaxValue;
      readonly public static uint DEFAULT_MIN_DISTANCE = 0U;
      readonly public static uint DEFAULT_FALLOFF_EXPONENT = 1U;


      //-----------------------------------------------------------------------------------------------
      public InfluenceFunction(float influenceMax, uint distanceMax, uint distanceMin, uint falloffExponent, string influenceID)
      {
         m_influenceMax = influenceMax;
         m_distanceMax = distanceMax;
         m_distanceMin = distanceMin;
         m_falloffExponent = falloffExponent;
         m_influenceID = influenceID;
      }


      //-----------------------------------------------------------------------------------------------
      public float CalculateInfluenceAtPoint(int x, int y, int centerX, int centerY, int templateSize)
      {
         // Use Manhattan distance to simulate propagation
         //int distance = Math.Abs(x - centerX) + Math.Abs(y - centerY);
         int xDif = centerX - x;
         int yDif = centerY - y;
         double distance = Math.Sqrt((xDif * xDif) + (yDif * yDif));

         // Check to be beyond min distance
         int distanceFromMin = (int)distance - (int)m_distanceMin;
         if (distanceFromMin < 0)
         {
            return 0f;
         }

         float distanceMultiplier = distanceFromMin;
         if (distanceMultiplier > templateSize)
         {
            distanceMultiplier = templateSize;
         }

         if (templateSize > m_distanceMax)
         {
            distanceMultiplier /= m_distanceMax;
         }
         else
         {
            distanceMultiplier /= templateSize;
         }

         float influenceFalloff = m_influenceMax * distanceMultiplier;
         influenceFalloff = ApplyExponentToFalloff(influenceFalloff);

         float influence = m_influenceMax - influenceFalloff;

         if (influence < 0f)
         {
            return 0f;
         }

         return influence;
      }
      
      
      //-----------------------------------------------------------------------------------------------
      private float ApplyExponentToFalloff(float influenceFalloff)
      {
         float product = influenceFalloff;
         for (uint numMultiplications = 1; numMultiplications <= m_falloffExponent; ++numMultiplications)
         {
            product *= influenceFalloff;
         }

         return product;
      }
   }
}
