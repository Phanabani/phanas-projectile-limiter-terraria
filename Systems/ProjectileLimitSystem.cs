using System.Collections.Generic;
using System.Linq;
using PhanasProjectileLimiter.Common;
using PhanasProjectileLimiter.Config;
using Terraria;
using Terraria.ModLoader;

namespace PhanasProjectileLimiter.Systems;

public class ProjectileLimitSystem : ModSystem
{
    private readonly HashSet<Projectile> _projectilesAlive = new();
    private Config.Config _config;
    private LinkedList<Projectile> _projectilesOrderedByCreation = new();

    public override void OnWorldLoad()
    {
        base.OnWorldLoad();

        _config = (Config.Config)Mod.GetConfig("Config");
        SetProjectileLimit(_config.ProjectileLimit);
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

        msg = $"Projectile limit set to {limit}";
        Mod.Logger.Info(msg);
        Chat.Info(msg);
    }

    public override void PreUpdateProjectiles()
    {
        base.PreUpdateProjectiles();

        var projectileCount = UpdateProjectileInfo();
        RemoveProjectiles(projectileCount);
    }

    private int UpdateProjectileInfo()
    {
        var projectileCount = 0;
        foreach (var proj in Main.projectile)
            if (proj.active)
            {
                if (!_projectilesAlive.Contains(proj))
                {
                    // Projectile just spawned
                    _projectilesAlive.Add(proj);
                    _projectilesOrderedByCreation.AddFirst(proj);
                }

                projectileCount++;
            }
            else if (_projectilesAlive.Contains(proj))
            {
                // Projectile just died
                _projectilesAlive.Remove(proj);

                LinkedList<Projectile> newList = new();
                foreach (var p in _projectilesOrderedByCreation.Where(p => p != proj))
                    newList.AddFirst(p);

                _projectilesOrderedByCreation = newList;
            }

        return projectileCount;
    }

    private void RemoveProjectiles(int projectileCount)
    {
        while (projectileCount > _config.ProjectileLimit)
        {
            Projectile proj;
            if (_config.ProjectileRemovalBehavior == ProjectileRemovalBehavior.RemoveOldest)
            {
                proj = _projectilesOrderedByCreation.Last!.ValueRef;
                _projectilesOrderedByCreation.RemoveLast();
            }
            else
            {
                proj = _projectilesOrderedByCreation.First!.ValueRef;
                _projectilesOrderedByCreation.RemoveFirst();
            }

            proj.Kill();
            projectileCount--;
        }
    }
}
