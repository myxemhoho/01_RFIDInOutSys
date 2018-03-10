using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;

namespace Gold.Utility
{
    public class LogHelper
    {
        /// <summary>
        /// 获取异常类中Message和InnerException的Message
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string GetExceptionMsg(Exception ex)
        {
            System.Text.StringBuilder errorMsg = new System.Text.StringBuilder();
            if (ex != null)
            {
                errorMsg.Append(ex.Message == null ? "" : ex.Message);
                errorMsg.Append(Environment.NewLine);
                if (ex.Message != null && ex.InnerException != null && ex.InnerException.Message != null)
                    errorMsg.Append(ex.InnerException.Message);
            }
            return errorMsg.ToString();
        }



        //使用Log4net需配置两个个地方，1-AssemblyInfo.cs 2-WebConfig
        static ILog staticLog = null;
        public static ILog GetLogInstance()
        {
            if (staticLog == null)
            {
                staticLog = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                //log4net.LogManager.GetLogger("MyLoggerName");;

                return staticLog;
            }
            else
            {
                return staticLog;
            }
        }

        //日志级别
        public enum LogLevel
        {
            [Description("信息")]
            Info = 0,

            [Description("错误")]
            Error = 1
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="logLevel">日志级别</param>
        /// <param name="logMessage">日志信息</param>
        public static void WriteLog(LogLevel logLevel, string logMessage)
        {
            ILog logger = GetLogInstance();
            if (LogLevel.Info == logLevel)
            {
                if (logger.IsInfoEnabled)
                    logger.Info(logMessage);
            }
            else if (LogLevel.Error == logLevel)
            {
                if (logger.IsErrorEnabled)
                    logger.Error(logMessage);
            }
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="logLevel">日志级别</param>
        /// <param name="logMessage">日志信息</param>
        /// <param name="ex">异常信息</param>
        public static void WriteLog(LogLevel logLevel, string logMessage, Exception ex)
        {
            ILog logger = GetLogInstance();

            System.Text.StringBuilder strLogMsg = new System.Text.StringBuilder(logMessage);
            strLogMsg.Append(Environment.NewLine);
            strLogMsg.Append(GetExceptionMsg(ex));

            if (LogLevel.Info == logLevel)
            {
                if (logger.IsInfoEnabled)
                    logger.Info(strLogMsg.ToString(), ex);
            }
            else if (LogLevel.Error == logLevel)
            {
                if (logger.IsErrorEnabled)
                    logger.Error(strLogMsg.ToString(), ex);
            }
        }
                
    }
}