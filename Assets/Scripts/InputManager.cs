using UnityEngine;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{
    private Tile startTile;
    private Tile currentTile;
    private List<Tile> linkedTiles = new List<Tile>();
    private Dictionary<Tile, Vector3> originalPositions = new Dictionary<Tile, Vector3>();
    private bool isDragging = false;

    public BoardManager boardManager;
    public TileManager tileManager; // TileManager referansı
    public Color selectedColor = Color.gray;
    public float pushDistance = 0.2f;
    private void Start()
    {
        tileManager = new TileManager(boardManager);

        if (tileManager == null)
        {
            Debug.LogError("TileManager is null!");
        }
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnInputStart();
        }

        if (isDragging)
        {
            OnInputDrag();
        }

        if (Input.GetMouseButtonUp(0))
        {
            OnInputEnd();
        }
    }

    private void OnInputStart()
    {
        Tile tile = GetTileUnderMouse();

        if (tile != null && tile.ColorID != -1)
        {
            startTile = tile;
            currentTile = tile;
            AddTileToLink(tile);
            tileManager.PushNeighbors(tile, linkedTiles, pushDistance, originalPositions);
            isDragging = true;
        }
    }

    private void OnInputDrag()
    {
        Tile tile = GetTileUnderMouse();

        if (tile == currentTile) return;

        if (tile != null && tile.ColorID == startTile.ColorID)
        {
            if (!tileManager.IsTileLinkable(startTile, currentTile, tile))
            {
                return;
            }
            
            tileManager.ResetMovedNeighbors(originalPositions);
            
            if (linkedTiles.Count > 1 && tile == linkedTiles[linkedTiles.Count - 2])// Linklenmiş tile'a tekrar girme state'i
            {
                RemoveLastTileFromLink();
                tileManager.PushNeighbors(tile, linkedTiles, pushDistance, originalPositions);
            }
            else if (!linkedTiles.Contains(tile))// Linklenmemiş ama linklenebilir tile'a girme state'i
            {
                AddTileToLink(tile);
                tileManager.PushNeighbors(tile, linkedTiles, pushDistance, originalPositions);
            }

            currentTile = tile;
        }
    }

    private void OnInputEnd()
    {
        isDragging = false;

        if (linkedTiles.Count >= 3)
        {
            boardManager.DestroyTiles(linkedTiles);
        }
        else
        {
            tileManager.ResetTileColors(linkedTiles);
        }

        tileManager.ResetMovedNeighbors(originalPositions);
        linkedTiles.Clear();
        startTile = null;
        currentTile = null;
    }

    private Tile GetTileUnderMouse()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int gridPosition = boardManager.GetGridPositionFromWorld(mouseWorldPosition);

        if (boardManager.IsInsideBoard(gridPosition))
        {
            return boardManager.GetTileAtPosition(gridPosition);
        }

        return null;
    }

    private void AddTileToLink(Tile tile)
    {
        linkedTiles.Add(tile);
        tile.SetChipTemporaryColor(selectedColor);
    }

    private void RemoveLastTileFromLink()
    {
        if (linkedTiles.Count > 0)
        {
            Tile lastTile = linkedTiles[linkedTiles.Count - 1];
            linkedTiles.RemoveAt(linkedTiles.Count - 1);
            lastTile.ResetChipColor();
        }
    }
}
