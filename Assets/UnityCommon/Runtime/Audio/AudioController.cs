﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    public AudioListener Listener { get { return audioListener ?? FindOrAddListener(); } }
    public float Volume { get { return AudioListener.volume; } set { AudioListener.volume = value; } }
    public bool IsMuted { get { return AudioListener.pause; } set { AudioListener.pause = value; } }

    private AudioListener audioListener;
    private Tweener<FloatTween> listenerVolumeTweener;
    private Dictionary<AudioClip, AudioTrack> audioTracks = new Dictionary<AudioClip, AudioTrack>();
    private Stack<AudioSource> sourcesPool = new Stack<AudioSource>();

    private void Awake ()
    {
        listenerVolumeTweener = new Tweener<FloatTween>(this);
        FindOrAddListener();
    }

    /// <summary>
    /// Sets transform of the current <see cref="Listener"/> as a child of the provided target.
    /// </summary>
    public void AttachListener (Transform target)
    {
        Listener.transform.SetParent(target);
        Listener.transform.localPosition = Vector3.zero;
    }

    public void FadeVolume (float volume, float time)
    {
        if (listenerVolumeTweener.IsRunning)
            listenerVolumeTweener.CompleteInstantly();

        var tween = new FloatTween(Volume, volume, time, value => Volume = value, ignoreTimeScale: true);
        listenerVolumeTweener.Run(tween);
    }

    public bool IsClipPlaying (AudioClip clip)
    {
        if (!clip) return false;
        return audioTracks.ContainsKey(clip) && audioTracks[clip].IsPlaying;
    }

    public AsyncAction PlayClip (AudioClip clip, AudioSource audioSource = null, float volume = 1f, 
        float fadeInTime = 0f, bool loop = false, AudioMixerGroup mixerGroup = null)
    {
        if (!clip) return AsyncAction.CreateCompleted();

        if (audioTracks.ContainsKey(clip)) StopClip(clip);
        PoolUnusedSources();

        // In case user somehow provided one of our pooled sources, don't use it.
        if (audioSource && IsOwnedByController(audioSource)) audioSource = null;
        if (!audioSource) audioSource = GetPooledSource();

        var track = new AudioTrack(clip, audioSource, this, volume, loop, mixerGroup);
        audioTracks.Add(clip, track);
        return track.Play(fadeInTime);
    }

    public AsyncAction StopClip (AudioClip clip, float fadeOutTime)
    {
        if (!clip) return AsyncAction.CreateCompleted();
        if (!IsClipPlaying(clip)) return AsyncAction.CreateCompleted();
        return GetTrack(clip).Stop(fadeOutTime);
    }

    public void StopClip (AudioClip clip)
    {
        if (!clip) return;
        if (!IsClipPlaying(clip)) return;
        GetTrack(clip).Stop();
    }

    public AsyncAction StopAllClips (float fadeOutTime)
    {
        foreach (var track in audioTracks.Values)
            track.Stop(fadeOutTime);
        return new Timer(fadeOutTime, coroutineContainer: this).Run();
    }

    public void StopAllClips ()
    {
        foreach (var track in audioTracks.Values)
            track.Stop();
    }

    public AudioTrack GetTrack (AudioClip clip)
    {
        if (!clip) return null;
        return audioTracks.ContainsKey(clip) ? audioTracks[clip] : null;
    }

    public HashSet<AudioTrack> GetTracksByClipName (string clipName)
    {
        return new HashSet<AudioTrack>(audioTracks.Values.Where(track => track.Name == clipName));
    }

    private AudioListener FindOrAddListener ()
    {
        audioListener = FindObjectOfType<AudioListener>();
        if (!audioListener) audioListener = gameObject.AddComponent<AudioListener>();
        return audioListener;
    }

    private bool IsOwnedByController (AudioSource audioSource)
    {
        return GetComponents<AudioSource>().Contains(audioSource);
    }

    private AudioSource GetPooledSource ()
    {
        if (sourcesPool.Count > 0) return sourcesPool.Pop();
        return gameObject.AddComponent<AudioSource>();
    }

    private void PoolUnusedSources ()
    {
        foreach (var track in audioTracks.Values.ToList())
            if (!track.IsPlaying)
            {
                if (IsOwnedByController(track.Source))
                    sourcesPool.Push(track.Source);
                audioTracks.Remove(track.Clip);
            }
    }
}
