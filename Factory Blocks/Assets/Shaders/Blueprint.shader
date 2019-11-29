Shader "Sprites/Blueprint"{
	Properties{
		_Color("Tint", Color) = (0, 0, 0, 1)
		_MainTex("Texture", 2D) = "white" {}
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		_Wipe("Wipe", Vector) = (1,1,0,0) //posx,posy,0,slope
		_LineColor("Line Color", Color) = (0, 0, 0, 1)
		_BlueprintColor("Blueprint Color", Color) = (0, 0, 0, 1)
		_DissolveTexture("Dissolve Texture", 2D) = "white" {}
		_SpikeAmount("Spike Amount", Range(0,1)) = 0
		_EdgeColor("Spike Edge Color", Color) = (0, 0, 0, 1)
		_BlueprintPallete("Blueprint Pallete", 2D) = "white" {}

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
				float4 _Wipe;
				fixed4 _Color;
				fixed4 _EdgeColor;
				fixed4 _LineColor;
				fixed4 _BlueprintColor;
				sampler2D _DissolveTexture;
				half _SpikeAmount;
				sampler2D _BlueprintPallete;

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

					//Line
					float3 posDiff = i.worldPos - _Wipe.xyz;
					float3 lineNormal = normalize(float3(1, _Wipe.w, 0));
					float side = step(dot(normalize(posDiff), lineNormal), 0);
					float dist = length(cross(cross(lineNormal,float3(0,0,1)), posDiff));
					float brightness = clamp(1 - pow(dist, 2),0,1);

					fixed4 brightSide = col + _LineColor * brightness * side;

					//Get if Blueprint 
					float blueprintWhite = 1000;
					for (float n = 0; n < 10; n++) {
						float4 palCol = tex2D(_BlueprintPallete, float2(n, 0));
						float3 diff = abs(col - palCol);
						blueprintWhite = min(blueprintWhite, length(diff));
					}
					blueprintWhite = step(.4f, blueprintWhite);

					//Blueprint section
					fixed4 dullSide = blueprintWhite + (1 - blueprintWhite) * _BlueprintColor * col.a;
					col = side * brightSide + (1 - side) * dullSide;

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