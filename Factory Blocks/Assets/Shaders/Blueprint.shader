Shader "Sprites/Blueprint"{
	Properties{
		_Color("Tint", Color) = (0, 0, 0, 1)
		_MainTex("Texture", 2D) = "white" {}

		_Wipe("Wipe", Vector) = (1,1,0,0) //posx,posy,0,slope
		_LineColor("Line Color", Color) = (0, 0, 0, 1)
		_BlueprintColor("Blueprint Color", Color) = (0, 0, 0, 1)
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

				sampler2D _MainTex;
				float4 _MainTex_ST;
				float4 _Wipe;
				fixed4 _Color;
				fixed4 _LineColor;
				fixed4 _BlueprintColor;

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

					float greyscale = ((0.3 * col.r) + (0.59 * col.g) + (0.11 * col.b));
					fixed4 dullSide = _BlueprintColor * greyscale + (1-greyscale) * col;
					col = side * brightSide + (1 - side) * dullSide;

					return col;
				}

				ENDCG
			}
	}
}