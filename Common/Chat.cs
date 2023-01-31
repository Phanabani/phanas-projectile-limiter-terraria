using Microsoft.Xna.Framework;
using Terraria.Chat;
using Terraria.Localization;

namespace PhanasProjectileLimiter.Common;

public class Chat
{
    public static readonly Color InfoColor = new(0x81, 0xFF, 0xEE);
    public static readonly Color ErrorColor = new(0xEA, 0x4E, 0x4E);

    public static void Send(string text, Color? color = null)
    {
        color ??= Color.White;
        var netText = NetworkText.FromLiteral(text);
        ChatHelper.BroadcastChatMessage(netText, color.Value);
    }

    public static void Info(string text)
    {
        Send(text, InfoColor);
    }

    public static void Error(string text)
    {
        Send(text, ErrorColor);
    }
}
