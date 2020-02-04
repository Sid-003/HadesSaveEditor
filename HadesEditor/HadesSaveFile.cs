using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleDecoder
{
    internal struct HadesSaveFile
    {
        public int Version { get; internal set;}

        public string Location { get; internal set;}

        public int CompletedRuns { get; internal set;}

        public int ActiveMetaPoints { get; internal set; }

        public int ActiveShrinePoints { get; internal set; }

        public bool GodMode { get; internal set; }

        public bool HellMode { get; internal set;  }

        public List<string> LuaKeys { get; internal set; }

        public string CurrentMap { get; internal set; }

        public string NextMap { get; internal set; }

        public List<object> LuaState { get; internal set; }

    }
}
