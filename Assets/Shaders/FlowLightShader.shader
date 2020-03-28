Shader "Custom/FlowLightShader"
{
    Properties
    {
        [HideInInspector]
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _FlashColor ("Flash Color", Color) = (1,1,1,1)
        _Angle ("Flash Angle", Range(0, 180)) = 45
        _Width ("Flash Width", Range(0, 1)) = 0.2
        _LoopTime ("Loop Time", Float) = 0.5
        _Interval ("Time Interval", Float) = 1.0
        _Reverse ("Flash Reverse", Int) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200
        Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Lambert alpha exclude_path:prepass noforwardadd

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        float4 _FlashColor;
        float _Angle;
        float _Width;
        float _LoopTime;
        float _Interval;
        int _Reverse;

        struct Input
        {
            float2 uv_MainTex;
        };

        float inFlash(float2 uv) {
            float brightness = 0;

            float angleInRad = 0.0174444 * _Angle;

            float tanInverseInRad = 1.0 / tan(angleInRad);

            float currentTime = _Time.y;

            float totalTime = _Interval + abs(_LoopTime);

            float currentTurnStartTime = (int)((currentTime / totalTime)) * totalTime;

            float currentTurnTimePassed = currentTime - currentTurnStartTime - _Interval;

            bool onLeft = (tanInverseInRad > 0);

            float xBottomFarLeft = onLeft? 0.0 : tanInverseInRad;

            float xBottomFarRight = onLeft? (1.0 + tanInverseInRad) : 1.0;

            int sign = _Reverse < 0 ? -1 : 1;
            float percent = sign * currentTurnTimePassed / _LoopTime;

            float xBottomRightBound = xBottomFarLeft + percent * (xBottomFarRight - xBottomFarLeft);

            float xBottomLeftBound = xBottomRightBound - _Width;

            float xProj = uv.x + uv.y * tanInverseInRad;

            if(xProj > xBottomLeftBound && xProj < xBottomRightBound) {
                brightness = 1.0 - abs(2.0 * xProj - (xBottomLeftBound + xBottomRightBound)) / _Width;
            }

            return brightness;
        }

        void surf (Input IN, inout SurfaceOutput o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
            float brightness = inFlash(IN.uv_MainTex);
            o.Emission = c.rgb + _FlashColor.rgb * brightness; // 改为输出Emission，不受光影响
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
