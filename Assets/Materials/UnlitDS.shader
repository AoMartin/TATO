Shader "Unlit/UnlitDS"
{
    Properties {
        _Color ("Color", Color) = (1,1,1)
    }
    
    SubShader {
        Cull Off
        Color [_Color]
        Pass {}
    }
}
