using System.Linq;
using NUnit.Framework;
using Puffin.Core.Tiles;

namespace Puffin.Core.UnitTests.Tiles
{
    [TestFixture]
    public class TileMapTests
    {
        [Test]
        public void DefineSetsAndOverridesDefinition()
        {
            // Arrange
            var map = new TileMap(1, 1, "world.png", 32, 32);
            map.Define("Grass", 0, 0, false);
            map.Define("Tree", 2, 2, false);

            // Act
            map.Define("Tree", 0, 1, true);

            // Assert
            var actual = map.GetDefinitions();
            
            var grass = actual.Single(t => t.TileName == "Grass");
            Assert.That(grass.CellX, Is.EqualTo(0));
            Assert.That(grass.CellY, Is.EqualTo(0));
            Assert.That(grass.IsSolid, Is.False);

            var tree = actual.Single(t => t.TileName == "Tree");
            Assert.That(tree.CellX, Is.EqualTo(0));
            Assert.That(tree.CellY, Is.EqualTo(1)); // second definition
            Assert.That(tree.IsSolid, Is.True);
        }

        [Test]
        public void SetAndIndexerSetAndOverrideValue()
        {
            // Arrange
            var map = new TileMap(3, 3, "world.png", 32, 32);
            map.Define("Grass", 0, 0, false);
            map.Define("Tree", 0, 1, true);

            map.Set(0, 0, "Grass");
            map.Set(0, 1, "Grass");
            map.Set(1, 0, "Grass");
            map.Set(0, 1, "Grass");

            // Act
            map.Set(1, 1, "Tree");
            map[0, 0] = "Tree";

            // Assert
            Assert.That(map.Get(0, 0), Is.EqualTo("Tree"));
            Assert.That(map.Get(0, 1), Is.EqualTo("Grass"));
            Assert.That(map[1, 0], Is.EqualTo("Grass"));
            Assert.That(map[1, 1], Is.EqualTo("Tree"));
            Assert.That(map[2, 2], Is.Null); // never set
        }
    }
}