using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    public GameObject emptyPrefab;      
    public GameObject outsideCorner;   
    public GameObject outsideWall;      
    public GameObject insideCorner;     
    public GameObject insideWall;       
    public GameObject pelletPrefab;     
    public GameObject powerPelletPrefab; 
    public GameObject tJunction;         
    public GameObject ghostExit;         

    public float tileSize = 1.0f;

    int[,] levelMap =
    {
        {1,2,2,2,2,2,2,2,2,2,2,2,2,7},
        {2,5,5,5,5,5,5,5,5,5,5,5,5,4},
        {2,5,3,4,4,3,5,3,4,4,4,3,5,4},
        {2,6,4,0,0,4,5,4,0,0,0,4,5,4},
        {2,5,3,4,4,3,5,3,4,4,4,3,5,3},
        {2,5,5,5,5,5,5,5,5,5,5,5,5,5},
        {2,5,3,4,4,3,5,3,3,5,3,4,4,4},
        {2,5,3,4,4,3,5,4,4,5,3,4,4,3},
        {2,5,5,5,5,5,5,4,4,5,5,5,5,4},
        {1,2,2,2,2,1,5,4,3,4,4,3,0,4},
        {0,0,0,0,0,2,5,4,3,4,4,3,0,3},
        {0,0,0,0,0,2,5,4,4,0,0,0,0,0},
        {0,0,0,0,0,2,5,4,4,0,3,4,4,8},
        {2,2,2,2,2,1,5,3,3,0,4,0,0,0},
        {0,0,0,0,0,0,5,0,0,0,4,0,0,0},
    };

    class TileInfo
    {
        public GameObject prefab;
        public Vector3 position;
        public float rotation;
        public int tileType;
    }

    void Start()
    {
        GenerateLevel();
    }

    void GenerateLevel()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        int rows = levelMap.GetLength(0);
        int cols = levelMap.GetLength(1);

        List<TileInfo> originals = new List<TileInfo>();

        // 生成左上象限
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                int tile = levelMap[y, x];
                if (tile == 0) continue; // 空白不生成

                GameObject prefab = GetPrefab(tile);
                if (prefab != null)
                {
                    Vector3 pos = new Vector3(x * tileSize, -y * tileSize, 0);
                    float rot = GetRotation(levelMap, x, y, tile);
                    Instantiate(prefab, pos, Quaternion.Euler(0, 0, rot), transform);

                    originals.Add(new TileInfo
                    {
                        prefab = prefab,
                        position = pos,
                        rotation = rot,
                        tileType = tile
                    });
                }
            }
        }
    }

    GameObject GetPrefab(int tile)
    {
        switch (tile)
        {
            case 0: return emptyPrefab;
            case 1: return outsideCorner;
            case 2: return outsideWall;
            case 3: return insideCorner;
            case 4: return insideWall;
            case 5: return pelletPrefab;
            case 6: return powerPelletPrefab;
            case 7: return tJunction;
            case 8: return ghostExit;
            default: return null;
        }
    }

    float GetRotation(int[,] map, int x, int y, int tile)
    {
        int rows = map.GetLength(0);
        int cols = map.GetLength(1);

        int right = (x + 1 < cols) ? map[y, x + 1] : -1;
        int down  = (y + 1 < rows) ? map[y + 1, x] : -1;
        int left  = (x - 1 >= 0)   ? map[y, x - 1] : -1;
        int up    = (y - 1 >= 0)   ? map[y - 1, x] : -1;

        switch (tile)
        {
            case 1: 
            {
                bool r = (right == 2);
                bool d = (down  == 2);
                bool l = (left  == 2);
                bool u = (up    == 2);

                if (r && d) return 0f;
                if (r && u) return 90f;
                if (l && u) return 180f;
                if (l && d) return 270f;
                return 0f;
            }

            case 2:
            {
                bool horizontal = (left == tile) || (right == tile);
                return horizontal ? 0f : 90f;
            }

            case 4:
            {
                bool r = right is 4 or 3;
                bool d = (down is 4 or 3);
                bool l = (left is 4 or 3);
                bool u = (up   is 4 or 3);
                bool horizontal = (left == tile) || (right == tile);
                if (l && r) horizontal = true;
                if (u && d) horizontal = false;
                return horizontal ? 0f : 90f;
            }

            case 3: 
            {
                bool r = right is 4 or 3;
                bool d = (down is 4 or 3);
                bool l = (left is 4 or 3);
                bool u = (up   is 4 or 3);

                if (right == 4 && down == 4) return 0;
                if (up == 4 && right == 4) return 90;
                if (left == 4 && down == 4) return 270;
                
                if (r && d) return 0f;
                if (r && u) return 90f;
                if (l && u) return 180f;
                if (l && d) return 270f;

                if (u) return 90f;
                return 0f;
            }

            case 7: 
                return 0f;

            default:
                return 0f;
        }
    }
 

}
