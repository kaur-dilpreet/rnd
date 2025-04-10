using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reports.Core.Domain.Models
{
    public class JsonListModel<T> : JsonModel
    {
        public JsonListModel()
        {
            this.Message = String.Empty;
            this.List = new List<T>();
        }

        public List<T> List { get; set; }
    }

    public class JsonObjectModel<T> : JsonModel
    {
        public T Object { get; set; }
    }

    public class JsonModel
    {
        public JsonModel()
        {
            this.Message = String.Empty;
        }
        public Boolean Result { get; set; }
        public String Message { get; set; }
        public Int32 FileSize { get; set; }
        public Int32 ErrorCode { get; set; }
        public Guid UniqueId { get; set; }
    }

    public class JsonUploadModel : JsonModel
    {
        public String File { get; set; }
        public String FileName { get; set; }
        public String RealFile { get; set; }
        public String InputeId { get; set; }

        public String Thumbnail { get; set; }
        public Int32 Height { get; set; }
        public Int32 Width { get; set; }
        public String ImageId { get; set; }
    }
}
