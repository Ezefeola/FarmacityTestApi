using Adapter.SqlServer.Data;
using Core.Contracts.Repositories;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Adapter.SqlServer.Repositories;
public class CodigoBarraRepository : GenericRepository<CodigoBarra, int>, ICodigoBarraRepository
{
    public CodigoBarraRepository(ApplicationDbContext context) : base(context) { }

    public async Task<bool> CodigoBarraExistsAsync(string codigo, CancellationToken cancellationToken)
    {
        return await Query()
                     .AnyAsync(x => x.Codigo == codigo, cancellationToken);
    }
}