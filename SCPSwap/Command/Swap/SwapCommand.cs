﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Command System
using CommandSystem;
using RemoteAdmin;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using Exiled.Permissions.Extensions;
using MEC;
using SCPSwap;
using SCPSwap.Handlers;

namespace SCPSwap.Command.Swap
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class SwapCommand : ICommand
    {
        public string Command => "scpswap";

        public string[] Aliases => new string[] { "intercambio","intercambiar", "swap", "swp" };

        public string Description => "\n<color=yellow>| Te permite cambiar de SCP y si otra persona tiene el SCP que tu tienes, pueden intercambiar entre los dos | </color>";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            try
            {
                if (sender != null)
                {
                    IList<string> argList = arguments;

                    var plugin = Plugin.Instance;
                    Player player = Player.Get((sender as CommandSender).SenderId);
                    if (!EventHandlers.isRoundStarted)
                    {
                        response = "\n" + plugin.Config.SwapRequestLobby;
                        return false;
                    }
                    else if (EventHandlers.InEvent())
                    {
                        response = "\nEstamos en evento, Licht la tiene corta. - Atte: Andrés";
                        return false;
                    }

                    if (player.Team != Team.SCP)
                    {
                        response = "\n" + plugin.Config.SwapRequestDumb;

                        return false;
                    }

                    if (!EventHandlers.allowSwaps)
                    {
                        response = "\n" + plugin.Config.SwapRequestExpired;
                        return false;
                    }

                    if(arguments.Count == 0) {
                        response = "\n" + plugin.Config.SwapRequestUsage;
                        return true;
                    }

                    switch(argList[0].ToLower()) {
                        case "yes":
                        case "si":
                        case "sí":
                        case "s":
                        case "y":
                            Player swap = EventHandlers.ongoingReqs.FirstOrDefault(x => x.Value == player).Key;
                            if(swap != null) {
                                EventHandlers.PerformSwap(swap, player);
                                response = "\n" + plugin.Config.SwapRequestSuccess;
                                Timing.KillCoroutines(EventHandlers.reqCoroutines[swap]);
                                EventHandlers.reqCoroutines.Remove(swap);
                                return true;
                            }
                            response = plugin.Config.SwapRequestNone;
                            return true;
                        case "no":
                        case "nop":
                        case "No":
                        case "n":
                            swap = EventHandlers.ongoingReqs.FirstOrDefault(x => x.Value == player).Key;
                            if(swap != null) {
                                response = "\n" + plugin.Config.SwapRequestDenied;
                                swap.SendConsoleMessage(plugin.Config.SwapRequestDenied, "red");
                                Timing.KillCoroutines(EventHandlers.reqCoroutines[swap]);
                                EventHandlers.reqCoroutines.Remove(swap);
                                EventHandlers.ongoingReqs.Remove(swap);
                                return true;
                            }
                            response = "\n" + plugin.Config.SwapRequestNone;
                            return true;
                        case "cancel":
                        case "cancelar":
                        case "cancelo":
                        case "c":
                            if(EventHandlers.ongoingReqs.ContainsKey(player)) {
                                Player dest = EventHandlers.ongoingReqs[player];
                                dest.SendConsoleMessage(plugin.Config.SwapRequestCancelled, "red");
                                Timing.KillCoroutines(EventHandlers.reqCoroutines[player]);
                                EventHandlers.reqCoroutines.Remove(player);
                                EventHandlers.ongoingReqs.Remove(player);
                                response = "\n" + plugin.Config.SwapRequestCancel;
                                return true;
                            }
                            response = "\n" + plugin.Config.SwapRequestNoneOutgoing;
                            return true;
                        default:
                            if(!EventHandlers.valid.ContainsKey(argList[0])) {
                                response = "\n" + plugin.Config.SwapRequestInvalid;
                                return false;
                            }

                            if(EventHandlers.ongoingReqs.ContainsKey(player)) {
                                response = "\n" + plugin.Config.SwapRequestPending;
                                return false;
                            }

                            RoleType role = EventHandlers.valid[argList[0]];
                            if(plugin.Config.SwapBlacklist.Contains((int) role)) {
                                response = "\n" + plugin.Config.SwapRequestBlacklisted;
                                return false;
                            }

                            if(player.Role == role) {
                                response = "\n" + plugin.Config.SwapRequestOwnRole;
                                return true;
                            }

                            swap = Player.List.FirstOrDefault(x => role == RoleType.Scp93953 ? x.Role == role || x.Role == RoleType.Scp93989 : x.Role == role);
                            if(swap != null) {
                                EventHandlers.reqCoroutines.Add(player, Timing.RunCoroutine(EventHandlers.SendRequest(player, swap)));
                                response = "\n" + plugin.Config.SwapRequestSent;
                                return false;
                            }
                            if(plugin.Config.SwapAllowNewScps) {
                                player.ReferenceHub.characterClassManager.SetPlayersClass(role, player.ReferenceHub.gameObject);
                                response = "\n" + plugin.Config.SwapRequestSuccessFree;
                                return true;
                            }
                            response = "\n" + plugin.Config.SwapRequestNoPlayers;
                            return true;
                    }

                }
                else
                {
                    response = "\nSender null";
                    return false;
                }

            }
            catch (Exception e)
            {

                response = $"{e} ----- {e.StackTrace}";
                return false;
            }
           
        }
    }
}
