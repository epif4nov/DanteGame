using DanteGame.Model;
using DanteGame.View;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace DanteGame.Controller
{
    public enum GameState
    {
        Menu,
        Prologue,
        Playing,
        Pause,
        GameOver,
        Victory
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Player player;
        private List<Platform> platforms;
        private List<Enemy> enemies;

        private Texture2D playerTexture;
        private Texture2D platformTexture;
        private Texture2D bgLevel1;
        private Texture2D bgLevel2;
        private Texture2D bgLevel3;
        private Texture2D enemyTexture;
        private Texture2D backgroundTexture;

        private GameState currentState = GameState.Menu;

        private Menu menu;
        private Prologue prologue;
        private SpriteFont menuFont;
        private Texture2D menuBackground;
        private Texture2D prologueImage;

        private bool hasSeenPrologue = false;
        private Texture2D heartTexture;

        private Portal portal;
        private Texture2D portalTexture;

        private int currentLevel = 1;
        private Boss boss;
        private Texture2D bossTexture;

        private const float EnemyBaseSpeed = 80f;
        private const float EnemyAttackCooldown = 1f;
        private const float EnemyAttackRange = 45f;

        private const float BossSpeed = 55f;
        private const int BossHealth = 30;
        private const int BossDamage = 2;

        private const int PlayerSpeed = 450;
        private const float PlayerJumpPower = -750f;
        private const int PlayerMaxHealth = 3;

        private const int EnemiesToSpawn = 5;
        private const float SpawnDelay = 3.5f;

        private int totalEnemiesToSpawn = 8;
        private int enemiesKilledTotal = 0;
        private int currentWave = 0;
        private float waveTimer = 0f;
        private float waveDelay = 4f;
        private int enemiesToSpawnInCurrentWave = 1;

        private List<Spike> spikes;
        private Texture2D spikeTexture;

        private List<FallingBlock> fallingBlocks;

        private Song menuMusic;
        private Song level1Music;
        private Song level2Music;
        private Song bossMusic;

        private bool musicEnabled = true;

        private bool _wasPKeyPressed = false;

        private float _spikeDamageTimer = 0f;

        private Texture2D fallingBlockTexture;

        private Texture2D bonusHeartTexture;

        private List<Bonus> bonuses;

        private Texture2D bonusInvincibilityTexture;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            playerTexture = Content.Load<Texture2D>("Sprites/Dante3");
            bgLevel1 = Content.Load<Texture2D>("Sprites/Фон");
            bgLevel2 = Content.Load<Texture2D>("Sprites/Фон_2");
            bgLevel3 = Content.Load<Texture2D>("Sprites/Фон_3");
            platformTexture = Content.Load<Texture2D>("Sprites/Текстура_платформы");
            menuBackground = Content.Load<Texture2D>("Sprites/Фон");
            menuFont = Content.Load<SpriteFont>("Fonts/MenuFont");
            prologueImage = Content.Load<Texture2D>("Sprites/PrologueImage");

            enemyTexture = Content.Load<Texture2D>("Sprites/ice-spirit");
            heartTexture = Content.Load<Texture2D>("Sprites/heart");
            portalTexture = Content.Load<Texture2D>("Sprites/portal");
            bossTexture = Content.Load<Texture2D>("Sprites/Lucifer");

            menuMusic = Content.Load<Song>("Music/Requiem In D Minor_ K. 626_ Sequence_ VI. Lacrimosa Dies Illa - Slovak Philharmonic");
            level1Music = Content.Load<Song>("Music/Requiem In D Minor_ K. 626_ Introit_ Requiem Aeternam - Slovak Philharmonic");
            level2Music = Content.Load<Song>("Music/Requiem In D Minor_ K. 626_ Sequence_ I. Dies Irae - Slovak Philharmonic");
            bossMusic = Content.Load<Song>("Music/Requiem In D Minor_ K. 626_ Sequence_ V. Confutatis Maledictis - Slovak Philharmonic");

            spikeTexture = Content.Load<Texture2D>("Sprites/Spike");
            fallingBlockTexture = Content.Load<Texture2D>("Sprites/Block");
            bonusHeartTexture = Content.Load<Texture2D>("Sprites/heart1");
            bonusInvincibilityTexture = Content.Load<Texture2D>("Sprites/bonus_invincibility");

            SoundManager.LoadSounds(Content);

            string[] prologueStanzas = {
                "Чернеет неба пасть, как жуть, как антрацит,\nИ поглощает всё: мольбы, проклятья, крики.\nЛёд плющит плоть предавших, искажает лики\nВ девятом круге ада, в озере Коцит.",
                "Заломлены в страданье руки, рот открыт\nПродавшего свой край в погоне за наживой.\nМертвы своей душой, телесной болью живы\nГрех искупающие в озере Коцит.",
                "Мужей и жён безнравственно-неверных вид\nУжасен, омерзителен, безумно жалок,\nВ глазах лишь стужа без конца и без начала,\nА чресла раздавило озеро Коцит.",
                "Здесь те, кто дружбе изменил, забыв про стыд,\nТрещат их сухожилия, и рвётся кожа,\nФизическую боль вдвойне боль сердца множит,\nВмерзают зло и подлость в озеро Коцит.",
                "При жизни у предавших совесть не болит,\nНо ждёт расплата их в девятом, тёмном круге,\nЗа грех измен заплатят неизбывной мукой\nВ аду, в тисках смертельных озера Коцит.",
                "...В плаще лазурном, словно столб, застыл пиит,\nВ пурпурном одеянье рядом с ним - Вергилий,\nВзирают молча на предателей усилья,\nЧто рвутся из объятий озера Коцит."
            };
            prologue = new Prologue(prologueImage, prologueStanzas, menuFont);
            Texture2D titleTexture = Content.Load<Texture2D>("Sprites/title");
            Texture2D buttonTexture = Content.Load<Texture2D>("Sprites/button");
            Texture2D buttonHoverTexture = Content.Load<Texture2D>("Sprites/button_hover");
            menu = new Menu(menuFont, menuBackground, titleTexture, buttonTexture, buttonHoverTexture);


            CreateLevel();
        }

        private void LoadLevel(int level)
        {
            MediaPlayer.Stop();

            if (platforms == null)
                platforms = new List<Platform>();
            else
                platforms.Clear();

            if (enemies == null)
                enemies = new List<Enemy>();
            else
                enemies.Clear();

            if (bonuses != null)
                bonuses.Clear();

            spikes = null;
            fallingBlocks = null;
            portal = null;
            boss = null;

            switch (level)
            {
                case 1:
                    platforms.Add(new Platform(platformTexture, 0, 700, 1280, 20));
                    platforms.Add(new Platform(platformTexture, 100, 600, 150, 20));
                    platforms.Add(new Platform(platformTexture, 900, 550, 150, 20));
                    platforms.Add(new Platform(platformTexture, 500, 450, 120, 20));
                    platforms.Add(new Platform(platformTexture, 250, 350, 100, 20));
                    platforms.Add(new Platform(platformTexture, 950, 300, 100, 20));

                    bonuses = new List<Bonus>();
                    bonuses.Add(new Bonus(bonusHeartTexture, 270, 350 - 48, 32, 32, BonusType.Health));
                    bonuses.Add(new Bonus(bonusHeartTexture, 970, 300 - 48, 32, 32, BonusType.Health));

                    var enemyFast = new Enemy(enemyTexture, 400, 600 - 48, 48, 48, EnemyType.Fast);
                    enemies.Add(enemyFast);

                    var enemySlow = new Enemy(enemyTexture, 700, 550 - 48, 48, 48, EnemyType.Slow);
                    enemies.Add(enemySlow);

                    var enemyFlying = new Enemy(enemyTexture, 600, 450 - 48, 48, 48, EnemyType.Flying);
                    enemies.Add(enemyFlying);

                    totalEnemiesToSpawn = 8;
                    enemiesKilledTotal = 0;
                    currentWave = 0;
                    waveTimer = 0f;
                    enemiesToSpawnInCurrentWave = 1;
                    SpawnWave();

                    portal = new Portal(portalTexture, 950, 300 - 64, 64, 64);

                    MediaPlayer.Play(level1Music);
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Volume = 0.4f;
                    backgroundTexture = bgLevel1;
                    break;

                case 2:
                    platforms.Add(new Platform(platformTexture, 0, 700, 1280, 20));

                    platforms.Add(new Platform(platformTexture, 100, 600, 80, 20));
                    platforms.Add(new Platform(platformTexture, 250, 580, 60, 20));
                    platforms.Add(new Platform(platformTexture, 380, 550, 60, 20));

                    platforms.Add(new Platform(platformTexture, 520, 520, 60, 20));
                    platforms.Add(new Platform(platformTexture, 650, 490, 60, 20));
                    platforms.Add(new Platform(platformTexture, 780, 460, 60, 20));
                    platforms.Add(new Platform(platformTexture, 900, 430, 60, 20));

                    platforms.Add(new Platform(platformTexture, 1050, 400, 60, 20));
                    platforms.Add(new Platform(platformTexture, 1150, 350, 60, 20));

                    platforms.Add(new Platform(platformTexture, 650, 300, 80, 20));

                    spikes = new List<Spike>();
                    for (var x = 0; x < 1280; x += 32)
                    {
                        spikes.Add(new Spike(spikeTexture, x, 699, 32, 30));
                    }

                    fallingBlocks = new List<FallingBlock>();
                    fallingBlocks.Add(new FallingBlock(fallingBlockTexture, 300, 250, 40, 40));
                    fallingBlocks.Add(new FallingBlock(fallingBlockTexture, 500, 200, 40, 40));
                    fallingBlocks.Add(new FallingBlock(fallingBlockTexture, 700, 180, 40, 40));
                    fallingBlocks.Add(new FallingBlock(fallingBlockTexture, 900, 150, 40, 40));
                    fallingBlocks.Add(new FallingBlock(fallingBlockTexture, 1100, 200, 40, 40));
                    fallingBlocks.Add(new FallingBlock(fallingBlockTexture, 650, 100, 40, 40));

                    totalEnemiesToSpawn = 0;
                    enemiesKilledTotal = 0;

                    portal = new Portal(portalTexture, 1160, 350 - 64, 64, 64);
                    portal.Activate();

                    MediaPlayer.Play(level2Music);
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Volume = 0.5f;

                    backgroundTexture = bgLevel2;
                    break;

                case 3:
                    platforms.Add(new Platform(platformTexture, 0, 700, 1280, 20));
                    platforms.Add(new Platform(platformTexture, 300, 550, 200, 20));
                    platforms.Add(new Platform(platformTexture, 780, 550, 200, 20));
                    platforms.Add(new Platform(platformTexture, 500, 420, 280, 20));

                    boss = new Boss(bossTexture, 1280 / 2 - 64, 420 - 128, 128, 128, menuFont);
                    boss.Health = BossHealth;

                    bonuses = new List<Bonus>();
                    bonuses.Add(new Bonus(bonusInvincibilityTexture, 400, 550 - 48, 32, 32, BonusType.Invincibility));
                    bonuses.Add(new Bonus(bonusInvincibilityTexture, 800, 550 - 48, 32, 32, BonusType.Invincibility));

                    MediaPlayer.Play(bossMusic);
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Volume = 0.6f;
                    backgroundTexture = bgLevel3;
                    break;
            }
        }

        private void CreateLevel()
        {
            currentLevel = 1;
            LoadLevel(currentLevel);

            Vector2 startPosition = new Vector2(110, 600 - 128);
            player = new Player(playerTexture, startPosition);

            player.BestScore = SaveData.LoadBestScore();
        }

        private void ResetLevel(bool showPrologue = true)
        {
            currentLevel = 1;
            LoadLevel(currentLevel);

            player.Health = PlayerMaxHealth;
            player.Score = 0;
            enemiesKilledTotal = 0;
            currentWave = 0;
            waveTimer = 0f;
            enemiesToSpawnInCurrentWave = 1;

            player.Position = new Vector2(110, 600 - player.Height);
            player.Velocity = Vector2.Zero;


            if (showPrologue)
            {
                prologue.Reset();
                currentState = GameState.Prologue;
            }
            else
            {
                currentState = GameState.Playing;
            }
        }

        private void SpawnWave()
        {
            if (enemiesKilledTotal >= totalEnemiesToSpawn)
                return;

            currentWave++;

            enemiesToSpawnInCurrentWave = currentWave;

            var remainingEnemies = totalEnemiesToSpawn - enemiesKilledTotal;
            var enemiesToSpawnCount = Math.Min(enemiesToSpawnInCurrentWave, remainingEnemies);

            var spawnPoints = new List<Vector2>
            {
                new Vector2(920, 550 - 48),
                new Vector2(920, 300 - 48),
                new Vector2(560, 450 - 48),
            };

            var enemyTypes = new[] { EnemyType.Slow, EnemyType.Fast, EnemyType.Flying };

            for (var i = 0; i < enemiesToSpawnCount; i++)
            {
                var spawnPoint = spawnPoints[Random.Shared.Next(spawnPoints.Count)];
                var randomType = enemyTypes[Random.Shared.Next(enemyTypes.Length)];

                var newEnemy = new Enemy(enemyTexture, spawnPoint.X, spawnPoint.Y, 48, 48, randomType);
                enemies.Add(newEnemy);
            }
        }

        protected override void Update(GameTime gameTime)
        {

            if (Keyboard.GetState().IsKeyDown(Keys.M))
            {
                if (musicEnabled)
                {
                    MediaPlayer.Volume = 0f;
                    musicEnabled = false;
                }
                else
                {
                    MediaPlayer.Volume = 0.5f;
                    musicEnabled = true;
                }
            }

            switch (currentState)
            {
                case GameState.Menu:
                    menu.Update(gameTime, 1280, 720);

                    if (MediaPlayer.State != MediaState.Playing || MediaPlayer.Queue.ActiveSong != menuMusic)
                    {
                        MediaPlayer.Play(menuMusic);
                        MediaPlayer.IsRepeating = true;
                        MediaPlayer.Volume = 0.5f;
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    {
                        var selected = menu.GetSelectedIndex();
                        if (selected == 0)
                        {
                            MediaPlayer.Stop();
                            if (!hasSeenPrologue)
                            {
                                hasSeenPrologue = true;
                                ResetLevel(true);
                            }
                            else
                            {
                                ResetLevel(false);
                            }
                        }
                        else if (selected == 1)
                            Exit();
                    }

                    var clicked = menu.GetMouseClickedIndex();
                    if (clicked != -1)
                    {
                        if (clicked == 0)
                        {
                            if (!hasSeenPrologue)
                            {
                                hasSeenPrologue = true;
                                ResetLevel(true);
                            }
                            else
                            {
                                ResetLevel(false);
                            }
                        }
                        else if (clicked == 1)
                            Exit();
                    }
                    break;

                case GameState.Prologue:
                    prologue.Update(gameTime);
                    if (prologue.IsFinished())
                        currentState = GameState.Playing;
                    break;

                case GameState.Playing:
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                        currentState = GameState.Menu;

                    if (Keyboard.GetState().IsKeyDown(Keys.D1))
                    {
                        currentLevel = 1;
                        LoadLevel(currentLevel);
                        player.Position = new Vector2(110, 600 - player.Height);
                        player.Velocity = Vector2.Zero;
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.P))
                    {
                        currentState = GameState.Pause;
                        MediaPlayer.Pause();
                    }

                    if (bonuses != null)
                    {
                        for (var i = bonuses.Count - 1; i >= 0; i--)
                        {
                            var bonus = bonuses[i];
                            bonus.Update(gameTime);

                            if (bonus.IsActive && player.GetBounds().Intersects(bonus.Bounds))
                            {
                                bonus.Apply(player);
                                bonuses.RemoveAt(i);
                            }
                        }
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.D2))
                    {
                        currentLevel = 2;
                        LoadLevel(currentLevel);
                        player.Position = new Vector2(110, 600 - player.Height);
                        player.Velocity = Vector2.Zero;
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.D3))
                    {
                        currentLevel = 3;
                        LoadLevel(currentLevel);
                        player.Position = new Vector2(110, 600 - player.Height);
                        player.Velocity = Vector2.Zero;
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.K) && currentLevel < 3)
                    {
                        currentLevel++;
                        LoadLevel(currentLevel);
                        player.Position = new Vector2(110, 600 - player.Height);
                        player.Velocity = Vector2.Zero;
                    }

                    player.Update(gameTime, GraphicsDevice, platforms, enemies);

                    if (player.IsAttacking)
                    {
                        foreach (Enemy enemy in enemies)
                        {
                            if (player.GetAttackBounds().Intersects(enemy.Bounds))
                            {
                                enemy.TakeDamage(1);
                                player.AttackHitDone = true;
                                if (enemy.Health <= 0)
                                {
                                    if (new Random().Next(100) < 10)
                                    {
                                        SoundManager.WilhelmScream?.Play();
                                    }
                                    else
                                    {
                                        SoundManager.EnemyDeathSound?.Play();
                                    }

                                    player.Score += 10;

                                    if (player.Score > player.BestScore)
                                    {
                                        player.BestScore = player.Score;
                                        SaveData.SaveBestScore(player.BestScore);
                                    }

                                    enemiesKilledTotal++;
                                }
                                break;
                            }
                        }

                        if (boss != null && player.GetAttackBounds().Intersects(boss.Bounds))
                        {
                            boss.TakeDamage(1);
                        }
                    }

                    foreach (Enemy enemy in enemies)
                        enemy.Update(gameTime, player, enemies, platforms);

                    if (boss != null)
                        boss.Update(gameTime, player);

                    if (fallingBlocks != null)
                    {
                        foreach (var block in fallingBlocks)
                            block.Update(gameTime, player, platforms);
                    }

                    enemies.RemoveAll(e => e.Health <= 0);

                    if (currentLevel == 1 && enemiesKilledTotal < totalEnemiesToSpawn)
                    {
                        waveTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                        if (waveTimer >= waveDelay && enemies.Count == 0)
                        {
                            waveTimer = 0f;
                            SpawnWave();
                        }
                    }

                    if (currentLevel < 3 && portal != null && !portal.IsActive())
                    {
                        if (currentLevel == 1 && enemiesKilledTotal >= totalEnemiesToSpawn)
                            portal.Activate();
                        else if (currentLevel == 2)
                            portal.Activate();
                    }

                    if (currentLevel < 3 && portal != null && portal.IsActive() && player.GetBounds().Intersects(portal.Bounds))
                    {
                        currentLevel++;
                        LoadLevel(currentLevel);
                        player.Position = new Vector2(110, 600 - player.Height);
                        player.Velocity = Vector2.Zero;
                    }

                    if (boss != null && player.GetBounds().Intersects(boss.Bounds) && player.RespawnInvincibility <= 0)
                    {
                        player.Health--;
                        player.RespawnInvincibility = 1.5f;
                        player.Position = new Vector2(110, 600 - player.Height);
                        player.Velocity = Vector2.Zero;
                    }

                    if (boss != null && boss.Health <= 0)
                    {
                        MediaPlayer.Stop();
                        SoundManager.VictorySound?.Play();
                        currentState = GameState.Victory;
                    }

                    if (spikes != null)
                    {
                        foreach (var spike in spikes)
                        {
                            if (player.GetBounds().Intersects(spike.Bounds) && player.RespawnInvincibility <= 0)
                            {
                                player.Health--;
                                player.RespawnInvincibility = 1f;
                                player.Position = new Vector2(110, 600 - player.Height);
                                player.Velocity = Vector2.Zero;
                                break;
                            }
                        }
                    }

                    if (fallingBlocks != null)
                    {
                        foreach (var block in fallingBlocks)
                        {
                            if (block.Bounds.Intersects(player.GetBounds()) && player.RespawnInvincibility <= 0 && block.IsActive)
                            {
                                player.Health--;
                                player.RespawnInvincibility = 1.5f;
                                player.Position = new Vector2(110, 600 - player.Height);
                                player.Velocity = Vector2.Zero;
                            }
                        }
                    }

                    if (player.Health <= 0)
                    {
                        SoundManager.GameOverSound?.Play();
                        currentState = GameState.GameOver;
                    }
                    break;

                case GameState.Pause:

                    if (Keyboard.GetState().IsKeyDown(Keys.P) || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {
                        currentState = GameState.Playing;
                        MediaPlayer.Resume();
                    }
                    break;

                case GameState.GameOver:

                    if (MediaPlayer.State == MediaState.Playing)
                    {
                        MediaPlayer.Stop();
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    {
                        ResetLevel(false);
                        currentState = GameState.Menu;
                        MediaPlayer.Play(menuMusic);
                    }
                    break;

                case GameState.Victory:
                    if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    {
                        ResetLevel(false);
                        currentState = GameState.Menu;
                    }
                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            switch (currentState)
            {
                case GameState.Menu:
                    spriteBatch.Begin(samplerState: SamplerState.PointClamp);
                    menu.Draw(spriteBatch, 1280, 720);
                    spriteBatch.End();
                    break;

                case GameState.Prologue:
                    spriteBatch.Begin(samplerState: SamplerState.PointClamp);
                    prologue.Draw(spriteBatch, 1280, 720);
                    spriteBatch.End();
                    break;

                case GameState.Playing:
                    GraphicsDevice.Clear(Color.CornflowerBlue);
                    spriteBatch.Begin(samplerState: SamplerState.PointClamp);

                    spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, 1280, 720), Color.White);

                    foreach (Platform platform in platforms)
                        platform.Draw(spriteBatch);

                    foreach (Enemy enemy in enemies)
                        enemy.Draw(spriteBatch);

                    if (spikes != null)
                    {
                        foreach (var spike in spikes)
                            spike.Draw(spriteBatch);
                    }

                    if (fallingBlocks != null)
                    {
                        foreach (var block in fallingBlocks)
                            block.Draw(spriteBatch);
                    }

                    if (boss != null)
                        boss.Draw(spriteBatch);

                    if (portal != null)
                        portal.Draw(spriteBatch);

                    player.Draw(spriteBatch, gameTime);
                    player.DrawHealthBar(spriteBatch, menuFont);

                    for (var i = 0; i < player.Health; i++)
                    {
                        spriteBatch.Draw(heartTexture, new Rectangle(20 + i * 35, 20, 32, 32), Color.White);
                    }

                    var scoreText = $"Очки: {player.Score}";
                    var scoreSize = menuFont.MeasureString(scoreText);
                    spriteBatch.DrawString(menuFont, scoreText, new Vector2(1280 - scoreSize.X - 20, 20), Color.Yellow);

                    if (currentLevel == 1 || currentLevel == 2)
                    {
                        var skipText = "Нажми K, чтобы перейти к следующему уровню";
                        var skipSize = menuFont.MeasureString(skipText);
                        spriteBatch.DrawString(menuFont, skipText, new Vector2(1280 - skipSize.X - 20, 60), Color.Gray * 0.7f);
                    }

                    if (bonuses != null)
                    {
                        foreach (var bonus in bonuses)
                            bonus.Draw(spriteBatch);
                    }

                    spriteBatch.End();
                    break;

                case GameState.Pause:
                    GraphicsDevice.Clear(Color.CornflowerBlue);
                    spriteBatch.Begin(samplerState: SamplerState.PointClamp);

                    spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, 1280, 720), Color.White);

                    foreach (Platform platform in platforms)
                        platform.Draw(spriteBatch);

                    foreach (Enemy enemy in enemies)
                        enemy.Draw(spriteBatch);

                    if (spikes != null)
                    {
                        foreach (var spike in spikes)
                            spike.Draw(spriteBatch);
                    }

                    if (fallingBlocks != null)
                    {
                        foreach (var block in fallingBlocks)
                            block.Draw(spriteBatch);
                    }

                    if (boss != null)
                        boss.Draw(spriteBatch);

                    if (portal != null)
                        portal.Draw(spriteBatch);

                    player.Draw(spriteBatch, gameTime);
                    player.DrawHealthBar(spriteBatch, menuFont);

                    for (var i = 0; i < player.Health; i++)
                    {
                        spriteBatch.Draw(heartTexture, new Rectangle(20 + i * 35, 20, 32, 32), Color.White);
                    }

                    var pauseScoreText = $"Очки: {player.Score}";
                    var pauseScoreSize = menuFont.MeasureString(pauseScoreText);
                    spriteBatch.DrawString(menuFont, pauseScoreText, new Vector2(1280 - pauseScoreSize.X - 20, 20), Color.Yellow);

                    var pauseText = "ПАУЗА";
                    var pauseSize = menuFont.MeasureString(pauseText);
                    spriteBatch.DrawString(menuFont, pauseText, new Vector2(1280 / 2 - pauseSize.X / 2, 360), Color.White);

                    spriteBatch.End();
                    break;

                case GameState.GameOver:
                    spriteBatch.Begin(samplerState: SamplerState.PointClamp);

                    var pixel = new Texture2D(GraphicsDevice, 1, 1);
                    pixel.SetData(new[] { Color.Black });
                    spriteBatch.Draw(pixel, new Rectangle(0, 0, 1280, 720), Color.Black * 0.7f);

                    var gameOverSize = menuFont.MeasureString("GAME OVER");
                    var gameOverPos = new Vector2(1280 / 2 - gameOverSize.X / 2, 300);
                    spriteBatch.DrawString(menuFont, "GAME OVER", gameOverPos, Color.Red);

                    var hint = "Нажми Enter для возврата в меню";
                    var hintSize = menuFont.MeasureString(hint);
                    var hintPos = new Vector2(1280 / 2 - hintSize.X / 2, 400);
                    spriteBatch.DrawString(menuFont, hint, hintPos, Color.White);

                    var gameOverBestText = $"Лучший счёт: {player.BestScore}";
                    var gameOverBestSize = menuFont.MeasureString(gameOverBestText);
                    spriteBatch.DrawString(menuFont, gameOverBestText, new Vector2(1280 / 2 - gameOverBestSize.X / 2, 470), Color.Yellow);

                    spriteBatch.End();
                    break;

                case GameState.Victory:
                    spriteBatch.Begin(samplerState: SamplerState.PointClamp);

                    var victoryPixel = new Texture2D(GraphicsDevice, 1, 1);
                    victoryPixel.SetData(new[] { Color.Black });
                    spriteBatch.Draw(victoryPixel, new Rectangle(0, 0, 1280, 720), Color.Black * 0.5f);

                    var victorySize = menuFont.MeasureString("ПОБЕДА!");
                    var victoryPos = new Vector2(1280 / 2 - victorySize.X / 2, 300);
                    spriteBatch.DrawString(menuFont, "ПОБЕДА!", victoryPos, Color.Gold);

                    var victoryHint = "Нажми Enter для возврата в меню";
                    var victoryHintSize = menuFont.MeasureString(victoryHint);
                    var victoryHintPos = new Vector2(1280 / 2 - victoryHintSize.X / 2, 400);
                    spriteBatch.DrawString(menuFont, victoryHint, victoryHintPos, Color.White);

                    var victoryBestText = $"Лучший счёт: {player.BestScore}";
                    var victoryBestSize = menuFont.MeasureString(victoryBestText);
                    spriteBatch.DrawString(menuFont, victoryBestText, new Vector2(1280 / 2 - victoryBestSize.X / 2, 470), Color.Yellow);

                    spriteBatch.End();
                    break;
            }

            base.Draw(gameTime);
        }
    }
}