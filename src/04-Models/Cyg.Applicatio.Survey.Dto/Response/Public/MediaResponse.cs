using Cyg.Applicatio.Dto.Enums;

namespace Cyg.Applicatio.Survey.Dto
{
    /// <summary>
    /// 多媒体
    /// </summary>
    public class MediaResponse
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 工程编号
        /// </summary>
        public string ProjectId { get; set; }

        /// <summary>
        /// 多媒体类型
        /// </summary>
        public MediaType Type { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 存储路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 所属设备ID
        /// </summary>
        public string Main_ID { get; set; }

        /// <summary>
        /// 所属设备类型
        /// </summary>
        public SurveyDeviceType Main_Type { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
