Shader ".naturegaze/Sky"{
    Properties{
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader{
        Tags { "Queue"="Geometry" "RenderType"="Opaque" }
		Lighting Off
		Cull Off

        Pass{
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata{
                float4 vertex : POSITION;
            };

            struct v2f{
                float4 vertex : SV_POSITION;
                float4 screenPos : TEXCOORD4;
            };

            v2f vert(appdata v){
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }

            sampler2D _MainTex;
            float _SkyAngle_x;
            float _SkyAngle_y;
            float _AspectRatio;
            float4 _MainTex_TexelSize;

            fixed4 frag(v2f i) : SV_Target{
                float2 uv = (i.screenPos.xy / i.screenPos.w);
                uv.x = uv.x * _AspectRatio * (_MainTex_TexelSize.w / _MainTex_TexelSize.z) + _SkyAngle_x;
                uv.y -= _SkyAngle_y;

                fixed4 color = tex2D(_MainTex, uv);
                if(color.a != 1) discard;

                return color;
            }

            ENDCG
        }
    }
}