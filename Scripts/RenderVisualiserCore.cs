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
                EnableOverdrawFeature();
            }
            else
            {
                DisableOverdrawFeature();
            }
        }

        [MenuItem("Tools/Render Visualiser/Overdraw", true)]
        private static bool ToggleActionValidate()
        {
            Menu.SetChecked("Tools/Render Visualiser/Overdraw", overdraw);
            return true;
        }

        static void EnableOverdrawFeature()
        {
            var renderPipeline = GraphicsSettings.currentRenderPipeline;
            FieldInfo propertyInfo = renderPipeline.GetType()
                .GetField("m_RendererDataList", BindingFlags.Instance | BindingFlags.NonPublic);
            ForwardRendererData rendererData = ((ScriptableRendererData[]) propertyInfo?.GetValue(renderPipeline))?[0] as ForwardRendererData;

            rendererData.opaqueLayerMask = 0;
            rendererData.transparentLayerMask = 0;
            
            var feature = new RenderObjects();
            feature.name = "Overdraw";
            feature.settings.Event = RenderPassEvent.AfterRendering;
            feature.settings.filterSettings.LayerMask = ~0;
            
            var material = new Material(Shader.Find("RenderVisualiser/OverdrawShader"));
            material.color = new Color(0.2f, 0.04f, 0.04f);
            feature.settings.overrideMaterial = material;
            rendererData.rendererFeatures.Add(feature);
            
            EditorUtility.SetDirty(rendererData);
            EditorUtility.SetDirty(renderPipeline);
        }
        
        static void DisableOverdrawFeature()
        {
            var renderPipeline = GraphicsSettings.currentRenderPipeline;
            FieldInfo propertyInfo = renderPipeline.GetType()
                .GetField("m_RendererDataList", BindingFlags.Instance | BindingFlags.NonPublic);
            ForwardRendererData rendererData = ((ScriptableRendererData[]) propertyInfo?.GetValue(renderPipeline))?[0] as ForwardRendererData;

            rendererData.opaqueLayerMask = ~0;
            rendererData.transparentLayerMask = ~0;
            
            rendererData.rendererFeatures.RemoveAll(f => f.name == "Overdraw");
            
            EditorUtility.SetDirty(rendererData);
            EditorUtility.SetDirty(renderPipeline);
        }
    }
}