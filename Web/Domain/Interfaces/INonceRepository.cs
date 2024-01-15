using System.Collections.Generic;
using Web.Infrastructure.Entities;

namespace Web.Domain.Interfaces
{
  public interface INonceRepository
  {
    string GenerateNonce();
    long AddNonce(Nonce nonce);
    bool UpdateNonce(Nonce nonce);
    IEnumerable<Nonce> GetNonce();
    Nonce GetByNonce(string nonce);
  }
}