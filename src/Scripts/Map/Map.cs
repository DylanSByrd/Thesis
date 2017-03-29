using UnityEngine;


//-----------------------------------------------------------------------------------------------
public class Map : MonoBehaviour
{
   //-----------------------------------------------------------------------------------------------
   public int ColumnCount
   {
      get { return m_numColumns; }
   }

   public int RowCount
   {
      get { return m_numRows; }
   }

   public Tile[] Tiles
   {
      get { return m_tiles; }
   }


   //-----------------------------------------------------------------------------------------------
   public GameObject FloorPrefab;
   public GameObject WallPrefab;
   public GameObject CoverPrefab;


   //-----------------------------------------------------------------------------------------------
   [SerializeField]
   private int m_numColumns;

   [SerializeField]
   private int m_numRows;

   [SerializeField]
   private Tile[] m_tiles;


   //-----------------------------------------------------------------------------------------------
   public void TryResizeMap(int newNumRows, int newNumColumns)
   {
      if ((newNumRows == RowCount) && (newNumColumns == ColumnCount))
      {
         return;
      }

      if (Tiles != null)
      {
         foreach (Tile tile in Tiles)
         {
            if (tile != null)
            {
               DestroyImmediate(tile.gameObject);
            }
         }
      }

      Tile[] newTiles = new Tile[newNumRows * newNumColumns];

      m_numRows = newNumRows;
      m_numColumns = newNumColumns;
      m_tiles = newTiles;

      InitializeTiles();
   }


   //-----------------------------------------------------------------------------------------------
   public int GetTileIndexForCoords(int x, int y)
   {
      return (y * ColumnCount) + x;
   }
        
        
   //-----------------------------------------------------------------------------------------------
   public bool AreCoordinatesOnMapEdge(int x, int y)
   {
      const bool COORDS_ARE_ON_EDGE = true;

      if (x == 0 || (x == (ColumnCount - 1)) || y == 0 || (y == (RowCount - 1)))
      {
         return COORDS_ARE_ON_EDGE;
      }

      return !COORDS_ARE_ON_EDGE;
   }
        
        
   //-----------------------------------------------------------------------------------------------
   public void InitializeTiles()
   {
      int tileIndex = 0;
      for (int rowIndex = 0; rowIndex < RowCount; ++rowIndex)
      {
         for (int colIndex = 0; colIndex < ColumnCount; ++colIndex)
         {
            if ((rowIndex == 0) || (colIndex == 0) || (rowIndex == RowCount - 1) || (colIndex == ColumnCount - 1))
            {
               Tile newTile = Instantiate(WallPrefab.GetComponent<Tile>());
               Tiles[tileIndex] = newTile;
            }
            else
            {
               Tile newTile = Instantiate(FloorPrefab.GetComponent<Tile>());
               Tiles[tileIndex] = newTile;
            }

            Tiles[tileIndex].transform.parent = transform;
            Tiles[tileIndex].transform.position = new Vector2(rowIndex + .5f, colIndex + .5f);

            ++tileIndex;
         }
      }
   }
}
