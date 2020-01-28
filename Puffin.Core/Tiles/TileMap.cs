using System;
using System.Collections.Generic;
using System.Linq;

namespace Puffin.Core.Tiles
{
    /// <summary>
    /// A 2D tilemap. Contains a tileset (defined tiles) and a spritesheet, which it uses
    /// to render the tiles. For usage, see the docs.
    /// Define some tiles, set some tiles, and you're golden.
    /// </summary>
    public class TileMap
    {
        internal readonly string TileImageFile;
        internal readonly int MapWidth; // in tiles
        internal readonly int MapHeight; // in tiles

        internal readonly int TileWidth; // in pixels
        internal readonly int TileHeight; // in pixels

        // List of all tile definitions, indexed by name.
        private IDictionary<string, TileDefinition> tileSet = new Dictionary<string, TileDefinition>();
    
        // (x, y) => tile name (eg. (13, 2) => grass)
        private string[,] tileData;
        
        /// <summary>
        /// Constructs a tilemap, given an image file name, and the size of a single tile.
        /// Tile width/height are in pixels.
        /// Map width/height are in tiles.
        /// </summary>
        public TileMap(int mapWidth, int mapHeight, string tileImageFile, int tileWidth, int tileHeight)
        {
            this.MapWidth = mapWidth;
            this.MapHeight = mapHeight;
            this.TileImageFile = tileImageFile;
            this.TileWidth = tileWidth;
            this.TileHeight = tileHeight;

            this.tileData = new string[mapWidth, mapHeight];
        }

        /// <summary>
        /// Define a new tile, at specific coordinates, and whether it's solid or not.
        /// Cell x/y are the coordinates of the tile (not pixels).
        /// Solidity affects entities with a collision component.
        /// Calling this twice with the same tile name will overwrite the previous definition.
        /// </summary>
        public void Define(string tileName, int cellX, int cellY, bool isSolid = false)
        {
            this.tileSet[tileName] = new TileDefinition(tileName, cellX, cellY, isSolid);
        }

        /// <summary>
        /// Set a specific tile to a defition.
        /// </summary>
        public void Set(int cellX, int cellY, string tileName)
        {
            if (!this.tileSet.ContainsKey(tileName))
            {
                throw new InvalidOperationException($"{tileName} can't be set, hasn't been defined yet.");
            }
            this.tileData[cellX, cellY] = tileName;
        }

        /// <summary>
        /// Get the tile at a specific location. Returns null if not set.
        /// </summary>
        public string Get(int cellX, int cellY)
        {
            if (cellX < 0 || cellY < 0 || cellX >= this.MapWidth || cellY >= this.MapHeight)
            {
                return null;
            }
            
            return this.tileData[cellX, cellY];
        }

        public string this[int cellX, int cellY]
        {
            get { return this.Get(cellX, cellY); }
            set { this.Set(cellX, cellY, value); }
        }

        // For testing.
        internal TileDefinition[] GetDefinitions()
        {
            return this.tileSet.Values.ToArray();
        }

        internal TileDefinition GetDefinition(string tileName)
        {
            if (this.tileSet.ContainsKey(tileName))
            {
                return this.tileSet[tileName];
            }
            else
            {
                throw new ArgumentException($"{tileName} isn't defined; defintiions are: {this.tileSet.Keys}");
            }
        }

        internal struct TileDefinition
        {
            public string TileName { get; }
            public int CellX { get; }
            public int CellY { get; }
            public bool IsSolid { get; }
            
            public TileDefinition(string tileName, int cellX, int cellY, bool isSolid)
            {
                this.TileName = tileName;
                this.CellX = cellX;
                this.CellY = cellY;
                this.IsSolid = isSolid;
            }
        }
    }
}