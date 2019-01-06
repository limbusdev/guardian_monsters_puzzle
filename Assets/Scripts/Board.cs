using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject tilePrefab;
    public GameObject[] blocks;
    public GameObject[,] allBlocks;

    private BackgroundTile[,] allTiles;

    
	void Start ()
    {
        allTiles = new BackgroundTile[width, height];
        allBlocks = new GameObject[width, height];
        SetUp();
	}
	

    private void SetUp()
    {
        for(int col = 0; col < width; col++)
        {
            for(int row = 0; row < height; row++)
            {
                var tempPosition = new Vector2(col, row);
                var backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity);
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "(" + col + "|" + row + ")";

                int blockElement = Random.Range(0, 6);
                var block = Instantiate(blocks[blockElement], tempPosition, Quaternion.identity);
                block.transform.parent = this.transform;
                block.name = "(" + col + "|" + row + ")";
                allBlocks[col, row] = block;
            }
        }
    }
}
