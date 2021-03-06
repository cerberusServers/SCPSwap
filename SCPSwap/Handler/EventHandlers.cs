﻿using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SCPSwap.Base;

namespace SCPSwap.Handlers
{
	public sealed class EventHandlers : Base.Handler
	{
		public static Dictionary<Player, Player> ongoingReqs = new Dictionary<Player, Player>();

		public static List<CoroutineHandle> coroutines = new List<CoroutineHandle>();
		public static Dictionary<Player, CoroutineHandle> reqCoroutines = new Dictionary<Player, CoroutineHandle>();

		public static bool allowSwaps = false;
		public static bool isRoundStarted = false;

		public EventHandlers(Plugin plugin) : base(plugin) { }
		public override void Start()
        {
			Exiled.Events.Handlers.Player.ChangingRole += SetRole;
			Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
			Exiled.Events.Handlers.Server.RoundStarted += OnRoundStart;
			Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnd;
			Exiled.Events.Handlers.Server.RestartingRound += OnRoundRestart;
			Exiled.Events.Handlers.Server.SendingConsoleCommand += OnConsoleCommand;
		}

        public override void Stop()
        {
			Exiled.Events.Handlers.Player.ChangingRole -= SetRole;
			Exiled.Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;
			Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStart;
			Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnd;
			Exiled.Events.Handlers.Server.RestartingRound -= OnRoundRestart;
			Exiled.Events.Handlers.Server.SendingConsoleCommand -= OnConsoleCommand;
		}



		public static Dictionary<string, RoleType> valid = new Dictionary<string, RoleType>()
		{
			{"173", RoleType.Scp173},
			{"peanut", RoleType.Scp173},
			{"mani", RoleType.Scp173},
			{"maní", RoleType.Scp173},
			{"scp173", RoleType.Scp173},
			{"939", RoleType.Scp93953},
			{"dog", RoleType.Scp93953},
			{"perro", RoleType.Scp93953},
			{"perra", RoleType.Scp93989},
			{"scp93953", RoleType.Scp93953},
			{"079", RoleType.Scp079},
			{"computer", RoleType.Scp079},
			{"computadora", RoleType.Scp079},
			{"camara", RoleType.Scp079},
			{"pc", RoleType.Scp079},
			{"scp079", RoleType.Scp079},
			{"106", RoleType.Scp106},
			{"scp106", RoleType.Scp106},
			{"negro", RoleType.Scp106},
			{"larry", RoleType.Scp106},
			{"sombra", RoleType.Scp106},
			{"096", RoleType.Scp096},
			{"shyguy", RoleType.Scp096},
			{"shy", RoleType.Scp096},
			{"llorona", RoleType.Scp096},
			{"llorón", RoleType.Scp096},
			{"lloron", RoleType.Scp096},
			{"scp096", RoleType.Scp096},
			{"049", RoleType.Scp049},
			{"doctor", RoleType.Scp049},
			{"plaga", RoleType.Scp049},
			{"scp049", RoleType.Scp049},
			{"0492", RoleType.Scp0492},
			{"zombie", RoleType.Scp0492},
			{"zombi", RoleType.Scp0492},
			{"scp0492", RoleType.Scp0492}
		};

		public void SetRole(ChangingRoleEventArgs ev)
		{
			if (ev.NewRole.GetTeam() == Team.SCP)
			{
				ev.Player.Broadcast(plugin.Config.SwapInfoHintDur, plugin.Config.SwapInfoHint);
			}
		}

		public static IEnumerator<float> SendRequest(Player source, Player dest)
		{
			ongoingReqs.Add(source, dest);
			dest.Broadcast(7, Plugin.Instance.Config.SwapRequestBroadcast.Replace("%player", source.Nickname).Replace("%role2", source.Role.ToString()).Replace("%role1", dest.Role.ToString()));
			dest.SendConsoleMessage(Plugin.Instance.Config.SwapRequestConsoleMessage.Replace("%player", $"{source.ReferenceHub.nicknameSync.Network_myNickSync}").Replace("%scp", source.Role.ToString()), "yellow");//{valid.FirstOrDefault(x => x.Value == source.Role).Key}
			yield return Timing.WaitForSeconds(Plugin.Instance.Config.SwapRequestTimeout);
			TimeoutRequest(source);
		}

		public static void TimeoutRequest(Player source)
		{
			if (ongoingReqs.ContainsKey(source))
			{
				Player dest = ongoingReqs[source];
				source.SendConsoleMessage(Plugin.Instance.Config.SwapRequestNoResponse, "red");
				dest.SendConsoleMessage(Plugin.Instance.Config.SwapRequestTimedOut, "red");
				ongoingReqs.Remove(source);
			}
		}

		public static void PerformSwap(Player source, Player dest)
		{
			source.SendConsoleMessage(Plugin.Instance.Config.SwapRequestSuccess, "green");

			RoleType sRole = source.Role;
			RoleType dRole = dest.Role;

			Vector3 sPos = source.Position;
			Vector3 dPos = dest.Position;

			float sHealth = source.Health;
			float dHealth = dest.Health;

			source.Role = dRole;
			source.Position = dPos;
			source.Health = dHealth;

			dest.Role = sRole;
			dest.Position = sPos;
			dest.Health = sHealth;

			ongoingReqs.Remove(source);
		}

		public void OnRoundStart()
		{
			allowSwaps = true;
			isRoundStarted = true;
			Timing.CallDelayed(plugin.Config.SwapTimeout, () => allowSwaps = false);
		}

		public void OnRoundRestart()
		{
			// fail safe
			isRoundStarted = false;
			Timing.KillCoroutines(coroutines.ToArray());
			Timing.KillCoroutines(reqCoroutines.Values.ToArray());
			coroutines.Clear();
			reqCoroutines.Clear();
		}

		public void OnRoundEnd(RoundEndedEventArgs ev)
		{
			isRoundStarted = false;
			Timing.KillCoroutines(coroutines.ToArray());
			Timing.KillCoroutines(reqCoroutines.Values.ToArray());
			coroutines.Clear();
			reqCoroutines.Clear();
		}

		public void OnWaitingForPlayers()
		{
			allowSwaps = false;
		}

		public void OnConsoleCommand(SendingConsoleCommandEventArgs ev)
		{
			/*string name = ev.Name.ToLower();
			if (name.Equals("scpswap") || name.Equals("intercambio") || name.Equals("intercambiar") || name.Equals(""))
			{
				ev.Allow = false;
				if (!isRoundStarted)
				{
					ev.ReturnMessage = plugin.Config.SwapRequestLobby;
					ev.Color = "red";
					return;
				}
				else if (InEvent())
				{
					ev.ReturnMessage = "Estamos en evento, Licht la tiene corta. - Atte: Andrés";
					ev.Color = "red";
					return;
				}

				if (!(ev.Player.Team == Team.SCP))
				{
					ev.ReturnMessage = plugin.Config.SwapRequestDumb;
					ev.Color = "red";
					return;
				}

				if (!allowSwaps)
				{
					ev.ReturnMessage = plugin.Config.SwapRequestExpired;
					ev.Color = "red";
					return;
				}

				switch (ev.Arguments.Count)
				{
					case 1:
						switch (ev.Arguments[0].ToLower())
						{
							case "yes":
							case "si":
							case "sí":
							case "s":
							case "y":
								Player swap = ongoingReqs.FirstOrDefault(x => x.Value == ev.Player).Key;
								if (swap != null)
								{
									PerformSwap(swap, ev.Player);
									ev.ReturnMessage = plugin.Config.SwapRequestSuccess;
									Timing.KillCoroutines(reqCoroutines[swap]);
									reqCoroutines.Remove(swap);
									ev.Color = "green";
									return;
								}
								ev.ReturnMessage = plugin.Config.SwapRequestNone;
								break;
							case "no":
							case "nop":
							case "No":
							case "n":
								swap = ongoingReqs.FirstOrDefault(x => x.Value == ev.Player).Key;
								if (swap != null)
								{
									ev.ReturnMessage = plugin.Config.SwapRequestDenied;
									swap.SendConsoleMessage(plugin.Config.SwapRequestDenied, "red");
									Timing.KillCoroutines(reqCoroutines[swap]);
									reqCoroutines.Remove(swap);
									ongoingReqs.Remove(swap);
									return;
								}
								ev.ReturnMessage = plugin.Config.SwapRequestNone;
								break;
							case "cancel":
							case "cancelar":
							case "cancelo":
							case "c":
								if (ongoingReqs.ContainsKey(ev.Player))
								{
									Player dest = ongoingReqs[ev.Player];
									dest.SendConsoleMessage(plugin.Config.SwapRequestCancelled, "red");
									Timing.KillCoroutines(reqCoroutines[ev.Player]);
									reqCoroutines.Remove(ev.Player);
									ongoingReqs.Remove(ev.Player);
									ev.ReturnMessage = plugin.Config.SwapRequestCancel;
									return;
								}
								ev.ReturnMessage = plugin.Config.SwapRequestNoneOutgoing;
								break;
							default:
								if (!valid.ContainsKey(ev.Arguments[0]))
								{
									ev.ReturnMessage = plugin.Config.SwapRequestInvalid;
									ev.Color = "red";
									return;
								}

								if (ongoingReqs.ContainsKey(ev.Player))
								{
									ev.ReturnMessage = plugin.Config.SwapRequestPending;
									ev.Color = "red";
									return;
								}

								RoleType role = valid[ev.Arguments[0]];
								if (plugin.Config.SwapBlacklist.Contains((int)role))
								{
									ev.ReturnMessage = plugin.Config.SwapRequestBlacklisted;
									ev.Color = "red";
									return;
								}

								if (ev.Player.Role == role)
								{
									ev.ReturnMessage = plugin.Config.SwapRequestOwnRole;
									ev.Color = "red";
									return;
								}

								swap = Player.List.FirstOrDefault(x => role == RoleType.Scp93953 ? x.Role == role || x.Role == RoleType.Scp93989 : x.Role == role);
								if (swap != null)
								{
									reqCoroutines.Add(ev.Player, Timing.RunCoroutine(SendRequest(ev.Player, swap)));
									ev.ReturnMessage = plugin.Config.SwapRequestSent;
									ev.Color = "green";
									return;
								}
								if (plugin.Config.SwapAllowNewScps)
								{
									ev.Player.ReferenceHub.characterClassManager.SetPlayersClass(role, ev.Player.ReferenceHub.gameObject);
									ev.ReturnMessage = plugin.Config.SwapRequestSuccessFree;
									ev.Color = "green";
									return;
								}
								ev.ReturnMessage = plugin.Config.SwapRequestNoPlayers;
								ev.Color = "red";
								break;
						}
						break;
					default:
						ev.ReturnMessage = plugin.Config.SwapRequestUsage;
						ev.Color = "red";
						break;
				}
			}*/
		}

		public static bool InEvent()
		{
			try
			{
				return CerberusTweaks.Plugin.InEvent;
			}
			catch (Exception)
			{
				Log.Error("No se detectó CerberusTweaks");
				return false;
			}
		}

    }
}
