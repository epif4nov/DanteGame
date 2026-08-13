using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace DanteGame.Model
{
    public class FallingBlock
    {
        public Rectangle Bounds;
        private Texture2D texture;
        private bool isFalling = false;
        private float fallTimer = 0f;
        private float fallDelay = 0.5f;
        private float fallSpeed = 500f;
        private float respawnTimer = 0f;
        private float respawnDelay = 2f;
        private Vector2 startPosition;
        public bool IsActive = true;
        private bool hasTriggered = false;

        public FallingBlock(Texture2D texture, int x, int y, int width, int height)
        {
            this.texture = texture;
            Bounds = new Rectangle(x, y, width, height);
            startPosition = new Vector2(x, y);
        }

        public void Update(GameTime gameTime, Player player, List<Platform> platforms)
        {
            if (!IsActive) return;

            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!isFalling && !hasTriggered)
            {
                bool playerUnderBlock = player.Position.X + player.Width > Bounds.X &&
                                         player.Position.X < Bounds.X + Bounds.Width;

                if (playerUnderBlock)
                {
                    hasTriggered = true;
                    fallTimer = 0f;
                }
            }

            if (!isFalling && hasTriggered)
            {
                fallTimer += deltaTime;

                if (fallTimer >= fallDelay)
                {
                    isFalling = true;
                }
            }

            if (isFalling)
            {
                Bounds = new Rectangle(Bounds.X, Bounds.Y + (int)(fallSpeed * deltaTime), Bounds.Width, Bounds.Height);

                if (Bounds.Y > 800)
                {
                    IsActive = false;
                }
            }

            if (!IsActive)
            {
                respawnTimer += deltaTime;
                if (respawnTimer >= respawnDelay)
                {
                    respawnTimer = 0;
                    Bounds = new Rectangle((int)startPosition.X, (int)startPosition.Y, Bounds.Width, Bounds.Height);
                    IsActive = true;
                    isFalling = false;
                    hasTriggered = false;
                    fallTimer = 0f;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive)
            {
                Color blockColor;
                if (isFalling)
                    blockColor = Color.Red;
                else if (hasTriggered)
                    blockColor = Color.Orange;
                else
                    blockColor = Color.DarkRed;

                spriteBatch.Draw(texture, Bounds, blockColor);
            }
        }
    }
}