namespace footballAssignment.Helpers
{
    public static class Directories
    {
        public static string GetSourceDirectory ()
        {
            var root    = Directory.GetCurrentDirectory();
            var target  = Directory.GetParent(root).Parent.Parent;

            return target.ToString();
        }
    }
}
