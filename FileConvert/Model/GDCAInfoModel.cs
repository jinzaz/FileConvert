using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace FileConvert.Model
{
    [DataContract]
    [Serializable]
    public class GDCAInfoModel
    {
        [DataMember]
        public string CAVersion { get; set; }

        [DataMember]
        public List<GDCADetail> GDCADetailList { get; set; }
    }
    [DataContract]
    [Serializable]
    public class GDCADetail
    {
        [DataMember]
        public string GDCACode { get; set; }
        [DataMember]
        public string Msg { get; set; }
        [DataMember]
        public List<PageModel> Page { get; set; }
    }

    [DataContract]
    [Serializable]
    public class PageModel
    {
        [DataMember]
        public int? VerifyResult { get; set; }
        [DataMember]
        public string VerifyComment { get; set; }
    }
}
