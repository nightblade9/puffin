using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Puffin.Infrastructure.MonoGame;

namespace MonoGame
{
    public class MyGame : PuffinGame
    {
        override protected void Initialize()
        {
            // TODO: don't use references to source for Puffin.Infrastructure.MonoGame!
            this.ShowScene(new CoreGameScene());
        }
    }
}
