void GetMainLightDirection(out float3 lightDir)
{
    // Unity의 기본 메인 라이트 방향 값을 가져옴
    lightDir = _WorldSpaceLightPos0.xyz;
}