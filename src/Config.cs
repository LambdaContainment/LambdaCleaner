using Exiled.API.Interfaces;

namespace LambdaCleaner;

public class Config : IConfig
{
    public bool IsEnabled { get; set; } = true;
    public bool Debug  { get; set; } = false;
    public static float CleanLoopInterval { get; set; } = 120f;
}