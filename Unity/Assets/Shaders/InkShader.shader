Shader "Plant/InkShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_FriendColor ("Friend", Color) = (1, 0, 0, 0)
		_EnemyColor ("Enemy", Color) = (0, 1, 0, 0)
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
				float2 uv0 : TEXCOORD0;
				float4 uv1 : TEXCOORD1;
			};

			struct v2f
			{
				float2 uv0 : TEXCOORD0;
				float4 uv1 : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv0 = v.uv0;
				o.uv1 = v.uv1;
				return o;
			}
			
			sampler2D _MainTex;
			fixed4 _FriendColor;
			fixed4 _EnemyColor;

			fixed4 frag (v2f i) : SV_Target
			{
				const float SCALE = 0.24;
				fixed4 col = tex2D(_MainTex, i.uv0);
				float theta1 = sin( i.uv1.x * 3 ) + sin( i.uv1.y * 5 );
				float theta2 = sin( i.uv1.x * 5 ) + sin( i.uv1.y * 3 );
				if(_FriendColor.a == 0 && _EnemyColor.a == 0) return col;

				if(i.uv1.w > i.uv1.z)
				{
					if(i.uv1.z > 1 - theta1 * theta1 * SCALE){ col = _FriendColor; }
					if(i.uv1.w > 1 - theta2 * theta2 * SCALE){ col = _EnemyColor; }
				}
				else
				{
					if(i.uv1.w > 1 - theta2 * theta2 * SCALE){ col = _EnemyColor; }
					if(i.uv1.z > 1 - theta1 * theta1 * SCALE){ col = _FriendColor; }
				}
				return col;
			}
			ENDCG
		}
	}
}
