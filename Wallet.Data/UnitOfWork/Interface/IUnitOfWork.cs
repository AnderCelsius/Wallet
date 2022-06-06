using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Data.Repositories.Interfaces;
using Wallet.Models;

namespace Wallet.Data.UnitOfWork.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Product> Products { get; }
        IGenericRepository<UserWallet> Wallets { get; }

        Task Save();
    }
}
