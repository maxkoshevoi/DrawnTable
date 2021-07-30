using System.Threading.Tasks;

namespace DrawnTableControl.Services
{
    internal static class Compatibility
    {
        public static async Task TaskDelay(int millisecondsDelay)
        {
#if NET40
            await TaskEx.Delay(millisecondsDelay);
#else
            await Task.Delay(millisecondsDelay);
#endif
        }
    }
}
