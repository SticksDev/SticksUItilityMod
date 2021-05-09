using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SticksUItilityMod
{
    class MainMod : MelonMod
    {        
        public override void OnApplicationStart()
        {
            MelonLogger.Msg("Loading SticksUitilityMod..");
            SticksUItilityMod.UpdateLoader.callupdater();
        }
    }
}
