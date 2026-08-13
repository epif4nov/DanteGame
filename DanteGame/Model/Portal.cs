using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DanteGame.Model
{
    public class Portal
    {
        public Rectangle Bounds;
        private Texture2D texture;
        private bool isActive = false;

        public Portal(Texture2D texture, int x, int y, int width, int height)
        {
            this.texture = texture;
            Bounds = new Rectangle(x, y, width, height);
        }

        public void Activate()
        {
            isActive = true;
        }

        public bool IsActive()
        {
            return isActive;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (texture == null) return;

            Color color = isActive ? Color.White : Color.Gray * 0.5f;
            spriteBatch.Draw(texture, Bounds, color);
        }
    }
}