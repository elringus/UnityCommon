﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class LocalResourceProvider : MonoRunnerResourceProvider
{
    /// <summary>
    /// Path to the folder where resources are located (realtive to <see cref="Application.dataPath"/>).
    /// </summary>
    public string RootPath { get; set; }

    private Dictionary<Type, IConverter> converters = new Dictionary<Type, IConverter>();

    /// <summary>
    /// Adds a resource type converter.
    /// </summary>
    public void AddConverter<T> (IRawConverter<T> converter) where T : class
    {
        converters.Add(typeof(T), converter);
    }

    protected override AsyncRunner<Resource<T>> CreateLoadRunner<T> (Resource<T> resource)
    {
        return new LocalResourceLoader<T>(RootPath, resource, ResolveConverter<T>(), this);
    }

    protected override AsyncRunner<List<Resource<T>>> CreateLocateRunner<T> (string path)
    {
        return new LocalResourceLocator<T>(RootPath, path, ResolveConverter<T>(), this);
    }

    protected override void UnloadResource (Resource resource)
    {
        if (resource.IsValid && resource.IsUnityObject)
            Destroy(resource.AsUnityObject);
    }

    private IRawConverter<T> ResolveConverter<T> ()
    {
        var resourceType = typeof(T);
        if (!converters.ContainsKey(resourceType))
        {
            Debug.LogError(string.Format("Converter for resource of type '{0}' is not available.", resourceType.Name));
            return null;
        }
        return converters[resourceType] as IRawConverter<T>;
    }
}
