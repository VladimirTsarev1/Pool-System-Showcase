using System;
using System.Collections.Generic;
using UnityEngine;

namespace Showcase.Scripts.Core.Patterns
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> Map = new();

        public static void Register<T>(T instance)
        {
            Map[typeof(T)] = instance;
        }

        public static T Resolve<T>()
        {
            if (Map.TryGetValue(typeof(T), out var service))
            {
                return (T)service;
            }

            Debug.LogError($"Service not registered: {typeof(T).Name}");
            return default;
        }

        public static bool TryResolve<T>(out T service)
        {
            if (Map.TryGetValue(typeof(T), out var obj))
            {
                service = (T)obj;
                return true;
            }

            service = default!;
            return false;
        }

        public static void Clear()
        {
            Map.Clear();
        }
    }
}