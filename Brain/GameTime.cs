namespace game_2.Brain
{
    public static class GameTime
    {
        public static float Time { get; private set; }
        private static bool Started = false;

        public static void Next()
        {
            if (Started) Time++;
        }

        public static void NextFaster()
        {
            if (Started) Time += 3;
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

        public static bool IsRunning
        {
            get
            {
                return Started;
            }
        }
    }
}
