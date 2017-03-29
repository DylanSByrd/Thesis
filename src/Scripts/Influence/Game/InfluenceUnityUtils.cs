using UnityEngine;
using Influence;


//-----------------------------------------------------------------------------------------------
public static class InfluenceUnityUtils
{
   //-----------------------------------------------------------------------------------------------
   public static Texture2D GetInfluenceMapAsTexture(BaseMap map)
   {
      byte[] dataAsByte = new byte[map.Data.Length];

      for (int pixelIndex = 0; pixelIndex < map.Data.Length; ++pixelIndex)
      {
         dataAsByte[pixelIndex] = (byte)(map.Data[pixelIndex] * 255);
      }

      Texture2D mapTexture = new Texture2D((int)BaseMap.INFLUENCE_MAP_RESOLUTION, (int)BaseMap.INFLUENCE_MAP_RESOLUTION, TextureFormat.Alpha8, false);
      mapTexture.LoadRawTextureData(dataAsByte);
      mapTexture.Apply(false);

      return mapTexture;
   }


   //-----------------------------------------------------------------------------------------------
   public static Texture2D GetInfluenceMapAsTexture(WorkingMap map)
   {
      byte[] dataAsByte = new byte[map.Data.Length];

      for (int pixelIndex = 0; pixelIndex < map.Data.Length; ++pixelIndex)
      {
         float influenceValue = map.Data[pixelIndex];

         if (influenceValue < 0f)
         {
            dataAsByte[pixelIndex] = 0;
         }
         else if (influenceValue > 1f)
         {
            dataAsByte[pixelIndex] = 1;
         }
         else
         {
            dataAsByte[pixelIndex] = (byte)(map.Data[pixelIndex] * 255);
         }
      }

      Texture2D mapTexture = new Texture2D((int)BaseMap.INFLUENCE_MAP_RESOLUTION, (int)BaseMap.INFLUENCE_MAP_RESOLUTION, TextureFormat.Alpha8, false, false);
      mapTexture.LoadRawTextureData(dataAsByte);
      mapTexture.Apply();

      return mapTexture;
   }
}
