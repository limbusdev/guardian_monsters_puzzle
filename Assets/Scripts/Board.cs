using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    WAIT, MOVE
}

public class Board : MonoBehaviour
{
    public GameState currentState = GameState.MOVE;
    public int width;
    public int height;
    public int offset;
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
                var tempPosition = new Vector2(col, row + offset);
                var backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity);
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "(" + col + "|" + row + ")";

                int blockElement = Random.Range(0, blocks.Length);

                int maxIterations = 0;
                while(MatchesAt(col, row, blocks[blockElement]) && maxIterations < 100)
                {
                    blockElement = Random.Range(0, blocks.Length);
                    maxIterations++;
                }

                var block = Instantiate(blocks[blockElement], tempPosition, Quaternion.identity);
                block.GetComponent<Block>().row = row;
                block.GetComponent<Block>().col = col;
                block.transform.parent = this.transform;
                block.name = "(" + col + "|" + row + ")";
                allBlocks[col, row] = block;
            }
        }
    }

    private bool MatchesAt(int col, int row, GameObject block)
    {
        if(col > 1 && row > 1)
        {
            if(allBlocks[col-1,row].tag == block.tag && allBlocks[col-2,row].tag == block.tag)
            {
                return true;
            }

            if(allBlocks[col, row-1].tag == block.tag && allBlocks[col, row-2].tag == block.tag)
            {
                return true;
            }
        } else if(col <= 1 || row <= 1)
        {
            if(row > 1)
            {
                if(allBlocks[col, row-1].tag == block.tag && allBlocks[col, row-2].tag == block.tag)
                {
                    return true;
                }

                if(col > 1)
                {
                    if(allBlocks[col - 1, row].tag == block.tag && allBlocks[col - 2, row].tag == block.tag)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private void DestroyMatchesAt(int col, int row)
    {
        if(allBlocks[col, row].GetComponent<Block>().isMatched)
        {
            Destroy(allBlocks[col, row]);
            allBlocks[col, row] = null;
        }
    }

    public void DestroyMatches()
    {
        for(int col = 0; col < width; col++)
        {
            for(int row = 0; row < height; row++)
            {
                if(allBlocks[col, row] != null)
                {
                    DestroyMatchesAt(col, row);
                }
            }
        }
        StartCoroutine(DecreaseRowCo());
    }

    // Co-Routine to collapse Columns
    private IEnumerator DecreaseRowCo()
    {
        int nullCount = 0;

        for(int col = 0; col < width; col++)
        {
            for(int row = 0; row < height; row++)
            {
                if(allBlocks[col, row] == null)
                {
                    nullCount++;
                } else if(nullCount > 0)
                {
                    allBlocks[col, row].GetComponent<Block>().row -= nullCount;
                    allBlocks[col, row] = null;
                }
            }
            nullCount = 0;
        }

        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoardCo());
    }


    // Refill Board
    private void RefillBoard()
    {
        for(int col=0; col<width; col++)
        {
            for(int row = 0; row<height; row++)
            {
                if(allBlocks[col, row] == null)
                {
                    var tempPosition = new Vector2(col, row + offset);
                    int blockToUse = Random.Range(0, blocks.Length);
                    var block = Instantiate(blocks[blockToUse], tempPosition, Quaternion.identity);
                    allBlocks[col, row] = block;
                    block.GetComponent<Block>().row = row;
                    block.GetComponent<Block>().col = col;
                }
            }
        }
    }

    // Checks for unwanted matches on board
    private bool MatchesOnBoard()
    {
        for(int col=0; col<width; col++)
        {
            for(int row=0; row<height; row++)
            {
                if(allBlocks[col, row] != null)
                {
                    if(allBlocks[col,row].GetComponent<Block>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoardCo()
    {
        RefillBoard();
        yield return new WaitForSeconds(.5f);

        while(MatchesOnBoard())
        {
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }
        yield return new WaitForSeconds(.5f);
        currentState = GameState.MOVE;
    }

}
