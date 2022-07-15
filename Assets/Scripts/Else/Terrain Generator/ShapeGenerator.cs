using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ShapeGenerator
{
    public static void AddQuad(Block block, ref List<Vector3> vertices, ref List<int> triangles, ref List<Vector2> uvs, BlockShape shape)
    {
        int startIndex = vertices.Count;
        Vector3 offSet = new Vector3(block.chunk.chunkPosition.x * block.chunk.dimensions.x, block.chunk.chunkPosition.y * block.chunk.dimensions.y, block.chunk.chunkPosition.z * block.chunk.dimensions.z);

        Vector3[] vertPositions = GameData.newQuadPositions[shape];

        vertices.Add(block.blockPosition + offSet + vertPositions[0]);
        vertices.Add(block.blockPosition + offSet + vertPositions[1]);
        vertices.Add(block.blockPosition + offSet + vertPositions[2]);
        vertices.Add(block.blockPosition + offSet + vertPositions[3]);

        triangles.Add(startIndex);
        triangles.Add(startIndex + 2);
        triangles.Add(startIndex + 1);
        triangles.Add(startIndex + 2);
        triangles.Add(startIndex + 3);
        triangles.Add(startIndex + 1);

        uvs.Add(block.bottomLeft);
        uvs.Add(block.bottomRight);
        uvs.Add(block.topLeft);
        uvs.Add(block.topRight);
    }

    public static void AddQuad(Block block, ref List<Vector3> vertices, ref List<int> triangles, ref List<Vector2> uvs)
    {
        AddQuad(block, ref vertices, ref triangles, ref uvs, block.blockShape);
    }

    public static void AddTriangle(Block block, ref List<Vector3> vertices, ref List<int> triangles, ref List<Vector2> uvs, BlockShape shape)
    {
        int startIndex = vertices.Count;
        Vector3 offSet = new Vector3(block.chunk.chunkPosition.x * block.chunk.dimensions.x, block.chunk.chunkPosition.y * block.chunk.dimensions.y, block.chunk.chunkPosition.z * block.chunk.dimensions.z);

        Vector3[] vertPositions = GameData.cornerConvexTrianglePositions[shape];

        vertices.Add(block.blockPosition + offSet + vertPositions[0]);
        vertices.Add(block.blockPosition + offSet + vertPositions[1]);
        vertices.Add(block.blockPosition + offSet + vertPositions[2]);

        triangles.Add(startIndex);
        triangles.Add(startIndex + 1);
        triangles.Add(startIndex + 2);

        // 2   3
        //   1
        if (shape >= BlockShape.CORNER_BACK_LEFT_CONCAVE && shape <= BlockShape.CORNER_FRONT_RIGHT_CONCAVE_INV)
        {
            uvs.Add(new Vector2(block.bottomLeft.x + 0.5f * (block.bottomRight.x - block.bottomLeft.x), block.bottomLeft.y));
            uvs.Add(block.topLeft);
            uvs.Add(block.topRight);
            /*
            uvs.Add(new Vector2(0.5f, 0));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(1, 1));
            */
        }

        //   2
        // 1   3
        else
        {
            uvs.Add(block.bottomLeft);
            uvs.Add(new Vector2(block.topLeft.x + 0.5f * (block.bottomRight.x - block.bottomLeft.x), block.topLeft.y));
            uvs.Add(block.bottomRight);
            /*
            uvs.Add(new Vector2(block.bottomLeft.x + 0.5f * (block.bottomRight.x - block.bottomLeft.x), block.bottomLeft.y));
            uvs.Add(block.topRight);
            uvs.Add(new Vector2(block.bottomRight.x + 0.5f * (block.bottomRight.x - block.bottomLeft.x), block.bottomLeft.y));
            */
            /*
            uvs.Add(new Vector2(0.5f, 0));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(1.5f, 0));
            */
        }

        

        /*
        uvs.Add(Vector2.zero);
        uvs.Add(new Vector2(0.5f, 1));
        uvs.Add(new Vector2(1, 0));*/
    }

    public static void AddTriangle(Block block, ref List<Vector3> vertices, ref List<int> triangles, ref List<Vector2> uvs)
    {
        AddTriangle(block, ref vertices, ref triangles, ref uvs, block.blockShape);
    }

    public static void AddOptimizedCube(Block block, ref List<Vector3> vertices, ref List<int> triangles, ref List<Vector2> uvs)
    {
        if (block.IsEmptyInDirection(Direction.BACK1)) AddQuad(block, ref vertices, ref triangles, ref uvs, BlockShape.BACK);
        if (block.IsEmptyInDirection(Direction.FRONT1)) AddQuad(block, ref vertices, ref triangles, ref uvs, BlockShape.FRONT);
        if (block.IsEmptyInDirection(Direction.LEFT1)) AddQuad(block, ref vertices, ref triangles, ref uvs, BlockShape.LEFT);
        if (block.IsEmptyInDirection(Direction.RIGHT1)) AddQuad(block, ref vertices, ref triangles, ref uvs, BlockShape.RIGHT);
        if (block.IsEmptyInDirection(Direction.BOTTOM)) AddQuad(block, ref vertices, ref triangles, ref uvs, BlockShape.BOTTOM);
        if (block.IsEmptyInDirection(Direction.TOP)) AddQuad(block, ref vertices, ref triangles, ref uvs, BlockShape.TOP);
    }

    public static void AddCube(Block block, ref List<Vector3> vertices, ref List<int> triangles, ref List<Vector2> uvs)
    {
        AddQuad(block, ref vertices, ref triangles, ref uvs, BlockShape.BACK);
        AddQuad(block, ref vertices, ref triangles, ref uvs, BlockShape.FRONT);
        AddQuad(block, ref vertices, ref triangles, ref uvs, BlockShape.LEFT);
        AddQuad(block, ref vertices, ref triangles, ref uvs, BlockShape.RIGHT);
        AddQuad(block, ref vertices, ref triangles, ref uvs, BlockShape.BOTTOM);
        AddQuad(block, ref vertices, ref triangles, ref uvs, BlockShape.TOP);
    }
}
