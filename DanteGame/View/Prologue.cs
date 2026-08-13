using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DanteGame.View
{
    public class Prologue
    {
        private Texture2D image;
        private string[] stanzas;
        private SpriteFont font;

        private float imageAlpha = 1f;
        private float textAlpha = 0f;
        private float fadeTimer = 0f;
        private float _fadeSpeed = 1.5f;

        private int currentStanza = -1;
        private bool isFadingIn = false;
        private bool isFadingOut = false;
        private bool isFinished = false;

        private bool wasMousePressed = false;
        private bool wasKeyPressed = false;

        public Prologue(Texture2D image, string[] stanzas, SpriteFont font)
        {
            this.image = image;
            this.stanzas = stanzas;
            this.font = font;
        }

        public void Reset()
        {
            currentStanza = -1;
            textAlpha = 0f;
            imageAlpha = 1f;
            isFadingIn = false;
            isFadingOut = false;
            isFinished = false;
            fadeTimer = 0f;
            ShowNextStanza();
        }

        private void ShowNextStanza()
        {
            currentStanza++;

            if (currentStanza >= stanzas.Length)
            {
                isFadingOut = true;
                isFadingIn = false;
                fadeTimer = 0f;
            }
            else
            {
                textAlpha = 0f;
                isFadingIn = true;
                isFadingOut = false;
                fadeTimer = 0f;
            }
        }

        public void Update(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            MouseState mouseState = Mouse.GetState();
            KeyboardState keyState = Keyboard.GetState();

            var isPressed = mouseState.LeftButton == ButtonState.Pressed ||
                             keyState.IsKeyDown(Keys.Space) ||
                             keyState.IsKeyDown(Keys.Enter);

            var wasPressed = wasMousePressed || wasKeyPressed;

            if (isPressed && !wasPressed && !isFadingOut && !isFinished)
            {
                if (isFadingIn)
                {
                    textAlpha = 1f;
                    isFadingIn = false;
                    ShowNextStanza();
                }
                else if (textAlpha >= 0.99f)
                {
                    ShowNextStanza();
                }
            }

            wasMousePressed = mouseState.LeftButton == ButtonState.Pressed;
            wasKeyPressed = keyState.IsKeyDown(Keys.Space) || keyState.IsKeyDown(Keys.Enter);

            if (isFadingIn)
            {
                fadeTimer += deltaTime;
                textAlpha = MathHelper.Clamp(fadeTimer * _fadeSpeed, 0f, 1f);

                if (textAlpha >= 1f)
                {
                    textAlpha = 1f;
                    isFadingIn = false;
                }
            }

            if (isFadingOut)
            {
                fadeTimer += deltaTime;
                imageAlpha = MathHelper.Clamp(1f - fadeTimer * _fadeSpeed, 0f, 1f);
                textAlpha = MathHelper.Clamp(1f - fadeTimer * _fadeSpeed, 0f, 1f);

                if (imageAlpha <= 0f && textAlpha <= 0f)
                {
                    isFadingOut = false;
                    isFinished = true;
                }
            }
        }

        public bool IsFinished()
        {
            return isFinished;
        }

        public void Draw(SpriteBatch spriteBatch, int screenWidth, int screenHeight)
        {
            Texture2D pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.Black });
            spriteBatch.Draw(pixel, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);

            if (image != null && imageAlpha > 0)
            {
                Vector2 imageSize = new Vector2(image.Width, image.Height);
                Vector2 imagePos = new Vector2(
                    screenWidth / 2 - imageSize.X / 2,
                    screenHeight / 2 - imageSize.Y / 2
                );
                spriteBatch.Draw(image, imagePos, Color.White * imageAlpha);
            }

            if (currentStanza >= 0 && currentStanza < stanzas.Length && textAlpha > 0)
            {
                string[] lines = stanzas[currentStanza].Split('\n');

                Texture2D pixelTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                pixelTexture.SetData(new[] { Color.Black });

                float maxWidth = 0;
                foreach (string line in lines)
                {
                    var lineWidth = font.MeasureString(line).X;
                    if (lineWidth > maxWidth)
                        maxWidth = lineWidth;
                }

                var lineHeight = font.MeasureString(lines[0]).Y;
                var totalHeight = lines.Length * lineHeight;

                var padding = 25;
                var startY = screenHeight / 2 + 100;

                Rectangle textBgRect = new Rectangle(
                    (int)(screenWidth / 2 - maxWidth / 2 - padding),
                    (int)(startY - padding),
                    (int)maxWidth + padding * 2,
                    (int)totalHeight + padding * 2
                );

                spriteBatch.Draw(pixelTexture, textBgRect, Color.Black * 0.50f * textAlpha);

                for (var i = 0; i < lines.Length; i++)
                {
                    Vector2 textSize = font.MeasureString(lines[i]);
                    Vector2 textPos = new Vector2(
                        screenWidth / 2 - textSize.X / 2,
                        startY + i * lineHeight
                    );

                    spriteBatch.DrawString(font, lines[i], textPos + new Vector2(2, 2), Color.Black * textAlpha);

                    spriteBatch.DrawString(font, lines[i], textPos, Color.White * textAlpha);
                }
            }

            if (!isFadingOut && !isFinished && currentStanza >= 0)
            {
                var hint = "[Пробел / Enter / Клик]";
                Vector2 hintSize = font.MeasureString(hint);
                Vector2 hintPos = new Vector2(
                    screenWidth / 2 - hintSize.X / 2,
                    screenHeight - 60
                );

                spriteBatch.DrawString(font, hint, hintPos + new Vector2(1, 1), Color.Black * 0.5f);
                spriteBatch.DrawString(font, hint, hintPos, Color.Gray * 0.7f);
            }
        }
    }
}