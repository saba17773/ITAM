using System.Collections.Generic;
using Web.Infrastructure.Entities;

namespace Web.Domain.Interfaces
{
  public interface INumberSeqRepository
  {
    NumberSeq GetNumberSeq(string key);
    long AddNumberSeq(NumberSeq numberSeq);
    bool UpdateNumberSeq(NumberSeq numberSeq);
  }
}