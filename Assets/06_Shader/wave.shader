Shader "Unlit/wave"
{
	Properties
	{
		ModelY("ModelY",Range(-1,1)) = 0
		BridgeWaveRange("BridgeWaveRange",Range(0,1)) = 0
		RimColorRange("RimColorRange",Range(0,1))=0

		BarriorColor("BarriorColor" , Color) = (1,1,1,1)
		GlowColor("GlowColor",Color) = (1,1,1,1)
	}
		SubShader
		{
			Tags { "RenderType" = "Transparent"
					"Queue" = "Transparent"}
			Blend One	One
			ZWrite Off
			Cull Off

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 3.0

				#include "UnityCG.cginc"

				struct vIn
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
					float3 normal : NORMAL;
				};

				struct vOut
				{
					float4 vertex : SV_POSITION;
				
					float4 worldPos : TEXCOORD3;
					float3 worldNormal : TEXCOORD4;
					float4 oPos : TEXCOORD5;
					float3 oNormal : NORMAL;
				};

				sampler2D _MainTex;
				sampler2D _CameraDepthTexture;

				fixed4 BarriorColor;
				fixed4 GlowColor;
				float RimColorRange;
				float DissolveToggle;

				float ModelY;
				float BridgeWaveRange;

				vOut vert(vIn i)
				{
					vOut o;
					o.vertex = UnityObjectToClipPos(i.vertex);

					o.oPos = i.vertex;
					o.oNormal = i.normal;
					o.worldPos = mul(unity_ObjectToWorld,i.vertex);
					o.worldNormal = UnityObjectToWorldNormal(i.normal);

					return o;
				}

				fixed4 frag(vOut i) : SV_Target
				{
					float rimDegree = 0;
					float3 wCameraDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
					float3 wNormal = normalize(i.worldNormal);
					float dotForRim = abs(dot(wCameraDir, wNormal));
					rimDegree = 1 - smoothstep(0, RimColorRange, dotForRim);
					float waveDegree = 0;
					float waveEndDegree=0;
					if (abs(ModelY - i.oPos.y) > BridgeWaveRange) waveDegree = 0;
					else
					{
						waveDegree = 1 - smoothstep(0, BridgeWaveRange, abs(ModelY - i.oPos.y));
					}
					if (1-abs(i.oPos.y)  < BridgeWaveRange*2.5)
					{
						waveEndDegree = 1 - smoothstep(0, BridgeWaveRange * 2.5,( 1-abs(i.oPos.y)));
					}
					waveDegree = max(waveDegree, waveEndDegree);

					float glowDegree = max(rimDegree, waveDegree);
					//림 정도, 웨이브 정도(웨이브에 속했는지)를 모두 0~1 로 스케일후 맥스 값을 이용함.

					glowDegree = max(waveDegree, glowDegree);

					fixed4 glowColor = fixed4(lerp(BarriorColor.rgb, GlowColor.rgb, glowDegree), 1);

					fixed4 col = BarriorColor * BarriorColor.a + glowColor * glowDegree;//Blend One One 대응


					return col;


					/*

					float4 clipPos = UnityObjectToClipPos(i.oPos);
					//Vertex 함수에서 넘어온 투영공간 position을 쓰면 fragment 처리에서 오차가 생기므로 clipPos를 fragment함수에서 또 연산
					float2 screenUV = ((clipPos.xy / clipPos.w) + 1) / 2;//투영 공간인 -1~ +1 공간을 0~1 인 UV 공간으로 스위치

	#if UNITY_UV_STARTS_AT_TOP	//uv 플립 대응 https://docs.unity3d.com/Manual/SL-PlatformDifferences.html
					screenUV.y = 1 - screenUV.y;
	#endif
					float screen01Depth = Linear01Depth(tex2D(_CameraDepthTexture, screenUV));

					//https://docs.unity3d.com/kr/current/Manual/SL-PlatformDifferences.html
					//뎁스의 리니어한 분포 등 고려하여 리니어01뎁스 함수로 사용 UNITY_REVERSED_Z 에도 대응

					float depthDiffer = screen01Depth - i.vertexsDepth;
					float differDegree = 0;
					float differAcceptRange = _ProjectionParams.w * DepthDifferMax;

					if (depthDiffer > 0)
					{
						differDegree = 1 - smoothstep(0, differAcceptRange, depthDiffer);
					}

					float rimDegree = 0;
					float3 wCameraDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
					float3 wNormal = normalize(i.worldNormal);
					float dotForRim = abs(dot(wCameraDir, wNormal));
					rimDegree = 1 - smoothstep(0, RimColorRange, dotForRim);

					float testDegree = abs(ModelY - i.oPos.y) < 0.1 ? 1 : 0;

					float poleDegree = max(0,dot(normalize(i.oNormal), float3(0, 1, 0)));//max로 내적 값이 마이너스인 포지션 배제
					poleDegree = smoothstep(1 - PoleRange, 1,poleDegree);	//1-PoleRange 보다 낮으면 0이므로

					float glowDegree = max(max(rimDegree, differDegree) , poleDegree);
					//림 정도, 웨이브 정도(웨이브에 속했는지), 간섭 정도, 폴 정도 를 모두 0~1 로 전사후 맥스 값을 이용함.

					glowDegree = max(testDegree, glowDegree);

					if (WaveToggle)
					{
						float wave = abs(dot(normalize(i.oNormal), float3(0, 1, 0)));
						float setWaveMid = cos(((_Time.y - WaveValueForStartFromPole)) * WaveSpeed);//폴에서 부터 웨이브가 시작하도록.
						if (setWaveMid < 0) setWaveMid *= -1;

						float waveDegree = 1 - smoothstep(0, WaveRange, abs(setWaveMid - wave));

						glowDegree = max(glowDegree, waveDegree);
					}

					fixed4 glowColor = fixed4(lerp(BarriorColor.rgb, GlowColor.rgb, glowDegree),1);

					fixed4 col = BarriorColor * BarriorColor.a + glowColor * glowDegree;//Blend One One 대응

					//Cut Plane Normal vector 위에 있는 것만 출력.
					if (DissolveToggle)
					{
						float2 dissolveUv = i.uv * DissolveTex_ST.xy;	//디졸브 텍스쳐 타일링.
						dissolveUv += _Time.x * DissolveTex_ST.zw;	//디졸브 텍스쳐의 오프셋 프로퍼티를 uv 이동 벡터 값으로 사용.
						fixed4 dissolveTexInfo = tex2D(DissolveTex, dissolveUv);

						float4 fromPlanePointV = i.worldPos - PlanePoint;//플레인 점 부터의 벡터
						float planeDot = dot(normalize(PlaneNormal), fromPlanePointV); //평면에서부터의 수직 거리.(월드)
						if (planeDot < 0) discard;
						if (planeDot > DissolveRange) return col;
						//if (planeDot < 0.01) return fixed4(1,0,0,1);	//디버깅
						//if (planeDot > DissolveRange && planeDot < DissolveRange+0.01) return fixed4(1, 1, 0, 1);	//디버깅

						float interpolateForDissolveTexRegular = 0.2 * DissolveRange;	//단순히 현재 사용중인 디졸브 텍스쳐 알파값의 균일 정도를 맞춰주기 위한 값
						float degree = smoothstep(0 - interpolateForDissolveTexRegular, DissolveRange + interpolateForDissolveTexRegular, planeDot);
						if (dissolveTexInfo.r > degree + DissolveEdgeWidth) discard;
						float dissolvedEdge = abs(degree - dissolveTexInfo.r);
						if (dissolvedEdge < DissolveEdgeWidth)
							return lerp(GlowColor, BarriorColor,
								pow(smoothstep(0, DissolveEdgeWidth, dissolvedEdge), DissolveEdgeDegree)
							);
					}
					return col;
					*/
				}
				ENDCG
			}
		}
			FallBack "Mobile/Diffuse"
}
