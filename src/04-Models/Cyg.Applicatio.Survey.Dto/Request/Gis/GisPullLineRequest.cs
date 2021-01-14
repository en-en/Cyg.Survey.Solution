using Cyg.Resource.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cyg.Applicatio.Survey.Dto.Request.Gis
{
    /// <summary>
    /// 设计拉线数据
    /// </summary>
    public class GisPullLineRequest
    {
        /// <summary>
        ///      主键
        /// </summary>
        [DisplayName("主键")]
        [Required(ErrorMessage = "{0} 不能为空")]
        public string Id { get; set; }

        /// <summary>
        ///     杆塔id
        /// </summary>
        public string Main_Id { get; set; }


        /// <summary>
        ///     方位角
        /// </summary>
        public float? Azimuth { get; set; }

        /// <summary>
        ///     拉线型号
        /// </summary>
        public string Mode { get; set; }

        /// <summary>
        ///     拉线型号id
        /// </summary>
        public string Mode_Id { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [DefineEnum(typeof(SurveyState))]
        public SurveyState State { get; set; }

        /// <summary>
        ///     项目id
        /// </summary>
        public string ProjectId { get; set; }

        /// <summary>
        /// 几何列，类型为点
        /// </summary>
        public string Geom { get; set; }

        /// <summary>
        ///     备注
        /// </summary>
        public string Remark { get; set; }

    }
}
