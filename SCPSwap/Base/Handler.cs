using SCPSwap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPSwap.Base
{
    public abstract class Handler
    {
        protected Plugin plugin;

        public Handler(Plugin plugin) => this.plugin = plugin;

        abstract public void Start();
        abstract public void Stop();
    }
}