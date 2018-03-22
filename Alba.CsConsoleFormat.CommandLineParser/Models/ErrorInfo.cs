using System;
using System.Collections.Generic;

namespace Alba.CsConsoleFormat.CommandLineParser
{
    public sealed partial class ErrorInfo
    {
        private readonly object _data;

        public string Message { get; private set; }
        public string Details { get; private set; }
        public ErrorKind Kind { get; private set; }
        public string Key { get; private set; }
        public Type TypeKey { get; private set; }

        private ErrorInfo(object data)
        {
            _data = data;
        }

        public ErrorInfo(string message, ErrorKind kind = ErrorKind.Error, string details = null)
        {
            Message = message;
            Details = details;
            Kind = kind;
        }

        public static ErrorInfo Exception(Exception ex) => new ErrorInfo(ex.Message, ErrorKind.Error, ex.ToString());
        public static ErrorInfo Error(string message, string details = null) => new ErrorInfo(message, ErrorKind.Error, details);
        public static ErrorInfo Warning(string message, string details = null) => new ErrorInfo(message, ErrorKind.Warning, details);
        public static ErrorInfo Info(string message, string details = null) => new ErrorInfo(message, ErrorKind.Info, details);

        internal static ErrorInfo FromError(object data)
        {
            var error = new ErrorInfo(data);
          #if CLP_19
            if (ClpUtils.IsVersion19)
                return error.FromError19(data);
          #endif
          #if CLP_22
            if (ClpUtils.IsVersion22)
                return error.FromError22(data);
          #endif
            throw ClpUtils.UnsupportedVersion();
        }

        internal static void FormatSetErrors(List<ErrorInfo> errorInfos)
        {
          #if CLP_22
            if (ClpUtils.IsVersion22)
                FormatSetErrorsAndRemoveEmpty22(errorInfos);
          #endif
        }
    }
}