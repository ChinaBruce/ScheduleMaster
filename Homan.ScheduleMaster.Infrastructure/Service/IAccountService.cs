using Homan.ScheduleMaster.Core.Common;
using Homan.ScheduleMaster.Base.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homan.ScheduleMaster.Core.Service
{
    public interface IAccountService
    {
        /// <summary>
        /// 账号登入
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        SystemUser LoginCheck(string userName, string password);

        /// <summary>
        /// 根据id查询用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        SystemUser GetUserById(int id);

        /// <summary>
        /// 查询用户分页数据
        /// </summary>
        /// <param name="pager"></param>
        /// <returns></returns>
        ListPager<SystemUser> GetUserPager(ListPager<SystemUser> pager);

        /// <summary>
        /// 判断用户名时候已存在
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        bool CheckUserName(string userName, int id = 0);

        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool AddUser(SystemUser model);

        /// <summary>
        /// 编辑用户信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool EditUser(SystemUser model);

        /// <summary>
        /// 更新用户状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        bool UpdateUserStatus(int id, int status);

        /// <summary>
        /// 修改用户密码
        /// </summary>
        /// <param name="id"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        bool UpdateUserPassword(int id, string password);
    }
}
