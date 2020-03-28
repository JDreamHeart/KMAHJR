Shader "Custom/BreathLightShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _MaskTex("Mask Texture",2D) = "white"{} // 遮罩图
        _LightColor("Breath Light Color", Color) = (1,1,1,1) // 呼吸颜色
        _Speed("Breath Light Speed", Range(-5,5)) = 1 // 呼吸速度
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _MaskTex;
        float4 _LightColor;
        float _Speed;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // 通过cos来进行-1到1之间周期计算
            fixed v = cos(_Time.y * _Speed);
            // 获取要进行呼吸灯效果处理的纹理，由于使用透明图占内存，故使用蓝色通道来处理，而不用alpha来处理
            fixed maskb = tex2D(_MaskTex, IN.uv_MainTex).b;
            // 算出呼吸叠加的颜色，cos算出来是-1到1，但是我们要0到1，所以只能（算出来的数值+1）/2
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color + (v + 1)*0.5f * _LightColor;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
