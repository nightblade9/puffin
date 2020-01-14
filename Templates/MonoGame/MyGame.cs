using Puffin.Infrastructure.MonoGame;

namespace MonoGame
{
    public class MyGame : PuffinGame
    {
        override protected void Ready()
        {
            // TODO: don't use references to source for Puffin.Infrastructure.MonoGame!
            this.ShowScene(new CoreGameScene());
        }
    }
}
