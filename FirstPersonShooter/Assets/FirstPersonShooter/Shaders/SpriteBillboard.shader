Shader "Custom/Sprite Billboard" {
     Properties
     {
         [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
         _Color ("Tint", Color) = (1,1,1,1)
        _MarginLeft ("Margin left", float) = 0
         
     }
 
     SubShader
     {
         Tags
         { 
             "Queue"="Transparent"
             "SortingLayer"="Resources_Sprites" 
             "IgnoreProjector"="True" 
             "RenderType"="Transparent" 
             "PreviewType"="Plane"
             "CanUseSpriteAtlas"="True"
             "DisableBatching" = "True"
         }
 
         Cull Off
         Lighting Off
         ZWrite Off
         Blend One OneMinusSrcAlpha
 
         Pass
         {
         CGPROGRAM
             #pragma vertex vert
             #pragma fragment frag
             #pragma target 2.0
             #pragma multi_compile _ PIXELSNAP_ON
             #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
             #include "UnityCG.cginc"
 
             struct appdata_t
             {
                 float4 vertex   : POSITION;
                 float4 color    : COLOR;
                 float2 texcoord : TEXCOORD0;
                 UNITY_VERTEX_INPUT_INSTANCE_ID
             };
 
             struct v2f
             {
                 float4 vertex   : SV_POSITION;
                 fixed4 color    : COLOR;
                 float2 texcoord  : TEXCOORD0;
                 UNITY_VERTEX_OUTPUT_STEREO
             };
             
             fixed4 _Color;
             sampler2D _MainTex;
             float _MarginLeft;
 
             v2f vert(appdata_t IN)
             {
                 v2f OUT;
                 UNITY_SETUP_INSTANCE_ID(IN);
                 UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                 OUT.texcoord = IN.texcoord;
                 OUT.color = IN.color * _Color;
 
                half3x3 m = (half3x3)UNITY_MATRIX_M;
                half3 objectScale = half3(
                    length( half3( m[0][0], m[1][0], m[2][0] ) ),
                    length( half3( m[0][1], m[1][1], m[2][1] ) ),
                    length( half3( m[0][2], m[1][2], m[2][2] ) )
                );

                 OUT.vertex = mul(UNITY_MATRIX_P, 
                  mul(UNITY_MATRIX_MV, float4(0, 0.0, 0.0, 1.0))
                 + float4(IN.vertex.x + _MarginLeft, IN.vertex.y, 0.0, 0.0)
                  * float4(objectScale, 1.0)
                  );

                 return OUT;
             }
 
             
             sampler2D _AlphaTex;
 
             fixed4 SampleSpriteTexture (float2 uv)
             {
                 fixed4 color = tex2D (_MainTex, uv);
 
 #if ETC1_EXTERNAL_ALPHA
                 // get the color from an external texture (usecase: Alpha support for ETC1 on android)
                 color.a = tex2D (_AlphaTex, uv).r;
 #endif //ETC1_EXTERNAL_ALPHA
 
                 return color;
             }
 
             fixed4 frag(v2f IN) : SV_Target
             {
                 fixed4 c = SampleSpriteTexture (IN.texcoord) * IN.color;
                 c.rgb *= c.a;
                 return c;
             }
         ENDCG
         }
     }
 }