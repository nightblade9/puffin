using System;

namespace MonoGame
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            // Is it this? Is it `var game = new MonoGameDrawingSystem()`?
            // Something else?
            using (var game = new Scene(new MonoGameDrawingSystem()))
            {
                game.Run();
            }
        }
    }
}
