using System.ComponentModel;
using Microsoft.Xna.Framework;
using Terraria.Chat;
using Terraria.Localization;
using Terraria.ModLoader.Config;

namespace PhanasProjectileLimiter.Config;

public class Config : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ClientSide;

    private Color _infoColor = new(0x81, 0xFF, 0xEE);
    
    [Label("Projectile Limit")]
    [Tooltip("The maximum number of projectiles allowed in the game")]
    [Range(0, 1000)]
    [DefaultValue(1000)]
    public int ProjectileLimit;

    private void SendChat(string text, Color? color = null)
    {
        color ??= _infoColor;
        var netText = NetworkText.FromLiteral(text);
        ChatHelper.BroadcastChatMessage(netText, color.Value);
    }
    
    public override void OnChanged()
    {
        base.OnChanged();
        SendChat($"Changed max projectile limit to {ProjectileLimit}.");
    }
}
