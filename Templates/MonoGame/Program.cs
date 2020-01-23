using System;

namespace MyGame
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            // Is it this? Is it `var game = new MonoGameDrawingSystem()`?
            // Something else?
            using (var game = new MyGame())
            {
                game.Run();
            }
        }
    }
}
