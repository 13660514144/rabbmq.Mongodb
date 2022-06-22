using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rabbmq.Pageing.MongoDb
{
    public class DelFindUpFilter
    {
        public string Mothed { get; set; }
        public string FieldName { get; set; }
        public string FieldType { get; set; }
        public string FieldValue { get; set; }
        public string FieldFiler { get; set; } = "EQ";//默认等于方法
    }
}
