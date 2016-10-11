﻿// Copyright (C) 2016 Filip Cyrus Bober

Shader "Custom/ApplyDepthGradient" 
{
    Properties
	{
        _MainTex("", 2D) = "white" {}		
        
        // Set from script
		_Rgb("Base (RGB)", 2D) = "white" {}
		_Depth("Depth (RGB)", 2D) = "white" {} 

		_Gradient("Gradient [default 0]", Float) = 0

		// Debug purposes only        
		_DynamicModelsColor("Dynamic models color", Vector) = (1, 0,0)
    }

	SubShader
	{
		Tags{ "RenderType" = "Opaque" }

		Pass
		{

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _CameraDepthNormalsTexture;
			float _DepthView = 1; // 1 - texture colors; 0 - depth (for debugging purpose)
			sampler2D _BgColor;
			sampler2D _BgDepth;
			sampler2D _MainTex;
			float4 _DynamicModelsColor;
			uniform float _Gradient;

			struct v2f
			{
				float4 pos : SV_POSITION;
				half2 uv   : TEXCOORD0;
				float4 scrPos: TEXCOORD1;
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.scrPos = ComputeScreenPos(o.pos);
				//o.scrPos.y = 1 - o.scrPos.y;
				o.uv = v.texcoord.xy;

				return o;
			}			

			struct fragOut
			{
				half4 color : SV_Target;
			};

			fragOut frag(v2f i) //: COLOR
			{

				fragOut o;

				float3 normalValues;
				float depthValue;

				DecodeDepthNormal(tex2D(_CameraDepthNormalsTexture, i.scrPos.xy), depthValue, normalValues);    
	
				depthValue = depthValue - ((1- i.scrPos.y) * _Gradient);		// -0.18

				if (_DepthView == 1)
				{
					float2 mirrorTexCoords = { i.uv.x,1 - i.uv.y };

					float envDepth = tex2D(_BgDepth, mirrorTexCoords);
					float dynamicDepth = depthValue;

					bool isObjectOcculedByBackground = (envDepth < dynamicDepth);

                    float4 dynamicObjectColor = tex2D(_MainTex, i.uv);
                    float4 backgroundColor = tex2D(_BgColor, mirrorTexCoords);                     
                    
					if (isObjectOcculedByBackground)
					{			
                        o.color = backgroundColor;
					}
					else
					{

                        o.color = dynamicObjectColor;
					}

					return o;
				}
				else
				{		
					float2 mirrorTexCoords = { i.uv.x,1 - i.uv.y };

                    float4 backgroundColor = tex2D(_BgDepth, mirrorTexCoords);
					if (depthValue >= tex2D(_BgDepth, mirrorTexCoords).x)
					{
                        o.color = backgroundColor;
					}
					else
					{				
						float4 depth = float4(depthValue, depthValue, depthValue, 1.0);

						depth.x = (_DynamicModelsColor.x == 0) ? depthValue : _DynamicModelsColor.x;
						depth.y = (_DynamicModelsColor.y == 0) ? depthValue : _DynamicModelsColor.y;
						depth.z = (_DynamicModelsColor.z == 0) ? depthValue : _DynamicModelsColor.z;

						o.color = depth;
					}

					return o;
				}
			}

			ENDCG
		}
    }
    
	FallBack "Diffuse"
}