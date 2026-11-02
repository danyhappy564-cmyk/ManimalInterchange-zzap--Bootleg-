using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using SPT.Reflection.Patching;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Manimal.Interchange.Client;

internal static class AreaLightBindings
{
    private static readonly Dictionary<int, Scope> Scopes = new();
    private static readonly HashSet<string> ProbePaths = new(StringComparer.OrdinalIgnoreCase);
    private static readonly HashSet<int> ForeignScenes = new();
    private sealed class PathScope : IDisposable
    {
        private readonly string _path;
        internal PathScope(string path) => _path = path;
        public void Dispose() => ProbePaths.Remove(_path);
    }
    internal static IDisposable ClaimProbePath(string path)
    {
        if (string.IsNullOrEmpty(path) || !ProbePaths.Add(path)) throw new InvalidOperationException("Duplicate or empty area-light probe path");
        ForeignScenes.Clear();
        return new PathScope(path);
    }

    internal sealed class Scope : IDisposable
    {
        private readonly int _sceneHandle;
        internal readonly Shader Proxy;
        internal readonly Shader Shadow;
        internal readonly Shader Blur;
        internal Scope(Scene scene)
        {
            _sceneHandle = scene.handle;
            Proxy = Resolve("Hidden/AreaLight");
            Shadow = Resolve("Hidden/Shadowmap");
            Blur = Resolve("Hidden/BlurShadowmap");
        }
        public void Dispose() => Scopes.Remove(_sceneHandle);
    }

    internal static Scope ClaimScene(Scene scene)
    {
        // Awake runs during scene activation, before isLoaded must be true.
        if (!scene.IsValid() || Scopes.ContainsKey(scene.handle))
            throw new InvalidOperationException("Invalid or already owned area-light scene");
        var scope = new Scope(scene);
        Scopes.Add(scene.handle, scope);
        return scope;
    }

    private static Shader Resolve(string name)
    {
        if (ShadersFinder.SHADERS.TryGetValue(name, out var registered) && registered && registered.isSupported)
            return registered;
        var found = Shader.Find(name);
        if (found && found.isSupported) return found;
        throw new InvalidOperationException("Required target area-light shader unavailable: " + name);
    }

    internal static void BeforeInit(AreaLight light)
    {
        // zzap: outside an Interchange raid nothing can match, so skip the native gameObject/scene lookups too.
        if (Scopes.Count == 0 && ProbePaths.Count == 0 && !SceneLoader.OwnsAny) return;
        var scene = light.gameObject.scene;
        if (!Scopes.TryGetValue(scene.handle, out var scope))
        {
            // zzap: AreaLight.Init runs every frame for every area light (Labs: ~140 lights, ~800k calls a minute) and
            // Scene.path allocates a new string on each read - 220-250 MB a minute of garbage on Labs. A scene found
            // foreign once stays foreign: owned paths are registered before their scenes load.
            if (ForeignScenes.Contains(scene.handle)) return;
            var path = scene.path;
            if (!SceneLoader.Owns(path) && !ProbePaths.Contains(path))
            {
                ForeignScenes.Add(scene.handle);
                return;
            }
            scope = ClaimScene(scene);
        }
        light.m_ProxyShader = scope.Proxy;
        light.m_ShadowmapShader = scope.Shadow;
        light.m_BlurShadowmapShader = scope.Blur;
        if (!light.TryGetComponent<MeshFilter>(out _)) light.gameObject.AddComponent<MeshFilter>();
        if (!light.TryGetComponent<MeshRenderer>(out _)) light.gameObject.AddComponent<MeshRenderer>();
    }

    internal static void SceneUnloaded(Scene scene)
    {
        ForeignScenes.Remove(scene.handle);
        if (Scopes.TryGetValue(scene.handle, out var scope)) scope.Dispose();
    }
}

internal sealed class AreaLightInitPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod() => AccessTools.Method(typeof(AreaLight), "Init", Type.EmptyTypes);
    [PatchPrefix]
    private static void Prefix(AreaLight __instance) => AreaLightBindings.BeforeInit(__instance);
}
