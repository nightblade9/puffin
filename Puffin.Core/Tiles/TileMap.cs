using System;
using System.Collections.Generic;
using System.Linq;
using Puffin.Core.Events;

namespace Puffin.Core.Tiles
{
    /// <summary>
    /// A 2D tilemap. Contains a tileset (defined tiles) and a spritesheet, which it uses
    /// to render the tiles. Define some tiles, set some tiles, and they appear on-screen.
    /// </summary>
    public class TileMap
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string TileImageFile {
            get {
                return this.imageFile;
            }
            set
            {
                this.imageFile = value;
                this.Scene?.EventBus.Broadcast(EventBusSignal.TilesetSpriteChanged, this);
            }
        }

        internal readonly int MapWidth; // in tiles
        internal readonly int MapHeight; // in tiles

        internal readonly int TileWidth; // in pixels
        internal readonly int TileHeight; // in pixels

        internal Scene Scene;

        // List of all tile definitions, indexed by name.
        private readonly IDictionary<string, TileDefinition> tileSet = new Dictionary<string, TileDefinition>();
    
        // (x, y) => tile name (eg. (13, 2) => grass). Null if empty/unset.
        private readonly string[,] tileData;
        private string imageFile;
        
        /// <summary>
        /// Constructs a tilemap, given an image file name, and the size of a single tile.
        /// </summary>
        /// <param name="mapWidth">The width of the map, in tiles.</param>
        /// <param name="mapHeight">The height of the map, in tiles.</param>
        /// <param name="tileImageFile">The image file of the spritesheet</param>
        /// <param name="tileWidth">The width of a single tile, in pixels.</param>
        /// <param name="tileHeight">The height of a single tile, in pixels.</param>
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
        /// Define a new tile, at specific coordinates, and specify whether it's solid or not.
        /// Calling this twice with the same tile name will overwrite the previous definition.
        /// </summary>
        /// <param name="tileName">The name of the tile (used in <c>Set</c></param>
        /// <param name="cellX">The x-index (not pixels) of the tile in the tilesheet</param>
        /// <param name="cellY">The y-index (not pixels) of the tile in the tilesheet</param>
        /// <param name="isSolid">true if this tile should prevent collidable entities from moving onto it.</param>
        public void Define(string tileName, int cellX, int cellY, bool isSolid = false)
        {
            this.tileSet[tileName] = new TileDefinition(tileName, cellX, cellY, isSolid);
        }

        /// <summary>
        /// Set a specific tile to a defition. Throws if that definition isn't defined yet via <c>Define</c>.
        /// </summary>
        public void Set(int cellX, int cellY, string tileName)
        {
            if (tileName != null && !this.tileSet.ContainsKey(tileName))
            {
                throw new InvalidOperationException($"{tileName} can't be set, hasn't been defined yet.");
            }
            this.tileData[cellX, cellY] = tileName;
        }

        /// <summary>
        /// Get the tile at a specific location. Returns null if coordinates are out of bounds or that tile is not set.
        /// </summary>
        public string Get(int cellX, int cellY)
        {
            if (cellX < 0 || cellY < 0 || cellX >= this.MapWidth || cellY >= this.MapHeight)
            {
                return null;
            }
            
            return this.tileData[cellX, cellY];
        }

        /// <summary>
        /// Sets the tile at a specific location, overriding the previous value at that location.
        /// </summary>
        public string this[int cellX, int cellY]
        {
            get { return this.Get(cellX, cellY); }
            set { this.Set(cellX, cellY, value); }
        }

        /// <summary>
        /// Clears all the tiles set on this map. Doesn't remove any defined tiles.
        /// </summary>
        public void Clear()
        {
            for (var y = 0; y < this.MapHeight; y++)
            {
                for (var x = 0; x < this.MapWidth; x++)
                {
                    this.Set(x, y, null);
                }
            }
        }

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