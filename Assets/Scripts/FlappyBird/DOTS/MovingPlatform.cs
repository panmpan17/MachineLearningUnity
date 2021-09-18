using Unity.Entities;
// using Unity.Transforms;

[GenerateAuthoringComponent]
public struct MovingPlatform : IComponentData
{
    public float speed;
}
