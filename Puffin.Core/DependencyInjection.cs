using Ninject;

namespace Puffin.Core
{
    internal static class DependencyInjection
    {
        internal static StandardKernel Kernel = new StandardKernel();

        internal static void Reset()
        {
            Kernel = new StandardKernel();
        }
    }
}