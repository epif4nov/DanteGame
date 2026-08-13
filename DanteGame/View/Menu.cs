using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DanteGame.View
{
    public class Menu
    {
        private string[] menuItems = { "Начать игру", "Выход" };
        private int selectedIndex = 0;
        private SpriteFont font;
        private Texture2D backgroundTexture;
        private Texture2D titleTexture;
        private Texture2D buttonTexture;
        private Texture2D buttonHoverTexture;

        private float clickScale = 1f;
        private float clickTimer = 0f;

        private const int ButtonWidth = 250;
        private const int ButtonHeight = 60;

        public Menu(SpriteFont font, Texture2D backgroundTexture, Texture2D titleTexture, Texture2D buttonTexture, Texture2D buttonHoverTexture)
        {
            this.font = font;
            this.backgroundTexture = backgroundTexture;
            this.titleTexture = titleTexture;
            this.buttonTexture = buttonTexture;
            this.buttonHoverTexture = buttonHoverTexture;
        }

        public void Update(GameTime gameTime, int screenWidth, int screenHeight)
        {
            MouseState mouseState = Mouse.GetState();
            KeyboardState keyState = Keyboard.GetState();

            if (clickTimer > 0)
            {
                clickTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (clickTimer <= 0)
                    clickScale = 1f;
            }

            if (keyState.IsKeyDown(Keys.Down) && !wasDownPressed)
            {
                selectedIndex = (selectedIndex + 1) % menuItems.Length;
            }
            if (keyState.IsKeyDown(Keys.Up) && !wasUpPressed)
            {
                selectedIndex = (selectedIndex - 1 + menuItems.Length) % menuItems.Length;
            }

            wasDownPressed = keyState.IsKeyDown(Keys.Down);
            wasUpPressed = keyState.IsKeyDown(Keys.Up);

            for (var i = 0; i < menuItems.Length; i++)
            {
                Vector2 buttonPos = new Vector2(
                    screenWidth / 2 - ButtonWidth / 2,
                    340 + i * 80
                );
                Rectangle buttonBounds = new Rectangle((int)buttonPos.X, (int)buttonPos.Y, ButtonWidth, ButtonHeight);

                if (buttonBounds.Contains(mouseState.X, mouseState.Y))
                {
                    selectedIndex = i;

                    if (mouseState.LeftButton == ButtonState.Pressed && wasMouseLeftPressed == false)
                    {
                        clickScale = 0.95f;
                        clickTimer = 0.1f;
                        mouseClickedIndex = i;
                    }
                }
            }

            wasMouseLeftPressed = mouseState.LeftButton == ButtonState.Pressed;
        }

        public int GetSelectedIndex()
        {
            return selectedIndex;
        }

        public int GetMouseClickedIndex()
        {
            var clicked = mouseClickedIndex;
            mouseClickedIndex = -1;
            return clicked;
        }

        public void Draw(SpriteBatch spriteBatch, int screenWidth, int screenHeight)
        {
            spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);

            if (titleTexture != null)
            {
                var scale = 0.8f;
                var drawWidth = (int)(titleTexture.Width * scale);
                var drawHeight = (int)(titleTexture.Height * scale);
                Vector2 titlePos = new Vector2(
                    screenWidth / 2 - drawWidth / 2,
                    50
                );
                spriteBatch.Draw(titleTexture, new Rectangle((int)titlePos.X, (int)titlePos.Y, drawWidth, drawHeight), Color.White);
            }
            else
            {
                var title = "Dante's Inferno: Circle 9";
                Vector2 titleSize = font.MeasureString(title);
                Vector2 titlePosition = new Vector2(screenWidth / 2 - titleSize.X / 2, 100);
                spriteBatch.DrawString(font, title, titlePosition, Color.DarkRed);
            }

            for (var i = 0; i < menuItems.Length; i++)
            {
                Vector2 buttonPos = new Vector2(
                    screenWidth / 2 - ButtonWidth / 2,
                    340 + i * 80
                );

                Texture2D currentButton = (i == selectedIndex && clickTimer <= 0) ? buttonHoverTexture : buttonTexture;

                var buttonScale = (i == mouseClickedIndex && clickTimer > 0) ? 0.98f : 1f;
                var scaledWidth = (int)(ButtonWidth * buttonScale);
                var scaledHeight = (int)(ButtonHeight * buttonScale);
                Vector2 scaledPos = new Vector2(
                    screenWidth / 2 - scaledWidth / 2,
                    340 + i * 80 + (ButtonHeight - scaledHeight) / 2
                );

                spriteBatch.Draw(currentButton, new Rectangle((int)scaledPos.X, (int)scaledPos.Y, scaledWidth, scaledHeight), Color.White);

                Vector2 textSize = font.MeasureString(menuItems[i]);
                Vector2 textPos = new Vector2(
                    scaledPos.X + scaledWidth / 2 - textSize.X / 2,
                    scaledPos.Y + scaledHeight / 2 - textSize.Y / 2
                );

                Color textColor;
                if (i == selectedIndex)
                {
                    textColor = new Color(201, 168, 124); // #C9A87C
                }
                else
                {
                    textColor = new Color(230, 230, 230); // #E6E6E6
                }
                spriteBatch.DrawString(font, menuItems[i], textPos, textColor);
            }
        }

        private bool wasDownPressed;
        private bool wasUpPressed;
        private bool wasMouseLeftPressed;
        private int mouseClickedIndex = -1;
    }
}