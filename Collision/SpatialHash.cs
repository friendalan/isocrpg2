using Microsoft.Xna.Framework;
using System.Collections.Generic;
using AiGame1.Entities;

namespace AiGame1.Collision
{
    public class SpatialHash
    {
        private readonly int _cellSize;
        private readonly Dictionary<Point, List<GameEntity>> _cells = new Dictionary<Point, List<GameEntity>>();
        private readonly Dictionary<GameEntity, Point> _entityCellMap = new Dictionary<GameEntity, Point>();

        public SpatialHash(int cellSize)
        {
            _cellSize = cellSize;
        }

        private Point GetCellKey(Vector2 position)
        {
            return new Point(
                (int)System.Math.Floor(position.X / _cellSize),
                (int)System.Math.Floor(position.Y / _cellSize)
            );
        }

        public void Add(GameEntity entity)
        {
            Point key = GetCellKey(entity.WorldPosition);
            if (!_cells.ContainsKey(key))
            {
                _cells[key] = new List<GameEntity>();
            }
            _cells[key].Add(entity);
            _entityCellMap[entity] = key;
        }

        public void Update(GameEntity entity)
        {
            if (!_entityCellMap.ContainsKey(entity))
            {
                Add(entity);
                return;
            }

            Point oldKey = _entityCellMap[entity];
            Point newKey = GetCellKey(entity.WorldPosition);

            if (oldKey != newKey)
            {
                if (_cells.ContainsKey(oldKey))
                {
                    _cells[oldKey].Remove(entity);
                }
                Add(entity);
            }
        }

        public List<GameEntity> GetNearby(GameEntity entity)
        {
            List<GameEntity> nearby = new List<GameEntity>();
            Rectangle bounds = entity.BoundingBox;

            // Get the cell keys for the corners of the bounding box
            Point minKey = GetCellKey(new Vector2(bounds.Left, bounds.Top));
            Point maxKey = GetCellKey(new Vector2(bounds.Right, bounds.Bottom));

            // Iterate over all cells the entity might be in
            for (int x = minKey.X; x <= maxKey.X; x++)
            {
                for (int y = minKey.Y; y <= maxKey.Y; y++)
                {
                    Point key = new Point(x, y);
                    if (_cells.ContainsKey(key))
                    {
                        nearby.AddRange(_cells[key]);
                    }
                }
            }
            return nearby;
        }

        public void Clear()
        {
            _cells.Clear();
            _entityCellMap.Clear();
        }
    }
}
