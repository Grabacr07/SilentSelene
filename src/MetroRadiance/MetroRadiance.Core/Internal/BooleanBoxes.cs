namespace MetroRadiance.Internal
{
    internal static class BooleanBoxes
    {
        internal static object TrueBox = true;
        internal static object FalseBox = false;

        internal static object Box(bool value)
            => value ? TrueBox : FalseBox;
    }
}
