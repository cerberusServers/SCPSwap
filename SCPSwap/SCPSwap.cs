using Exiled.API.Features;

namespace SCPSwap
{
    public class ScpSwap : Plugin<Config>
    {
		public EventHandlers Handler { get; private set; }
		public override string Name => nameof(ScpSwap);
		public override string Author => "Beryl";

		public ScpSwap() { }

		public override void OnEnabled()
		{
			Handler = new EventHandlers(this);
			Handler.Start();
		}

		public override void OnDisabled()
		{
			Handler = null;
			Handler.Stop();
		}

		public override void OnReloaded() { }
	}
}
