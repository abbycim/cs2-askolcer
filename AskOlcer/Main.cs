/*
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <https://www.gnu.org/licenses/>.
 */


using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Commands.Targeting;
using CounterStrikeSharp.API.Modules.Utils;

namespace AskOlcer;

public class Main : BasePlugin
{
    public override string ModuleName => "AskOlcer";
    public override string ModuleAuthor => "Abby";
    public override string ModuleVersion => "1.0.0";
    
    public List<CCSPlayerController> _countdown = new List<CCSPlayerController>();

    private static readonly Random _random = new();

    public override void Load(bool hotReload)
    {
        AddCommand("askolc", "Aşk Ölçer", AskOlcer);
    }

    public override void Unload(bool hotReload)
    {
        RemoveCommand("askolc", AskOlcer);
    }
    
    public void AskOlcer(CCSPlayerController? player, CommandInfo commandInfo)
    {
        
        
        if (_countdown.Contains(player))
        {
            commandInfo.ReplyToCommand($" {ChatColors.Blue}[ABBY] {ChatColors.Red}Kullanmak için 3 saniye beklemelisiniz.");
            return;
        }

        var target = FindTarget(commandInfo);
        if (target == null) return;

        var targetPlayer = target.Players.FirstOrDefault();
        if (targetPlayer == null || !targetPlayer.IsValid)
        {
            commandInfo.ReplyToCommand($" {ChatColors.Blue}[ABBY] {ChatColors.Red}Geçersiz hedef.");
            return;
        }

        if (player == targetPlayer)
        {
            commandInfo.ReplyToCommand($"  {ChatColors.Blue}[ABBY] {ChatColors.Red}Kendini sevmek en büyük aşktır! {ChatColors.White}%100");
            return;
        }

        int lovePercentage = CalculateLovePercentage(player, targetPlayer);
        string loveMessage = GetLoveMessage(lovePercentage);
        _countdown.Add(targetPlayer);
        AddTimer(3, () =>
        {
            _countdown.Remove(targetPlayer);
        });
        Server.PrintToChatAll(
            $" {ChatColors.LightBlue}[{ChatColors.Gold}ABBY ÖLÇER{ChatColors.LightBlue}] " +
            $"{ChatColors.Green}{player.PlayerName} {ChatColors.White}ile " +
            $"{ChatColors.Green}{targetPlayer.PlayerName} {ChatColors.White}arasındaki aşk yüzdesi: " +
            $"{ChatColors.Red}{lovePercentage}% {ChatColors.White}- {loveMessage}");
    }

    private static TargetResult? FindTarget(CommandInfo command)
    {
        var targets = command.GetArgTargetResult(1);

        if (!targets.Any())
        {
            command.ReplyToCommand($"\"{command.GetArg(1)}\" için hedef bulunamadı.");
            return null;
        }

        if (command.GetArg(1).StartsWith('@'))
            return targets;

        return targets.Count() == 1 ? targets : null;
    }

    private static int CalculateLovePercentage(CCSPlayerController player, CCSPlayerController target)
    {
        ulong seed = player.SteamID ^ target.SteamID;
        var random = new Random((int)(seed & 0xFFFFFFFF));
        return random.Next(101);
    }

    private static string GetLoveMessage(int percentage)
    {
        return percentage switch
        {
            >= 90 => $"{ChatColors.Red}İlahi Aşk!",
            >= 70 => $"{ChatColors.Purple}Çok Güzel Bir Aşk!",
            >= 50 => $"{ChatColors.LightBlue}Orta Halli Bir Aşk!",
            >= 30 => $"{ChatColors.Green}Eh İşte!",
            >= 10 => $"{ChatColors.Grey}Zoraki Aşk!",
            _ => $"{ChatColors.DarkRed}Yok Böyle Bir Aşk!"
        };
    }
}