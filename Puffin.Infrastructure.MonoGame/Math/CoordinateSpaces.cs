using Microsoft.Xna.Framework;

namespace Puffin.Infrastructure.MonoGame
{
    internal static class CoordinateSpaces
    {
        // Translate the given screen space xy-coordinate position to the equivilant world space xy-coordinate position.
        public static Vector2 ScreenToWorld(Vector2 position, Matrix inverseMatrix)
        {
            return Vector2.Transform(position, inverseMatrix);
        }

        // Translates the given world space xy-coordinate position to the equivilant screen space xy-coordinate position.
        public static Vector2 WorldToScreen(Vector2 position, Matrix matrix)
        {
            return Vector2.Transform(position, matrix);
        }
    }
}