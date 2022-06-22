using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rabbmq.Pageing.MongoDb
{
    /// <summary>
    /// 描述：分页实体
    /// 创建人：苏本东
    /// 创建时间：2019-3-5 19:05:20
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagingModel<T> where T : class, new()
    {
        /// <summary>
        /// 最后的 _id
        /// </summary>
        public string PageLastId { get; set; }
        /// <summary>
        /// 当前页码 不用低效率
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 每页大小
        /// </summary>
        public int PageSize { get; set; } = 20;

        /// <summary>
        /// 总记录数  不用低效率
        /// </summary>
        public int TotalRecords { get; set; }

        /// <summary>
        /// 总页数  不用低效率
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// 每页数据
        /// </summary>
        public List<T> Items { get; set; }
    }

}
