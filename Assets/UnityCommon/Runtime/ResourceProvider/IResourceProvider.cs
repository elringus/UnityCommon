﻿using System;

/// <summary>
/// Implementation is able to asynchronously load and unload <see cref="UnityEngine.Object"/> at runtime.
/// </summary>
public interface IResourceProvider
{
    /// <summary>
    /// Event executed when load progress is changed.
    /// </summary>
    event Action<float> OnLoadProgress;

    /// <summary>
    /// Whether any resource loading operations are currently active.
    /// </summary>
    bool IsLoading { get; }

    /// <summary>
    /// Current resources loading progress, in 0.0 to 1.0 range.
    /// </summary>
    float LoadProgress { get; }

    /// <summary>
    /// Preloads resource asynchronously.
    /// </summary>
    /// <typeparam name="T">Type of the resource to load.</typeparam>
    /// <param name="path">Path to the resource location.</param>
    /// <param name="onLoaded">Delegate to execute when the resource is loaded.</param>
    void LoadResourceAsync<T> (string path, Action<string, T> onLoaded = null) where T : UnityEngine.Object;

    /// <summary>
    /// Unloads resource asynchronously.
    /// </summary>
    /// <param name="path">Path to the resource location.</param>
    void UnloadResourceAsync (string path);

    /// <summary>
    /// Retrieves previously loaded resource. Should attempt to load the resource if it's not available. 
    /// The implementation should be completely synchronous (blocking).
    /// </summary>
    /// <typeparam name="T">Type of the resource to retrieve.</typeparam>
    /// <param name="path">Path to the resource location.</param>
    /// <returns>The requested resource.</returns>
    T GetResource<T> (string path) where T : UnityEngine.Object;
}
