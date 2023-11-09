
public class Enums
{
    public enum ColDir
    {
        DEFAULT = -1,
        UP,
        DOWN,
        LEFT,
        RIGHT,
    }

    public enum PlayerAbility
    {
        
        DEFAULT = -1,
        GRAVITY,
        WARP,
    }

    public enum MapNum
    {
        DEFAULT = -1,
        STAGE_1 = 0x0000,
        STAGE_2 = 0b0001,
        STAGE_3 = 0b0010,
    }

    public enum MapOrientation
    {
        DEFAULT = -1,
        TOP = 0b01,
        BOTTOM = 0b10
    }
}
