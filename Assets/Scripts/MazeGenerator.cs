using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class MazeGenerator : MonoBehaviour
{
    public int width = 20;
    public int height = 20;

    public float cellSize = 4f;

    public int startWalls = 10;
    public int maxWalls = 60;

    public float updateDelay = 3f;

    public float safePlayerRadius = 6f;

    public int wallsGrowth = 1;

    public float mapMinX = -40f;
    public float mapMaxX = 40f;
    public float mapMinZ = -40f;
    public float mapMaxZ = 40f;

    public float wallHeight = 3f;
    public float spawnHeight = 1f;

    List<GameObject> walls = new List<GameObject>();

    bool[,] occupied;

    void Start()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        width = Mathf.RoundToInt((mapMaxX - mapMinX) / cellSize);
        height = Mathf.RoundToInt((mapMaxZ - mapMinZ) / cellSize);

        occupied = new bool[width, height];

        SpawnInitialWalls();

        InvokeRepeating(nameof(UpdateMaze), updateDelay, updateDelay);
    }

    void SpawnInitialWalls()
    {
        int spawned = 0;

        while (spawned < startWalls)
        {
            Vector2Int cell = RandomCell();

            if (CanSpawn(cell))
            {
                SpawnWall(cell);
                spawned++;
            }
        }
    }

    void UpdateMaze()
    {
        if (walls.Count > 0)
        {
            int index = Random.Range(0, walls.Count);
            RemoveWall(index);
        }

        int spawnAmount = 1;

        if (walls.Count < maxWalls)
        spawnAmount += wallsGrowth;

        for (int i = 0; i < spawnAmount; i++)
        {
            Vector2Int cell = RandomCell();

            if (CanSpawn(cell))
                SpawnWall(cell);
        }
    }   

    Vector2Int RandomCell()
    {
        return new Vector2Int(
            Random.Range(0, width),
            Random.Range(0, height)
        );
    }

    bool CanSpawn(Vector2Int cell)
    {
        if (occupied[cell.x, cell.y])
            return false;

        Vector3 pos = CellToWorld(cell);

        foreach (var player in FindObjectsOfType<PlayerTag>())
        {
            if (Vector3.Distance(player.transform.position, pos) < safePlayerRadius)
                return false;
        }

        return true;
    }

    void SpawnWall(Vector2Int cell)
    {
        Vector3 pos = CellToWorld(cell);

        GameObject wall = PhotonNetwork.Instantiate(
            "WallCube",
            pos,
            Quaternion.identity
        );

        wall.transform.localScale = new Vector3(
            cellSize,
            wallHeight,
            cellSize
        );

        walls.Add(wall);
        occupied[cell.x, cell.y] = true;
    }

    void RemoveWall(int index)
    {
        GameObject wall = walls[index];

        Vector2Int cell = WorldToCell(wall.transform.position);

        occupied[cell.x, cell.y] = false;

        PhotonNetwork.Destroy(wall);

        walls.RemoveAt(index);
    }

    Vector3 CellToWorld(Vector2Int cell)
    {
        return new Vector3(
            mapMinX + cell.x * cellSize,
            spawnHeight,
            mapMinZ + cell.y * cellSize
        );
    }

    Vector2Int WorldToCell(Vector3 pos)
    {
        int x = Mathf.RoundToInt((pos.x - mapMinX) / cellSize);
        int z = Mathf.RoundToInt((pos.z - mapMinZ) / cellSize);

        return new Vector2Int(x, z);
    }
}