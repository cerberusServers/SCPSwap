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
        public string SwapRequestBroadcast { get; set; } = 
            "\n<size=36><b><color=#006eff>%player" +
            "\n<color=#4ce300>te envió una solicitud de intercambio.</color>" +
            "\n<i><color=#81b4f7>[Ñ] <color=#f29f05>o <color=#81b4f7>[~] <color=#f29f05>para abrir la consola y aceptarla</i>" +
            "\n\n<color=#f7ff9c>Su rol: <color=red>%role2</size>";
        public string SwapRequestConsoleMessage { get; set; } = 
            "\n<color=#f7ff9c>Solicitud de intercambio de:</color> <color=#006eff>%player</color> " +
            "\n<color=#f7ff9c>Su rol:</color> <color=red>%scp</color>" +
            "\n<color=#f7ff9c>¿Te gustaría aceptar su solicitud?</color>" +
            "\n" +
            "\n<color=#f7ff9c>Para elegir:</color> <color=#4ce300>\".swap sí/yes\" - \".scpswap no\"</color><color=#f7ff9c>.</color>";
        public string SwapRequestNoResponse { get; set; } = "<color=red>La solicitud de intercambio expiró sin respuesta.</color>";
        public string SwapRequestTimedOut { get; set; } = "<color=red>La solicitud de intercambio expiró.</color>";
        public string SwapRequestSuccess { get; set; } = "<color=#4ce300>¡Solicitud de intercambio aceptada y realizada con éxito!</color>";
        public string SwapRequestLobby { get; set; } = "<color=red>No puedes utilizar éste comando en el Lobby.</color>";
        public string SwapRequestDumb { get; set; } = "<color=red>Éste comando sólo puede ser utilizado por SCPs.</color>";
        public string SwapRequestExpired { get; set; } = "<color=red>Se acabó el tiempo de intercambio.</color>";
        public string SwapRequestNone { get; set; } = "<color=red>No tienes ninguna solicitud de intercambio.</color>";
        public string SwapRequestDenied { get; set; } = "<color=red>La solicitud de intercambio fue cancelada.</color>";
        public string SwapRequestCancelled { get; set; } = "<color=red>La solicitud de intercambio fue cancelada.</color>";
        public string SwapRequestCancel { get; set; } = "<color=red>Cancelaste tu solicitud de intercambio.</color>";
        public string SwapRequestNoneOutgoing { get; set; } = "<color=red>No tienes ninguna solicitud de intercambio pendiente.</color>";
        public string SwapRequestInvalid { get; set; } = "<color=red>No se pudo reconocer el rol, por favor, inténtalo de nuevo.</color>";
        public string SwapRequestPending { get; set; } = "<color=red>Ya tienes una solicitud de intercambio pendiente.</color>";
        public string SwapRequestBlacklisted { get; set; } = "<color=red>Éste rol está en la lista negra, intenta con otro.</color>";
        public string SwapRequestOwnRole { get; set; } = "<color=red>No puedes intercambiar con tu propio rol.</color>";
        public string SwapRequestSent { get; set; } = "Swap request sent!";
        public string SwapRequestSuccessFree { get; set; } = "<color=#4ce300>No se encontró a nadie con ese rol, reclamando...\n\n¡Rol reclamado con éxito!</color>";
        public string SwapRequestNoPlayers { get; set; } = "<color=red>No se encontró ese SCP.</color>";
        public string SwapRequestUsage { get; set; } = "" +
            "<color=#f7ff9c>Uso correcto:</color> <color=#4ce300>.swap [Nombre/ID]</color>" +
            "\nEjemplos: \n\".swap plaga\" - \".swap scp173\" - \".swap 106\"";
        public string SwapInfoHint { get; set; } = "\n<size=30><color=#f29f05><b>¡Te recordamos que puedes abrir la consola con <color=#81b4f7>[Ñ] <color=#f29f05>o <color=#81b4f7>[~] <color=#f29f05>para ver información sobre tus <color=#4ce300><u>comandos</u> <u>especiales</u><color=#f29f05>.</b></size>";
        public float SwapInfoHintDur { get; set; } = 9;
        public List<int> SwapBlacklist { get; private set; } = new List<int>() { 10 };
    }
}
