using System.Collections.Generic;
using PhanasProjectileLimiter.Common;
using PhanasProjectileLimiter.Config;
using Terraria;
using Terraria.ModLoader;

namespace PhanasProjectileLimiter.Systems;

public class ProjectileLimitSystem : ModSystem
{
    private readonly HashSet<Projectile> _projectilesAlive = new();
    private readonly Queue<Projectile> _projectilesOrderedByCreation = new();
    private int ProjectileLimit { get; set; }

    public override void OnWorldLoad()
    {
        base.OnWorldLoad();

        var config = (Config.Config)Mod.GetConfig("Config");
        SetProjectileLimit(config.ProjectileLimit);
        config.ProjectileLimitChanged += OnProjectileLimitChanged;
    }

    private void OnProjectileLimitChanged(object sender,
        ProjectileLimitChangedEventArgs e)
    {
        SetProjectileLimit(e.ProjectileLimit);
    }

    private void SetProjectileLimit(int limit)
    {
        string msg;

        if (limit is < 0 or > 1000)
        {
            msg = "Projectile limit must be between 0 and 1000, inclusive";
            Mod.Logger.Error(msg);
            Chat.Error("Error: " + msg);
        }

        ProjectileLimit = limit;
        msg = $"Projectile limit set to {limit}";
        Mod.Logger.Info(msg);
        Chat.Info(msg);
    }

    public override void PreUpdateProjectiles()
    {
        base.PreUpdateProjectiles();

        var projectileCount = 0;
        foreach (var proj in Main.projectile)
            if (proj.active)
            {
                if (!_projectilesAlive.Contains(proj))
                {
                    // Projectile just spawned
                    _projectilesAlive.Add(proj);
                    _projectilesOrderedByCreation.Enqueue(proj);
                }

                projectileCount++;
            }
            else if (_projectilesAlive.Contains(proj))
            {
                // Projectile just died
                // We don't need to remove it from the queue here since that'll
                // be costly. We can just check active status in the removal
                // loop
                _projectilesAlive.Remove(proj);
            }

        while (projectileCount > ProjectileLimit)
        {
            var proj = _projectilesOrderedByCreation.Dequeue();
            if (!proj.active)
            {
                _projectilesAlive.Remove(proj);
                continue;
            }

            proj.Kill();
            projectileCount--;
        }
    }
}
