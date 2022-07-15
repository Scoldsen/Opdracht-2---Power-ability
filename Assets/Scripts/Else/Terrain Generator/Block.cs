using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Block
{
    public Vector3Int blockPosition;
    public BlockShape blockShape;
    private BlockShape oldBlockShape = BlockShape.EMPTY;
    private uint blockID = 0;

    public Chunk chunk;
    public Neighbours<Block> neighBours = new Neighbours<Block>();

    int nBlocksHor = 2;
    int nBlocksVer = 2;

    float xLengthPerBlock = 1;
    float yLengthPerBlock = 1;

    public Vector2 bottomLeft;
    public Vector2 bottomRight;
    public Vector2 topLeft;
    public Vector2 topRight;

    public Block(Vector3Int _blockPosition, BlockShape _blockShape, uint _blockID)
    {
        blockPosition = _blockPosition;
        blockShape = _blockShape;
        blockID = _blockID;
        SetupUVs();
    }

    private void SetupUVs()
    {
        xLengthPerBlock = 1.0f / nBlocksHor;
        yLengthPerBlock = 1.0f / nBlocksVer;

        int xPos = 0;
        int yPos = 0;

        switch (blockID)
        {
            default:
                xPos = 1;
                yPos = 0;
                break;
            case 1:
                xPos = 0;
                yPos = 0;
                break;
            case 2:
                xPos = 0;
                yPos = 0;
                break;
            case 3:
                xPos = 1;
                yPos = 1;
                break;
            case 4:
                xPos = 0;
                yPos = 1;
                break;
            case 5:
                xPos = 0;
                yPos = 1;
                break;
                /*
            case 6:
                xPos = 0;
                yPos = 1;
                break;
            case 7:
                xPos = 0;
                yPos = 1;
                break;
            case 8:
                xPos = 0;
                yPos = 1;
                break;
            case 9:
                xPos = 1;
                yPos = 1;
                break;
            case 10:
                xPos = 1;
                yPos = 1;
                break;
                */
        }

        bottomLeft = new Vector2(xPos * xLengthPerBlock, yPos * yLengthPerBlock);
        bottomRight = bottomLeft + new Vector2(xLengthPerBlock, 0);
        topLeft = bottomLeft + new Vector2(0, yLengthPerBlock);
        topRight = bottomRight + new Vector2(0, yLengthPerBlock);
    }

    public void ResetShape()
    {
        oldBlockShape = blockShape;
        blockShape = BlockShape.FULL;
    }

    #region NeighbourChecks
    public bool IsEmptyInDirection(Direction direction)
    {
        if (neighBours.GetNeighbour(direction) == null) return true;
        else return neighBours.GetNeighbour(direction).blockShape == BlockShape.EMPTY;
    }

    public bool IsEmptyInDirections(Direction[] directions)
    {
        for (int i = 0; i < directions.Length; i++)
        {
            if (!IsEmptyInDirection(directions[i])) return false;
        }

        return true;
    }

    public bool IsOfTypeInDirection(Direction direction, BlockShape shape)
    {
        return neighBours.GetNeighbour(direction)?.blockShape == shape;
    }

    public bool IsOfTypesInDirection(Direction direction, BlockShape[] shapes)
    {
        var neigh = neighBours.GetNeighbour(direction);
        if (neigh == null) return false;

        for (int i = 0; i < shapes.Length; i++)
        {
            if (neigh.blockShape == shapes[i]) return true;
        }
        return false;
    }

    public bool IsOfTypesInDirections(Direction direction1, Direction direction2, BlockShape mutualShape, BlockShape shape1, BlockShape shape2)
    {
        var neigh1 = neighBours.GetNeighbour(direction1);
        var neigh2 = neighBours.GetNeighbour(direction2);

        return (neigh1?.blockShape == shape1 && neigh2?.blockShape == shape2) ||
            (neigh1?.blockShape == shape1 && neigh2?.blockShape == mutualShape) ||
            (neigh1?.blockShape == mutualShape && neigh2?.blockShape == shape2) ||
            (neigh1?.blockShape == mutualShape && neigh2?.blockShape == mutualShape);
    }
    #endregion

    public bool ShapeHasChanged()
    {
        return (blockShape != oldBlockShape);
    }

    #region ShapeCalculations
    //https://www.youtube.com/watch?v=poz6W0znOfk

    public void UpdateDiagonalQuads()
    {
        //blokken in terrein worden in de volgorde z, y, x bijgewerkt
        //dus BACK -> FRONT, BOTTOM -> TOP, LEFT -> RIGHT
        if (blockShape != BlockShape.FULL) return;
        
        if (IsOfTypeInDirection(Direction.TOP, BlockShape.FULL))
        {
            if (IsEmptyInDirection(Direction.BACK1))
            {
                //DIAGONAL_BACK

                //      |
                //      \_

                if (IsOfTypeInDirection(Direction.FRONT1, BlockShape.FULL) && IsOfTypeInDirection(Direction.RIGHT1, BlockShape.FULL) ||
                    IsOfTypeInDirection(Direction.FRONT_LEFT1, BlockShape.FULL) && IsOfTypeInDirection(Direction.RIGHT1, BlockShape.FULL) ||
                    IsOfTypeInDirection(Direction.FRONT1, BlockShape.FULL) && IsOfTypeInDirection(Direction.BACK_RIGHT1, BlockShape.FULL) ||
                    IsOfTypeInDirection(Direction.FRONT_LEFT1, BlockShape.FULL) && IsOfTypeInDirection(Direction.BACK_RIGHT1, BlockShape.FULL))
                    
                {
                    if (IsOfTypesInDirection(Direction.BOTTOM, new BlockShape[] { BlockShape.FULL, BlockShape.DIAGONAL_BACK }))
                    {
                        if (IsEmptyInDirection(Direction.BACK_LEFT1) && IsEmptyInDirection(Direction.LEFT1))
                        {
                            blockShape = BlockShape.DIAGONAL_BACK;
                        }
                    }
                }

                //DIAGONAL_RIGHT

                //       |
                //      _/

                if (IsOfTypeInDirection(Direction.FRONT1, BlockShape.FULL) && IsOfTypeInDirection(Direction.LEFT1, BlockShape.FULL) ||
                    IsOfTypeInDirection(Direction.FRONT_RIGHT1, BlockShape.FULL) && IsOfTypeInDirection(Direction.LEFT1, BlockShape.FULL) ||
                    IsOfTypeInDirection(Direction.FRONT1, BlockShape.FULL) && IsOfTypeInDirection(Direction.BACK_LEFT1, BlockShape.FULL) ||
                    IsOfTypeInDirection(Direction.FRONT_RIGHT1, BlockShape.FULL) && IsOfTypeInDirection(Direction.BACK_LEFT1, BlockShape.FULL))

                {
                    if (IsOfTypesInDirection(Direction.BOTTOM, new BlockShape[] { BlockShape.FULL, BlockShape.DIAGONAL_RIGHT }))
                    {
                        if (IsEmptyInDirection(Direction.BACK_RIGHT1) && IsEmptyInDirection(Direction.RIGHT1))
                        {
                            blockShape = BlockShape.DIAGONAL_RIGHT;
                        }
                    }
                }    
            }

            else if (IsEmptyInDirection(Direction.FRONT1))
            {
                //DIAGONAL_FRONT

                //      _
                //       \
                //       |

                if (IsOfTypeInDirection(Direction.BACK1, BlockShape.FULL) && IsOfTypeInDirection(Direction.LEFT1, BlockShape.FULL) ||
                    IsOfTypeInDirection(Direction.BACK_RIGHT1, BlockShape.FULL) && IsOfTypeInDirection(Direction.LEFT1, BlockShape.FULL) ||
                    IsOfTypeInDirection(Direction.BACK1, BlockShape.FULL) && IsOfTypeInDirection(Direction.FRONT_LEFT1, BlockShape.FULL) ||
                    IsOfTypeInDirection(Direction.BACK_RIGHT1, BlockShape.FULL) && IsOfTypeInDirection(Direction.FRONT_LEFT1, BlockShape.FULL))

                {
                    if (IsOfTypesInDirection(Direction.BOTTOM, new BlockShape[] { BlockShape.FULL, BlockShape.DIAGONAL_FRONT }))
                    {
                        if (IsEmptyInDirection(Direction.FRONT_RIGHT1) && IsEmptyInDirection(Direction.RIGHT1))
                        {
                            blockShape = BlockShape.DIAGONAL_FRONT;
                        }
                    }
                }

                //DIAGONAL_LEFT

                //       _
                //      /
                //      |

                if (IsOfTypeInDirection(Direction.BACK1, BlockShape.FULL) && IsOfTypeInDirection(Direction.RIGHT1, BlockShape.FULL) ||
                    IsOfTypeInDirection(Direction.BACK_RIGHT1, BlockShape.FULL) && IsOfTypeInDirection(Direction.RIGHT1, BlockShape.FULL) ||
                    IsOfTypeInDirection(Direction.BACK1, BlockShape.FULL) && IsOfTypeInDirection(Direction.FRONT_RIGHT1, BlockShape.FULL) ||
                    IsOfTypeInDirection(Direction.BACK_RIGHT1, BlockShape.FULL) && IsOfTypeInDirection(Direction.FRONT_RIGHT1, BlockShape.FULL))

                {
                    if (IsOfTypesInDirection(Direction.BOTTOM, new BlockShape[] { BlockShape.FULL, BlockShape.DIAGONAL_LEFT }))
                    {
                        if (IsEmptyInDirection(Direction.FRONT_LEFT1) && IsEmptyInDirection(Direction.LEFT1))
                        {
                            blockShape = BlockShape.DIAGONAL_LEFT;
                        }
                    }
                }
            }
        }
    }

    public void UpdateSlopes()
    {
        //blokken in terrein worden in de volgorde z, y, x bijgewerkt
        //dus BACK -> FRONT, BOTTOM -> TOP, LEFT -> RIGHT
        if (blockShape != BlockShape.FULL) return;

        int inversionNumber = 0;
        if (IsEmptyInDirection(Direction.TOP)) inversionNumber = 1;
        else if (IsEmptyInDirection(Direction.BOTTOM)) inversionNumber = 2;

        if (inversionNumber != 0)
        {
            Direction heightDirection = inversionNumber == 1 ? Direction.BOTTOM : Direction.TOP;
            BlockShape backSlope = inversionNumber == 1 ? BlockShape.SLOPE_BACK : BlockShape.SLOPE_BACK_INV;
            BlockShape frontSlope = inversionNumber == 1 ? BlockShape.SLOPE_FRONT : BlockShape.SLOPE_FRONT_INV;
            BlockShape leftSlope = inversionNumber == 1 ? BlockShape.SLOPE_LEFT : BlockShape.SLOPE_LEFT_INV;
            BlockShape rightSlope = inversionNumber == 1 ? BlockShape.SLOPE_RIGHT : BlockShape.SLOPE_RIGHT_INV;

            Direction diagonalBackCheck = inversionNumber == 1 ? Direction.BACK2 : Direction.BACK0;
            Direction diagonalFrontCheck = inversionNumber == 1 ? Direction.FRONT2 : Direction.FRONT0;
            Direction diagonalLeftCheck = inversionNumber == 1 ? Direction.LEFT2 : Direction.LEFT0;
            Direction diagonalRightCheck = inversionNumber == 1 ? Direction.RIGHT2 : Direction.RIGHT0;

            if (IsOfTypeInDirection(heightDirection, BlockShape.FULL))
            {
                if (IsOfTypeInDirection(Direction.RIGHT1, BlockShape.FULL))
                {
                    //SLOPE_BACK
                    if (IsEmptyInDirection(Direction.BACK1) && IsEmptyInDirection(diagonalBackCheck) && !IsEmptyInDirection(Direction.FRONT1))
                    {
                        if (IsOfTypesInDirection(Direction.LEFT1, new BlockShape[] { BlockShape.FULL, backSlope }))
                        {
                            blockShape = backSlope;
                            return;
                        }
                    }

                    //SLOPE_FRONT
                    else if (IsEmptyInDirection(Direction.FRONT1) && IsEmptyInDirection(diagonalFrontCheck) && !IsEmptyInDirection(Direction.BACK1))
                    {
                        if (IsOfTypesInDirection(Direction.LEFT1, new BlockShape[] { BlockShape.FULL, frontSlope }))
                        {
                            blockShape = frontSlope;
                            return;
                        }
                    }
                }

                if (IsOfTypeInDirection(Direction.FRONT1, BlockShape.FULL))
                {
                    //SLOPE_LEFT
                    if (IsEmptyInDirection(Direction.LEFT1) && IsEmptyInDirection(diagonalLeftCheck) && !IsEmptyInDirection(Direction.RIGHT1))
                    {
                        if (IsOfTypesInDirection(Direction.BACK1, new BlockShape[] { BlockShape.FULL, leftSlope }))
                        {
                            blockShape = leftSlope;
                        }
                    }

                    //SLOPE_RIGHT
                    else if (IsEmptyInDirection(Direction.RIGHT1) && IsEmptyInDirection(diagonalRightCheck) && !IsEmptyInDirection(Direction.LEFT1))
                    {
                        if (IsOfTypesInDirection(Direction.BACK1, new BlockShape[] { BlockShape.FULL, rightSlope }))
                        {
                            blockShape = rightSlope;
                        }
                    }
                }
            }
        }
    }

    public void UpdateSlopeCorners()
    {
        //blokken in terrein worden in de volgorde z, y, x bijgewerkt
        //dus BACK -> FRONT, BOTTOM -> TOP, LEFT -> RIGHT
        if (blockShape != BlockShape.FULL) return;

        int inversionNumber = 0;
        if (IsEmptyInDirection(Direction.TOP)) inversionNumber = 1;
        else if (IsEmptyInDirection(Direction.BOTTOM)) inversionNumber = 2;

        if (inversionNumber != 0)
        {
            Direction heightDirection = inversionNumber == 1 ? Direction.BOTTOM : Direction.TOP;
            BlockShape backSlope = inversionNumber == 1 ? BlockShape.SLOPE_BACK : BlockShape.SLOPE_BACK_INV;
            BlockShape frontSlope = inversionNumber == 1 ? BlockShape.SLOPE_FRONT : BlockShape.SLOPE_FRONT_INV;
            BlockShape leftSlope = inversionNumber == 1 ? BlockShape.SLOPE_LEFT : BlockShape.SLOPE_LEFT_INV;
            BlockShape rightSlope = inversionNumber == 1 ? BlockShape.SLOPE_RIGHT : BlockShape.SLOPE_RIGHT_INV;

            //CORNER_BACK_LEFT
            if (IsOfTypesInDirection(heightDirection, new BlockShape[] {BlockShape.DIAGONAL_BACK, BlockShape.FULL }))
            {
                if (IsEmptyInDirections(new Direction[] { Direction.LEFT1, Direction.BACK1 }))
                {
                    if (IsOfTypesInDirections(Direction.FRONT1, Direction.RIGHT1, BlockShape.FULL, leftSlope, backSlope))
                    {
                        blockShape = inversionNumber == 1 ? BlockShape.CORNER_BACK_LEFT : BlockShape.CORNER_BACK_LEFT_INV;
                        return;
                    }
                }
            }

            //CORNER_BACK_RIGHT
            if (IsOfTypesInDirection(heightDirection, new BlockShape[] { BlockShape.DIAGONAL_RIGHT, BlockShape.FULL }))
            {
                if (IsEmptyInDirections(new Direction[] { Direction.RIGHT1, Direction.BACK1 }))
                {
                    if (IsOfTypesInDirections(Direction.FRONT1, Direction.LEFT1, BlockShape.FULL, rightSlope, backSlope))
                    {
                        blockShape = inversionNumber == 1 ? BlockShape.CORNER_BACK_RIGHT : BlockShape.CORNER_BACK_RIGHT_INV;
                        return;
                    }
                }
            }

            //CORNER_FRONT_LEFT
            if (IsOfTypesInDirection(heightDirection, new BlockShape[] { BlockShape.DIAGONAL_RIGHT, BlockShape.FULL }))
            {
                if (IsEmptyInDirections(new Direction[] { Direction.LEFT1, Direction.FRONT1 }))
                {
                    if (IsOfTypesInDirections(Direction.BACK1, Direction.RIGHT1, BlockShape.FULL, leftSlope, frontSlope))
                    {
                        blockShape = inversionNumber == 1 ? BlockShape.CORNER_FRONT_LEFT : BlockShape.CORNER_FRONT_LEFT_INV;
                        return;
                    }
                }
            }

            //CORNER_FRONT_RIGHT
            if (IsOfTypesInDirection(heightDirection, new BlockShape[] { BlockShape.DIAGONAL_RIGHT, BlockShape.FULL }))
            {
                if (IsEmptyInDirections(new Direction[] { Direction.RIGHT1, Direction.FRONT1 }))
                {
                    if (IsOfTypesInDirections(Direction.BACK1, Direction.LEFT1, BlockShape.FULL, rightSlope, frontSlope))
                    {
                        blockShape = inversionNumber == 1 ? BlockShape.CORNER_FRONT_RIGHT : BlockShape.CORNER_FRONT_RIGHT_INV;
                        return;
                    }
                }
            }
        }
    }

    public void UpdateSlopeCornersConcave()
    {
        //blokken in terrein worden in de volgorde z, y, x bijgewerkt
        //dus BACK -> FRONT, BOTTOM -> TOP, LEFT -> RIGHT
        if (blockShape != BlockShape.FULL) return;

        //CORNER_BACK_LEFT_CONCAVE
        if (IsOfTypesInDirections(Direction.LEFT1, Direction.BACK1, BlockShape.CORNER_BACK_LEFT, BlockShape.SLOPE_BACK, BlockShape.SLOPE_LEFT))
        {
            blockShape = BlockShape.CORNER_BACK_LEFT_CONCAVE;
        }

        //CORNER_BACK_RIGHT_CONCAVE
        else if (IsOfTypesInDirections(Direction.RIGHT1, Direction.BACK1, BlockShape.CORNER_BACK_RIGHT, BlockShape.SLOPE_BACK, BlockShape.SLOPE_RIGHT))
        {
            blockShape = BlockShape.CORNER_BACK_RIGHT_CONCAVE;
        }

        //CORNER_FRONT_LEFT_CONCAVE
        else if (IsOfTypesInDirections(Direction.LEFT1, Direction.FRONT1, BlockShape.CORNER_FRONT_LEFT, BlockShape.SLOPE_FRONT, BlockShape.SLOPE_LEFT))
        {
            blockShape = BlockShape.CORNER_FRONT_LEFT_CONCAVE;
        }

        //CORNER_FRONT_RIGHT_CONCAVE
        else if (IsOfTypesInDirections(Direction.RIGHT1, Direction.FRONT1, BlockShape.CORNER_FRONT_RIGHT, BlockShape.SLOPE_FRONT, BlockShape.SLOPE_RIGHT))
        {
            blockShape = BlockShape.CORNER_FRONT_RIGHT_CONCAVE;
        }

        //CORNER_BACK_LEFT_CONCAVE
        else if (IsOfTypesInDirections(Direction.LEFT1, Direction.BACK1, BlockShape.CORNER_BACK_LEFT_INV, BlockShape.SLOPE_BACK_INV, BlockShape.SLOPE_LEFT_INV))
        {
            blockShape = BlockShape.CORNER_BACK_LEFT_CONCAVE_INV;
        }

        //CORNER_BACK_RIGHT_CONCAVE
        else if (IsOfTypesInDirections(Direction.RIGHT1, Direction.BACK1, BlockShape.CORNER_BACK_RIGHT_INV, BlockShape.SLOPE_BACK_INV, BlockShape.SLOPE_RIGHT_INV))
        {
            blockShape = BlockShape.CORNER_BACK_RIGHT_CONCAVE_INV;
        }

        //CORNER_FRONT_LEFT_CONCAVE
        else if (IsOfTypesInDirections(Direction.LEFT1, Direction.FRONT1, BlockShape.CORNER_FRONT_LEFT_INV, BlockShape.SLOPE_FRONT_INV, BlockShape.SLOPE_LEFT_INV))
        {
            blockShape = BlockShape.CORNER_FRONT_LEFT_CONCAVE_INV;
        }

        //CORNER_FRONT_RIGHT_CONCAVE
        else if (IsOfTypesInDirections(Direction.RIGHT1, Direction.FRONT1, BlockShape.CORNER_FRONT_RIGHT_INV, BlockShape.SLOPE_FRONT_INV, BlockShape.SLOPE_RIGHT_INV))
        {
            blockShape = BlockShape.CORNER_FRONT_RIGHT_CONCAVE_INV;
        }
    }
    #endregion
}
