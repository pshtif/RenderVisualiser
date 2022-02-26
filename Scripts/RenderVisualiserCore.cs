/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace RenderVisualiser
{
    public class RenderVisualiserCore
    {
        private static bool overdraw = false;

        [MenuItem("Tools/Render Visualiser/Overdraw")]
        private static void ToggleOverdraw()
        {
            overdraw = !overdraw;

            if (overdraw)
            {
                EnableOpaqueOverdrawFeature();
                EnableTransparentOverdrawFeature();
            }
            else
            {
                DisableOpaqueOverdrawFeature();
                DisableTransparentOverdrawFeature();
            }
        }

        [MenuItem("Tools/Render Visualiser/Overdraw", true)]
        private static bool ToggleActionValidate()
        {
            Menu.SetChecked("Tools/Render Visualiser/Overdraw", overdraw);
            return true;
        }

        static void EnableOpaqueOverdrawFeature()
        {
            var renderPipeline = GraphicsSettings.currentRenderPipeline;
            ForwardRendererData rendererData = GetRendererData(renderPipeline);

            rendererData.opaqueLayerMask = 0;
            rendererData.transparentLayerMask = 0;
            
            var feature = new RenderObjects();
            feature.name = "OverdrawOpaque";
            feature.settings.Event = RenderPassEvent.AfterRendering;
            feature.settings.filterSettings.RenderQueueType = RenderQueueType.Opaque;
            feature.settings.filterSettings.LayerMask = ~0;
            
            var material = new Material(Shader.Find("RenderVisualiser/OverdrawShader"));
            material.color = new Color(0.2f, 0.04f, 0.04f);
            feature.settings.overrideMaterial = material;
            rendererData.rendererFeatures.Add(feature);
            
            EditorUtility.SetDirty(rendererData);
            EditorUtility.SetDirty(renderPipeline);
        }
        
        static void DisableOpaqueOverdrawFeature()
        {
            var renderPipeline = GraphicsSettings.currentRenderPipeline;
            ForwardRendererData rendererData = GetRendererData(renderPipeline);

            rendererData.opaqueLayerMask = ~0;
            rendererData.transparentLayerMask = ~0;
            
            rendererData.rendererFeatures.RemoveAll(f => f.name == "OverdrawOpaque");
            
            EditorUtility.SetDirty(rendererData);
            EditorUtility.SetDirty(renderPipeline);
        }
        
        static void EnableTransparentOverdrawFeature()
        {
            var renderPipeline = GraphicsSettings.currentRenderPipeline;
            ForwardRendererData rendererData = GetRendererData(renderPipeline);

            rendererData.opaqueLayerMask = 0;
            rendererData.transparentLayerMask = 0;
            
            var feature = new RenderObjects();
            feature.name = "OverdrawTransparent";
            feature.settings.Event = RenderPassEvent.AfterRendering;
            feature.settings.filterSettings.RenderQueueType = RenderQueueType.Transparent;
            feature.settings.filterSettings.LayerMask = ~0;
            
            var material = new Material(Shader.Find("RenderVisualiser/OverdrawShader"));
            material.color = new Color(0.2f, 0.04f, 0.04f);
            feature.settings.overrideMaterial = material;
            rendererData.rendererFeatures.Add(feature);
            
            EditorUtility.SetDirty(rendererData);
            EditorUtility.SetDirty(renderPipeline);
        }
        
        static void DisableTransparentOverdrawFeature()
        {
            var renderPipeline = GraphicsSettings.currentRenderPipeline;
            ForwardRendererData rendererData = GetRendererData(renderPipeline);

            rendererData.opaqueLayerMask = ~0;
            rendererData.transparentLayerMask = ~0;
            
            rendererData.rendererFeatures.RemoveAll(f => f.name == "OverdrawTransparent");
            
            EditorUtility.SetDirty(rendererData);
            EditorUtility.SetDirty(renderPipeline);
        }

        static ForwardRendererData GetRendererData(RenderPipelineAsset p_renderPipeline)
        {
            FieldInfo propertyInfo = p_renderPipeline.GetType()
                .GetField("m_RendererDataList", BindingFlags.Instance | BindingFlags.NonPublic);
            return ((ScriptableRendererData[]) propertyInfo?.GetValue(p_renderPipeline))?[0] as ForwardRendererData;
        }
    }
}