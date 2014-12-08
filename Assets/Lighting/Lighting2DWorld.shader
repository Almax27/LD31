Shader "Lighting2D/World"
{
	Properties
	{
		[PerRendererData] _MainTex ("Lit Texture", 2D) = "white" {}
		_UnlitTex ("Unlit Texture", 2D) = "white" {}
		_LightingTex ("Lighting Texture", 2D) = "black" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile DUMMY PIXELSNAP_ON
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
				float2 screenPos  : TEXCOORD1;
			};
			
			fixed4 _Color;
			
			sampler2D _MainTex;
			sampler2D _UnlitTex;
			sampler2D _LightingTex;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.screenPos = ComputeScreenPos (OUT.vertex).xy;
				
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 light = tex2D (_LightingTex, IN.screenPos);
				
				float4 unlitColor = tex2D (_UnlitTex, IN.texcoord);
				unlitColor.rgb *= (1 - light.a);
				
				float4 litColor = tex2D (_MainTex, IN.texcoord) ;
				litColor.rgb *= light.rgb * light.a;				
				
				float4 c = unlitColor + litColor;
				
				return c * IN.color;
				//return l;
			}
		ENDCG
		}
	}
}
