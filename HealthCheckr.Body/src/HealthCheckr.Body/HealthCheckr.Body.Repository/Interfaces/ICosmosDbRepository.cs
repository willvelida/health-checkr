using HealthCheckr.Body.Common.Envelopes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCheckr.Body.Repository.Interfaces
{
    public interface ICosmosDbRepository
    {
        Task CreateWeightDocument(WeightEnvelope weightEnvelope);
    }
}
