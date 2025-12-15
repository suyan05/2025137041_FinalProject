using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 월드/청크 설정
[System.Serializable]
public class VoxelSettings
{
    // 청크 크기
    [Header("청크 Settings")]
    public int chunkSizeX = 16;
    public int chunkSizeZ = 16;
    public int chunkHeight = 128;

    // 월드 시드
    [Header("월드 시드")]
    public int worldSeed = 12345;

    // 지형
    [Header("지형 Settings")]
    public float baseHeight = 50f;
    public float heightNoiseScale = 0.0075f;
    public float heightNoiseAmp = 18f;

    // 바이옴/지층
    [Header("바이옴 Settings")]
    public float biomeScale = 0.0025f; // 온도/습도 등 합성에 사용
    public int soilDepthMin = 3;
    public int soilDepthMax = 6;

    // 산(지표)
    [Header("산 Settings")]
    public float mountainNoiseScale = 0.0015f;   // 산 노이즈 스케일
    public float mountainNoiseAmp = 100f;         // 산 높이 증폭
    public float mountainThreshold = 0.65f;      // 산으로 판정할 기준값

    // 강(지표)
    [Header("강 Settings")]
    public float riverNoiseScale = 0.002f;
    public float riverWidth = 0.08f; // 0~1 범위로 폭
    public float riverDepth = 6f;

    // 동굴(지하)
    [Header("동굴 Settings")]
    public float caveNoiseScale = 0.035f;  // 3D 노이즈
    public float caveThreshold = 0.58f;    // 임계값
    public float caveRidgeScale = 0.012f;  // 보조 리지 노이즈로 굴 연결성 조절

    // 광석 노이즈 스케일
    [Header("광석 Settings")]
    public float oreNoiseScale = 0.05f;

    // 광석 확률 (0~1)
    public float coalChance = 0.12f;
    public float ironChance = 0.08f;
    public float goldChance = 0.04f;
    public float diamondChance = 0.02f;

    // 광석 최대 깊이
    public int coalMaxHeight = 128;
    public int ironMaxHeight = 80;
    public int goldMaxHeight = 40;
    public int diamondMaxHeight = 20;


    // 바다/수면
    [Header("수면 Settings")]
    public int seaLevel = 48;

    // 샘플링 오프셋
    [Header("노이즈 오프셋")]
    public Vector3 noiseOffset = new Vector3(1000, 1000, 1000);
}
