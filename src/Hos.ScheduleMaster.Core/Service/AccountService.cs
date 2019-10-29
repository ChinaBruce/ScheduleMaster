using Hos.ScheduleMaster.Core.Common;
using Hos.ScheduleMaster.Base.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hos.ScheduleMaster.Core.Service
{
    public class AccountService : BaseService, IAccountService
    {
        /// <summary>
        /// 用户名和密码登录判断
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public SystemUser LoginCheck(string userName, string password)
        {
            string encodePwd = SecurityHelper.MD5(password);
            var user = _repositoryFactory.SystemUser.Single(x => x.UserName == userName && x.Password == encodePwd);
            if (user == null || user.Status != (int)SystemUserStatus.Available)
            {
                return null;
            }
            //更新登录时间
            user.LastLoginTime = DateTime.Now;
            _repositoryFactory.SystemUser.Modify(user, "LastLoginTime");
            UnitOfWork.Commit();
            //System.Web.Security.FormsAuthentication.SetAuthCookie(userName, false);
            return user;
        }

        /// <summary>
        /// 根据id查询用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SystemUser GetUserById(int id)
        {
            return _repositoryFactory.SystemUser.Single(x => x.Id == id);
        }

        /// <summary>
        /// 查询用户分页数据
        /// </summary>
        /// <param name="pager"></param>
        /// <returns></returns>
        public ListPager<SystemUser> GetUserPager(ListPager<SystemUser> pager)
        {
            return _repositoryFactory.SystemUser.WherePager(pager, m => m.Status != (int)SystemUserStatus.Deleted, m => m.CreateTime, false);
        }

        /// <summary>
        /// 判断用户名时候已存在
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool CheckUserName(string userName, int id = 0)
        {
            if (id > 0)
            {
                var model = GetUserById(id);
                if (model != null && model.UserName.ToLower() == userName.ToLower())
                {
                    return true;
                }
            }
            return _repositoryFactory.SystemUser.Single(x => x.UserName.ToLower() == userName.ToLower()) == null;
        }

        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddUser(SystemUser model)
        {
            model.Password = SecurityHelper.MD5(model.Password);
            model.Status = (int)SystemUserStatus.Available;
            model.CreateTime = DateTime.Now;
            _repositoryFactory.SystemUser.Add(model);
            return UnitOfWork.Commit() > 0;
        }

        /// <summary>
        /// 编辑用户信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool EditUser(SystemUser model)
        {
            _repositoryFactory.SystemUser.ModifyBy(x => x.Id == model.Id, new
            {
                model.Email,
                model.Phone,
                model.RealName,
                model.UserName
            });
            return UnitOfWork.Commit() > 0;
        }

        /// <summary>
        /// 更新用户状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool UpdateUserStatus(int id, int status)
        {
            _repositoryFactory.SystemUser.ModifyBy(x => x.Id == id, new
            {
                Status = status
            });
            return UnitOfWork.Commit() > 0;
        }

        /// <summary>
        /// 修改用户密码
        /// </summary>
        /// <param name="id"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool UpdateUserPassword(int id, string password)
        {
            password = SecurityHelper.MD5(password);
            _repositoryFactory.SystemUser.ModifyBy(x => x.Id == id, new
            {
                Password = password
            });
            return UnitOfWork.Commit() > 0;
        }
    }
}
