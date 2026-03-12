using System;
using System.Collections.Generic;
using System.Text;

namespace Scheri.PETPanel.Network.Contract;

public class BuildInfo
{
    public BuildInfo()
    {

    }

    public Dev_Info dev_info { get; set; } = new();

    public StudyJson? study_info { get; set; } = null;

    public int input_type { get; set; } = 0; //重建输入数据类型，0：LMC、1：IH

    public string build_name { get; set; } = ""; //年月日时分秒毫秒组成的名称
    public string build_info { get; set; } = ""; //备注信息


    //采集时的设备参数
    public bool use_collect_param { get; set; } = false; //是否使用自定义的采集参数，否则使用系统默认的采集参数

    public string dev_ip { get; set; } = "192.168.0.2";
    public int dev_port { get; set; } = 8080;
    public string local_ip { get; set; } = "192.168.0.3";

    public int power_min { get; set; } = 90; //能量校正窗,取值范围:0-65535，默认:90，代表前后30%
    public int power_max { get; set; } = 100;//100
    public int peak_min { get; set; } = 2000; //全能峰能窗，取值范围:0-65535，默认:2000-7000
    public int peak_max { get; set; } = 7000;
    public int wave_width { get; set; } = 20; //波形宽度，默认20   
    public int threshold { get; set; } = 80; //采集阈值，默认120
    public int death_time { get; set; } = 100; //死时间，默认100
    public int threshold_low { get; set; } = 60; //低阈值，默认60
    public int time_win { get; set; } = 10000; ///符合时间窗，默认1000


    public int collect_mode { get; set; } = 0; //采集模式，0：prompt符合模式，1：single单一模式
    //采集时的文件记录参数
    public long collect_prompt_count { get; set; } = 0; //采集的符合事件的数量，0为不限制
    public long collect_duration { get; set; } = 1000 * 60 * 10; //采集时长，单位毫秒，0代表不限制
    public long collect_size { get; set; } = 0; //采集文件大小，单位字节，0代表不限制
    public int segment_type { get; set; } = 2; //自动分段类型。0：不分段，1：按大小分段，2：按时长分段
    public long segment_len { get; set; } = 1000 * 60 * 10; //自动分段长度，单位字节或毫秒


    public int save_type { get; set; } = 1; //保存类型，0：LMC，1：RawData, 2:LMC+RAW

    //重建算法库，0:V1，1:V2，2:FBP
    public int recon_algorithm { get; set; } = 1;

    //重建时的文件输入列表或者输入目录，两者基本是互斥的
    public List<string> src_file_list { get; set; } = new();


    public string src_path { get; set; } = ""; //RawData路径

    //重建RawData源的文件前缀，默认应为PetData
    public string file_prefix { get; set; } = "PetData";

    public string src_collect_id { get; set; } = ""; //需要重建的数据ID

    public string dst_path { get; set; } = ""; //结果存储路径

    //后面会废除使用
    public string tag_path { get; set; } = ""; //DCM  Tag文件读取路径


    //重建参数
    //扫描目标的类型，0:人脑   1:Hoffman， 2:自制园柱ImageQuality, 3:SUV， 4:NECR， 5:德伦佐，6:点源数据
    public int scan_type { get; set; } = 0;

    //划分子集的数量
    public int subset_count { get; set; } = 3;

    //是否启用归一化校准
    public bool bEnableNormalizationCorrection { get; set; } = false;

    //归一化因子路径
    public string normalization_path { get; set; } = "";

    //随机校准类型,0：禁用， 1：随机除法， 2：随机减法
    public int random_correction_type { get; set; } = 0;

    //系统矩阵路径
    public string matrix_path { get; set; }

    //迭代次数
    public int iter_count { get; set; } = 5;
    public int umap_mode { get; set; } //UMAP生成方式，0：禁用，1：手动指定umap, 2:伪CT方式自动生成

    public string umap_path { get; set; } //m_umap_mode为1时该属性有效
    public string model_path { get; set; }
    public int umap_iter_count { get; set; } //UMAP推理图像的迭代次数
    public bool bEnableAC { get; set; } //是否开启衰减校准
    public bool bEnableSC { get; set; } //是否开启散射校准
    public int sc_iter_count { get; set; } //散射参与的迭代次数
    public bool bEnableIPP { get; set; } //是否启用后处理
    public bool bSaveFrame { get; set; } = true;//是否同时保存单层Dicom文件
    public int nPassFrame { get; set; } //图像数据归一化时的起始层

    //分辨率参数
    public double numberX { get; set; } = 216; //径向分辨率，LYSO设备默认200，BGO设备默认216
    public double numberY { get; set; } = 166; //轴向分辨率，LYSO设备默认125，BGO设备默认166
    public double numberZ { get; set; } = 216; //切向分辨率，LYSO设备默认200，BGO设备默认216

    //重建图像尺寸的6个属性，从m_dev_info  中移到 build_info里面了，你只需要给dx，dy，dz赋值，0.5代表512，1代表256，2代表128
    //体素大小参数
    public double dx { get; set; } = 1; //径向体素大小，LYSO设备默认1.0，BGO设备默认1.5
    public double dy { get; set; } = 1; //轴向体素大小，LYSO设备默认1.0，BGO设备默认1.5
    public double dz { get; set; } = 1; //切向体素大小，LYSO设备默认1.0，BGO设备默认1.5


    public Dictionary<string, string> GetParamKey()
    {
        Dictionary<string, string> dic = new()
        {
            { "扫描时长", collect_duration / 1000 + "秒" },
            { "子集数量", subset_count.ToString() },
            { "迭代次数", iter_count.ToString() },
            { "衰减校准", bEnableAC ? "启用" : "未启用" },
            { "散射校准", bEnableSC ? "启用" : "未启用" },
            { "后处理", bEnableIPP ? "启用" : "未启用" }
        };
        switch (recon_algorithm)
        {
            case 0: dic.Add("重建算法", "V1"); break;
            case 1: dic.Add("重建算法", "V2"); break; ;
            case 2: dic.Add("重建算法", "FBP"); break; ;
        }
        dic.Add("矩阵大小", dxToStr());
        return dic;
    }


    /// <summary>
    /// 体素转换为字符串
    /// </summary>
    /// <returns></returns>
    public string dxToStr()
    {
        switch (dx)
        {
            case 0.5: return "512*512";
            case 1: return "256*256";
            case 2: return "128*128";
        }

        return "none";
    }

    public float dxFromStr()
    {
        switch (dxToStr())
        {
            case "512*512": return 0.5f;
            case "256*256": return 1;
            case "128*128": return 2;
        }

        return 0;
    }
}

public class Dev_Info
{
    public int type { get; set; } = 0; //设备类型，0：BGO， 1：LYSO, 2:BGO2
    public int ringNum { get; set; } = 4; //环的数量，LYSO设备默认为2，BGO设备默认为4, BGO2:8
    public double wallThickness { get; set; } = 1.5; //筒壁厚度(毫米)，默认为1.5
    public double m_detectorHeightOutside { get; set; } = 60; //探测器高度(毫米)，默认为60;

    public double radius { get; set; } = 163.87; //晶体环中心内切圆的半径(毫米)， LYSO设备默认为131.04，BGO设备默认为163.87, , BGO2:156.3
    public double CutOutR { get; set; } = 140; // FOV可视角度半径(毫米)，LYSO设备默认为100，BGO设备默认为140
    public int blocknumber { get; set; } = 15; //每个晶体环的块数量，LYSO设备默认为12，BGO设备默认为15, BGO2:30
    public double ringInsideGap { get; set; } = 4; //内层环间隙(毫米)，默认为4, BGO2:1
    public double ringOutsideGap { get; set; } = 4; //外层环间隙(毫米)，默认为4, BGO2:1

    public int numberOfCrystalInsideWidth { get; set; } = 20; //内层模块宽度晶体数，LYSO设备默认为30，BGO设备默认为20, BGO2:16
    public int numberOfCrystalInsideLength { get; set; } = 20; //内层模块长度晶体数，LYSO设备默认为30，BGO设备默认为20, BGO2:16
    public int m_numberOfCrystalOutsideWidth { get; set; } = 20; //外层模块宽度晶体数，LYSO设备默认为30，BGO设备默认为20, BGO2:16
    public int numberOfCrystalOutsideLength { get; set; } = 20; //外层模块长度晶体数，LYSO设备默认为30，BGO设备默认为20, BGO2:16
    public int blockselectnumber { get; set; } = 2; //组合选取块数，默认为2
    public int mindiff { get; set; } = 4; //最小环(块)差，默认为4


    //晶体尺寸参数
    public double crystalWidth { get; set; } = 2.8; //晶体宽度(毫米)，LYSO晶体默认1.8，BGO晶体默认2.8,  BGO2:2
    public double crystalLength { get; set; } = 2.8; //晶体长度(毫米)，LYSO晶体默认1.8，BGO晶体默认2.8,  BGO2:2
    public double crystalHeightInside { get; set; } = 15; //内层晶体高度(毫米)，默认为15
    public double crystalHeightOutside { get; set; } = 15; //外层晶体高度(毫米)，默认为15
    public double crystalDistanceInside { get; set; } = 0.2; //内层晶体反射层厚度(毫米)，默认为0.2
    public double crystalDistanceOutside { get; set; } = 0.2; //外层晶体反射层厚度(毫米)，默认为0.2




    //0，正弦图参数 sinogram_r=216
    public double sinogram_r { get; set; } = 216;

    //1，正弦图参数 sinogram_phi=180
    public double sinogram_phi { get; set; } = 180;

    //10，FOV半径,单位mm， FOVR = 128
    public double FOVR { get; set; } = 128;

    //z和y方向缝隙等效近似晶体个数
    public int gap_crystal_count_z { get; set; } = 0;
    public int gap_crystal_count_y { get; set; } = 0;

    //临时属性
    public int build_type { get; set; } = 0; //重建类型，0：Siddon，1：SiddonV2, 2：TOR

    public double rescale_slope { get; set; } = 1; //SUV定量校准因子
}

public class StudyJson()
{
    /// <summary>
    /// //扫描目标类型，0:人,1:NEMA RES, 2:NEMA SEN, 3:NEMA NECR, 4:NEMA IQ
    /// </summary>
    public int type { get; set; }

    /// <summary>
    /// //NEMA实验标准，0:2007，1:2018
    /// </summary>
    public int nema_type { get; set; }


    public string patient_name { get; set; } = "";

    /// <summary>
    /// //0:未知，1:男，2:女
    /// </summary>
    public int patient_sex { get; set; }

    public int patient_age { get; set; }

    public int patient_weight { get; set; }

    /// <summary>
    /// //注射日期和时间
    /// </summary>
    public string acquisition_date { get; set; } = "";
    public string acquisition_time { get; set; } = "";

    /// <summary>
    /// //注射药物，0:F18FDG
    /// </summary>
    public int drug_type { get; set; }

    /// <summary>
    /// //初始活度(单位KBg)
    /// </summary>
    public int dose { get; set; }

    public string scan_date { get; set; } = "";
    public string scan_time { get; set; } = "";
    public string scan_end_date { get; set; } = "";
    public string scan_end_time { get; set; } = "";

    /// <summary>
    /// //扫描时长(单位室秒)
    /// </summary>
    public int scan_duration { get; set; }

    public int scan_size { get; set; }
    public int scan_prompt { get; set; }
    public int scan_random { get; set; }

    //suv定量校准因子
    public double rescale_slope { get; set; }

    public string info { get; set; } = "";
    public int avg_rate { get; set; }

    public int min_rate { get; set; }
    public int max_rate { get; set; }

}