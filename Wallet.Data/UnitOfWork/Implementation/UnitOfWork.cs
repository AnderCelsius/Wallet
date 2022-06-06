using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Data.Repositories.Implementations;
using Wallet.Data.Repositories.Interfaces;
using Wallet.Data.UnitOfWork.Interface;
using Wallet.Models;

namespace Wallet.Data.UnitOfWork.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly WalletDbContext _context;
        private IGenericRepository<Product> _products;
        private IGenericRepository<UserWallet> _wallets;
        public UnitOfWork(WalletDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns a new instance of GenericRepositoy<Product> if _products is null
        /// </summary>
        public IGenericRepository<Product> Products => _products ??= new GenericRepository<Product>(_context);


        /// <summary>
        /// Returns a new instance of GenericRepositoy<UserWallet> if _wallets is null
        /// </summary>
        public IGenericRepository<UserWallet> Wallets => _wallets ??= new GenericRepository<UserWallet>(_context);

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
