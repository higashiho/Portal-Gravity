using System.Collections.Generic;
using Unity.VisualScripting;

public class ObjectFactory
{
    public static ObjectFactory Instance;

    public PlayerController Player;
    public MapController Map;
    public WarpBeatController WarpBeat;
    public List<SpearController> Spears = new List<SpearController>();
}
