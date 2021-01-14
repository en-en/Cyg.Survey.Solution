using Cyg.Resource.Enums;
using System.ComponentModel.DataAnnotations;

namespace Cyg.Applicatio.Survey.Dto
{
    public class ResourceOptionsRequest : BaseResourceRquest
    {
        /// <summary>
        /// 设计类型
        /// </summary>
        [DefineEnum(typeof(DesignType))]
        public DesignType ForDesign { get; set; }
    }
}
