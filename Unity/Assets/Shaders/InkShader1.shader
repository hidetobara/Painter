Shader "Plant/InkShader1"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color1 ("Color1", Color) = (1, 0, 0, 0)
		_Color2 ("Color2", Color) = (0, 1, 0, 0)
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 normal : NORMAL;
				float2 uv0 : TEXCOORD0;
				float4 uv1 : TEXCOORD1;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 normal : NORMAL;
				float2 uv0 : TEXCOORD0;
				float4 uv1 : TEXCOORD1;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				float4 vertex = mul(UNITY_MATRIX_MV, v.vertex);
				float4 normal = mul(UNITY_MATRIX_MV, v.normal) - mul(UNITY_MATRIX_MV, float4(0,0,0,1));
				float impulse = -dot(normalize(vertex.xyz), normalize(normal.xyz));
				if(impulse < 0) impulse = 0;
				impulse = pow(impulse, 30);
				o.normal = float4(impulse,impulse,impulse,1);
				o.uv0 = v.uv0;
				o.uv1 = v.uv1;
				return o;
			}
			
			sampler2D _MainTex;
			fixed4 _Color1;
			fixed4 _Color2;

			fixed4 frag (v2f i) : SV_Target
			{
				const float SCALE = 0.9;
				fixed4 col = tex2D(_MainTex, i.uv0);
				float theta1 = sin( i.uv1.x * 3 ) * sin( i.uv1.y * 5 );
				float theta2 = sin( i.uv1.x * 5 ) * sin( i.uv1.y * 3 );
				theta1 = theta1 * theta1;
				theta2 = theta2 * theta2;
				float4 lighten = i.normal;
				if(_Color1.a == 0 && _Color2.a == 0) return col;

				if(i.uv1.w > i.uv1.z)
				{
					if(i.uv1.z > 1 - theta1 * SCALE){ col = _Color1 + lighten; }
					if(i.uv1.w > 1 - theta2 * SCALE){ col = _Color2 + lighten; }
				}
				else
				{
					if(i.uv1.w > 1 - theta2 * SCALE){ col = _Color2 + lighten; }
					if(i.uv1.z > 1 - theta1 * SCALE){ col = _Color1 + lighten; }
				}
				return col;
			}
			ENDCG
		}
	}
}
