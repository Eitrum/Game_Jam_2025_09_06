Shader "VFXshaders/Passes/ShaderPassZwrite"
{

    SubShader
    {
		Tags
		{
			"Queue" = "Transparent"
			"RenderType" = "Transparent"
		}

        Pass
        {
		Name "Pass"
		ZWrite On
		ColorMask 0
           
        }
    }
}
