using System;
#pragma warning disable 1591

namespace webdemo.DAL
{
    public class UnitOfWork : IDisposable
    {
        private EfProducts context = new EfProducts();
        private  GenericRepository<ProductEntity> _repository;

        
        public GenericRepository<ProductEntity> ProductRepository
        {
            get
            {
                if (this._repository == null)
                {
                    this._repository = new GenericRepository<ProductEntity>(context);
                }
                return _repository;
            }
        }

        public void Save()
        {
            context.SaveChanges();
        }
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}