﻿// ---------------------------------------------------------------------------------------------
//  Copyright (c) 2021-2023, Jiaqi Liu. All rights reserved.
//  See LICENSE file in the project root for license information.
// ---------------------------------------------------------------------------------------------

namespace Pal3.Game.Rendering.Renderer
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using Core.DataReader.Pol;
    using Core.Primitives;
    using Dev.Presenters;
    using Engine.Core.Abstraction;
    using Engine.Core.Implementation;
    using Engine.Coroutine;
    using Engine.DataLoader;
    using Engine.Extensions;
    using Engine.Logging;
    using Engine.Renderer;
    using Material;
    using UnityEngine;
    using Color = Core.Primitives.Color;

    /// <summary>
    /// Poly(.pol) model renderer
    /// </summary>
    public class PolyModelRenderer : GameEntityScript, IDisposable
    {
        private const string ANIMATED_WATER_TEXTURE_DEFAULT_NAME_PREFIX = "w00";
        private const string ANIMATED_WATER_TEXTURE_DEFAULT_NAME = "w0001";
        private const string ANIMATED_WATER_TEXTURE_DEFAULT_EXTENSION = ".dds";
        private const int ANIMATED_WATER_ANIMATION_FRAMES = 30;
        private const float ANIMATED_WATER_ANIMATION_FPS = 20f;

        private ITextureResourceProvider _textureProvider;
        private IMaterialFactory _materialFactory;
        private Dictionary<string, Texture2D> _textureCache = new ();

        private bool _isStaticObject;
        private Color _tintColor;
        private bool _isWaterSurfaceOpaque;
        private CancellationTokenSource _animationCts;

        private readonly int _mainTexturePropertyId = Shader.PropertyToID("_MainTex");

        public void Render(PolFile polFile,
            ITextureResourceProvider textureProvider,
            IMaterialFactory materialFactory,
            bool isStaticObject,
            Color? tintColor = default,
            bool isWaterSurfaceOpaque = default)
        {
            _textureProvider = textureProvider;
            _materialFactory = materialFactory;
            _isStaticObject = isStaticObject;
            _tintColor = tintColor ?? Color.White;
            _isWaterSurfaceOpaque = isWaterSurfaceOpaque;
            _textureCache = BuildTextureCache(polFile, textureProvider);

            _animationCts = new CancellationTokenSource();

            for (var i = 0; i < polFile.Meshes.Length; i++)
            {
                RenderMeshInternal(
                    polFile.NodeDescriptions[i],
                    polFile.Meshes[i],
                    _animationCts.Token);
            }
        }

        public Bounds GetRendererBounds()
        {
            var renderers = GameEntity.GetComponentsInChildren<StaticMeshRenderer>();
            if (renderers.Length == 0)
            {
                return new Bounds(Transform.Position, Vector3.one);
            }
            Bounds bounds = renderers[0].GetRendererBounds();
            for (var i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].GetRendererBounds());
            }
            return bounds;
        }

        public Bounds GetMeshBounds()
        {
            var renderers = GameEntity.GetComponentsInChildren<StaticMeshRenderer>();
            if (renderers.Length == 0)
            {
                return new Bounds(Vector3.zero, Vector3.one);
            }
            Bounds bounds = renderers[0].GetMeshBounds();
            for (var i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].GetMeshBounds());
            }
            return bounds;
        }

        private Dictionary<string, Texture2D> BuildTextureCache(PolFile polFile,
            ITextureResourceProvider textureProvider)
        {
            Dictionary<string, Texture2D> textureCache = new();
            foreach (PolMesh mesh in polFile.Meshes)
            {
                foreach (PolTexture texture in mesh.Textures)
                {
                    foreach (var textureName in texture.Material.TextureFileNames)
                    {
                        if (string.IsNullOrEmpty(textureName)) continue;
                        if (textureCache.ContainsKey(textureName)) continue;

                        Texture2D texture2D;

                        if (_materialFactory.ShaderType == MaterialShaderType.Lit)
                        {
                            // No need to load pre-baked shadow texture if
                            // material is lit material, since shadow texture
                            // will be generated by shader in runtime.
                            // Note: all shadow texture name starts with "^"
                            texture2D = textureName.StartsWith("^") ?
                                null : textureProvider.GetTexture(textureName);
                        }
                        else
                        {
                            texture2D = textureProvider.GetTexture(textureName);
                        }

                        textureCache[textureName] = texture2D;
                    }
                }
            }
            return textureCache;
        }

        private void RenderMeshInternal(PolGeometryNode meshNode,
            PolMesh mesh,
            CancellationToken cancellationToken)
        {
            for (var i = 0; i < mesh.Textures.Length; i++)
            {
                var textures = new List<(string name, Texture2D texture)>();
                foreach (var textureName in mesh.Textures[i].Material.TextureFileNames)
                {
                    if (string.IsNullOrEmpty(textureName))
                    {
                        textures.Add((textureName, Texture2D.whiteTexture));
                        continue;
                    }

                    if (_textureCache.TryGetValue(textureName, out Texture2D textureInCache))
                    {
                        textures.Add((textureName, textureInCache));
                    }
                }

                if (textures.Count == 0)
                {
                    EngineLogger.LogWarning($"0 texture found for {meshNode.Name}");
                    return;
                }

                IGameEntity meshEntity = GameEntityFactory.Create(meshNode.Name, GameEntity, worldPositionStays: false);
                meshEntity.IsStatic = _isStaticObject;

                // Attach BlendFlag and GameBoxMaterial to the GameEntity for better debuggability
                #if UNITY_EDITOR
                var materialInfoPresenter = meshEntity.AddComponent<MaterialInfoPresenter>();
                materialInfoPresenter.blendFlag = mesh.Textures[i].BlendFlag;
                materialInfoPresenter.material = mesh.Textures[i].Material;
                #endif

                var meshRenderer = meshEntity.AddComponent<StaticMeshRenderer>();
                var blendFlag = mesh.Textures[i].BlendFlag;

                Material[] CreateMaterials(bool isWaterSurface, int mainTextureIndex, int shadowTextureIndex = -1)
                {
                    Material[] materials;
                    float waterSurfaceOpacity = 1.0f;

                    if (isWaterSurface)
                    {
                        materials = new Material[1];

                        if (!_isWaterSurfaceOpaque)
                        {
                            waterSurfaceOpacity = textures[mainTextureIndex].texture.GetPixel(0, 0).a;
                        }
                        else
                        {
                            blendFlag = GameBoxBlendFlag.Opaque;
                        }

                        materials[0] = _materialFactory.CreateWaterMaterial(
                            textures[mainTextureIndex],
                            shadowTextureIndex >= 0 ? textures[shadowTextureIndex] : (null, null),
                            waterSurfaceOpacity,
                            blendFlag);
                    }
                    else
                    {
                        materials = _materialFactory.CreateStandardMaterials(
                            RendererType.Pol,
                            textures[mainTextureIndex],
                            shadowTextureIndex >= 0 ? textures[shadowTextureIndex] : (null, null),
                            _tintColor,
                            blendFlag);
                    }
                    return materials;
                }

                if (textures.Count >= 1)
                {
                    int mainTextureIndex = textures.Count == 1 ? 0 : 1;
                    int shadowTextureIndex = textures.Count == 1 ? -1 : 0;

                    bool isWaterSurface = textures[mainTextureIndex].name
                        .StartsWith(ANIMATED_WATER_TEXTURE_DEFAULT_NAME, StringComparison.OrdinalIgnoreCase);

                    Material[] materials = CreateMaterials(isWaterSurface, mainTextureIndex, shadowTextureIndex);

                    if (isWaterSurface)
                    {
                        StartWaterSurfaceAnimation(materials[0], textures[mainTextureIndex].texture, cancellationToken);
                    }

                    _ = meshRenderer.Render(mesh.VertexInfo.GameBoxPositions.ToUnityPositions(),
                        mesh.Textures[i].GameBoxTriangles.ToUnityTriangles(),
                        mesh.VertexInfo.GameBoxNormals.ToUnityNormals(),
                        mesh.VertexInfo.Uvs[mainTextureIndex].ToUnityVector2s(),
                        mesh.VertexInfo.Uvs[Math.Max(shadowTextureIndex, 0)].ToUnityVector2s(),
                        materials,
                        false);
                }
            }
        }

        private IEnumerator AnimateWaterTextureAsync(Material material,
            Texture2D defaultTexture,
            CancellationToken cancellationToken)
        {
            var waterTextures = new List<Texture2D> { defaultTexture };

            for (var i = 2; i <= ANIMATED_WATER_ANIMATION_FRAMES; i++)
            {
                Texture2D texture = _textureProvider.GetTexture(
                    ANIMATED_WATER_TEXTURE_DEFAULT_NAME_PREFIX +
                    $"{i:00}" +
                    ANIMATED_WATER_TEXTURE_DEFAULT_EXTENSION);
                waterTextures.Add(texture);
            }

            var waterAnimationDelay = CoroutineYieldInstruction.WaitForSeconds(1 / ANIMATED_WATER_ANIMATION_FPS);

            while (!cancellationToken.IsCancellationRequested)
            {
                for (var i = 0; i < ANIMATED_WATER_ANIMATION_FRAMES; i++)
                {
                    if (cancellationToken.IsCancellationRequested) break;
                    material.SetTexture(_mainTexturePropertyId, waterTextures[i]);
                    yield return waterAnimationDelay;
                }
            }
        }

        private void StartWaterSurfaceAnimation(Material material,
            Texture2D defaultTexture,
            CancellationToken cancellationToken)
        {
            StartCoroutine(AnimateWaterTextureAsync(material, defaultTexture, cancellationToken));
        }

        protected override void OnDisableGameEntity()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_animationCts is {IsCancellationRequested: false})
            {
                _animationCts.Cancel();
            }

            foreach (StaticMeshRenderer meshRenderer in GameEntity.GetComponentsInChildren<StaticMeshRenderer>())
            {
                _materialFactory.ReturnToPool(meshRenderer.GetMaterials());
                meshRenderer.Dispose();
                meshRenderer.Destroy();
            }
        }
    }
}