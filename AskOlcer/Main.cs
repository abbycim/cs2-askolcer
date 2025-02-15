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

    private readonly List<CCSPlayerController> _cooldowns = new();

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


        if (_cooldowns.Contains(player))
        {
            commandInfo.ReplyToCommand(
                $" {ChatColors.Blue}[{ChatColors.Gold}ABBY{ChatColors.Blue}] {ChatColors.White}Lütfen {ChatColors.Red}3 {ChatColors.White}saniye sonra tekrar deneyin!");
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
        _cooldowns.Add(player);
        AddTimer(3, () => { _cooldowns.Remove(player); });
        int lovePercentage = CalculateLovePercentage();
        string loveMessage = GetLoveMessage(lovePercentage);
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

    private static int CalculateLovePercentage()
    {
        var random = new Random((int)(DateTime.Now.Millisecond));
        return random.Next(101);
    }



    private static string GetLoveMessage(int percentage)
{
    return percentage switch
    {
        >= 90 => $"{ChatColors.Red}Seninle T-Rex'in kolu kadar eksiksiz bir aşk! 🦖❤️",
        >= 70 => $"{ChatColors.Purple}Aşkın gözü kör olsun, neredeyse 'kurban olduğum' seviyedesin! 😈", 
        >= 50 => $"{ChatColors.LightBlue}Bizimki tıpkı çekirdek - izliyoruz ama bir şey çıkmıyor! �",
        >= 30 => $"{ChatColors.Green}Seninle aşkımız Nescafe gibi - üçüncü bardakta ancak koyuldu! ☕",
        >= 10 => $"{ChatColors.Grey}Bizim 'aşk' dediğin, balık hafızalının zoraki flörtü! 🐟",
        _ => $"{ChatColors.DarkRed}Bizimki Homer'ın diyeti gibi - YOK BÖYLE BİR ŞEY! 🍩"
    };
}
}
