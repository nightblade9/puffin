using Microsoft.Xna.Framework;

namespace Puffin.Infrastructure.MonoGame
{
    public static class CoordinateSpaces
    {
        /// <summary>
        /// Translate the given screen space xy-coordinate position to the equivilant world space xy-coordinate position
        /// </summary>
        /// <param name="position">The xy-coordinate position in screen space to translate</param>
        /// <returns>The xy-coodinate position in world space<returns>
        public static Vector2 ScreenToWorld(Vector2 position, Matrix inverseMatrix)
        {
            return Vector2.Transform(position, inverseMatrix);
        }

        /// <summary>
        /// Translates the given world space xy-coordinate position to the equivilant screen space xy-coordinate position
        /// </summary>
        /// <param name="position">The xy-coordinate position in world space to translate</param>
        /// <returns>The xy-coodinate position in screen space<returns>
        public static Vector2 WorldToScreen(Vector2 position, Matrix matrix)
        {
            return Vector2.Transform(position, matrix);
        }
    }
}