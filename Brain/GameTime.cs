namespace game_2.Brain
{
    public static class GameTime
    {
        public static float Time { get; private set; }
        private static bool Started = false;
        private static bool Paused = false;

        public static void Next()
        {
            if (Started && !Paused) Time++;
        }

        public static void Next(long delta)
        {
            if (Started && !Paused) Time += delta;
        }

        public static void NextFaster()
        {
            if (Started && !Paused) Time += 3;
        }

        public static void Reset()
        {
            if (Started) Time = 0;
        }

        public static void Start()
        {
            if (!Started)
            {
                Time = 0;
                Started = true;
            }
        }

        public static void End()
        {
            if (Started)
            {
                Time = 0;
                Started = false;
            }
        }

        public static void PlayOrPause()
        {
            if (Started && !Paused)
            {
                Paused = true;
            }
        }

        public static bool IsRunning
        {
            get
            {
                return Started;
            }
        }
    }
}
