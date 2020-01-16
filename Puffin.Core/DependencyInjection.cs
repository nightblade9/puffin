using System;
using Ninject;

namespace Puffin.Core
{
    public static class DependencyInjection
    {
        public static StandardKernel Kernel = new StandardKernel();

        public static void Reset()
        {
            Kernel = new StandardKernel();
        }
    }
}