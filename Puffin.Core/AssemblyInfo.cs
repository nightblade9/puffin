using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("Puffin.Core.UnitTests")]
[assembly:InternalsVisibleTo("Puffin.Infrastructure.MonoGame")]
[assembly:InternalsVisibleTo("Puffin.UI.UnitTests")]
// For Mock to access IDrawingSurface and other internals
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]