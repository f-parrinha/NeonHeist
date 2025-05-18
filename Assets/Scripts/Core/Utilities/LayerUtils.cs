namespace Core.Utilities
{
    public class LayerUtils
    {
        public const int IGNORE_PLAYER_LAYER = ~(1 << 6);
        public const int IGNORE_RAYCAST = 1 << 2;
        public const int PLAYER = 1 << 6;
        public const int AGENT = 1 << 7; 
        public static int Ignore(params int[] layers)
        {
            int final = 0;

            foreach (var layer in layers)
            {
                final |= layer;
            }

            return ~final;
        }
    }
}