namespace FadeJson
{
    public static class NumberUtils
    {
        /// <summary>
        /// x in [min, max)
        /// </summary>
        public static bool IsInRange(this int x, int min, int max) {
            return min <= x && x < max;
        }

        public static bool IsNumber(this char chr) {
            return '0' <= chr && chr <= '9';
        }
        
    }
}