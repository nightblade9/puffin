using System.Collections.Generic;
using Puffin.Core.Ecs;
using Puffin.Core.Tiles;

namespace Puffin.Core.Drawing
{
    /// <summary>
    /// An internal interface that wraps around the thing we actually draw on (eg. SpriteBatch).
    /// </summary>
    interface IDrawingSurface
    {
        void DrawAll(int backgroundColour);
        void AddEntity(Entity entity);
        void RemoveEntity(Entity entity);
        void AddTileMap(TileMap tileMap);
        void RemoveTileMap(TileMap tileMap);
    }
}