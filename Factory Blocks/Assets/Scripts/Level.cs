using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Level
{
    public int[] stars = new int[] { 0, 0, 0 };
    public int width = 0, height = 0, bestMoves = 99999;
    public string name = "";
    public bool permanent;
    public string realLevelName;
    [System.Serializable]
    public struct block
    {
        public int type, tileID;
        public Vector2Int pos;
        public bool isMaster;
        public int masterID;
        public int[] slavesIDs;
    }
    public block[] tiles;
}
