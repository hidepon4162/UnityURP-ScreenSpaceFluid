﻿using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SphRendererFeature : ScriptableRendererFeature
{
    [Serializable]
    public sealed class SphSettings
    {
        public Shader SphShader;
        public RenderPassEvent Event = RenderPassEvent.BeforeRenderingTransparents;

        public LayerMask LayerMask;

        public Color Tint = Color.white;

        [Range(0f, 0.1f)]
        public float DepthThreshold = 0.001f;

        public float DistortionStrength = 1f;

        [Range(1, 16)]
        public int BlurryIterations = 1;

        [Range(0, 5000)]
        public int RenderQueueLowerBound = 0;

        [Range(0, 5000)]
        public int RenderQueueUpperBound = 5000;
    }

    [SerializeField]
    SphSettings settings = new SphSettings();

    SphPass pass;

    public override void Create()
    {
        var sphShader = settings.SphShader;
        if (sphShader == null)
            sphShader = Shader.Find("SampleSph/Hidden/Sph");

        var sphMaterial = CoreUtils.CreateEngineMaterial(sphShader);
        sphMaterial.enableInstancing = true;
        sphMaterial.SetColor("_Tint", settings.Tint);
        sphMaterial.SetFloat("_DepthThreshold", settings.DepthThreshold);
        sphMaterial.SetFloat("_DistortionStrength", settings.DistortionStrength);

        var renderQueueRange = new RenderQueueRange(
            settings.RenderQueueLowerBound,
            settings.RenderQueueUpperBound);

        pass = new SphPass(
            settings.Event,
            sphMaterial,
            settings.BlurryIterations,
            settings.LayerMask,
            renderQueueRange);
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        pass.SetUp(renderer.cameraColorTarget);
        renderer.EnqueuePass(pass);
    }
}