Shader "Custom/SpikableCrate"
{
    Properties
    {
        _Color("Tint", Color) = (0, 0, 0, 1)
		_MainTex("Texture", 2D) = "white" {}
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
        _DissolveTexture("Dissolve Texture", 2D) = "white" {}
		_SpikeAmount("Spike Amount", Range(0,1)) = 0
		_EdgeColor("Spike Edge Color", Color) = (0, 0, 0, 1)
    }
    SubShader{
		Tags{
			"RenderType" = "Transparent"
			"Queue" = "Transparent"
		}

		Blend SrcAlpha OneMinusSrcAlpha

		ZWrite off
		Cull off

		Pass{

			CGPROGRAM

			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _DissolveTexture;
			half _SpikeAmount;
			fixed4 _EdgeColor;

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				fixed4 color : COLOR;
			};

			struct v2f {
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
				fixed4 color : COLOR;
			};

			v2f vert(appdata v) {
				v2f o;
				o.position = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.color = v.color;
				#ifdef PIXELSNAP_ON
					o.position = UnityPixelSnap(o.position);
				#endif

				return o;
			}

			fixed4 frag(v2f i) : SV_TARGET{
				fixed4 col = tex2D(_MainTex, i.uv);

				//Dissolve
				half dissolve_value = 1 - tex2D(_DissolveTexture, i.uv).r;
				clip(dissolve_value - _SpikeAmount);
				half edge = step(dissolve_value - _SpikeAmount, 0.03f) * step(0.0001f, _SpikeAmount);
				col = edge * _EdgeColor + (1 - edge) * col;
				return col;
			}
		    ENDCG
        }
	}
}
