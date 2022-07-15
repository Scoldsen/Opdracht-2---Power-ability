using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Benedenlaag
//6  7  8
//3  4  5
//0  1  2

//Middenlaag. 13 is het midden en dus voor chunks en blocks het object zelf en niet een van zijn neighbours
//15 16 17
//12 13 14
//9  10 11

//Bovenlaag
//24 25 26
//21 22 23
//18 19 20

public enum Direction
{
    BACK_LEFT0 =    0,
    BACK0 =         1,
    BACK_RIGHT0 =   2,
    LEFT0 =         3,
    BOTTOM =        4,
    RIGHT0 =        5,
    FRONT_LEFT0 =   6,
    FRONT0 =        7,
    FRONT_RIGHT0 =  8,

    BACK_LEFT1 =    9,
    BACK1 =         10,
    BACK_RIGHT1 =   11,
    LEFT1 =         12,
    CENTER =        13,
    RIGHT1 =        14,
    FRONT_LEFT1 =   15,
    FRONT1 =        16,
    FRONT_RIGHT1 =  17,

    BACK_LEFT2 =    18,
    BACK2 =         19,
    BACK_RIGHT2 =   20,
    LEFT2 =         21,
    TOP =           22,
    RIGHT2 =        23,
    FRONT_LEFT2 =   24,
    FRONT2 =        25,
    FRONT_RIGHT2 =  26,
}
