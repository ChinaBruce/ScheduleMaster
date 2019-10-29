using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Homan.ScheduleMaster.Base.Models;

namespace Homan.ScheduleMaster.Core.Repository
{
    public class RepositoryFactory
    {
        private IUnitOfWork _unitOfWork;

        public RepositoryFactory(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public BaseRepository<Task> Tasks => new BaseRepository<Task>(_unitOfWork);

        public BaseRepository<SystemLog> SystemLogs => new BaseRepository<SystemLog>(_unitOfWork);

        public BaseRepository<SystemUser> SystemUser => new BaseRepository<SystemUser>(_unitOfWork);
    }
}
