using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Scheri.PETPanel.Network.Contract;


public class StatusInfo
{
    /// <summary>
    /// -1:未知，0:空闲，1：采集中， 2：暂停
    /// </summary>
    public int collect_state { get; set; } = 0;
    public string collect_id { get; set; }
    public long total_recv_size { get; set; } //接受到的网络包总字节数
    public int dev_test_state { get; set; } //pet设备的链接情况，自检状态，-1:未知状态， 0：连接异常， 1：连接正常， 2：自检中
    public long collect_start_ts { get; set; } //采集开始时间，单位毫秒
    public long collect_pause_start_ts { get; set; } //当此暂停开始时间
    public long collect_pause_duration { get; set; } //此次采集过程中累计暂停时长
    public long collect_size { get; set; } //已采集文件大小，单位字节
    public int collect_segment_idx { get; set; } //当前采集的分片数量
    public long collect_segment_size { get; set; } //当前分片的大小
    public long collect_segment_start_ts { get; set; } //当前分片开始的时间
    public long collect_segment_pause_start_ts { get; set; } //当前分片暂停的开始时间
    public long collect_segment_pause_duration { get; set; } //当前分片累计暂停时长
    public long collect_prompt_count { get; set; } //已采集符合事件的数量
    public long collect_random_count { get; set; } //已采集随机事件的数量

    public long srv_update_ts { get; set; } //

    public int board_err { get; set; }
    public int link_err { get; set; }
    public int adc_err { get; set; }
    public double temp_min { get; set; }
    public double temp_max { get; set; }
    public double temp_avg { get; set; }

    /// <summary>
    /// -1：未知，0:空闲，1：重建中
    /// </summary>
    public int recon_state { get; set; } = 0;

    public string recon_id { get; set; }
    public double last_update_ts { get; set; }

    public List<StatusInfo> Cache { get; set; } = new();

    public string ToStr()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"采集状态 : {collect_state}");
        sb.AppendLine($"采集开始时间 : {collect_start_ts}");
        sb.AppendLine($"采集暂停开始时间 : {collect_pause_start_ts}");
        sb.AppendLine($"采集暂停时长 : {collect_pause_duration}");
        sb.AppendLine($"已采集文件大小 : {collect_size}");
        sb.AppendLine($"已采集分片数量 : {collect_segment_idx}");
        sb.AppendLine($"当前分片大小 : {collect_segment_size}");
        sb.AppendLine($"当前分片开始时间 : {collect_segment_start_ts}");
        sb.AppendLine($"当前分片暂停开始时间 : {collect_segment_pause_start_ts}");
        sb.AppendLine($"当前分片累计暂停时长 : {collect_segment_pause_duration}");
        sb.AppendLine($"已采集符合事件数量 : {collect_prompt_count}");
        sb.AppendLine($"已采集随机事件数量 : {collect_random_count}");
        sb.AppendLine($"重建状态 : {recon_state}");
        sb.AppendLine($"状态更新时间 : {last_update_ts}");
        sb.AppendLine($"设备自检状态 : {dev_test_state}");
        sb.AppendLine($"符合板通信故障 : {link_err}");
        sb.AppendLine($"设备温度 : {temp_min} {temp_max} {temp_avg}");
        sb.AppendLine($"ADC故障 : {adc_err}");
        sb.AppendLine($"符合板故障 : {board_err}");
        sb.AppendLine($"设备总接收字节数 : {total_recv_size}");
        return sb.ToString();
    }

    public long GetCollectDuration()
    {
        if (collect_state == 2)
            return (collect_pause_start_ts - collect_start_ts - collect_pause_duration) / 1000;

        if (collect_state == 1)
            return (long)((last_update_ts - collect_start_ts - collect_pause_duration) / 1000);
        return 0;
    }

}
