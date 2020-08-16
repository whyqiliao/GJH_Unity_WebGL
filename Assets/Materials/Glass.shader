// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "MyUnlit/GlassRefraction"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	//这里的法线贴图用于计算折射产生的扭曲
	_BumpMap("Normal Map",2D) = "bump"{}
	//这里的环境贴图用于反射周围环境的部分残影
	_Cubemap("Environment Map",cube) = "_Skybox"{}
	_Distortion("Distortion",range(0,100)) = 10
		//一个折射系数，用于控制折射和反射的占比
		_RefractAmount("Refract Amount",range(0,1)) = 1
	}
		SubShader
	{
		//保证该物体渲染时，其他不透明物体都已经渲染完成
		Tags { "RenderType" = "Opaque" "Queue" = "Transparent"}
		//抓取当前屏幕的渲染图像并存入指定纹理
		GrabPass{"_RefractionTex"}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal:NORMAL;
				float4 tangent:TANGENT;
			};

			struct v2f
			{
				float4 uv : TEXCOORD0;
				float4 pos : SV_POSITION;
				float4 scrPos : TEXCOORD4;
				float4 TtoW0:TEXCOORD1;
				float4 TtoW1:TEXCOORD2;
				float4 TtoW2:TEXCOORD3;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _BumpMap;
			float4 _BumpMap_ST;
			samplerCUBE _Cubemap;
			float _Distortion;
			fixed _RefractAmount;
			sampler2D _RefractionTex;
			float4 _RefractionTex_TexelSize;

			v2f vert(appdata v)
			{
				v2f o;

				o.pos = UnityObjectToClipPos(v.vertex);

				o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv.zw = TRANSFORM_TEX(v.uv, _BumpMap);
				//得到屏幕采样坐标
				o.scrPos = ComputeGrabScreenPos(o.pos);

				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				float3 worldNormal = UnityObjectToWorldNormal(v.normal);
				float3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
				float3 worldBinormal = cross(worldTangent, worldNormal)*v.tangent.w;

				o.TtoW0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
				o.TtoW1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
				o.TtoW2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float3 worldPos = float3(i.TtoW0.w,i.TtoW1.w,i.TtoW2.w);
				float3x3 TtoW = float3x3(i.TtoW0.xyz, i.TtoW1.xyz, i.TtoW2.xyz);

				fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));

				fixed3 tanNormal = UnpackNormal(tex2D(_BumpMap, i.uv.zw));
				fixed3 worldNormal = mul(TtoW, tanNormal);
				//对采集的屏幕图像进行关于法线方向上的扭曲和偏移，也就是模拟折射的效果
				float2 offset = tanNormal.xy*_Distortion*_RefractionTex_TexelSize.xy;
				i.scrPos.xy += offset;
				fixed3 refractCol = tex2D(_RefractionTex, i.scrPos.xy / i.scrPos.w).xyz;
				//这一块用来模拟反射的效果，反射越强，也就是透光度越低，越能看到主贴图纹理以及周围环境反射的残影
				fixed3 reflectDir = reflect(-worldViewDir, worldNormal);
				fixed4 mainTexCol = tex2D(_MainTex, i.uv.xy);
				fixed4 cubemapCol = texCUBE(_Cubemap, reflectDir);
				fixed3 reflectCol = mainTexCol.rgb*cubemapCol.rgb;
				//最后将折射和反射进行一个综合叠加，_RefractAmount可以认为是透光率，当它为1时，就是全透过而没有反射，为0时就是全反射跟镜子一样
				fixed3 color = refractCol * _RefractAmount + reflectCol * (1 - _RefractAmount);
				return fixed4(color,1.0);
			}
			ENDCG
		}
	}
		fallback "Diffuse"
}