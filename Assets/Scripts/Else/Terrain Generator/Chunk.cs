using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Chunk : MonoBehaviour
{
    public Vector3Int chunkPosition;
    public Vector3Int dimensions;
    public Block[,,] terrainData;

    public Neighbours<Chunk> neighBours = new Neighbours<Chunk>();

    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public MeshCollider meshCollider;

    public Mesh terrainModel;
    private List<Vector3> terrainVertices = new List<Vector3>();
    private List<int> terrainTriangles = new List<int>();
    private List<Vector2> terrainUVs = new List<Vector2>();

    public bool shouldUpdateTerrainModel = false;
    private List<Block> calcShapeBlocksTodoList = new List<Block>();

    public void Init(Material mat)
    {
        terrainModel = new Mesh();
        terrainModel.MarkDynamic();
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshCollider = gameObject.AddComponent<MeshCollider>();
        meshRenderer.sharedMaterial = mat;
        meshFilter.mesh = terrainModel;
    }

    public void AddGeometryBasedOnBlockShape(Block block)
    {
        //VOLLE BLOKKEN
        if (block.blockShape == BlockShape.FULL)
        {
            //ShapeGenerator.AddCube(block, ref terrainVertices, ref terrainTriangles, ref terrainUVs);
            ShapeGenerator.AddOptimizedCube(block, ref terrainVertices, ref terrainTriangles, ref terrainUVs);
        }
        //VIERKANTEN
        else if (block.blockShape >= BlockShape.DIAGONAL_BACK && block.blockShape <= BlockShape.SLOPE_RIGHT_INV)
        {
            ShapeGenerator.AddQuad(block, ref terrainVertices, ref terrainTriangles, ref terrainUVs);
        }
        //DRIEHOEKEN
        else if (block.blockShape >= BlockShape.CORNER_BACK_LEFT && block.blockShape <= BlockShape.CORNER_FRONT_RIGHT_CONCAVE_INV)
        {
            ShapeGenerator.AddTriangle(block, ref terrainVertices, ref terrainTriangles, ref terrainUVs);
        }
    }

    public void BlockLoop(out Block block, Action action)
    {
        block = default;
        for (int x = 0; x < dimensions.x; x++)
        {
            for (int y = 0; y < dimensions.y; y++)
            {
                for (int z = 0; z < dimensions.z; z++)
                {
                    block = GetBlockAt(new Vector3Int(x, y, z));
                    action();
                }
            }
        }
    }

    public void BlockTodoListLoop(out Block block, string funcName, Action action)
    {
        float startTime = Time.realtimeSinceStartup;

        block = default;
        for (int i = 0; i < calcShapeBlocksTodoList.Count - 1; i++)
        {
            block = calcShapeBlocksTodoList[i];
            action();
            //block.UpdateDiagonalQuads();
            if (block.blockShape != BlockShape.FULL)
            {
                calcShapeBlocksTodoList.RemoveAt(i);
                i--;
            }
        }

        if (funcName != "") Debug.Log($"{funcName} duurde {Time.realtimeSinceStartup - startTime} seconden");
    }

    public void ResetBlockShapes()
    {
        calcShapeBlocksTodoList.Clear();

        Block block = default;
        BlockLoop(out block, delegate
        {
            if (block.blockShape != BlockShape.EMPTY)
            {
                calcShapeBlocksTodoList.Add(block);
                block.ResetShape();
            }
        });
    }

    public void UpdateTerrainModifiedBool()
    {
        Block block = default;
        BlockLoop(out block, delegate
        {
            if (block.ShapeHasChanged()) shouldUpdateTerrainModel = true;
        });
    }

    public void UpdateBlockShapesDiagonals()
    {   
        Block block = default;
        BlockLoop(out block, delegate
        {
            block.UpdateDiagonalQuads();
        });
    }

    public void UpdateBlockShapesSlopes()
    {
        Block block = default;
        BlockLoop(out block, delegate
        {
            block.UpdateSlopes();
        });
    }

    public void UpdateBlockShapesSlopeCorners()
    {
        Block block = default;
        BlockLoop(out block, delegate
        {
            block.UpdateSlopeCorners();
        });
    }

    public void UpdateBlockShapesSlopeCornersConcave()
    {
        Block block = default;
        BlockLoop(out block, delegate
        {
            block.UpdateSlopeCornersConcave();
        });
    }

    public void UpdateTerrainModel()
    {
        terrainModel.Clear();
        terrainVertices.Clear();
        terrainTriangles.Clear();
        terrainUVs.Clear();
        terrainModel = new Mesh();
        terrainModel.MarkDynamic();

        Block block = default;
        BlockLoop(out block, delegate
        {
            AddGeometryBasedOnBlockShape(block);
        });

        terrainModel.vertices = terrainVertices.ToArray();
        terrainModel.triangles = terrainTriangles.ToArray();
        terrainModel.uv = terrainUVs.ToArray();
        terrainModel.RecalculateNormals();
        meshFilter.mesh = terrainModel;
        meshCollider.sharedMesh = terrainModel;
    }

    public bool IsValidBlockPosition(Vector3Int blockPosition)
    {
        return (blockPosition.x >= 0 && blockPosition.x < dimensions.x &&
                blockPosition.y >= 0 && blockPosition.y < dimensions.y &&
                blockPosition.z >= 0 && blockPosition.z < dimensions.z);
    }

    public Block GetBlockAt(Vector3Int blockPosition)
    {
        if (!IsValidBlockPosition(blockPosition)) return null;
        return terrainData[blockPosition.x, blockPosition.y, blockPosition.z];
    }

    public Block GetBlockAtPosition(Block block, Direction direction)
    {
        Chunk currentChunk = block.chunk;
        var targetPosition = block.blockPosition + GameData.directions[direction];
        if (IsValidBlockPosition(targetPosition)) return GetBlockAt(targetPosition);
        else
        {
            if (targetPosition.x < 0)
            {
                currentChunk = currentChunk?.neighBours.GetNeighbour(Direction.LEFT1);
                targetPosition.x = dimensions.x - 1;
            }
            else if (targetPosition.x > dimensions.x - 1)
            {
                currentChunk = currentChunk?.neighBours.GetNeighbour(Direction.RIGHT1);
                targetPosition.x = 0;
            }
            if (targetPosition.y < 0)
            {
                currentChunk = currentChunk?.neighBours.GetNeighbour(Direction.BOTTOM);
                targetPosition.y = dimensions.y - 1;
            }
            else if (targetPosition.y > dimensions.y - 1)
            {
                currentChunk = currentChunk?.neighBours.GetNeighbour(Direction.TOP);
                targetPosition.y = 0;
            }
            if (targetPosition.z < 0)
            {
                currentChunk = currentChunk?.neighBours.GetNeighbour(Direction.BACK1);
                targetPosition.z = dimensions.z - 1;
            }
            else if (targetPosition.z > dimensions.z - 1)
            {
                currentChunk = currentChunk?.neighBours.GetNeighbour(Direction.FRONT1);
                targetPosition.z = 0;
            }
        }
        return currentChunk?.GetBlockAt(targetPosition);
    }

    public void SetupBlockNeighbours()
    {
        Block block = default;
        BlockLoop(out block, delegate
        {
            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                block.neighBours.SetNeighbour(GetBlockAtPosition(block, direction), direction);
            }
        });
    }
}