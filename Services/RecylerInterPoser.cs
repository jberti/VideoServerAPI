using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VideoServerAPI.Services
{
    /// <summary>
    /// Server como meio campo entre o recyclerController e o recyclerservice porque o recyclerservice é do tipo
    /// backgroundservice e este, por sua vez, não fica no contexto na aplicação e não pode ser injetado no recyclerservice.
    /// Então a tática é injetar o interpose, no controller e no service, como singleton, e aí a sua lista, tipo blockingcollection,
    /// tem suas ininformações compartilhadas entre eles.
    /// </summary>
    public class RecylerInterPoser
    {
        public BlockingCollection<Guid> Videos { get; } = new();
        
    }
}
