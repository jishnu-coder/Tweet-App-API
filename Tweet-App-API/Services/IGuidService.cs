using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tweet_App_API.Services
{
    public interface IGuidService { Guid NewGuid(); }

    class GuidService : IGuidService
    {
        public Guid NewGuid() { return Guid.NewGuid(); }
    }
}
