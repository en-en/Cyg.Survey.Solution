using Cyg.Applicatio.Dto;
using Cyg.Applicatio.Dto.Enums;
using Cyg.Resource.Enums;
using System;

namespace Cyg.Applicatio.Survey.Dto
{
    /// <summary>
    /// 项目信息表
    /// </summary>
    public class ProjectResponse
    {
        /// <summary>
        /// 工程主键
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 工程名称
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// 项目主键
        /// </summary>
        public string ProjectCollectionId { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectCollectionName { get; set; }

        /// <summary>
        /// 项目类型
        /// </summary>
        public ProjectType ProjectType { get; set; }

        /// <summary>
        /// 项目编号
        /// </summary>
        public string ProjectCode { get; set; }

        /// <summary>
        /// 电压等级
        /// </summary>
        public KVLevel KVLevel { get; set; }

        /// <summary>
        /// 建设性质
        /// </summary>
        public ConstructionNature Construction { get; set; }

        /// <summary>
        /// 项目阶段
        /// </summary>
        public ProjectStage Stage { get; set; }

        /// <summary>
        /// 批次
        /// </summary>
        public ProjectBatch Batch { get; set; }

        /// <summary>
        /// 批次年份
        /// </summary>
        public int BatchYear { get; set; }

        /// <summary>
        /// 桩位范围
        /// </summary>
        public int PileRange { get; set; }

        /// <summary>
        /// 交底范围
        /// </summary>
        public int DisclosureRange { get; set; }

        /// <summary>
        /// 省
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// 市
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// 区/县
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// 气象区
        /// </summary>
        public MeteorologicLevel Meteorologic { get; set; }

        /// <summary>
        /// 所属公司编号
        /// </summary>
        public string CompanyId { get; set; }

        /// <summary>
        ///     项目创建日期
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// 流程状态
        /// </summary>
        public ProjectFlowState FlowState { get; set; }

        /// <summary>
        /// 流程状态更新时间
        /// </summary>
        public DateTime? FlowStateUpdateTime { get; set; }

        /// <summary>
        /// 是否处于复勘状态
        /// </summary>
        public bool IsResetSurvey { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        ///     工程创建日期
        /// </summary>
        public DateTime EngineerCreatedOn { get; set; }
    }
}
