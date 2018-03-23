using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventWebApp.Models;
using EventWebApp.Models.Filter;
using Microsoft.AspNetCore.Mvc;

namespace EventWebApp.Service
{
    public interface IDaoService<E,F> 
        where F : IRequestFilter 
        where E : IEntity
    {
        IEnumerable<E> GetAll();
        Task<E> GetById(string id);
        void Edit(E e);
        void Delete(E e);
        E Create(E e);
        PageResponse<E> GetPage(int page, int size, F filter);
        Task<int> Save();
        bool EventExists(string id);
        bool Validate(E e);
    }
}
