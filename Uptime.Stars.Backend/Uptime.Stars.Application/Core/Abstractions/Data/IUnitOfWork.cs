﻿using Microsoft.EntityFrameworkCore.Storage;

namespace Uptime.Stars.Application.Core.Abstractions.Data;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}