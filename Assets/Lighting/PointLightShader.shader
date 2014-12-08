// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Lighting2D/PointLight" {
Properties {
	_Color ("Tint", Color) = (1,1,1,1)
	_Radius ("Radius", float) = 0
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 100
	
	ZWrite Off
	Blend SrcAlpha One
	//Blend SrcAlpha OneMinusSrcAlpha
	//Blend DstColor Zero
	//Blend OneMinusDstColor One
	
	Pass {  
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				float4 position : TEXCOORD1;
			};

			float4 _MainTex_ST;
			float4 _Color;
			float _Radius;
			
			v2f vert (appdata_base v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.position = v.vertex;
				
				return o;
			}
			
			float4 frag (v2f i) : COLOR0 
			{		
				float dist = length(i.position);
				
				float att = clamp((1.0 - dist*dist/(_Radius*_Radius)) * _Color.a, 0.0, 1.0); 
				
				float4 c = _Color;
				c.a = att;
						
				return c;
			}
		ENDCG
	}
}

}