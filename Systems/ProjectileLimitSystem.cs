using System.Collections.Generic;
using System.Linq;
using PhanasProjectileLimiter.Common;
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
                var proj1 = proj;
                foreach (var p in _projectilesOrderedByCreation.Where(p => p != proj1))
                    newList.AddFirst(p);

                _projectilesOrderedByCreation = newList;
            }

        while (projectileCount > _config.ProjectileLimit)
        {
            var proj = _projectilesOrderedByCreation.Last!.ValueRef;
            _projectilesOrderedByCreation.RemoveLast();
            proj.Kill();
            projectileCount--;
        }
    }
}
