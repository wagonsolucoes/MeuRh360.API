using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class ResponseBase<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }
        public IEnumerable<string>? Errors { get; set; }

        public static ResponseBase<T> SuccessResponse(string message, T? data = default) =>
            new() { Success = true, Message = message, Data = data };

        public static ResponseBase<T> ErrorResponse(string message, IEnumerable<string>? errors = null) =>
            new() { Success = false, Message = message, Errors = errors };
    }
}
