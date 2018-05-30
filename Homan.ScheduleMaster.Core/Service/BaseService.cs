
using Homan.ScheduleMaster.Core.Common;
using Homan.ScheduleMaster.Core.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;

namespace Homan.ScheduleMaster.Core.Service
{
    public abstract class BaseService
    {
        [Ninject.Inject]
        public IUnitOfWork UnitOfWork { get; set; }

        protected RepositoryFactory _repositoryFactory => new RepositoryFactory(UnitOfWork);

        

        protected ApiResponseMessage ServiceResult(ResultStatus status, string msg = "", object data = null)
        {
            return new ApiResponseMessage(status, msg, data);
        }


        /// <summary>
        /// 这里的token用来验证请求者的身份，与quartz host约定好即可
        /// </summary>
        public Dictionary<string, string> Header
        {
            get
            {
                Dictionary<string, string> header = new Dictionary<string, string>();
                header.Add("token", "1b4423b5440c48069eaf7b46b17ac586");
                return header;
            }
        }

    }
}