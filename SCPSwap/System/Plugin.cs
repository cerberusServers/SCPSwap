using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using SCPSwap.Base;
using SCPSwap.Handlers;

namespace SCPSwap
{
    public class Plugin : Plugin<Config>
    {
		public override string Name => "ScpSwap";
        public override string Author => "Beryl";
		public static Plugin Instance;
        private List<Handler> handlers = new List<Handler>();
        public override void OnEnabled()
        {
            (from t in typeof(Plugin).Assembly.GetTypes()
             where t.IsClass && t.Namespace == "SCPSwap.Handlers" && t.IsSubclassOf(typeof(Handler))
             select t).ToList().ForEach(handler => handlers.Add((Handler)Activator.CreateInstance(handler, this)));

            //Start
            Instance = this;

            handlers.ForEach(x => x.Start());

        }

		public override void OnDisabled() {

            handlers.ForEach(x => x.Stop());
            handlers.Clear();
            Instance = null;
        }

		public override void OnReloaded() { }
	}
}
