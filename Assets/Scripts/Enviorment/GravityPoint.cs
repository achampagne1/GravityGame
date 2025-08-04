using System.Runtime.InteropServices;
[StructLayout(LayoutKind.Sequential)]
public struct GravityPoint
{
    public float x;
    public float y;
    public float fieldSize;

    public override string ToString()
    {
        return $"x: {x}, y: {y}, fieldSize: {fieldSize}";
    }
}