using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct SpawnGroundData : IComponentData
{
    public Entity prefab;
    public int amount;
    public float3 startPosition;
    public float xGap;
    public float yMin;
    public float yMax;
}
