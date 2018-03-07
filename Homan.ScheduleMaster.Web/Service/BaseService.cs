using Homan.ScheduleMaster.Web.Common;
using Homan.ScheduleMaster.Web.Dto;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;

namespace Homan.ScheduleMaster.Web.Service
{
    public abstract class BaseService
    {
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

        public ListPager<T> WherePager<T, TOrder>(DbSet<T> dbSet, ListPager<T> pager,
    Expression<Func<T, bool>> where, Expression<Func<T, TOrder>> orderBy, bool isAsc = true) where T : class
        {
            var list = dbSet.Where(where);
            if (pager.Filters != null)
            {
                foreach (var filter in pager.Filters)
                {
                    list = list.Where(filter);
                }
            }
            var orderList = isAsc ? list.OrderBy(orderBy) : list.OrderByDescending(orderBy);
            pager.Rows = orderList.Skip(pager.SkipCount).Take(pager.PageSize).ToList();
            pager.Total = orderList.Count();
            return pager;
        }

        /// <summary>
        /// 根据条件修改模型指定字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbSet"></param>
        /// <param name="where"></param>
        /// <param name="updater"></param>
        public void UpdateModel<T>(DbSet<T> dbSet, Expression<Func<T, bool>> where, Expression<Func<T, object>> updater) where T : class
        {
            //获取Update的赋值语句
            var updateMemberExpr = (MemberInitExpression)updater.Body;
            var updateMemberCollection = updateMemberExpr.Bindings.Cast<MemberAssignment>().Select(c =>
            new
            {
                c.Member.Name,
                Value = c.Expression.NodeType == ExpressionType.Constant ? ((ConstantExpression)c.Expression).Value : Expression.Lambda(c.Expression, null).Compile().DynamicInvoke()
            }).ToArray();
            string[] modifyPropertyNames = updateMemberCollection.Select(c => c.Name).ToArray();
            object[] modifyPropertyValues = updateMemberCollection.Select(c => c.Value).ToArray();

            Type typeObj = typeof(T);
            //获取要修改的 属性类型 对象 PropertyInfo
            List<PropertyInfo> listModifyProperty = new List<PropertyInfo>();
            foreach (var propertyName in modifyPropertyNames)
            {
                //将要修改的 属性类型对象 存入集合
                listModifyProperty.Add(typeObj.GetProperty(propertyName));
            }
            var listModifing = dbSet.Where(where);
            foreach (var item in listModifing)
            {
                //遍历要修改的 属性类型 集合
                for (int index = 0; index < listModifyProperty.Count; index++)
                {
                    PropertyInfo proModify = listModifyProperty[index];
                    proModify.SetValue(item, modifyPropertyValues[index], null);
                }
            }
        }
    }
}