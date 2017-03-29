using UnityEngine;


//-----------------------------------------------------------------------------------------------
public enum TileType : byte
{
    TILE_TYPE_FLOOR,
    TILE_TYPE_WALL,
    TILE_TYPE_COVER,
}


//-----------------------------------------------------------------------------------------------
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(InfluenceGameObject))]
public class Tile : MonoBehaviour
{
   //-----------------------------------------------------------------------------------------------
   public TileType Type
   {
      get { return m_type; }

      set
      {
         m_type = value;
         InitTileBasedOnType();
      }
   }


   //-----------------------------------------------------------------------------------------------
   [SerializeField]
   private TileType m_type;
   private SpriteRenderer m_renderer;
   private BoxCollider2D m_collision;
   private InfluenceGameObject m_influenceGameObject;


   //-----------------------------------------------------------------------------------------------
   private void InitTileBasedOnType()
   {
      if (m_renderer == null)
      {
         m_renderer = GetComponent<SpriteRenderer>();
      }

      if (m_collision == null)
      {
         m_collision = GetComponent<BoxCollider2D>();
      }

      if (m_influenceGameObject == null)
      {
         m_influenceGameObject = GetComponent<InfluenceGameObject>();
      }

      switch (Type)
      {
         case TileType.TILE_TYPE_FLOOR:
         {
            m_renderer.sprite = Resources.Load<Sprite>("Sprites/Square");
            m_renderer.color = Color.HSVToRGB(0, 0, 88f / 255f);

            m_collision.enabled = false;
            m_influenceGameObject.enabled = false;


            break;
         }

         case TileType.TILE_TYPE_WALL:
         {
            m_renderer.sprite = Resources.Load<Sprite>("Sprites/Square");
            m_renderer.color = Color.HSVToRGB(0, 0, 0);

            m_collision.enabled = true;
            m_influenceGameObject.enabled = false;


            break;
         }

         case TileType.TILE_TYPE_COVER:
         {
            m_renderer.sprite = Resources.Load<Sprite>("Sprites/Square_Grid");
            m_renderer.color = Color.white;

            m_collision.enabled = true;
            m_influenceGameObject.enabled = true;

            break;
         }
      }
   }
}