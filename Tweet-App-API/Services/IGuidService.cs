using System;

namespace Tweet_App_API.Services
{
    public interface IGuidService { Guid NewGuid(); }

    class GuidService : IGuidService
    {
        public Guid NewGuid() { return Guid.NewGuid(); }
    }
}
