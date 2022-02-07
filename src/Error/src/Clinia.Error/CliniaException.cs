namespace Clinia.Error;

public class CliniaException : Exception
{
    public sealed class ExceptionData
    {
        private ExceptionData(string message,
            Exception cause,
            int code,
            bool retryable,
            string reason,
            string location,
            string debugInfo)
        {
            Message = message;
            Cause = cause;
            Code = code;
            Retryable = retryable;
            Reason = reason;
            Location = location;
            DebugInfo = debugInfo;
        }

        public string Message { get; }
        
        public Exception? Cause { get; }
        
        public int Code { get; }
        
        public bool Retryable { get; }
        
        public string Reason { get; }
        
        public string Location { get; }
        
        public string DebugInfo { get; }

        public static Builder NewBuilder() => new Builder();

        public static ExceptionData From(int code, string message, string reason, bool retryable, Exception? cause = null) 
        {
            return NewBuilder()
                .WithCode(code)
                .WithMessage(message)
                .WithReason(reason)
                .WithRetryable(retryable)
                .WithCause(cause)
                .Build();
        }

        public sealed class Builder
        {
            private string _message;
            private Exception? _cause;
            private int _code;
            private bool _retryable;
            private string _reason;
            private string _location;
            private string _debugInfo;
            
            public Builder() {}

            public Builder WithMessage(string message)
            {
                _message = message;
                return this;
            }

            public Builder WithCause(Exception cause)
            {
                _cause = cause;
                return this;
            }

            public Builder WithCode(int code)
            {
                _code = code;
                return this;
            }

            public Builder WithRetryable(bool retryable)
            {
                _retryable = retryable;
                return this;
            }

            public Builder WithReason(string reason)
            {
                _reason = reason;
                return this;
            }

            public Builder WithLocation(string location)
            {
                _location = location;
                return this;
            }

            public Builder WithDebugInfo(string debugInfo)
            {
                _debugInfo = debugInfo;
                return this;
            }

            public ExceptionData Build()
            {
                return new ExceptionData(_message, _cause, _code, _retryable, _reason, _location, _debugInfo);
            }
        }
    }

    protected CliniaException(ExceptionData exceptionData) : base(exceptionData.Message, exceptionData.Cause)
    {
        Code = exceptionData.Code;
        Retryable = exceptionData.Retryable;
        Reason = exceptionData.Reason;
        Location = exceptionData.Location;
        DebugInfo = exceptionData.DebugInfo;
    }
    
    public int Code { get; }
    
    public bool Retryable { get; }
    
    public string Reason { get; }
    
    public string Location { get; }
    
    public string DebugInfo { get; }
}