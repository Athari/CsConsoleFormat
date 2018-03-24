extern alias CommandLineParser_2_2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CommandLineParser_2_2::CommandLine;

namespace Alba.CsConsoleFormat.CommandLineParser
{
    public partial class ErrorInfo
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        private ErrorInfo FromError22(object data)
        {
            var error = (Error)data;
            Kind = GetErrorKind22(error.Tag);
            Key = GetKey22(error);
            TypeKey = GetTypeKey22(error);
            try {
                Message = error.Tag != ErrorType.MutuallyExclusiveSetError && Kind == ErrorKind.ParseError
                    ? ClpUtils.SentenceBuilder22.FormatError(error) : null;
            }
            catch (InvalidOperationException) {
                Message = null;
            }
            return this;
        }

        private static ErrorKind GetErrorKind22(ErrorType tag)
        {
            switch (tag) {
                case ErrorType.HelpRequestedError:
                case ErrorType.HelpVerbRequestedError:
                    return ErrorKind.HelpVerb;
                case ErrorType.VersionRequestedError:
                    return ErrorKind.VersionVerb;
                default:
                    return ErrorKind.ParseError;
            }
        }

        private static string GetKey22(Error error)
        {
            switch (error) {
                case TokenError e:
                    return e.Token;
                case NamedError e:
                    return e.NameInfo.NameText;
                case HelpVerbRequestedError e:
                    return e.Verb;
                default:
                    return null;
            }
        }

        private static Type GetTypeKey22(Error error)
        {
            switch (error) {
                case HelpVerbRequestedError e:
                    return e.Type;
                default:
                    return null;
            }
        }

        private static void FormatSetErrorsAndRemoveEmpty22(List<ErrorInfo> errorInfos)
        {
            List<ErrorInfo> normalErrorInfos = errorInfos.Where(e => !e.Kind.IsNormal() || e.Message != null).ToList();

            List<Error> errors = errorInfos.Select(e => (Error)e._data).ToList();
            List<MutuallyExclusiveSetError> setErrors = errors.OfType<MutuallyExclusiveSetError>().ToList();

            string setErrorMessages = ClpUtils.SentenceBuilder22.FormatMutuallyExclusiveSetErrors(setErrors);
            if (setErrorMessages.Length > 0) {
                normalErrorInfos.AddRange(setErrorMessages
                    .Split(new[] { Environment.NewLine }, StringSplitOptions.None)
                    .Select(message => new ErrorInfo(message)));
            }

            errorInfos.Clear();
            errorInfos.AddRange(normalErrorInfos);
        }
    }
}