using Unity.Mathematics;

namespace TMG.Zombies
{
    public static class MathHelpers
    {
        //using the math library to get the heading of the zombie so that it can be rotated to face the brain
        public static float GetHeading(float3 objectPosition, float3 targetPosition)
        {
            var x = objectPosition.x - targetPosition.x;
            var y = objectPosition.z - targetPosition.z;
            //atan2 returns the angle in radians
            return math.atan2(x, y) + math.PI;
        }
    }
}