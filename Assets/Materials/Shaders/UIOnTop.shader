Shader "Custom/UIOnTop"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1) // Base color (not directly used but available if needed)
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }

        Pass
        {
            // Disable depth writing and make sure the UI always renders on top
            ZWrite Off
            ZTest Always
            Cull Off

            // Proper blending mode for transparency
            Blend SrcAlpha OneMinusSrcAlpha

            // Set up the texture and transparency handling
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR; // Vertex color (includes UI Image color)
            };

            struct v2f
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR; // Pass vertex color to fragment shader
            };

            // Sampler for the texture and main color
            sampler2D _MainTex;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color; // Pass vertex color (includes alpha)
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                // Sample the texture at the given UV coordinates
                half4 texColor = tex2D(_MainTex, i.uv);

                // Multiply texture color by vertex color (including alpha)
                texColor *= i.color;

                // Return the final color (with transparency)
                return texColor;
            }
            ENDCG
        }
    }
    Fallback "UI/Default"
}
