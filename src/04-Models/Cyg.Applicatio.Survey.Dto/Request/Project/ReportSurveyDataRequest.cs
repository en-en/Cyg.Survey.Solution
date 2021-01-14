using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Cyg.Applicatio.Survey.Dto
{
    public class ReportSurveyDataRequest
    {
        /// <summary>
        /// 工程编号
        /// </summary>
        [DisplayName("工程编号")]
        [Required(ErrorMessage = "{0} 不能为空")]
        public string ProjectId { get; set; }

        /// <summary>
        /// 勘察数据
        /// </summary>
        public CollectionDataRequest SurveyData { get; set; }

        /// <summary>
        /// 设计数据
        /// </summary>
        public CollectionDataRequest DesignData { get; set; }
    }
}
