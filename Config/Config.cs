using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace PhanasProjectileLimiter.Config;

public class Config : ModConfig
{
    [Label("Projectile Limit")]
    [Tooltip("The maximum number of projectiles allowed in the game")]
    [Range(0, 1000)]
    [DefaultValue(1000)]
    public int ProjectileLimit;

    public override ConfigScope Mode => ConfigScope.ClientSide;

    public event EventHandler<ProjectileLimitChangedEventArgs> ProjectileLimitChanged;

    public override void OnChanged()
    {
        base.OnChanged();
        var handler = ProjectileLimitChanged;
        handler?.Invoke(
            this, new ProjectileLimitChangedEventArgs { ProjectileLimit = ProjectileLimit }
        );
    }
}

public class ProjectileLimitChangedEventArgs : EventArgs
{
    public int ProjectileLimit { get; set; }
}
