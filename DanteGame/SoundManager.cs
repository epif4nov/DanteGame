using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace DanteGame
{
    public static class SoundManager
    {
        public static SoundEffect AttackSound { get; private set; }
        public static SoundEffect HurtSound { get; private set; }
        public static SoundEffect EnemyDeathSound { get; private set; }
        public static SoundEffect VictorySound { get; private set; }
        public static SoundEffect GameOverSound { get; private set; }

        public static SoundEffect LandSound { get; private set; }

        public static SoundEffect WilhelmScream { get; private set; }

        public static void LoadSounds(ContentManager content)
        {
            AttackSound = content.Load<SoundEffect>("Sounds/Sword");
            HurtSound = content.Load<SoundEffect>("Sounds/Damage");
            VictorySound = content.Load<SoundEffect>("Sounds/Victory");
            GameOverSound = content.Load<SoundEffect>("Sounds/Game_Over");
            LandSound = content.Load<SoundEffect>("Sounds/Jump");
            WilhelmScream = content.Load<SoundEffect>("Sounds/Wscream");
        }
    }
}