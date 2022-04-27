namespace SomethingFromNothing
{
    public enum EVertexType
    {
        Sun,
        Nutrients,
        Water
    }

    class SFNGame {
        // Every possible permutation of vertices. Odd indices 
        public static readonly EVertexType[][] NodePermutation = new EVertexType[][]
        {
            new EVertexType[] {EVertexType.Sun, EVertexType.Nutrients, EVertexType.Water},
            new EVertexType[] {EVertexType.Sun, EVertexType.Water, EVertexType.Nutrients},
            new EVertexType[] {EVertexType.Nutrients, EVertexType.Water, EVertexType.Sun},
            new EVertexType[] {EVertexType.Nutrients, EVertexType.Sun, EVertexType.Water},
            new EVertexType[] {EVertexType.Water, EVertexType.Sun, EVertexType.Nutrients},
            new EVertexType[] {EVertexType.Water, EVertexType.Nutrients, EVertexType.Sun}
        };

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