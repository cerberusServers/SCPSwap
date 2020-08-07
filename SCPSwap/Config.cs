using System;
using System.Collections.Generic;
using Exiled.API.Interfaces;

namespace SCPSwap
{
    public sealed class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool SwapAllowNewScps { get; set; } = false;
        public float SwapTimeout { get; set; } = 60f;
        public float SwapRequestTimeout { get; set; } = 20f;
        public string SwapRequestBroadcast { get; set; } = "<i>You have an SCP Swap request!\nCheck your console by pressing [`] or [~]</i>";
        public string SwapRequestConsoleMessage { get; set; } = "You have received a swap request from %player who is SCP-%scp. Would you like to swap with them? Type \".scpswap yes\" to accept or \".scpswap no\" to decline.";
        public string SwapRequestNoResponse { get; set; } = "The player did not respond to your request.";
        public string SwapRequestTimedOut { get; set; } = "Your swap request has timed out.";
        public string SwapRequestSuccess { get; set; } = "Swap successful!";
        public string SwapRequestLobby { get; set; } = "The round hasn't started yet!";
        public string SwapRequestDumb { get; set; } = "You're not an SCP, why did you think that would work.";
        public string SwapRequestExpired { get; set; } = "SCP swap period has expired.";
        public string SwapRequestNone { get; set; } = "You do not have a swap request.";
        public string SwapRequestDenied { get; set; } = "Swap request denied.";
        public string SwapRequestCancelled { get; set; } = "Your swap request has been cancelled.";
        public string SwapRequestCancel { get; set; } = "You have cancelled your swap request.";
        public string SwapRequestNoneOutgoing { get; set; } = "You do not have an outgoing swap request.";
        public string SwapRequestInvalid { get; set; } = "Invalid SCP.";
        public string SwapRequestPending { get; set; } = "You already have a request pending!";
        public string SwapRequestBlacklisted { get; set; } = "That SCP is blacklisted.";
        public string SwapRequestOwnRole { get; set; } = "You cannot swap with your own role.";
        public string SwapRequestSent { get; set; } = "Swap request sent!";
        public string SwapRequestSuccessFree { get; set; } = "Could not find a player to swap with, you have been made the specified SCP.";
        public string SwapRequestNoPlayers { get; set; } = "No players found to swap with.";
        public string SwapRequestUsage { get; set; } = "USAGE: SCPSWAP [SCP NUMBER]";
        public List<int> SwapBlacklist { get; private set; } = new List<int>() { 10 };
    }
}
