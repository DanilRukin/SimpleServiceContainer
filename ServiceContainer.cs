using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleServiceContainer
{
    public enum Lifecycle
    {
        Transient,
        Singleton,
        Scoped
    }

    public class ServiceContainer : IServiceContainer
    {
        private readonly Dictionary<Type, (Func<object> Factory, Lifecycle Lifecycle)> _registrations =
            new Dictionary<Type, (Func<object>, Lifecycle)>();

        private readonly Dictionary<(Type, string), Func<object>> _namedRegistrations =
            new Dictionary<(Type, string), Func<object>>();

        private readonly Dictionary<Type, object> _scopedInstances = new Dictionary<Type, object>();
        private readonly HashSet<Type> _resolvingTypes = new HashSet<Type>();
        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        public void Register<TService, TImplementation>(Lifecycle lifecycle = Lifecycle.Transient, string name = null)
            where TImplementation : TService
        {
            Func<object> factory = () =>
            {
                var constructors = typeof(TImplementation).GetConstructors();
                var constructor = constructors.OrderByDescending(c => c.GetParameters().Length).First();
                var parameters = constructor.GetParameters()
                    .Select(p => Resolve(p.ParameterType))
                    .ToArray();
                return constructor.Invoke(parameters);
            };

            if (name == null)
            {
                _registrations[typeof(TService)] = (factory, lifecycle);
            }
            else
            {
                _namedRegistrations[(typeof(TService), name)] = factory;
            }
        }

        public void Register<TService>(Func<TService> serviceFactory, Lifecycle lifecycle = Lifecycle.Transient, string name = null)
        {
            Func<object> factory = () => serviceFactory();

            if (name == null)
            {
                _registrations[typeof(TService)] = (factory, lifecycle);
            }
            else
            {
                _namedRegistrations[(typeof(TService), name)] = factory;
            }
        }

        public void RegisterSingleton<TService, TImplementation>() where TImplementation : TService
        {
            var instance = Activator.CreateInstance<TImplementation>();
            _registrations[typeof(TService)] = (() => instance, Lifecycle.Singleton);
        }

        public void RegisterLazy<TService, TImplementation>() where TImplementation : TService
        {
            Lazy<TService> lazyInstance = new Lazy<TService>(() => (TService)Resolve(typeof(TImplementation)));
            _registrations[typeof(TService)] = (() => lazyInstance.Value, Lifecycle.Singleton);
        }

        public TService Resolve<TService>(string name = null)
        {
            return (TService)Resolve(typeof(TService), name);
        }

        public object Resolve(Type serviceType, string name = null)
        {
            if (_resolvingTypes.Contains(serviceType))
            {
                throw new InvalidOperationException($"Cyclic dependency detected for type {serviceType}.");
            }

            _resolvingTypes.Add(serviceType);

            try
            {
                Func<object> factory;
                if (name == null)
                {
                    if (!_registrations.TryGetValue(serviceType, out var registration))
                    {
                        throw new InvalidOperationException($"Service of type {serviceType} is not registered.");
                    }
                    factory = registration.Factory;

                    switch (registration.Lifecycle)
                    {
                        case Lifecycle.Singleton:
                            return factory();
                        case Lifecycle.Scoped:
                            if (!_scopedInstances.TryGetValue(serviceType, out var scopedInstance))
                            {
                                scopedInstance = factory();
                                _scopedInstances[serviceType] = scopedInstance;
                            }
                            return scopedInstance;
                        case Lifecycle.Transient:
                        default:
                            var instance = factory();
                            if (instance is IDisposable disposable)
                            {
                                _disposables.Add(disposable);
                            }
                            return instance;
                    }
                }
                else
                {
                    if (!_namedRegistrations.TryGetValue((serviceType, name), out factory))
                    {
                        throw new InvalidOperationException($"Service of type {serviceType} with name {name} is not registered.");
                    }
                    return factory();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to resolve service of type {serviceType}.", ex);
            }
            finally
            {
                _resolvingTypes.Remove(serviceType);
            }
        }

        public void Validate()
        {
            foreach (var registration in _registrations)
            {
                try
                {
                    Resolve(registration.Key);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Validation failed for service of type {registration.Key}.", ex);
                }
            }
        }

        public void BeginScope()
        {
            _scopedInstances.Clear();
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
            _disposables.Clear();
        }
    }
}