using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Enums
{
    public enum GameState
    {
        PREGAME,
        RUNNING,
        PAUSED,
        POSTGAME
    }

    public enum GameMode
    {
        NORMAL,
        HARD,
        EXTREAM
    }
    public enum BrickType
    {
        ZSHAPE,
        SSHAPE,
        ISHAPE,
        TSHAPE,
        OSHAPE,
        LSHAPE,
        JSHAPE
    }

    public enum Directions
    {
        FRONT,
        BACK,
        LEFT,
        RIGHT,
        TOP,
        BOTTOM        
    }

    public enum FacesColors
    {
        WHITE,
        RED,
        BLUE,
        ORANGE,
        GREEN,
        YELLOW
    }

    public enum SoundsEffects
    {
        FALL,
        MOVE_LR,
        ROTATION,
        ONE_LINE,
        TWO_LINE,
        TREE_LINE,
        FOUR_LINE,
        GAMEOVER
    }

    public enum BoardStats
    {
        GENERATING_NEW_BRICK,
        FALLING,
        UPDATING_BOARD
    }
}
