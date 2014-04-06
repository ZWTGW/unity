Shader "Custom/GrayShader" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		PowerOfGray ("Power of gray", float) = 0.5
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		
		uniform float PowerOfGray;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			//o.Albedo = PowerOfGray;//(c.r + c.g + c.b) / 3 * _Power; //c.rgb;
			float gray = (c.r + c.g + c.b) / 3;
			o.Albedo.r = gray * PowerOfGray + c.r * (1 - PowerOfGray);
			o.Albedo.g = gray * PowerOfGray + c.g * (1 - PowerOfGray);
			o.Albedo.b = gray * PowerOfGray + c.b * (1 - PowerOfGray); 
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
