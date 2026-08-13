using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DanteGame.Model
{
    public class Spike
    {
        public Rectangle Bounds;
        private Texture2D texture;
        public bool IsActive = true;

        public Spike(Texture2D texture, int x, int y, int width, int height)
        {
            this.texture = texture;
            Bounds = new Rectangle(x, y, width, height);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsActive) return;
            spriteBatch.Draw(texture, Bounds, Color.White);
        }
    }
}