using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class MovingPlatformSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities.WithAll<MovingPlatform>().ForEach((MovingPlatform platform, ref Translation translation) =>
        {
            translation.Value.x -= platform.speed * deltaTime;
            if (translation.Value.x < -4) {
                translation.Value.x = 12;
            }
        }).Run();
    }
}
