using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class CustomPostProcessRenderFeature : ScriptableRendererFeature
{
    [SerializeField] private Shader m_bloomShader;
    [SerializeField] private Shader m_compositeShader;
    private CustomPostProcessPass m_customPass;
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_customPass);
    }
    public override void Create()
    {
        m_customPass = new CustomPostProcessPass();
    }
}
