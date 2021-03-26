    Shader "SimplestShader" 
    {
       Properties 
       {
          _MainTex ("Texture", 2D) = "white" {}
          _Curvature ("_Brightness", float) = 0
          _ShadowStrength ("Shadow Strength", Range (0.0,1.0)) = 0.4
          _Curvature ("_LightDir", Vector) = (0.5,0.5,1,1) 
          _LightColor  ("_LightColor", Color) = (1,1,1,1)
          _ShadowColor  ("_ShadowColor", Color) = (1,1,1,1) 
       }
       SubShader 
       {
          Pass 
          {
          	
             Tags { "LightMode" = "ForwardBase" } 
                // make sure that all uniforms are correctly set
     
             CGPROGRAM
     
             #pragma vertex vert  
             #pragma fragment frag 
     
             #include "UnityCG.cginc"
             
            #pragma multi_compile_fwdbase
            #include "AutoLight.cginc"
            
              uniform float4 _LightColor0; 
             uniform fixed4 _LightColor, _ShadowColor; 
             float4 _Curvature;
             float _ShadowStrength;
             
            sampler2D _MainTex;
             float4 _MainTex_ST;
                
             fixed _Brightness;
             
             struct vertexInput 
             {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                float color : COLOR;
             };
             struct vertexOutput 
             {
                float4 pos : SV_POSITION;
                float4 col : COLOR;
                float2 uv : TEXCOORD0;
                SHADOW_COORDS(1)
             };
            
            
            
             vertexOutput vert(vertexInput v) 
             {
                vertexOutput o;
                
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
               float3 normal = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz);
               float3 lightDir = normalize(_WorldSpaceLightPos0.xyz); 
     
               float diff = max(0.0, dot(normal, lightDir)) *  _Curvature.x + _Curvature.y;
               float verticalAmbient =  saturate(lerp(1,  1 + (normal.y * 0.5), _Curvature.z ));
               fixed3 colored = lerp(_ShadowColor, _LightColor * _LightColor0, diff) * verticalAmbient;
              
     
                o.col = float4(colored, 1.0);
                o.pos = UnityObjectToClipPos(v.vertex);
                TRANSFER_SHADOW(o);
                return o;
             }
            

            
             float4 frag(vertexOutput i) : COLOR
             {
                float atten = 1-(1-SHADOW_ATTENUATION(i))*_ShadowStrength;
                fixed4 c = tex2D (_MainTex, i.uv) * i.col;
                return c*atten;
             }
     
             ENDCG
          }
       }
       Fallback "VertexLit"
      
    }