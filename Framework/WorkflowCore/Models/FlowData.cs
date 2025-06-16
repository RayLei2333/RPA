using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.WorkflowCore.Models
{
    [Serializable]
    public class FlowData
    {
        /// <summary>
        /// 流程配置
        /// key：流程Id,
        /// value：流程配置数据
        /// </summary>
        public Dictionary<string, FlowConfig> FlowConfigs { get; set; }

        /// <summary>
        /// 全局变量
        /// key：变量名称，
        /// value：全局变量配置
        /// </summary>
        public Dictionary<string, GlobalVar> GlobalVars { get; set; }

        public FlowData()
        {
            FlowConfigs = new Dictionary<string, FlowConfig>();
            GlobalVars = new Dictionary<string, GlobalVar>();
        }
    }
}
