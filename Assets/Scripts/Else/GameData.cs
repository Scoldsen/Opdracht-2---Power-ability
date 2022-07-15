using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameData
{
    public static readonly Dictionary<string, string> groupObjectNames = new Dictionary<string, string>()
    {
        {"Level",               "LEVEL" },
        {"Chunk",               "CHUNKS" },
        {"Coin",                "COINS" },
        {"FallingRock",         "ROCKS" },
        {"Player",              "PLAYERS" },
        {"Tree",                "TREES" },
        {"Rock",                "ROCKS" },
    };

    public static readonly Dictionary<int, Color> playerColors = new Dictionary<int, Color>()
    {
        {0, Color.red },
        {1, Color.blue },
        {2, Color.green },
        {3, Color.yellow },
        {4, Color.magenta },
        {5, new Color(1, 0, 1, 1) },
        {6, Color.cyan },
        {7, Color.black },
        {8, Color.white },
        {9, Color.gray },
    };

    //             ONDER                     MIDDEN                    BOVEN
    //
    //        |      |      |                                
    //        |      |      |                                
    //      |FL0---|F0----|FR0         FL1----F1-----FR1         FL2----F2-----FR2
    //      |/     |/     |/           /      /      /           /|     /|     /|
    //    | L0---|BO----|R0           L1----CE-----R1           L2|---TO-|---R2 |
    //    |/     |/     |/           /      /      /           /|     /|     /|
    //   BL0----B0----BR0          BL1----B1----BR1           BL2----B2|---BR2|
    //                                                        |      |      |
    //                                                        |      |      |

    public static readonly Dictionary<Direction, Vector3Int> directions = new Dictionary<Direction, Vector3Int>()
    {
        { Direction.BACK_LEFT0,     new Vector3Int(-1, -1, -1)},
        { Direction.BACK0,          new Vector3Int(0, -1, -1)},
        { Direction.BACK_RIGHT0,    new Vector3Int(1, -1, -1)},
        { Direction.LEFT0,          new Vector3Int(-1, -1, 0)},
        { Direction.BOTTOM,         new Vector3Int(0, -1, 0)},
        { Direction.RIGHT0,         new Vector3Int(1, -1, 0)},
        { Direction.FRONT_LEFT0,    new Vector3Int(-1, -1, 1)},
        { Direction.FRONT0,         new Vector3Int(0, -1, 1)},
        { Direction.FRONT_RIGHT0,   new Vector3Int(1, -1, 1)},

        { Direction.BACK_LEFT1,     new Vector3Int(-1, 0, -1)},
        { Direction.BACK1,          new Vector3Int(0, 0, -1)},
        { Direction.BACK_RIGHT1,    new Vector3Int(1, 0, -1)},
        { Direction.LEFT1,          new Vector3Int(-1, 0, 0)},
        { Direction.CENTER,         new Vector3Int(0, 0, 0)},
        { Direction.RIGHT1,         new Vector3Int(1, 0, 0)},
        { Direction.FRONT_LEFT1,    new Vector3Int(-1, 0, 1)},
        { Direction.FRONT1,         new Vector3Int(0, 0, 1)},
        { Direction.FRONT_RIGHT1,   new Vector3Int(1, 0, 1)},

        { Direction.BACK_LEFT2,     new Vector3Int(-1, 1, -1)},
        { Direction.BACK2,          new Vector3Int(0, 1, -1)},
        { Direction.BACK_RIGHT2,    new Vector3Int(1, 1, -1)},
        { Direction.LEFT2,          new Vector3Int(-1, 1, 0)},
        { Direction.TOP,            new Vector3Int(0, 1, 0)},
        { Direction.RIGHT2,         new Vector3Int(1, 1, 0)},
        { Direction.FRONT_LEFT2,    new Vector3Int(-1, 1, 1)},
        { Direction.FRONT2,         new Vector3Int(0, 1, 1)},
        { Direction.FRONT_RIGHT2,   new Vector3Int(1, 1, 1)},
    };

    public static readonly Dictionary<Direction, Direction> oppositeDirection = new Dictionary<Direction, Direction>()
    {
        { Direction.BACK_LEFT0, Direction.FRONT_RIGHT2 },
        { Direction.BACK0, Direction.FRONT2 },
        { Direction.BACK_RIGHT0, Direction.FRONT_LEFT2 },
        { Direction.LEFT0, Direction.RIGHT2 },
        { Direction.BOTTOM, Direction.TOP},
        { Direction.RIGHT0, Direction.LEFT2},
        { Direction.FRONT_LEFT0, Direction.BACK_RIGHT2},
        { Direction.FRONT0, Direction.BACK2},
        { Direction.FRONT_RIGHT0, Direction.BACK_LEFT2},

        { Direction.BACK_LEFT1, Direction.FRONT_RIGHT1 },
        { Direction.BACK1, Direction.FRONT1 },
        { Direction.BACK_RIGHT1, Direction.FRONT_LEFT1 },
        { Direction.LEFT1, Direction.RIGHT1 },
        { Direction.CENTER, Direction.CENTER},
        { Direction.RIGHT1, Direction.LEFT1},
        { Direction.FRONT_LEFT1, Direction.BACK_RIGHT1},
        { Direction.FRONT1, Direction.BACK1},
        { Direction.FRONT_RIGHT1, Direction.BACK_LEFT1},

        { Direction.BACK_LEFT2, Direction.FRONT_RIGHT0 },
        { Direction.BACK2, Direction.FRONT0 },
        { Direction.BACK_RIGHT2, Direction.FRONT_LEFT0 },
        { Direction.LEFT2, Direction.RIGHT0 },
        { Direction.TOP, Direction.BOTTOM},
        { Direction.RIGHT2, Direction.LEFT0},
        { Direction.FRONT_LEFT2, Direction.BACK_RIGHT0},
        { Direction.FRONT2, Direction.BACK0},
        { Direction.FRONT_RIGHT2, Direction.BACK_LEFT0},
    };

    //    6_______7
    //    /|     /|
    //  2/_|____3 |
    //   | 4---|--/5
    //   |/    | /
    //  0|_____|1

    public static readonly Dictionary<uint, Vector3> vPos = new Dictionary<uint, Vector3>()
    {
        {0, Vector3.zero },
        {1, Vector3.right },
        {2, Vector3.up },
        {3, new Vector3(1, 1, 0) },
        {4, Vector3.forward },
        {5, new Vector3(1, 0, 1) },
        {6, new Vector3(0, 1, 1) },
        {7, Vector3.one },
    };

    public static readonly Dictionary<BlockShape, Vector3[]> newQuadPositions = new Dictionary<BlockShape, Vector3[]>()
    {
        //QUADPOSITIONS
        { BlockShape.BACK, new Vector3[]{vPos[0], vPos[1], vPos[2], vPos[3]} },
        { BlockShape.FRONT, new Vector3[]{vPos[5], vPos[4], vPos[7], vPos[6]} },
        { BlockShape.LEFT, new Vector3[]{vPos[4], vPos[0], vPos[6], vPos[2]} },
        { BlockShape.RIGHT, new Vector3[]{vPos[1], vPos[5], vPos[3], vPos[7]} },
        { BlockShape.BOTTOM, new Vector3[]{vPos[4], vPos[5], vPos[0], vPos[1]} },
        { BlockShape.TOP, new Vector3[]{vPos[2], vPos[3], vPos[6], vPos[7]} },

        //QUADDIAGONAL
        { BlockShape.DIAGONAL_BACK, new Vector3[]{vPos[4], vPos[1], vPos[6], vPos[3]} },
        { BlockShape.DIAGONAL_FRONT, new Vector3[]{vPos[1], vPos[4], vPos[3], vPos[6]} },
        { BlockShape.DIAGONAL_LEFT, new Vector3[]{vPos[5], vPos[0], vPos[7], vPos[2]} },
        { BlockShape.DIAGONAL_RIGHT, new Vector3[]{vPos[0], vPos[5], vPos[2], vPos[7]} },

        //SLOPES
        { BlockShape.SLOPE_BACK, new Vector3[]{vPos[0], vPos[1], vPos[6], vPos[7]} },
        { BlockShape.SLOPE_FRONT, new Vector3[]{vPos[5], vPos[4], vPos[3], vPos[2]} },
        { BlockShape.SLOPE_LEFT, new Vector3[]{vPos[4], vPos[0], vPos[7], vPos[3]} },
        { BlockShape.SLOPE_RIGHT, new Vector3[]{vPos[1], vPos[5], vPos[2], vPos[6]} },

        //SLOPES_INV
        { BlockShape.SLOPE_BACK_INV, new Vector3[]{vPos[4], vPos[5], vPos[2], vPos[3]} },
        { BlockShape.SLOPE_FRONT_INV, new Vector3[]{vPos[1], vPos[0], vPos[7], vPos[6]} },
        { BlockShape.SLOPE_LEFT_INV, new Vector3[]{vPos[5], vPos[1], vPos[6], vPos[2]} },
        { BlockShape.SLOPE_RIGHT_INV, new Vector3[]{vPos[0], vPos[4], vPos[3], vPos[7]} },
    };

    //    6_______7
    //    /|     /|
    //  2/_|____3 |
    //   | 4---|--/5
    //   |/    | /
    //  0|_____|1

    public static readonly Dictionary<BlockShape, Vector3[]> cornerConvexTrianglePositions = new Dictionary<BlockShape, Vector3[]>()
    {
        { BlockShape.CORNER_BACK_LEFT, new Vector3[]{vPos[4], vPos[7], vPos[1]} },
        { BlockShape.CORNER_BACK_RIGHT, new Vector3[]{vPos[0], vPos[6], vPos[5]} },
        { BlockShape.CORNER_FRONT_LEFT, new Vector3[]{vPos[5], vPos[3], vPos[0]} },
        { BlockShape.CORNER_FRONT_RIGHT, new Vector3[]{vPos[1], vPos[2], vPos[4]} },

        { BlockShape.CORNER_BACK_LEFT_INV, new Vector3[]{vPos[6], vPos[3], vPos[5] } },
        { BlockShape.CORNER_BACK_RIGHT_INV, new Vector3[]{vPos[2], vPos[7], vPos[4] } },
        { BlockShape.CORNER_FRONT_LEFT_INV, new Vector3[]{vPos[7], vPos[2], vPos[1]} },
        { BlockShape.CORNER_FRONT_RIGHT_INV, new Vector3[]{vPos[3], vPos[6], vPos[0]} },

        { BlockShape.CORNER_BACK_LEFT_CONCAVE, new Vector3[]{vPos[0], vPos[6], vPos[3] } },
        { BlockShape.CORNER_BACK_RIGHT_CONCAVE, new Vector3[]{vPos[1], vPos[2], vPos[7] } },
        { BlockShape.CORNER_FRONT_LEFT_CONCAVE, new Vector3[]{vPos[4], vPos[7], vPos[2] } },
        { BlockShape.CORNER_FRONT_RIGHT_CONCAVE, new Vector3[]{vPos[5], vPos[3], vPos[6] } },

        { BlockShape.CORNER_BACK_LEFT_CONCAVE_INV, new Vector3[]{vPos[4], vPos[2], vPos[1] } },
        { BlockShape.CORNER_BACK_RIGHT_CONCAVE_INV, new Vector3[]{vPos[0], vPos[3], vPos[5] } },
        { BlockShape.CORNER_FRONT_LEFT_CONCAVE_INV, new Vector3[]{vPos[1], vPos[7], vPos[4] } },
        { BlockShape.CORNER_FRONT_RIGHT_CONCAVE_INV, new Vector3[]{vPos[5], vPos[6], vPos[0] } },
    };
}
