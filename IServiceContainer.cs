
namespace SimpleServiceContainer
{
    public interface IServiceContainer : IDisposable
    {
        void BeginScope();
        void Register<TService, TImplementation>(Lifecycle lifecycle = Lifecycle.Transient, string name = null) where TImplementation : TService;
        void Register<TService>(Func<TService> serviceFactory, Lifecycle lifecycle = Lifecycle.Transient, string name = null);
        void RegisterLazy<TService, TImplementation>() where TImplementation : TService;
        void RegisterSingleton<TService, TImplementation>() where TImplementation : TService;
        object Resolve(Type serviceType, string name = null);
        TService Resolve<TService>(string name = null);
    }
}