﻿using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Xml.Serialization;
using ALF.SYSTEM.DataModel;
using Microsoft.CSharp;

namespace ALF.SYSTEM
{
    /// <summary>
    /// Windows工具
    /// </summary>
    public static class WindowsTools
    {
        /// <summary>
        /// 判断是否安装软件
        /// </summary>
        /// <param name="softName">软件名称</param>
        /// <returns>是否安装</returns>
        public static bool IsSoftInstalled(string softName)
        {
            var uninstallNode = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            if (uninstallNode == null)
            {
                return false;
            }
            foreach (var subKeyName in uninstallNode.GetSubKeyNames())
            {

                var subKey = uninstallNode.OpenSubKey(subKeyName);
                if (subKey == null)
                {
                    continue;
                }
                var displayName = subKey.GetValue("DisplayName");

                if (displayName == null)
                {
                    continue;
                }
                if (displayName.ToString()==softName)
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// 判断服务是否启动
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns>是否启动</returns>
        public static bool IsServeiceStart(string serviceName)
        {
            if (serviceName == "")
            {
                return true;
            }

            var ser = new ServiceController { ServiceName = serviceName, MachineName = "." };

            try
            {
                return (ser.Status == ServiceControllerStatus.Running);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 获取文件名
        /// </summary>
        /// <param name="fileFullName">完整文件名</param>
        /// <returns>文件名称</returns>
        public static string GetBasicName(string fileFullName)
        {
            var index = fileFullName.LastIndexOf("\\", StringComparison.Ordinal);
            return fileFullName.Substring(index + 1);
        }

        /// <summary>
        /// 命令行执行完成事件
        /// </summary>
        public static Action<string> ExecCmdFinished;

        /// <summary>
        /// 运行命令行
        /// </summary>
        /// <param name="fileName">运行文件名称</param>
        /// <param name="argName">运行文件参数</param>
        /// <param name="hideWindow">是否隐藏运行窗口</param>
        /// <param name="isShowFinished">是否显示运行完成结果</param>
        /// <returns>运行结果</returns>
        public static string ExecCmd(string fileName, string argName, bool hideWindow = false, bool isShowFinished = false)
        {
            var process = new Process();
            var result = "";
            try
            {
                process.StartInfo.FileName = fileName;
                process.StartInfo.Arguments = argName;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                if (hideWindow)
                {
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                }

                process.StartInfo.RedirectStandardInput = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;

                process.Start();
                if (isShowFinished)
                {
                    result = process.StandardOutput.ReadToEnd().Trim();
                }
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cmd error ;{0}", ex.Message);
                result = ex.Message;
            }
            if (ExecCmdFinished != null)
            {
                ExecCmdFinished(result);
            }
            return result;
        }

        /// <summary>
        /// 写入TXT文件
        /// </summary>
        /// <param name="path">TXT文件路径</param>
        /// <param name="infoString">待写入信息</param>
        public static void WriteToTxt(string path, string infoString)
        {
            StreamWriter streamWriter = File.Exists(path) ? File.AppendText(path) : new StreamWriter(path);
            streamWriter.Write(infoString);
            streamWriter.Close();
        }

        /// <summary>
        /// 读取TXT文件
        /// </summary>
        /// <param name="path">TXT文件路径</param>
        /// <returns>TXT文件内容</returns>
        public static string ReadFromTxt(string path)
        {
            var t = File.ReadAllLines(path, Encoding.Default);
            return t.Aggregate("", (current, s) => current + (s + Environment.NewLine));
        }

        /// <summary>
        /// 创建XML配置文件
        /// </summary>
        /// <param name="filePath">目标文件夹路径</param>
        /// <param name="inputType">配置文件类型</param>
        /// <returns>创建结果</returns>
        public static string CreatXml(string filePath, ConfigType inputType)
        {
            var dir = new DirectoryInfo(string.Format(@"{0}\", filePath));
            if (!dir.Exists)
            {
                return filePath + @"路径不存在";
            }
            var fileList = GetFiles(dir);
            var updateFileList = fileList.Select(item => new UpdateFile
            {
                FileName = item.Name,
                FileSize = (int)item.Length,

            }).ToList();


            var xmlSerializer = new XmlSerializer(updateFileList.GetType());
            var file = new FileStream(string.Format(@"{0}\dataConfig.{1}", filePath, inputType), FileMode.Create);
            xmlSerializer.Serialize(file, updateFileList);
            file.Flush();
            file.Close();
            return "";
        }

        /// <summary>
        /// XML序列化
        /// </summary>
        /// <param name="itemList">待序列化列表</param>
        /// <param name="path">生成路径</param>
        /// <param name="result">序列化结果</param>
        public static void XmlSerialize(object itemList, string path, out string result)
        {
            result = "";
            try
            {
                var xmlSerializer = new XmlSerializer(itemList.GetType());
                var file = new FileStream(path, FileMode.Create);
                xmlSerializer.Serialize(file, itemList);

                file.Flush();
                file.Close();
                file.Dispose();
            }
            catch (Exception exception)
            {

                result = exception.Message;
            }
        }

        /// <summary>
        /// XML反序列化
        /// </summary>
        /// <param name="listType">反序列化文件类型</param>
        /// <param name="path">XML文件路径</param>
        /// <param name="result">反序列化结果</param>
        /// <param name="obj">序列化目标类型</param>
        /// <returns>反序列化结果</returns>
        public static object XmlDeseerializer(Type listType, string path, out string result, object obj = null)
        {
            result = "";
            try
            {
                if (!File.Exists(path))
                {
                    XmlSerialize(obj, path, out result);
                }
                var xmlSerializer = new XmlSerializer(listType);
                var file = new FileStream(path, FileMode.Open);
                var resultList = xmlSerializer.Deserialize(file);
                file.Close();
                file.Dispose();

                return resultList;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        /// <summary>
        /// Int根据ASCII转换为Char
        /// </summary>
        /// <param name="value">带转化数值</param>
        /// <returns>转化后结果</returns>
        public static string ConvertIntToChar(int value)
        {
            if (value > Math.Pow(26, 2))
            {
                return ((char)((value / (int)Math.Pow(26, 2)) + 64)).ToString(CultureInfo.InvariantCulture)
                    + ConvertIntToChar(value % (int)Math.Pow(26, 2));
            }
            if (value > 26)
            {
                return ConvertIntToChar(value / 26)
                    + ConvertIntToChar(value % 26);
            }
            return value == 0 ? "A" : ((char)(value % 27 + 64)).ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 编译代码文件
        /// </summary>
        /// <param name="sourceCode">待编译代码</param>
        /// <param name="filePath">生成文件路径</param>
        /// <param name="referenceString">引用字符串</param>
        /// <returns>编译结果</returns>
        public static string CompileCode(string sourceCode, string filePath, string[] referenceString)
        {
            var p = new CSharpCodeProvider();
#pragma warning disable 618
            var cc = p.CreateCompiler();
#pragma warning restore 618
            var options = new CompilerParameters(referenceString) { GenerateExecutable = true, OutputAssembly = filePath };
            var cu = new CodeSnippetCompileUnit(sourceCode);
            var cr = cc.CompileAssemblyFromDom(options, cu);
            return cr.Errors.Count == 0 ? "" : cr.Errors.Cast<CompilerError>().Aggregate("", (current, error) => current + (error + "\n"));
        }

        /// <summary>
        /// 获取全部子文件
        /// </summary>
        /// <param name="dir">查询目录路径</param>
        /// <returns>文件列表</returns>
        private static IEnumerable<FileInfo> GetFiles(DirectoryInfo dir)
        {
            var result = dir.GetFiles().ToList();
            var tmpDirList = dir.GetDirectories();
            foreach (var item in tmpDirList)
            {
                result.AddRange(GetFiles(item));
            }
            return result;
        }
    }
}
