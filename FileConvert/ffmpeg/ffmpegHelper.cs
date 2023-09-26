using FileConvert.Model;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileConvert.ffmpeg
{
    public static class ffmpegHelper 
    {

        private static readonly string ffmpegPath = Environment.CurrentDirectory + @"\ffmpeg\ffmpeg.exe";
        /// <summary>
        /// 视频压缩
        /// </summary>
        /// <param name="originalPath">原视频地址</param>
        /// <param name="newPath">新视频地址</param>
        /// <param name="Threads">执行线程数</param>
        /// <param name="Resolution">分辨率</param>
        /// <param name="VideoBits">视频码率</param>
        /// <returns></returns>
        public static void ConvertVideo(string originalPath, string newPath,string FpsSize,string Threads,string Resolution, string AudioSF,string VideoBits)
        {
            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = ffmpegPath;//要调用外部程序的绝对路径
                    process.StartInfo.UseShellExecute = false;//不使用操作系统外壳程序启动线程(一定为FALSE,详细的请看MSDN)
                    process.StartInfo.RedirectStandardInput = true;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;///把外部程序错误输出写到StandardError流中(这个一定要注意,FFMPEG的所有输出信息,都为错误输出流,用StandardOutput是捕获不到任何消息的...这是我耗费了2个多月得出来的经验...mencoder就是用standardOutput来捕获的)
                    process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
                    process.StartInfo.StandardErrorEncoding = Encoding.UTF8;
                    process.StartInfo.CreateNoWindow = true;//不创建进程窗口
                    string strArg = $"-i \"{originalPath}\" {FpsSize}  {Threads} {Resolution} {AudioSF} {VideoBits} \"{newPath}\"";
                    //process.ErrorDataReceived += new DataReceivedEventHandler(Output);///外部程序(这里是FFMPEG)输出流时候产生的事件,这里是把流的处理过程转移到下面的方法中,详细请查阅MSDN
                    process.StartInfo.Arguments = strArg;
                    process.Start();//启动线程
                    process.BeginErrorReadLine();//开始异步读取
                    process.WaitForExit();//阻塞等待进程结束
                    process.Close();//关闭进程
                    process.Dispose();//释放资源
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        /// <summary>
        /// 图片压缩
        /// </summary>
        /// <param name="originalPath">原图片地址</param>
        /// <param name="newPath">新图片地址</param>
        /// <param name="Threads">执行线程数</param>
        /// <param name="Quality">图片质量</param>
        /// <returns></returns>
        public static void ConvertImage(string originalPath, string newPath, string Threads,string Quality)
        {
            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = ffmpegPath;//要调用外部程序的绝对路径
                    process.StartInfo.UseShellExecute = false;//不使用操作系统外壳程序启动线程(一定为FALSE,详细的请看MSDN)
                    process.StartInfo.RedirectStandardInput = true;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;///把外部程序错误输出写到StandardError流中(这个一定要注意,FFMPEG的所有输出信息,都为错误输出流,用StandardOutput是捕获不到任何消息的...这是我耗费了2个多月得出来的经验...mencoder就是用standardOutput来捕获的)
                    process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
                    process.StartInfo.StandardErrorEncoding = Encoding.UTF8;
                    process.StartInfo.CreateNoWindow = true;//不创建进程窗口
                    string strArg = $"-i \"{originalPath}\" {Threads}  {Quality}    \"{newPath}\"";
                    //process.ErrorDataReceived += new DataReceivedEventHandler(Output);///外部程序(这里是FFMPEG)输出流时候产生的事件,这里是把流的处理过程转移到下面的方法中,详细请查阅MSDN
                    process.StartInfo.Arguments = strArg;
                    process.Start();//启动线程
                    process.BeginErrorReadLine();//开始异步读取
                    process.WaitForExit();//阻塞等待进程结束
                    process.Close();//关闭进程
                    process.Dispose();//释放资源
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }


        private static void Output(object sendProcess, DataReceivedEventArgs output)
        {
            if (!String.IsNullOrEmpty(output.Data))
            {
                //处理方法...
                Console.WriteLine(output.Data);
            }
        }
    }
}
