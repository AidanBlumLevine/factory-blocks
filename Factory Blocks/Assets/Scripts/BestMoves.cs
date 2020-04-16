[System.Serializable]
public class BestMoves
{
    public LevelNum[] levels;
    [System.Serializable]
    public struct LevelNum
    {
        public int bestMoves;
        public bool permanent;
        public string name;
    }
}
