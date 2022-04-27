namespace SomethingFromNothing
{
    public enum EVertexType
    {
        Sun,
        Nutrients,
        Water
    }

    class SFNGame {
        static int score;

        public static void AddScore(int points)
        {
            score += points;
        }

        public static void ResetScore()
        {
            score = 0;
        }
    }
    
}