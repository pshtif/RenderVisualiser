/*
 *	Created by:  Peter @sHTiF Stefcek
 */

Shader "RenderVisualiser/OverdrawShader" {
Properties {
    _MainTex ("Base", 2D) = "white" {}
    _Color ("Main Color", Color) = (0.15,1.0,0.0,0.0)
}
 
SubShader {
    Fog { Mode Off }
    ZWrite Off
    ZTest Always
    Blend One One // additive blending
 
    Pass {
        SetTexture[_MainTex] {
            constantColor [_Color]
            combine constant lerp(texture) previous
        }
    }
}
}