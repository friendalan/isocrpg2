namespace AiGame1.World
{
    public class Cell
    {
        public TileType Type { get; set; }
        public bool IsWalkable { get; set; }

        public Cell(TileType type)
        {
            Type = type;
            IsWalkable = (type == TileType.Floor);
        }
    }
}
